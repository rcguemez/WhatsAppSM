using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using WhatsAppApi;
using WhatsAppApi.Account;
using WhatsAppApi.Helper;
using WhatsAppApi.Register;
using WhatsAppApi.Response;
using System.IO;
using System.Data;

namespace WhatsAppSM
{
    public delegate void MessageReceivedEventHandler2(object sender, MessageEventArgs2 args);

    public class MSMQListenerHelper2
    {
        WhatsApp wa;
        WhatsApp wa_1;
        DataTable dtEmisores_UsuariosEN;
        DataTable dtEmisores_UsuariosEN_1;
        private bool _listen;
        private Type[] _types;
        private MessageQueue _queue;

        public event MessageReceivedEventHandler2 MessageReceived;

        public Type[] FormatterTypes
        {
            get { return _types; }
            set { _types = value; }
        }
        private string _usuario;
        public string Usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
        
        public MSMQListenerHelper2(string pUsuario)
        {
            _usuario = pUsuario;
            if (MessageQueue.Exists(@".\Private$\whatsapp_" + pUsuario))
            {
                _queue = new System.Messaging.MessageQueue(@".\Private$\whatsapp_" + pUsuario);
            }
            else
            {
                _queue = MessageQueue.Create(@".\Private$\whatsapp_" + pUsuario);
            }
            _queue.MessageReadPropertyFilter.SetAll();
        }

        System.Threading.Thread thRecv;
        byte[] nextChallenge = null;
        DateTime dtUltimaConexion;
        public void Start()
        {
            dtEmisores_UsuariosEN = SchoolManager.WhatsApp.LogicaNegocios.Emisores_UsuariosLN.DtEmisorActivoPorUsuario(Usuario, 0);
            dtEmisores_UsuariosEN_1 = SchoolManager.WhatsApp.LogicaNegocios.Emisores_UsuariosLN.DtEmisorActivoPorUsuario(Usuario, 1);
            if(dtEmisores_UsuariosEN.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("Usuario: {0}.- No hay emisor para enviar mensajes", Usuario));
                return;
            }
            if (dtEmisores_UsuariosEN_1.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("Usuario: {0}.- No hay emisor para enviar notificaciones", Usuario));
                return;
            }
            _listen = true;
            wa = new WhatsApp(dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString(), dtEmisores_UsuariosEN.Rows[0]["APIKEY"].ToString(), dtEmisores_UsuariosEN.Rows[0]["NOMBREPERFIL"].ToString(), true);
            wa_1 = new WhatsApp(dtEmisores_UsuariosEN_1.Rows[0]["EMISOR"].ToString(), dtEmisores_UsuariosEN_1.Rows[0]["APIKEY"].ToString(), dtEmisores_UsuariosEN_1.Rows[0]["NOMBREPERFIL"].ToString(), true);
            dtUltimaConexion = DateTime.Now;
            wa.Connect();
            wa.Login();
            wa_1.Connect();
            wa_1.Login();
            //string datFile = getDatFileName(listaEmisores_UsuariosEN[0].EMISOR);
            //if (File.Exists(datFile))
            //{
            //    try
            //    {
            //        string foo = File.ReadAllText(datFile);
            //        nextChallenge = Convert.FromBase64String(foo);
            //    }
            //    catch (Exception) { };
            //}
            //wa.Login(nextChallenge);
            //wa.SendGetServerProperties();

            //thRecv = new System.Threading.Thread(t =>
            //{
            //    try
            //    {
            //        while (wa != null)
            //        {
            //            wa.PollMessages();
            //            System.Threading.Thread.Sleep(100);
            //            continue;
            //        }

            //    }
            //    catch (System.Threading.ThreadAbortException)
            //    {
            //    }
            //}) { IsBackground = true };
            //thRecv.Start();

            if (_types != null && _types.Length > 0)
            {
                // Using only the XmlMessageFormatter. You can use other formatters as well
                _queue.Formatter = new XmlMessageFormatter(_types);
            }

