using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

namespace WhatsAppSM
{
    public delegate void MessageReceivedEventHandler(object sender, MessageEventArgs args);

    public class MSMQListenerHelper
    {
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
                _queue = new System.Messaging.MessageQueue(@".\" + queuePath);
            else
                _queue = MessageQueue.Create(@".\" + queuePath);
        }

        public void Start()
        {
            _listen = true;

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
            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageEventArgs(pMensaje));
            }
        }

        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.PonerStatusEnviado(Usuario, e.Message.Label);
            System.Messaging.Message msg = _queue.EndReceive(e.AsyncResult);
            FireRecieveEvent(msg);
            StartListening();
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
