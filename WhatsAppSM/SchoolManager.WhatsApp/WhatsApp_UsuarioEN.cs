using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolManager.WhatsApp.Entidades
{
    public class WhatsApp_UsuarioEN : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        private string _folio;
        public string FOLIO
        {
            get { return _folio; }
            set { _folio = value; }
        }
        private string _emisor;
        public string EMISOR
        {
            get { return _emisor; }
            set { _emisor = value; }
        }
        private string _receptor;
        public string RECEPTOR
        {
            get { return _receptor; }
            set { _receptor = value; }
        }
        private int _recurso;
        public int RECURSO
        {
            get { return _recurso; }
            set { _recurso = value; }
        }
        private string _mensaje;
        public string MENSAJE
        {
            get { return _mensaje; }
            set { _mensaje = value; }
        }
        private int _prioridad;
        public int PRIORIDAD
        {
            get { return _prioridad; }
            set { _prioridad = value; }
        }
        private int _status;
        public int STATUS
        {
            get { return _status; }
            set { _status = value; }
        }
        private DateTime _fechaHoraRegistrado;
        public DateTime FECHAHORAREGISTRADO
        {
            get { return _fechaHoraRegistrado; }
            set { _fechaHoraRegistrado = value; }
        }
        private DateTime? _fechaHoraEnviado;
        public DateTime? FECHAHORAENVIADO
        {
            get { return _fechaHoraEnviado; }
            set { _fechaHoraEnviado = value; }
        }
        private DateTime? _fechaHoraEntregado;
        public DateTime? FECHAHORAENTREGADO
        {
            get { return _fechaHoraEntregado; }
            set { _fechaHoraEntregado = value; }
        }
        private DateTime? _fechaHoraLeido;
        public DateTime? FECHAHORALEIDO
        {
            get { return _fechaHoraLeido; }
            set { _fechaHoraLeido = value; }
        }
        private string _observaciones;
        public string OBSERVACIONES
        {
            get { return _observaciones; }
            set { _observaciones = value; }
        }
    }
}
