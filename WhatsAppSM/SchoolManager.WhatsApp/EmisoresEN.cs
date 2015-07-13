using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolManager.WhatsApp.Entidades
{
    public class EmisoresEN : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        private string _emisor;
        public string EMISOR
        {
            get { return _emisor; }
            set { _emisor = value; }
        }
        private string _apiKey;
        public string APIKEY
        {
            get { return _apiKey; }
            set { _apiKey = value; }
        }
        private int _tipo;
        public int TIPO
        {
            get { return _tipo; }
            set { _tipo = value; }
        }
        private int _prioridad;
        public int PRIORIDAD
        {
            get { return _prioridad; }
            set { _prioridad = value; }
        }
        private bool _activo;
        public bool ACTIVO
        {
            get { return _activo; }
            set { _activo = value; }
        }
    }
}
