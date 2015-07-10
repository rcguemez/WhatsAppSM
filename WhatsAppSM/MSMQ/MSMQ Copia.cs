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

namespace MSMQ
{
    public partial class MSMQCopia : Form
    {
        System.Messaging.MessageQueue mq;
        public MSMQCopia()
        {
            InitializeComponent();
            GetPrivateQueues();
            this.cmbTipo.SelectedIndex = 0;
        }

        public void GetPrivateQueues()
        {
            MessageQueue[] QueueList = MessageQueue.GetPrivateQueuesByMachine(".");
            foreach (MessageQueue queueItem in QueueList)
            {
                if (queueItem.QueueName.Contains("whatsapp_"))
                {
                    this.cmbUsuarios.Items.Add(queueItem.QueueName.Substring(queueItem.QueueName.LastIndexOf("_") + 1));
                }
            }
            if(QueueList.Length > 0)
            {
                this.cmbUsuarios.SelectedIndex = 0;
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            this.txtRespuesta.Text = "";
            System.Messaging.Message msj = new System.Messaging.Message();
            if(this.cmbTipo.SelectedIndex == 0)
            {
                msj.Priority = MessagePriority.Normal;
            }
            else
            {
                msj.Priority = MessagePriority.High;
            }
            string[] strArr = null;
            char[] splitchar = { ';' };
            strArr = this.txtCelular.Text.Split(splitchar);
            for (int i = 0; i <= strArr.Length - 1; i++)
            {
                SchoolManager.WhatsApp.Entidades.WhatsApp_UsuarioEN objMensaje = new SchoolManager.WhatsApp.Entidades.WhatsApp_UsuarioEN();
                objMensaje.FOLIO = "";
                objMensaje.EMISOR = "9997371690";
                objMensaje.RECEPTOR = strArr[i];
                objMensaje.RECURSO = 0;
                objMensaje.MENSAJE = this.txtMensaje.Text;
                objMensaje.PRIORIDAD = this.cmbTipo.SelectedIndex;
                objMensaje.STATUS = 0;
                msj.Label = SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.Agregar(cmbUsuarios.Text, objMensaje);
                msj.Body = string.Format("{0}|{1}|{2}", objMensaje.RECEPTOR, objMensaje.RECURSO, objMensaje.MENSAJE);
                mq.Send(msj);
                this.txtRespuesta.AppendText(string.Format("{0}: {1}|{2}", i + 1, msj.Label, msj.Body) + Environment.NewLine);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            this.txtCelular.Text = "";
            this.txtMensaje.Text = "";
        }

        private void cmbUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MessageQueue.Exists(@".\Private$\whatsapp_" + this.cmbUsuarios.Items[this.cmbUsuarios.SelectedIndex].ToString()))
                mq = new System.Messaging.MessageQueue(@".\Private$\whatsapp_" + this.cmbUsuarios.Items[this.cmbUsuarios.SelectedIndex].ToString());
            else
                mq = MessageQueue.Create(@".\Private$\whatsapp_" + this.cmbUsuarios.Items[this.cmbUsuarios.SelectedIndex].ToString());
        }
    }
}
