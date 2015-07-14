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
using System.Net;

namespace WhatsAppSM
{
    public delegate void MessageReceivedEventHandlerConStatus(object sender, MessageEventArgs args);

    public class MSMQListenerHelperConStatus
    {
        WhatsApp wa;
        DataTable dtEmisores_UsuariosEN;
        private bool _listen;
        private Type[] _types;
        private MessageQueue _queue;

        public event MessageReceivedEventHandlerConStatus MessageReceived;

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
        private int _prioridad;
        public int Prioridad
        {
            get { return _prioridad; }
            set { _prioridad = value; }
        }

        public MSMQListenerHelperConStatus(string pUsuario, int pPrioridad)
        {
            _usuario = pUsuario;
            _prioridad = pPrioridad;
            if (MessageQueue.Exists(@".\Private$\whatsapp_" + pUsuario + "_" + pPrioridad.ToString()))
            {
                _queue = new System.Messaging.MessageQueue(@".\Private$\whatsapp_" + pUsuario + "_" + pPrioridad.ToString());
            }
            else
            {
                _queue = MessageQueue.Create(@".\Private$\whatsapp_" + pUsuario + "_" + pPrioridad.ToString());
            }
            _queue.MessageReadPropertyFilter.SetAll();
        }

        System.Threading.Thread thRecv;
        byte[] nextChallenge = null;
        DateTime dtUltimaConexion;
        public void Start()
        {
            dtEmisores_UsuariosEN = SchoolManager.WhatsApp.LogicaNegocios.Emisores_UsuariosLN.DtEmisorActivoPorUsuario(Usuario, _prioridad);
            if(dtEmisores_UsuariosEN.Rows.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("Usuario: {0}.- No hay emisor para enviar mensajes", Usuario));
                return;
            }
            _listen = true;
            wa = new WhatsApp(dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString(), dtEmisores_UsuariosEN.Rows[0]["APIKEY"].ToString(), dtEmisores_UsuariosEN.Rows[0]["NOMBREPERFIL"].ToString(), true);

            wa.OnLoginSuccess += wa_OnLoginSuccess;
            wa.OnLoginFailed += wa_OnLoginFailed;
            wa.OnGetMessage += wa_OnGetMessage;
            wa.OnGetMessageReceivedClient += wa_OnGetMessageReceivedClient;
            wa.OnGetMessageReceivedServer += wa_OnGetMessageReceivedServer;
            wa.OnNotificationPicture += wa_OnNotificationPicture;
            wa.OnGetPresence += wa_OnGetPresence;
            wa.OnGetGroupParticipants += wa_OnGetGroupParticipants;
            wa.OnGetLastSeen += wa_OnGetLastSeen;
            wa.OnGetTyping += wa_OnGetTyping;
            wa.OnGetPaused += wa_OnGetPaused;
            wa.OnGetMessageImage += wa_OnGetMessageImage;
            wa.OnGetMessageAudio += wa_OnGetMessageAudio;
            wa.OnGetMessageVideo += wa_OnGetMessageVideo;
            wa.OnGetMessageLocation += wa_OnGetMessageLocation;
            wa.OnGetMessageVcard += wa_OnGetMessageVcard;
            wa.OnGetPhoto += wa_OnGetPhoto;
            wa.OnGetPhotoPreview += wa_OnGetPhotoPreview;
            wa.OnGetGroups += wa_OnGetGroups;
            wa.OnGetSyncResult += wa_OnGetSyncResult;
            wa.OnGetStatus += wa_OnGetStatus;
            wa.OnGetPrivacySettings += wa_OnGetPrivacySettings;
            DebugAdapter.Instance.OnPrintDebug += Instance_OnPrintDebug;

            dtUltimaConexion = DateTime.Now;
            wa.Connect();
            //wa.Login();
            string datFile = getDatFileName(dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString());
            if (File.Exists(datFile))
            {
                try
                {
                    string foo = File.ReadAllText(datFile);
                    nextChallenge = Convert.FromBase64String(foo);
                }
                catch (Exception) { };
            }
            wa.Login(nextChallenge);
            wa.SendGetServerProperties();

            thRecv = new System.Threading.Thread(t =>
            {
                try
                {
                    while (_listen)
                    {
                        wa.PollMessages();
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }

                }
                catch (System.Threading.ThreadAbortException)
                {
                }
            }) { IsBackground = true };
            thRecv.Start();

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
            thRecv.Abort();
            wa.Disconnect();

