using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SchoolManager.WhatsApp.Entidades;
using SchoolManager.WhatsApp.AccesoDatos;

namespace SchoolManager.WhatsApp.LogicaNegocios
{
    public class UsuariosLN
    {
        public static int Eliminar(string pUsuario)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Usuarios.Eliminar(pUsuario);
            }
        }
        public static int Agregar(UsuariosEN pObjUsuariosEN)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Usuarios.Agregar(pObjUsuariosEN);
            }
        }
        public static int Editar(UsuariosEN pObjUsuariosEN)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Usuarios.Editar(pObjUsuariosEN);
            }
        }
        public static List<UsuariosEN> ObtenerTodo()
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Usuarios.ObtenerTodo();
            }
        }
        public static List<UsuariosEN> ObtenerPorUsuario(string pUsuario)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Usuarios.ObtenerPorUsuario(pUsuario);
            }
        }
        public static bool IniciarSesion(string pUsuario, string pPass)
        {
            using (ContextoAD oContextoAD = new ContextoAD())
            {
                return oContextoAD.Usuarios.IniciarSesion(pUsuario, pPass);
            }
        }
    }
}
