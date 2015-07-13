using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using SchoolManager.WhatsApp.Entidades;

namespace SchoolManager.WhatsApp.AccesoDatos
{
    public class WhatsApp_UsuarioAD
    {

        public ContextoAD _objContextoAD;
        private string ObtenerFolio()
        {
            string folio = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            System.Threading.Thread.Sleep(10);
            folio += DateTime.Now.ToString("fff");
            return folio;
        }
        public string Agregar(string pUsuario, WhatsApp_UsuarioEN pObjWhatsApp_UsuarioEN)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
        REGRESAR:
            pObjWhatsApp_UsuarioEN.FOLIO = ObtenerFolio();
        string sql = "INSERT INTO WHATSAPP_" + pUsuario.ToLower() + " (FOLIO,EMISOR,RECEPTOR,RECURSO,MENSAJE,PRIORIDAD,STATUS,OBSERVACIONES) " +
            "VALUES (@FOLIO," + (pObjWhatsApp_UsuarioEN.EMISOR != "" ? "'" + pObjWhatsApp_UsuarioEN.EMISOR + "'" : "NULL") + ",@RECEPTOR,@RECURSO,@MENSAJE,@PRIORIDAD,@STATUS,@OBSERVACIONES) RETURNING FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@FOLIO", pObjWhatsApp_UsuarioEN.FOLIO);
            cmd.Parameters.AddWithValue("@RECEPTOR", pObjWhatsApp_UsuarioEN.RECEPTOR);
            cmd.Parameters.AddWithValue("@RECURSO", pObjWhatsApp_UsuarioEN.RECURSO);
            cmd.Parameters.AddWithValue("@MENSAJE", pObjWhatsApp_UsuarioEN.MENSAJE);
            cmd.Parameters.AddWithValue("@PRIORIDAD", pObjWhatsApp_UsuarioEN.PRIORIDAD);
            cmd.Parameters.AddWithValue("@STATUS", pObjWhatsApp_UsuarioEN.STATUS);
            cmd.Parameters.AddWithValue("@OBSERVACIONES", pObjWhatsApp_UsuarioEN.OBSERVACIONES);
            try
            {
                if (_objContextoAD.EsTransaccion)
                {
                    cmd.Transaction = _objContextoAD.Transaccion;
                }
                return Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("PK_"))
                {
                    goto REGRESAR;
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        public int PonerStatusErrorEnvio(string pUsuario, string pEmisor, string pFolio, string pObservaciones)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE WHATSAPP_" + pUsuario.ToLower() + " SET STATUS = @STATUS," +
                                                "EMISOR = @EMISOR," +
                                                "OBSERVACIONES = @OBSERVACIONES " +
                                                "WHERE FOLIO = @FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@STATUS", -1);
            cmd.Parameters.AddWithValue("@EMISOR", pEmisor);
            cmd.Parameters.AddWithValue("@OBSERVACIONES", pObservaciones);
            cmd.Parameters.AddWithValue("@FOLIO", pFolio);
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
        public int PonerStatusListoParaEnviar(string pUsuario, string pEmisor, string pFolio, string pObservaciones)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE WHATSAPP_" + pUsuario.ToLower() + " SET STATUS = @STATUS," +
                                                "EMISOR = @EMISOR," +
                                                "OBSERVACIONES = @OBSERVACIONES " +
                                                "WHERE FOLIO = @FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@STATUS", 1);
            cmd.Parameters.AddWithValue("@EMISOR", pEmisor);
            cmd.Parameters.AddWithValue("@OBSERVACIONES", pObservaciones);
            cmd.Parameters.AddWithValue("@FOLIO", pFolio);
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
        public int PonerStatusEnviando(string pUsuario, string pFolio, string pObservaciones)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE WHATSAPP_" + pUsuario.ToLower() + " SET STATUS = @STATUS," +
                                                "OBSERVACIONES = @OBSERVACIONES " +
                                                "WHERE FOLIO = @FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@STATUS", 2);
            cmd.Parameters.AddWithValue("@OBSERVACIONES", pObservaciones);
            cmd.Parameters.AddWithValue("@FOLIO", pFolio);
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
        public int PonerStatusEnviado(string pUsuario, string pFolio, string pObservaciones)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE WHATSAPP_" + pUsuario.ToLower() + " SET STATUS = @STATUS," +
                                                "OBSERVACIONES = @OBSERVACIONES," +
                                                "FECHAHORAENVIADO = CURRENT_TIMESTAMP " +
                                                "WHERE FOLIO = @FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@STATUS", 3);
            cmd.Parameters.AddWithValue("@OBSERVACIONES", pObservaciones);
            cmd.Parameters.AddWithValue("@FOLIO", pFolio);
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
        public int PonerStatusEntregado(string pUsuario, string pFolio, string pObservaciones)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE WHATSAPP_" + pUsuario.ToLower() + " SET STATUS = @STATUS," +
                                                "OBSERVACIONES = @OBSERVACIONES," +
                                                "FECHAHORAENTREGADO = CURRENT_TIMESTAMP " +
                                                "WHERE FOLIO = @FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@STATUS", 4);
            cmd.Parameters.AddWithValue("@OBSERVACIONES", pObservaciones);
            cmd.Parameters.AddWithValue("@FOLIO", pFolio);
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
        public int PonerStatusLeido(string pUsuario, string pFolio, string pObservaciones)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "UPDATE WHATSAPP_" + pUsuario.ToLower() + " SET STATUS = @STATUS," +
                                                "OBSERVACIONES = @OBSERVACIONES," +
                                                "FECHAHORALEIDO = CURRENT_TIMESTAMP " +
                                                "WHERE FOLIO = @FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@STATUS", 5);
            cmd.Parameters.AddWithValue("@OBSERVACIONES", pObservaciones);
            cmd.Parameters.AddWithValue("@FOLIO", pFolio);
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
        public int Eliminar(string pUsuario, string pFolio)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "DELETE FROM WHATSAPP_" + pUsuario.ToLower() + " WHERE FOLIO = @FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@FOLIO", pFolio);
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
        public List<WhatsApp_UsuarioEN> ObtenerTodo(string pUsuario)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM WHATSAPP_" + pUsuario.ToLower() + " ORDER BY FECHAHORAREGISTRADO,FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        public List<WhatsApp_UsuarioEN> ObtenerPorEmisor(string pUsuario, string pEmisor)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM WHATSAPP_" + pUsuario.ToLower() + " WHERE EMISOR = @EMISOR ORDER BY FECHAHORAREGISTRADO,FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@EMISOR", pEmisor);
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        public List<WhatsApp_UsuarioEN> ObtenerPorFolio(string pUsuario, string pFolio)
        {
            if (_objContextoAD.Conexion.State == ConnectionState.Closed)
            {
                _objContextoAD.Conexion.Open();
            }
            string sql = "SELECT * FROM WHATSAPP_" + pUsuario.ToLower() + " WHERE FOLIO = @FOLIO";
            FbCommand cmd = new FbCommand(sql, _objContextoAD.Conexion);
            cmd.Parameters.AddWithValue("@FOLIO", pFolio);
            if (_objContextoAD.EsTransaccion)
            {
                cmd.Transaction = _objContextoAD.Transaccion;
            }
            return CargarDatos(cmd.ExecuteReader());
        }
        private List<WhatsApp_UsuarioEN> CargarDatos(IDataReader pLector)
        {
            List<WhatsApp_UsuarioEN> listaWhatsApp_UsuarioEN = new List<WhatsApp_UsuarioEN>();
            WhatsApp_UsuarioEN objWhatsApp_UsuarioEN;
            int colFolio = pLector.GetOrdinal("FOLIO");
            int colEmisor = pLector.GetOrdinal("EMISOR");
            int colReceptor = pLector.GetOrdinal("RECEPTOR");
            int colRecurso = pLector.GetOrdinal("RECURSO");
            int colMensaje = pLector.GetOrdinal("MENSAJE");
            int colPrioridad = pLector.GetOrdinal("PRIORIDAD");
            int colStatus = pLector.GetOrdinal("STATUS");
            int colFechaHoraRegistrado = pLector.GetOrdinal("FECHAHORAREGISTRADO");
            int colFechaHoraEnviado = pLector.GetOrdinal("FECHAHORAENVIADO");
            int colFechaHoraEntregado = pLector.GetOrdinal("FECHAHORAENTREGADO");
            int colFechaHoraLeido = pLector.GetOrdinal("FECHAHORALEIDO");
            object[] valores = new object[pLector.FieldCount];
            while (pLector.Read())
            {
                objWhatsApp_UsuarioEN = new WhatsApp_UsuarioEN();
                pLector.GetValues(valores);
                objWhatsApp_UsuarioEN.FOLIO = Convert.ToString(valores[colFolio]);
                objWhatsApp_UsuarioEN.EMISOR = Convert.ToString(valores[colEmisor]);
                objWhatsApp_UsuarioEN.RECEPTOR = Convert.ToString(valores[colRecurso]);
                objWhatsApp_UsuarioEN.RECURSO = Convert.ToInt16(valores[colReceptor]);
                objWhatsApp_UsuarioEN.MENSAJE = Convert.ToString(valores[colMensaje]);
                objWhatsApp_UsuarioEN.PRIORIDAD = Convert.ToInt16(valores[colPrioridad]);
                objWhatsApp_UsuarioEN.STATUS = Convert.ToInt16(valores[colStatus]);
                objWhatsApp_UsuarioEN.FECHAHORAREGISTRADO = Convert.ToDateTime(valores[colFechaHoraRegistrado]);
                if (Convert.IsDBNull(valores[colFechaHoraEnviado]) == false)
                {
                    objWhatsApp_UsuarioEN.FECHAHORAENVIADO = Convert.ToDateTime(valores[colFechaHoraEnviado]);
                }
                if (Convert.IsDBNull(valores[colFechaHoraEntregado]) == false)
                {
                    objWhatsApp_UsuarioEN.FECHAHORAENTREGADO = Convert.ToDateTime(valores[colFechaHoraEntregado]);
                }
                if (Convert.IsDBNull(valores[colFechaHoraLeido]) == false)
                {
                    objWhatsApp_UsuarioEN.FECHAHORALEIDO = Convert.ToDateTime(valores[colFechaHoraLeido]);
                }
                listaWhatsApp_UsuarioEN.Add(objWhatsApp_UsuarioEN);
            }
            return listaWhatsApp_UsuarioEN;
        }
    }
}