            wa.OnLoginSuccess -= wa_OnLoginSuccess;
            wa.OnLoginFailed -= wa_OnLoginFailed;
            wa.OnGetMessage -= wa_OnGetMessage;
            wa.OnGetMessageReceivedClient -= wa_OnGetMessageReceivedClient;
            wa.OnGetMessageReceivedServer -= wa_OnGetMessageReceivedServer;
            wa.OnNotificationPicture -= wa_OnNotificationPicture;
            wa.OnGetPresence -= wa_OnGetPresence;
            wa.OnGetGroupParticipants -= wa_OnGetGroupParticipants;
            wa.OnGetLastSeen -= wa_OnGetLastSeen;
            wa.OnGetTyping -= wa_OnGetTyping;
            wa.OnGetPaused -= wa_OnGetPaused;
            wa.OnGetMessageImage -= wa_OnGetMessageImage;
            wa.OnGetMessageAudio -= wa_OnGetMessageAudio;
            wa.OnGetMessageVideo -= wa_OnGetMessageVideo;
            wa.OnGetMessageLocation -= wa_OnGetMessageLocation;
            wa.OnGetMessageVcard -= wa_OnGetMessageVcard;
            wa.OnGetPhoto -= wa_OnGetPhoto;
            wa.OnGetPhotoPreview -= wa_OnGetPhotoPreview;
            wa.OnGetGroups -= wa_OnGetGroups;
            wa.OnGetSyncResult -= wa_OnGetSyncResult;
            wa.OnGetStatus -= wa_OnGetStatus;
            wa.OnGetPrivacySettings -= wa_OnGetPrivacySettings;
            DebugAdapter.Instance.OnPrintDebug -= Instance_OnPrintDebug;
            wa = null;
            
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
                        //wa.Login();
                        string datFile = getDatFileName(dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString());
                        if (File.Exists(datFile))
                        {
                            try
                            {
                                string foo = File.ReadAllText(datFile);
                                nextChallenge = Convert.FromBase64String(foo);
                            }
                            catch (Exception) { };
                        }
                        wa.Login(nextChallenge);
                        wa.SendGetServerProperties();
                    }
                    dtUltimaConexion = DateTime.Now;
                    if (wa.ConnectionStatus == ApiBase.CONNECTION_STATUS.CONNECTED | wa.ConnectionStatus == ApiBase.CONNECTION_STATUS.DISCONNECTED)
                    {
                        wa.Connect();
                        //wa.Login();
                        string datFile = getDatFileName(dtEmisores_UsuariosEN.Rows[0]["EMISOR"].ToString());
                        if (File.Exists(datFile))
                        {
                            try
                            {
                                string foo = File.ReadAllText(datFile);
                                nextChallenge = Convert.FromBase64String(foo);
                            }
                            catch (Exception) { };
                        }
                        wa.Login(nextChallenge);
                        wa.SendGetServerProperties();
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

        #region Eventos WhatsApp
        private void Instance_OnPrintDebug(object value)
        {
            //Console.WriteLine(value);
            try
            {
                if(value.ToString().ToLower().Contains("<ack type=\"read\""))
                {
                    string folio = value.ToString().Substring(value.ToString().IndexOf("id=\"") + 4, 20);
                    SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusLeido(Usuario, folio, "");
                }
            }
            catch { }
        }

        private void wa_OnGetPrivacySettings(Dictionary<ApiBase.VisibilityCategory, ApiBase.VisibilitySetting> settings)
        {
            throw new NotImplementedException();
        }

        private void wa_OnGetStatus(string from, string type, string name, string status)
        {
            //Console.WriteLine(String.Format("Got status from {0}: {1}", from, status));
        }

        private void wa_OnGetSyncResult(int index, string sid, Dictionary<string, string> existingUsers, string[] failedNumbers)
        {
            //Console.WriteLine("Sync result for {0}:", sid);
            //foreach (KeyValuePair<string, string> item in existingUsers)
            //{
            //    Console.WriteLine("Existing: {0} (username {1})", item.Key, item.Value);
            //}
            //foreach (string item in failedNumbers)
            //{
            //    Console.WriteLine("Non-Existing: {0}", item);
            //}
        }

        private void wa_OnGetGroups(WaGroupInfo[] groups)
        {
            //Console.WriteLine("Got groups:");
            //foreach (WaGroupInfo info in groups)
            //{
            //    Console.WriteLine("\t{0} {1}", info.subject, info.id);
            //}
        }

        private void wa_OnGetPhotoPreview(string from, string id, byte[] data)
        {
            //Console.WriteLine("Got preview photo for {0}", from);
            //File.WriteAllBytes(string.Format("preview_{0}.jpg", from), data);
        }

        private void wa_OnGetPhoto(string from, string id, byte[] data)
        {
            //Console.WriteLine("Got full photo for {0}", from);
            //File.WriteAllBytes(string.Format("{0}.jpg", from), data);
        }

        private void wa_OnGetMessageVcard(ProtocolTreeNode vcardNode, string from, string id, string name, byte[] data)
        {
            //Console.WriteLine("Got vcard \"{0}\" from {1}", name, from);
            //File.WriteAllBytes(string.Format("{0}.vcf", name), data);
        }

        private void wa_OnGetMessageLocation(ProtocolTreeNode locationNode, string from, string id, double lon, double lat, string url, string name, byte[] preview)
        {
            //Console.WriteLine("Got location from {0} ({1}, {2})", from, lat, lon);
            //if (!string.IsNullOrEmpty(name))
            //{
            //    Console.WriteLine("\t{0}", name);
            //}
            //File.WriteAllBytes(string.Format("{0}{1}.jpg", lat, lon), preview);
        }

        private void wa_OnGetMessageVideo(ProtocolTreeNode mediaNode, string from, string id, string fileName, int fileSize, string url, byte[] preview)
        {
            //Console.WriteLine("Got video from {0}", from, fileName);
            //OnGetMedia(fileName, url, preview);
        }

        private void OnGetMedia(string file, string url, byte[] data)
        {
            ////save preview
            //File.WriteAllBytes(string.Format("preview_{0}.jpg", file), data);
            ////download
            //using (WebClient wc = new WebClient())
            //{
            //    wc.DownloadFileAsync(new Uri(url), file, null);
            //}
        }

        private void wa_OnGetMessageAudio(ProtocolTreeNode mediaNode, string from, string id, string fileName, int fileSize, string url, byte[] preview)
        {
            //Console.WriteLine("Got audio from {0}", from, fileName);
            //OnGetMedia(fileName, url, preview);
        }

        private void wa_OnGetMessageImage(ProtocolTreeNode mediaNode, string from, string id, string fileName, int size, string url, byte[] preview)
        {
            //Console.WriteLine("Got image from {0}", from, fileName);
            //OnGetMedia(fileName, url, preview);
        }

        private void wa_OnGetPaused(string from)
        {
            //Console.WriteLine("{0} stopped typing", from);
        }

        private void wa_OnGetTyping(string from)
        {
            //Console.WriteLine("{0} is typing...", from);
        }

        private void wa_OnGetLastSeen(string from, DateTime lastSeen)
        {
            //Console.WriteLine("{0} last seen on {1}", from, lastSeen.ToString());
        }

        private void wa_OnGetMessageReceivedServer(string from, string id)
        {
            //Console.WriteLine("Message {0} to {1} received by server", id, from);
            try
            {
                SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusEnviado(Usuario, id, "");
            }
            catch { }
        }

        private void wa_OnGetMessageReceivedClient(string from, string id)
        {
            //Console.WriteLine("Message {0} to {1} received by client", id, from);
            try
            {
                SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusEntregado(Usuario, id, "");
            }
            catch { }
        }

        private void wa_OnGetGroupParticipants(string gjid, string[] jids)
        {
            //Console.WriteLine("Got participants from {0}:", gjid);
            //foreach (string jid in jids)
            //{
            //    Console.WriteLine("\t{0}", jid);
            //}
        }

        private void wa_OnGetPresence(string from, string type)
        {
            //Console.WriteLine("Presence from {0}: {1}", from, type);
        }

        private void wa_OnNotificationPicture(string type, string jid, string id)
        {
            //TODO
            //throw new NotImplementedException();
        }

        private void wa_OnGetMessage(ProtocolTreeNode node, string from, string id, string name, string message, bool receipt_sent)
        {
            //Console.WriteLine("Message from {0} {1}: {2}", name, from, message);
        }

        private void wa_OnLoginFailed(string data)
        {
            //Console.WriteLine("Login failed. Reason: {0}", data);
        }

        private void wa_OnLoginSuccess(string phoneNumber, byte[] data)
        {
            ////Console.WriteLine("Login success. Next password:");
            //string sdata = Convert.ToBase64String(data);
            ////Console.WriteLine(sdata);
            //try
            //{
            //    File.WriteAllText(getDatFileName(phoneNumber), sdata);
            //}
            //catch (Exception) { }
        }
        #endregion

    }

    public class MessageEventArgsConStatus : EventArgs
    {
        private System.Messaging.Message _mensaje;

        public System.Messaging.Message Mensaje
        {
            get { return _mensaje; }
        }

        public MessageEventArgsConStatus(System.Messaging.Message pMensaje)
        {
            _mensaje = pMensaje;

        }
    }
}
