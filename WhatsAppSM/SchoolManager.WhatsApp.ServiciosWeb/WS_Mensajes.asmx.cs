using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SchoolManager.WhatsApp.ServiciosWeb
{
    /// <summary>
    /// Descripción breve de Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WS_Mensajes : System.Web.Services.WebService
    {
        [WebMethod]
        public string[] Enviar(Mensajes pMensaje)
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
                if (SchoolManager.WhatsApp.LogicaNegocios.UsuariosLN.IniciarSesion(pMensaje.Usuario, pMensaje.Password))
                {
                    return SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.AgregarEnCola(pMensaje.Usuario, pMensaje.NombrePerfil, pMensaje.CodigoPais, pMensaje.Celular, 0, pMensaje.Mensaje, pMensaje.Prioridad);
                }
                else
                {
                    return new string[] { "False|El usuario y/o password son incorrectos" };
                }
            }
            catch (Exception ex)
            {
                return new string[] { "False|" + ex.Message };
            }
        }
    }

    public class Mensajes
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string NombrePerfil { get; set; }
        public string CodigoPais { get; set; }
        public string Celular { get; set; }
        public string Mensaje { get; set; }
        public int Prioridad { get; set; }
    }
}