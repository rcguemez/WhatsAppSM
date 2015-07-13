using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchoolManager.WhatsApp.Entidades;
using SchoolManager.WhatsApp.AccesoDatos;

namespace SchoolManager.WhatsApp.LogicaNegocios
{
    public class WhatsApp_UsuarioLN
    {
        public static int Eliminar(string pUsuario, string pFolio)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.Eliminar(pUsuario, pFolio);
            }
        }
        public static string Agregar(string pUsuario, WhatsApp_UsuarioEN pObjWhatsApp_UsuarioEN)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.Agregar(pUsuario, pObjWhatsApp_UsuarioEN);
            }
        }
        public static int PonerStatusErrorEnvio(string pUsuario, string pEmisor, string pFolio, string pObservaciones)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.PonerStatusErrorEnvio(pUsuario, pEmisor, pFolio, pObservaciones);
            }
        }
        public static int PonerStatusListoParaEnviar(string pUsuario, string pEmisor, string pFolio, string pObservaciones)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.PonerStatusListoParaEnviar(pUsuario, pEmisor, pFolio, pObservaciones);
            }
        }
        public static int PonerStatusEnviando(string pUsuario, string pEmisor, string pFolio, string pObservaciones)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.PonerStatusEnviando(pUsuario, pFolio, pObservaciones);
            }
        }
        public static int PonerStatusEnviado(string pUsuario, string pFolio, string pObservaciones)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.PonerStatusEnviado(pUsuario, pFolio, pObservaciones);
            }
        }
        public static int PonerStatusEntregado(string pUsuario, string pFolio, string pObservaciones)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.PonerStatusEntregado(pUsuario, pFolio, pObservaciones);
            }
        }
        public static int PonerStatusLeido(string pUsuario, string pFolio, string pObservaciones)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.PonerStatusLeido(pUsuario, pFolio, pObservaciones);
            }
        }
        public static List<WhatsApp_UsuarioEN> ObtenerTodo(string pUsuario)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.ObtenerTodo(pUsuario);
            }
        }
        public static List<WhatsApp_UsuarioEN> ObtenerPorUsuario(string pUsuario, string pEmisor)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.ObtenerPorEmisor(pUsuario, pEmisor);
            }
        }
        public static List<WhatsApp_UsuarioEN> ObtenerPorFolio(string pUsuario, string pFolio)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.WhatsApp_Usuario.ObtenerPorFolio(pUsuario, pFolio);
            }
        }
        public static string[] AgregarEnCola(string pUsuario, string pCodigoPais, string pCelulares, int pRecurso, string pMensaje, int pPrioridad)
        {
            System.Messaging.MessageQueue mq;
            if (System.Messaging.MessageQueue.Exists(@".\Private$\whatsapp_" + pUsuario))
            {
                mq = new System.Messaging.MessageQueue(@".\Private$\whatsapp_" + pUsuario);
            }
            else
            {
                mq = System.Messaging.MessageQueue.Create(@".\Private$\whatsapp_" + pUsuario);
            }
            System.Messaging.Message msj = new System.Messaging.Message();
            if (pPrioridad == 1)
            {
                msj.Priority = System.Messaging.MessagePriority.High;
            }
            else
            {
                pPrioridad = 0;
                msj.Priority = System.Messaging.MessagePriority.Normal;
            }
            string[] strArr = null;
            char[] splitchar = { ';' };
            strArr = pCelulares.Split(splitchar);
            string[] respuesta = new string[strArr.Length];
            for (int i = 0; i <= strArr.Length - 1; i++)
            {
                try
                {
                    strArr[i] = strArr[i].Trim();
                    if (strArr[i].Length == 10)
                    {
                        SchoolManager.WhatsApp.Entidades.WhatsApp_UsuarioEN objMensaje = new SchoolManager.WhatsApp.Entidades.WhatsApp_UsuarioEN();
                        objMensaje.FOLIO = "";
                        objMensaje.EMISOR = "";
                        objMensaje.RECEPTOR = pCodigoPais + strArr[i];
                        objMensaje.RECURSO = pRecurso;
                        objMensaje.MENSAJE = pMensaje;
                        objMensaje.PRIORIDAD = pPrioridad;
                        objMensaje.STATUS = 0;
                        objMensaje.FOLIO = SchoolManager.WhatsApp.LogicaNegocios.WhatsApp_UsuarioLN.Agregar(pUsuario, objMensaje);
                        msj.Label = string.Format("{0}|{1}|{2}", objMensaje.FOLIO, objMensaje.RECEPTOR, objMensaje.RECURSO);
                        msj.Body = objMensaje.MENSAJE;
                        mq.Send(msj);
                        respuesta[i] = "True|" + objMensaje.FOLIO;
                    }
                    else
                    {
                        respuesta[i] = "False|El celular debe ser a 10 dígitos";
                    }
                }
                catch(Exception ex)
                {
                    respuesta[i] = "False|" + ex.Message;
                }
            }
            return respuesta;
        }
    }
}