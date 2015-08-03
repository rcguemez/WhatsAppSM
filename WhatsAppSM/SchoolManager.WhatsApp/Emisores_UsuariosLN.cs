using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchoolManager.WhatsApp.Entidades;
using SchoolManager.WhatsApp.AccesoDatos;

namespace SchoolManager.WhatsApp.LogicaNegocios
{
    public class Emisores_UsuariosLN
    {
        public static int Eliminar(string pUsuario, string pEmisor)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores_Usuarios.Eliminar(pUsuario, pEmisor);
            }
        }
        public static int Agregar(Emisores_UsuariosEN pObjEmisores_UsuariosEN)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores_Usuarios.Agregar(pObjEmisores_UsuariosEN);
            }
        }
        public static int Editar(Emisores_UsuariosEN pObjEmisores_UsuariosEN)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores_Usuarios.Editar(pObjEmisores_UsuariosEN);
            }
        }
        public static List<Emisores_UsuariosEN> ObtenerTodo()
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores_Usuarios.ObtenerTodo();
            }
        }
        public static List<Emisores_UsuariosEN> ObtenerPorUsuario(string pUsuario)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores_Usuarios.ObtenerPorUsuario(pUsuario);
            }
        }
        public static System.Data.DataTable DtEmisorActivoPorUsuario(string pUsuario, int pPrioridad)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores_Usuarios.DtEmisorActivoPorUsuario(pUsuario, pPrioridad);
            }
        }
        public static System.Data.DataTable DtActivos()
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Emisores_Usuarios.DtActivos();
            }
        }
    }
}
