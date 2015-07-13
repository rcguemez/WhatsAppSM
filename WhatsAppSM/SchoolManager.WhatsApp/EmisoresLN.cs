using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchoolManager.WhatsApp.Entidades;
using SchoolManager.WhatsApp.AccesoDatos;

namespace SchoolManager.WhatsApp.LogicaNegocios
{
    public class EmisoresLN
    {
        public static int Eliminar(string pEmisor)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores.Eliminar(pEmisor);
            }
        }
        public static int Agregar(EmisoresEN pObjEmisoresEN)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores.Agregar(pObjEmisoresEN);
            }
        }
        public static int Editar(EmisoresEN pObjEmisoresEN)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores.Editar(pObjEmisoresEN);
            }
        }
        public static List<EmisoresEN> ObtenerTodo()
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores.ObtenerTodo();
            }
        }
        public static List<EmisoresEN> ObtenerPorEmisor(string pEmisor)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores.ObtenerPorEmisor(pEmisor);
            }
        }
    }
}
