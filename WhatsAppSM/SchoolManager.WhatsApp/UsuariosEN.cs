using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchoolManager.WhatsApp.Entidades
{
    public class UsuariosEN : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        private string _usuario;
        public string USUARIO
        {
            get { return _usuario; }
            set { _usuario = value; }
        }
        private string _pass;
        public string PASS
        {
            get { return _pass; }
            set { _pass = value; }
        }
    }
}
