using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Messaging;

namespace WhatsAppSM
{
    public partial class frmMain : Form
    {
        List<MSMQListenerHelper> listaMLH = new List<MSMQListenerHelper>();
        public frmMain()
        {
            InitializeComponent();
            GetPrivateQueues();
        }
        public void GetPrivateQueues()
        {
            MessageQueue[] QueueList = MessageQueue.GetPrivateQueuesByMachine(".");
            foreach (MessageQueue queueItem in QueueList)
            {
                if (queueItem.QueueName.Contains("whatsapp_"))
                {
                    this.listBox1.Items.Add(queueItem.QueueName);
                    listaMLH.Add(new MSMQListenerHelper(queueItem.QueueName));
                    listaMLH[listaMLH.Count - 1].FormatterTypes = new Type[] { typeof(string) };
                    listaMLH[listaMLH.Count - 1].MessageReceived += new MessageReceivedEventHandler(MessageReceived);
                    listaMLH[listaMLH.Count - 1].Start();
                }
            }
        }
        private void MessageReceived(object sender, MessageEventArgs e)
        {
            SetControlPropertyValue(this, "Text", ".\\" + ((MSMQListenerHelper)sender).Cola + ": " + e.Mensaje.Label);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (MSMQListenerHelper objMHL in listaMLH)
            {
                objMHL.Stop();
            }
        }
        public delegate void SetControlValueCallback(Control pControl, string pName, object pValue);
        public void SetControlPropertyValue(Control pControl, string pName, object pValue)
        {
            if (pControl.InvokeRequired)
            {
                SetControlValueCallback d = new SetControlValueCallback(SetControlPropertyValue);
                pControl.Invoke(d, new object[] {
			pControl,
			pName,
			pValue
		});
            }
            else
            {
                Type t = pControl.GetType();
                System.Reflection.PropertyInfo[] props = t.GetProperties();
                foreach (System.Reflection.PropertyInfo p in props)
                {
                    if (p.Name.ToUpper() == pName.ToUpper())
                    {
                        p.SetValue(pControl, pValue, null);
                    }
                }
            }
        }
    }
}