            _queue.PeekCompleted += new PeekCompletedEventHandler(OnPeekCompleted);
            _queue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);

            StartListening();
        }

        public void Stop()
        {
            _listen = false;
            wa.Disconnect();
            wa_1.Disconnect();
            _queue.PeekCompleted -= new PeekCompletedEventHandler(OnPeekCompleted);
            _queue.ReceiveCompleted -= new ReceiveCompletedEventHandler(OnReceiveCompleted);

        }

        private void StartListening()
        {
            if (!_listen)
            {
                return;
            }

            // The MSMQ class does not have a BeginRecieve method that can take in a 
            // MSMQ transaction object. This is a workaround – we do a BeginPeek and then 
            // recieve the message synchronously in a transaction.
            // Check documentation for more details
            if (_queue.Transactional)
            {
                _queue.BeginPeek();
            }
            else
            {
                _queue.BeginReceive();
            }
        }

        private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            _queue.EndPeek(e.AsyncResult);
            MessageQueueTransaction trans = new MessageQueueTransaction();
            System.Messaging.Message msg = null;
            try
            {
                trans.Begin();
                msg = _queue.Receive(trans);
                trans.Commit();

                StartListening();

                FireRecieveEvent(msg);
            }
            catch
            {
                trans.Abort();
            }
        }

        private void FireRecieveEvent(System.Messaging.Message pMensaje)
        {
            //if (MessageReceived != null)
            //{
            //    MessageReceived(this, new MessageEventArgs(pMensaje));
            //}
        }

        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                System.Messaging.Message msg = _queue.EndReceive(e.AsyncResult);
                FireRecieveEvent(msg);
                string[] strArr = null;
                char[] splitchar = { '|' };
                strArr = e.Message.Label.Split(splitchar);
                string folio, receptor, recurso;
                folio = strArr[0];
                receptor = strArr[1];
                recurso = strArr[2];
                bool nuevaConexion = false;
                if(e.Message.Priority == MessagePriority.High)
                {
                volver_conectar:
                    try
                    {
                        SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusListoParaEnviar(Usuario, dtEmisores_UsuariosEN_1.Rows[0]["EMISOR"].ToString(), folio, "");
                        WhatsUserManager usrMan = new WhatsUserManager();
                        var tmpUser = usrMan.CreateUser(receptor, "User" + receptor);
                        WhatsAppApi.Parser.FMessage.FMessageIdentifierKey key = new WhatsAppApi.Parser.FMessage.FMessageIdentifierKey(tmpUser.GetFullJid(), true, folio);
                        WhatsAppApi.Parser.FMessage msj = new WhatsAppApi.Parser.FMessage(key);
                        msj.data = e.Message.Body.ToString();
                        if (dtUltimaConexion.AddMinutes(1) < DateTime.Now | nuevaConexion)
                        {
                            wa_1.Connect();
                            wa_1.Login();
                        }
                        dtUltimaConexion = DateTime.Now;
                        if (wa_1.ConnectionStatus == ApiBase.CONNECTION_STATUS.CONNECTED | wa_1.ConnectionStatus == ApiBase.CONNECTION_STATUS.DISCONNECTED)
                        {
                            wa_1.Connect();
                            wa_1.Login();
                        }
                        else if (wa_1.ConnectionStatus == ApiBase.CONNECTION_STATUS.UNAUTHORIZED)
                        {
                            SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusErrorEnvio(Usuario, dtEmisores_UsuariosEN_1.Rows[0]["EMISOR"].ToString(), folio, "No Autorizado para enviar");
                            this.Stop();
                            return;
                        }
                        wa_1.SendMessage(msj);
                        Random objRandom = new Random();
                        System.Threading.Thread.Sleep(objRandom.Next(2000, 3000));
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Se ha anulado una conexión establecida por el software en su equipo host")
                        {
                            nuevaConexion = true;
                            wa_1.Disconnect();
                            System.Threading.Thread.Sleep(5000);
                            goto volver_conectar;
                        }
                        else
                        {
                            SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusErrorEnvio(Usuario, dtEmisores_UsuariosEN_1.Rows[0]["EMISOR"].ToString(), folio, "Error al enviar: " + ex.Message);
                            return;
                        }
                    }
                    SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusEnviando(Usuario, dtEmisores_UsuariosEN_1.Rows[0]["EMISOR"].ToString(), folio, "");
                }
                else
                {
                volver_conectar:
                    try
                    {
                        SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusListoParaEnviar(Usuario, dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString(), folio, "");
                        WhatsUserManager usrMan = new WhatsUserManager();
                        var tmpUser = usrMan.CreateUser(receptor, "User" + receptor);
                        WhatsAppApi.Parser.FMessage.FMessageIdentifierKey key = new WhatsAppApi.Parser.FMessage.FMessageIdentifierKey(tmpUser.GetFullJid(), true, folio);
                        WhatsAppApi.Parser.FMessage msj = new WhatsAppApi.Parser.FMessage(key);
                        msj.data = e.Message.Body.ToString();
                        if (dtUltimaConexion.AddMinutes(1) < DateTime.Now | nuevaConexion)
                        {
                            wa.Connect();
                            wa.Login();
                        }
                        dtUltimaConexion = DateTime.Now;
                        if (wa.ConnectionStatus == ApiBase.CONNECTION_STATUS.CONNECTED | wa.ConnectionStatus == ApiBase.CONNECTION_STATUS.DISCONNECTED)
                        {
                            wa.Connect();
                            wa.Login();
                        }
                        else if (wa.ConnectionStatus == ApiBase.CONNECTION_STATUS.UNAUTHORIZED)
                        {
                            SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusErrorEnvio(Usuario, dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString(), folio, "No Autorizado para enviar");
                            this.Stop();
                            return;
                        }
                        wa.SendMessage(msj);
                        Random objRandom = new Random();
                        System.Threading.Thread.Sleep(objRandom.Next(2000, 3000));
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Se ha anulado una conexión establecida por el software en su equipo host")
                        {
                            nuevaConexion = true;
                            wa.Disconnect();
                            System.Threading.Thread.Sleep(5000);
                            goto volver_conectar;
                        }
                        else
                        {
                            SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusErrorEnvio(Usuario, dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString(), folio, "Error al enviar: " + ex.Message);
                            return;
                        }
                    }
                    SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusEnviando(Usuario, dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString(), folio, "");
                }
                StartListening();
            }
            catch(Exception ex)
            {
                this.Stop();
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private string getDatFileName(string pn)
        {
            string filename = string.Format("{0}.next.dat", pn);
            return Path.Combine(Directory.GetCurrentDirectory(), filename);
        }

    }

    public class MessageEventArgs2 : EventArgs
    {
        private System.Messaging.Message _mensaje;

        public System.Messaging.Message Mensaje
        {
            get { return _mensaje; }
        }

        public MessageEventArgs2(System.Messaging.Message pMensaje)
        {
            _mensaje = pMensaje;

        }
    }
}
