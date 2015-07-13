using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using SchoolManager.WhatsApp.Entidades;

namespace SchoolManager.WhatsApp.AccesoDatos
{
    public class UsuariosAD
    {

        public ContextoAD _objContextoAD;
        public int Agregar(UsuariosEN pObjUsuariosEN)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "INSERT INTO USUARIOS (USUARIO,PASS) " +
                        "VALUES (@USUARIO,@PASS)";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@USUARIO", pObjUsuariosEN.USUARIO.ToLower());
            cmd.Parameters.AddWithValue("@PASS", pObjUsuariosEN.PASS);
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
                    throw new Exception("Ya existe el usuario: " + pObjUsuariosEN.USUARIO);
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public int Editar(UsuariosEN pObjUsuariosEN)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE USUARIOS SET PASS = @PASS " +
                                                "WHERE USUARIO = @USUARIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@PASS", pObjUsuariosEN.PASS);
            cmd.Parameters.AddWithValue("@USUARIO", pObjUsuariosEN.USUARIO.ToLower());
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
        public int Eliminar(string pUsuario)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "DELETE FROM USUARIOS WHERE USUARIO = @USUARIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@USUARIO", pUsuario.ToLower());
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
        public List<UsuariosEN> ObtenerTodo()
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM USUARIOS ORDER BY USUARIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        public List<UsuariosEN> ObtenerPorUsuario(string pUsuario)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM USUARIOS WHERE USUARIO = @USUARIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@USUARIO", pUsuario.ToLower());
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        public bool IniciarSesion(string pUsuario, string pPass)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT COUNT(USUARIO) FROM USUARIOS WHERE USUARIO = @USUARIO AND PASS = @PASS";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@USUARIO", pUsuario.ToLower());
            cmd.Parameters.AddWithValue("@PASS", pPass);
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
        private List<UsuariosEN> CargarDatos(IDataReader pLector)
        {
            List<UsuariosEN> listaUsuariosEN = new List<UsuariosEN>();
            UsuariosEN objUsuariosEN;
            int colUsuario = pLector.GetOrdinal("USUARIO");
            int colPass = pLector.GetOrdinal("PASS");
            object[] valores = new object[pLector.FieldCount];
            while (pLector.Read())
            {
                objUsuariosEN = new UsuariosEN();
                pLector.GetValues(valores);
                objUsuariosEN.USUARIO = Convert.ToString(valores[colUsuario]).ToLower();
                objUsuariosEN.PASS = Convert.ToString(valores[colPass]);
                listaUsuariosEN.Add(objUsuariosEN);
            }
            return listaUsuariosEN;
        }
    }
}
