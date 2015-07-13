using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SchoolManager.WhatsApp.AccesoDatos;

namespace SchoolManager.WhatsApp
{
    public class ContextoAD : IDisposable
    {

        public ContextoAD()
        {
            _esTransaccion = false;
            _conexion = new FbConnection(CadenaConexionBD());
            _esTransaccion = false;
        }

        public static string CadenaConexionBD()
        {
            string ruta = ConfigurationManager.AppSettings.Get("CADENA_CONEXION_BD");
            string servidor = ConfigurationManager.AppSettings.Get("SERVIDOR");
            string puerto = ConfigurationManager.AppSettings.Get("PUERTO");
            string pass = ConfigurationManager.AppSettings.Get("PASSWORD_BD");
            string cadenaConexion = string.Format("character set=NONE;data source={0};initial catalog={1};user id=SYSDBA;password={2};port={3};Pooling=false;", servidor, ruta, pass, puerto);
            return cadenaConexion;
        }

        private FbConnection _conexion;
        public FbConnection Conexion
        {
            get { return _conexion; }
            set { _conexion = value; }
        }
        private FbTransaction _transaccion;
        public FbTransaction Transaccion
        {
            get { return _transaccion; }
            set { _transaccion = value; }
        }
        private bool _esTransaccion;
        public bool EsTransaccion
        {
            get { return _esTransaccion; }
        }
        public void IniciarTransaccion()
        {
            try
            {
                if (_esTransaccion == false)
                {
                    if (_conexion.State == ConnectionState.Closed)
                    {
                        _conexion.Open();
                    }
                    _transaccion = _conexion.BeginTransaction();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _esTransaccion = true;
            }
        }
        public bool ConfirmarTransaccion()
        {
            bool functionReturnValue = false;
            try
            {
                if (_esTransaccion)
                {
                    _transaccion.Commit();
                    functionReturnValue = true;
                }
            }
            catch (Exception ex)
            {
                _transaccion.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                _esTransaccion = false;
                _transaccion = null;
            }
            return functionReturnValue;
        }
        public bool CancelarTransaccion()
        {
            bool functionReturnValue = false;
            try
            {
                if (_esTransaccion)
                {
                    _transaccion.Rollback();
                    functionReturnValue = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _esTransaccion = false;
                _transaccion = null;
            }
            return functionReturnValue;
        }

        #region "Tablas"
        private EmisoresAD _emisores;
        public EmisoresAD Emisores
        {
            get
            {
                if (_emisores == null)
                {
                    _emisores = new EmisoresAD();
                    _emisores._objContextoAD = this;
                }
                return _emisores;
            }
        }
        private UsuariosAD _usuarios;
        public UsuariosAD Usuarios
        {
            get
            {
                if (_usuarios == null)
                {
                    _usuarios = new UsuariosAD();
                    _usuarios._objContextoAD = this;
                }
                return _usuarios;
            }
        }
        private Emisores_UsuariosAD _emisores_usuarios;
        public Emisores_UsuariosAD Emisores_Usuarios
        {
            get
            {
                if (_emisores_usuarios == null)
                {
                    _emisores_usuarios = new Emisores_UsuariosAD();
                    _emisores_usuarios._objContextoAD = this;
                }
                return _emisores_usuarios;
            }
        }
        private WhatsApp_UsuarioAD _whatsapp_usuario;
        public WhatsApp_UsuarioAD WhatsApp_Usuario
        {
            get
            {
                if (_whatsapp_usuario == null)
                {
                    _whatsapp_usuario = new WhatsApp_UsuarioAD();
                    _whatsapp_usuario._objContextoAD = this;
                }
                return _whatsapp_usuario;
            }
        }
        #endregion

        #region "IDisposable Support"
        // Para detectar llamadas redundantes
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool pDisposing)
        {
            if (!this.disposedValue)
            {
                if (pDisposing)
                {
                    // TODO: eliminar estado administrado (objetos administrados).
                }
                // TODO: liberar recursos no administrados (objetos no administrados) e invalidar Finalize() below.
                // TODO: Establecer campos grandes como Null.
                _conexion.Dispose();
                //ClearMemory();
            }
            this.disposedValue = true;
        }

        // TODO: invalidar Finalize() sólo si la instrucción Dispose(ByVal disposing As Boolean) anterior tiene código para liberar recursos no administrados.
        //Protected Overrides Sub Finalize()
        //    ' No cambie este código. Ponga el código de limpieza en la instrucción Dispose(ByVal disposing As Boolean) anterior.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

        // Visual Basic agregó este código para implementar correctamente el modelo descartable.
        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en Dispose(disposing As Boolean).
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ////Declaración de la API
        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        //private static extern bool SetProcessWorkingSetSize(IntPtr pProcHandle, Int32 pMin, Int32 pMax);

        ////Funcion de liberacion de memoria

        //public void ClearMemory()
        //{
        //    try
        //    {
        //        Process Mem = Process.GetCurrentProcess();
        //        SetProcessWorkingSetSize(Mem.Handle, -1, -1);
        //    }
        //    catch (Exception)
        //    {
        //        //Control de errores
        //    }

        //}
        #endregion

    }
}
