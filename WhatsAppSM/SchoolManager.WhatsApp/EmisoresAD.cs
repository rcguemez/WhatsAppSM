using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using SchoolManager.WhatsApp.Entidades;

namespace SchoolManager.WhatsApp.AccesoDatos
{
    public class EmisoresAD
    {

        public ContextoAD _objContextoAD;
        public int Agregar(EmisoresEN pObjEmisoresEN)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "INSERT INTO EMISORES (EMISOR,APIKEY,TIPO,PRIORIDAD,ACTIVO,NOMBREPERFIL) " +
                        "VALUES (@EMISOR,@APIKEY,@TIPO,@PRIORIDAD,@ACTIVO,@NOMBREPERFIL)";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@EMISOR", pObjEmisoresEN.EMISOR);
            cmd.Parameters.AddWithValue("@APIKEY", pObjEmisoresEN.APIKEY);
            cmd.Parameters.AddWithValue("@TIPO", pObjEmisoresEN.TIPO);
            cmd.Parameters.AddWithValue("@PRIORIDAD", pObjEmisoresEN.PRIORIDAD);
            cmd.Parameters.AddWithValue("@ACTIVO", (pObjEmisoresEN.ACTIVO ? 1 : 0));
            cmd.Parameters.AddWithValue("@NOMBREPERFIL", pObjEmisoresEN.NOMBREPERFIL);
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
                    throw new Exception("Ya existe el número: " + pObjEmisoresEN.EMISOR);
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public int Editar(EmisoresEN pObjEmisoresEN)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE EMISORES SET APIKEY = @APIKEY," +
                                                "TIPO = @TIPO," +
                                                "PRIORIDAD = @PRIORIDAD," +
                                                "ACTIVO = @ACTIVO," +
                                                "NOMBREPERFIL = @NOMBREPERFIL " +
                                                "WHERE EMISOR = @EMISOR";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@APIKEY", pObjEmisoresEN.APIKEY);
            cmd.Parameters.AddWithValue("@TIPO", pObjEmisoresEN.TIPO);
            cmd.Parameters.AddWithValue("@PRIORIDAD", pObjEmisoresEN.PRIORIDAD);
            cmd.Parameters.AddWithValue("@ACTIVO", (pObjEmisoresEN.ACTIVO ? 1 : 0));
            cmd.Parameters.AddWithValue("@NOMBREPERFIL", pObjEmisoresEN.NOMBREPERFIL);
            cmd.Parameters.AddWithValue("@EMISOR", pObjEmisoresEN.EMISOR);
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
        public int Eliminar(string pEmisor)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "DELETE FROM EMISORES WHERE EMISOR = @EMISOR";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
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
        public List<EmisoresEN> ObtenerTodo()
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM EMISORES ORDER BY EMISOR";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        public List<EmisoresEN> ObtenerPorEmisor(string pEmisor)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM EMISORES WHERE EMISOR = @EMISOR";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@EMISOR", pEmisor);
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        private List<EmisoresEN> CargarDatos(IDataReader pLector)
        {
            List<EmisoresEN> listaEmisoresEN = new List<EmisoresEN>();
            EmisoresEN objEmisoresEN;
            int colEmisor = pLector.GetOrdinal("EMISOR");
            int colApiKey = pLector.GetOrdinal("APIKEY");
            int colTipo = pLector.GetOrdinal("TIPO");
            int colPrioridad = pLector.GetOrdinal("PRIORIDAD");
            int colActivo = pLector.GetOrdinal("ACTIVO");
            int colNombreEmisor = pLector.GetOrdinal("NOMBREPERFIL");
            int colImagenPerfil = pLector.GetOrdinal("IMAGENPERFIL");
            int colEstado = pLector.GetOrdinal("ESTADO");
            object[] valores = new object[pLector.FieldCount];
            while (pLector.Read())
            {
                objEmisoresEN = new EmisoresEN();
                pLector.GetValues(valores);
                objEmisoresEN.EMISOR = Convert.ToString(valores[colEmisor]);
                objEmisoresEN.APIKEY = Convert.ToString(valores[colApiKey]);
                objEmisoresEN.TIPO = Convert.ToInt16(valores[colTipo]);
                objEmisoresEN.PRIORIDAD = Convert.ToInt16(valores[colPrioridad]);
                objEmisoresEN.ACTIVO = Convert.ToBoolean(valores[colActivo]);
                objEmisoresEN.NOMBREPERFIL = Convert.ToString(valores[colNombreEmisor]);
                objEmisoresEN.IMAGENPERFIL = Convert.ToString(valores[colImagenPerfil]);
                objEmisoresEN.ESTADO = Convert.ToString(valores[colEstado]);
                listaEmisoresEN.Add(objEmisoresEN);
            }
            return listaEmisoresEN;
        }
    }
}
