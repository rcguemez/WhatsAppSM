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
using System.Net.Http;
using System.Net.Http.Headers;

namespace MSMQ
{
    public partial class MSMQ : Form
    {
        System.Messaging.MessageQueue mq;
        public MSMQ()
        {
            InitializeComponent();
            List<SchoolManager.WhatsApp.Entidades.UsuariosEN> listaUsuariosEN = SchoolManager.WhatsApp.LogicaNegocios.UsuariosLN.ObtenerTodo();
            for (int i = 0; i < listaUsuariosEN.Count; i++)
            {
                this.cmbUsuarios.Items.Add(listaUsuariosEN[i].USUARIO);
            }
            if (listaUsuariosEN.Count > 0)
            {
                this.cmbUsuarios.SelectedIndex = 0;
            }
            this.cmbTipo.SelectedIndex = 0;
        }

        public void GetPrivateQueues()
        {
            MessageQueue[] QueueList = MessageQueue.GetPrivateQueuesByMachine(".");
            foreach (MessageQueue queueItem in QueueList)
            {
                if (queueItem.QueueName.Contains("whatsapp_"))
                {
                    
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
            Mensajes objMensaje = new Mensajes() { Usuario = this.cmbUsuarios.Text, Password = "1234", CodigoPais = "521", Celular = this.txtCelular.Text, Mensaje = this.txtMensaje.Text, Prioridad = this.cmbTipo.SelectedIndex };
            RunAsync(objMensaje).Wait(100);
        }

        async Task RunAsync(Mensajes pMensaje)
        {
            string mensaje = pMensaje.Mensaje;
            for (int i = 1; i <= numMensajes.Value;i++ )
            {
                try
                {
                    pMensaje.Mensaje = i.ToString() + " - " + mensaje;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings.Get("URL_WEP_API"));
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response;
                        // HTTP POST
                        response = await client.PostAsJsonAsync("api/mensajes", pMensaje);
                        if (response.IsSuccessStatusCode)
                        {
                            string[] respuesta = await response.Content.ReadAsAsync<string[]>();
                            foreach (string res in respuesta)
                            {
                                this.txtRespuesta.AppendText(res + Environment.NewLine);
                            }
                        }
                        else
                        {
                            this.txtRespuesta.Text = "False|Error al enviar mensaje";
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.txtRespuesta.Text = "False|" + ex.Message;
                }
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            this.txtCelular.Text = "";
            this.txtMensaje.Text = "";
        }

        private void cmbUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnEnviarASMX_Click(object sender, EventArgs e)
        {
            this.txtRespuesta.Text = "";
            SchoolManager.WhatsApp.ServiciosWeb.WS_Mensajes.Mensajes objMensaje = new SchoolManager.WhatsApp.ServiciosWeb.WS_Mensajes.Mensajes() { Usuario = this.cmbUsuarios.Text, Password = "1234", CodigoPais = "521", Celular = this.txtCelular.Text, Mensaje = this.txtMensaje.Text, Prioridad = this.cmbTipo.SelectedIndex };
            string mensaje = objMensaje.Mensaje;
            for (int i = 1; i <= numMensajes.Value; i++)
            {
                try
                {
                    objMensaje.Mensaje = i.ToString() + " - " + mensaje;
                    using(SchoolManager.WhatsApp.ServiciosWeb.WS_Mensajes.WS_MensajesSoapClient objWS = new SchoolManager.WhatsApp.ServiciosWeb.WS_Mensajes.WS_MensajesSoapClient())
                    {
                        SchoolManager.WhatsApp.ServiciosWeb.WS_Mensajes.ArrayOfString respuesta = objWS.Enviar(objMensaje);
                        foreach (string res in respuesta)
                        {
                            this.txtRespuesta.AppendText(res + Environment.NewLine);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.txtRespuesta.Text = "False|" + ex.Message;
                }
            }
        }
    }
    public class Mensajes
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string CodigoPais { get; set; }
        public string Celular { get; set; }
        public string Mensaje { get; set; }
        public int Prioridad { get; set; }
    }
}
