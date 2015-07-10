using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SchoolManager.WhatsApp.Web.Models;

namespace SchoolManager.WhatsApp.Web.Controllers
{
    public class MensajesController : ApiController
    {
        // POST: api/Mensajes
        public string[] Post(Mensajes pMensaje)
        {
            try
            {
                if (pMensaje.NombrePerfil.Trim() == "")
                {
                    pMensaje.NombrePerfil = pMensaje.Usuario;
                }
                if (pMensaje.CodigoPais.Trim() == "")
                {
                    pMensaje.CodigoPais = "521";
                }
                pMensaje.Mensaje = pMensaje.Mensaje.Trim();
                if (pMensaje.Mensaje.Trim() == "")
                {
                    return new string[] { "False|No se especificó el mensaje a enviar" };
                }
                if (pMensaje.Prioridad != 1)
                {
                    pMensaje.Prioridad = 0;
                }
                if(SchoolManager.WhatsApp.LogicaNegocios.UsuariosLN.IniciarSesion(pMensaje.Usuario, pMensaje.Password))
                {
                    return SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.AgregarEnCola(pMensaje.Usuario, pMensaje.NombrePerfil, pMensaje.CodigoPais, pMensaje.Celular, 0, pMensaje.Mensaje, pMensaje.Prioridad);
                }
                else
                {
                    return new string[] { "False|El usuario y/o password son incorrectos" };
                }
            }
            catch(Exception ex)
            {
                return new string[] { "False|" + ex.Message };
            }
        }
    }
}
