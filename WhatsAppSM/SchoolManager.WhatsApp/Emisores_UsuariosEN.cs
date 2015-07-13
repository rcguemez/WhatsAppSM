using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolManager.WhatsApp.Entidades
{
    public class Emisores_UsuariosEN : ICloneable
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
        private string _usuario;
        public string USUARIO
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
        private string _nombrePerfil;
        public string NOMBREPERFIL
        {
            get { return _nombrePerfil; }
            set { _nombrePerfil = value; }
        }
    }
}
