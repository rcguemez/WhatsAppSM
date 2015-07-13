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

namespace WhatsAppSM
{
    public delegate void MessageReceivedEventHandler(object sender, MessageEventArgs args);

    public class MSMQListenerHelper
    {
        WhatsApp wa;
        List<SchoolManager.WhatsApp.Entidades.Emisores_UsuariosEN> listaEmisores_UsuariosEN;
        List<SchoolManager.WhatsApp.Entidades.EmisoresEN> listaEmisoresEN;
        private bool _listen;
        private Type[] _types;
        private MessageQueue _queue;

        public event MessageReceivedEventHandler MessageReceived;

        public Type[] FormatterTypes
        {
            get { return _types; }
            set { _types = value; }
        }
        private string _cola;
        public string Cola
        {
            get { return _cola; }
            set { _cola = value; }
        }
        public string Usuario
        {
            get 
            {
                if (_cola.Contains("whatsapp_"))
                {
                    return _cola.Substring(_cola.LastIndexOf("_") + 1);
                }
                else
                {
                    return "";
                }
            }
        }

        public MSMQListenerHelper(string queuePath)
        {
            _cola = queuePath;
            if (MessageQueue.Exists(@".\" + queuePath))
            {
                _queue = new System.Messaging.MessageQueue(@".\" + queuePath);
            }
            else
            {
                _queue = MessageQueue.Create(@".\" + queuePath);
            }
        }

        System.Threading.Thread thRecv;
        byte[] nextChallenge = null;
        DateTime dtUltimaConexion;
        public void Start()
        {
            _listen = true;
            listaEmisores_UsuariosEN = SchoolManager.WhatsApp.LogicaNegocios.Emisores_UsuariosLN.ObtenerPorUsuario(Usuario);
            listaEmisoresEN = SchoolManager.WhatsApp.LogicaNegocios.EmisoresLN.ObtenerPorEmisor(listaEmisores_UsuariosEN[0].EMISOR);
            wa = new WhatsApp(listaEmisores_UsuariosEN[0].EMISOR, listaEmisoresEN[0].APIKEY, listaEmisores_UsuariosEN[0].NOMBREPERFIL, true);
            dtUltimaConexion = DateTime.Now;
            wa.Connect();
            wa.Login();
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

                SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusListoParaEnviar(Usuario, listaEmisores_UsuariosEN[0].EMISOR, folio, "");
                //wa.SendMessage(receptor, e.Message.Body.ToString());

                try
                {
                    WhatsUserManager usrMan = new WhatsUserManager();
                    var tmpUser = usrMan.CreateUser(receptor, "User" + receptor);
                    WhatsAppApi.Parser.FMessage.FMessageIdentifierKey key = new WhatsAppApi.Parser.FMessage.FMessageIdentifierKey(tmpUser.GetFullJid(), true, folio);
                    WhatsAppApi.Parser.FMessage msj = new WhatsAppApi.Parser.FMessage(key);
                    msj.data = e.Message.Body.ToString();
                    if(dtUltimaConexion.AddMinutes(1) < DateTime.Now)
                    {
                        wa.Connect();
                        wa.Login();
                    }
                    dtUltimaConexion = DateTime.Now;
                    if (wa.ConnectionStatus == ApiBase.CONNECTION_STATUS.UNAUTHORIZED)
                    {
                        SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusErrorEnvio(Usuario, listaEmisores_UsuariosEN[0].EMISOR, folio, "No Autorizado para enviar");
                        this.Stop();
                        return;
                    }
                    wa.SendMessage(msj);
                    Random objRandom = new Random();
                    System.Threading.Thread.Sleep(objRandom.Next(1000, 1500));
                }
                catch (Exception ex)
                {
                    SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusErrorEnvio(Usuario, listaEmisores_UsuariosEN[0].EMISOR, folio, "Error al enviar: " + ex.Message);
                    return;
                }
                SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusEnviando(Usuario, listaEmisores_UsuariosEN[0].EMISOR, folio, "");
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

    public class MessageEventArgs : EventArgs
    {
        private System.Messaging.Message _mensaje;

        public System.Messaging.Message Mensaje
        {
            get { return _mensaje; }
        }

        public MessageEventArgs(System.Messaging.Message pMensaje)
        {
            _mensaje = pMensaje;

        }
    }
}
