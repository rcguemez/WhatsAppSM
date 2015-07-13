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
    public class Mensajes
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string CodigoPais { get; set; }
        public string Celular { get; set; }
        public string Mensaje { get; set; }
        public int Prioridad { get; set; }
    }
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
            Mensajes mensaje = new Mensajes() { Usuario = this.cmbUsuarios.Text, Password = "1234", CodigoPais = "521", Celular = this.txtCelular.Text, Mensaje = this.txtMensaje.Text, Prioridad = this.cmbTipo.SelectedIndex };
            RunAsync(mensaje).Wait(100);
        }

        async Task RunAsync(Mensajes pMensaje)
        {
            for (int i = 1; i <= 100;i++ )
            {
                try
                {
                    pMensaje.Mensaje = "test" + i.ToString();
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:3395/");
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
    }
}
