using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using SchoolManager.WhatsApp.Entidades;

namespace SchoolManager.WhatsApp.AccesoDatos
{
    public class Emisores_UsuariosAD
    {

        public ContextoAD _objContextoAD;
        public int Agregar(Emisores_UsuariosEN pObjEmisores_UsuariosEN)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "INSERT INTO EMISORES_USUARIOS (EMISOR,USUARIO,NOMBREPERFIL) " +
                        "VALUES (@EMISOR,@USUARIO,@NOMBREPERFIL)";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@EMISOR", pObjEmisores_UsuariosEN.EMISOR);
            cmd.Parameters.AddWithValue("@USUARIO", pObjEmisores_UsuariosEN.USUARIO.ToLower());
            cmd.Parameters.AddWithValue("@NOMBREPERFIL", pObjEmisores_UsuariosEN.USUARIO.ToLower());
            try
            {
                if (_objContextoAD.EsTransaccion)
                {
                    cmd.Transaction = _objContextoAD.Transaccion;
                }
                return Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("PK_"))
                {
                    throw new Exception("Ya está asignado el número: " + pObjEmisores_UsuariosEN.EMISOR + " para el usuario: " + pObjEmisores_UsuariosEN.USUARIO);
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public int Editar(Emisores_UsuariosEN pObjEmisores_UsuariosEN)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE EMISORES_USUARIOS SET EMISOR = @EMISOR," +
                                                "NOMBREPERFIL = @NOMBREPERFIL " +
                                                "WHERE USUARIO = @USUARIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@EMISOR", pObjEmisores_UsuariosEN.EMISOR);
            cmd.Parameters.AddWithValue("@USUARIO", pObjEmisores_UsuariosEN.USUARIO.ToLower());
            cmd.Parameters.AddWithValue("@NOMBREPERFIL", pObjEmisores_UsuariosEN.USUARIO.ToLower());
            try
            {
                if (_objContextoAD.EsTransaccion)
                {
                    cmd.Transaction = _objContextoAD.Transaccion;
                }
                return Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("PK_"))
                {
                    throw new Exception("Ya está asignado el número: " + pObjEmisores_UsuariosEN.EMISOR + " para el usuario: " + pObjEmisores_UsuariosEN.USUARIO.ToLower());
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public int Eliminar(string pUsuario, string pEmisor)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "DELETE FROM EMISORES_USUARIOS WHERE USUARIO = @USUARIO AND EMISOR = @EMISOR";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@USUARIO", pUsuario.ToLower());
            cmd.Parameters.AddWithValue("@EMISOR", pEmisor);
            try
            {
                if (_objContextoAD.EsTransaccion)
                {
                    cmd.Transaction = _objContextoAD.Transaccion;
                }
                return Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Emisores_UsuariosEN> ObtenerTodo()
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM EMISORES_USUARIOS ORDER BY USUARIO,EMISOR";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        public List<Emisores_UsuariosEN> ObtenerPorUsuario(string pUsuario)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM EMISORES_USUARIOS WHERE USUARIO = @USUARIO ORDER BY EMISOR";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@USUARIO", pUsuario.ToLower());
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        private List<Emisores_UsuariosEN> CargarDatos(IDataReader pLector)
        {
            List<Emisores_UsuariosEN> listaEmisores_UsuariosEN = new List<Emisores_UsuariosEN>();
            Emisores_UsuariosEN objEmisores_UsuariosEN;
            int colEmisor = pLector.GetOrdinal("EMISOR");
            int colUsuario = pLector.GetOrdinal("USUARIO");
            int colNombrePerfil = pLector.GetOrdinal("NOMBREPERFIL");
            object[] valores = new object[pLector.FieldCount];
            while (pLector.Read())
            {
                objEmisores_UsuariosEN = new Emisores_UsuariosEN();
                pLector.GetValues(valores);
                objEmisores_UsuariosEN.EMISOR = Convert.ToString(valores[colEmisor]);
                objEmisores_UsuariosEN.USUARIO = Convert.ToString(valores[colUsuario]).ToLower();
                objEmisores_UsuariosEN.NOMBREPERFIL = Convert.ToString(valores[colNombrePerfil]);
                listaEmisores_UsuariosEN.Add(objEmisores_UsuariosEN);
            }
            return listaEmisores_UsuariosEN;
        }
    }
}
