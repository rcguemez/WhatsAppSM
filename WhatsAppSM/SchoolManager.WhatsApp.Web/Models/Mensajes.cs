using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolManager.WhatsApp.Web.Models
{
    public class Mensajes
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string NombrePerfil { get; set; }
        public string CodigoPais { get; set; }
        public string Celular { get; set; }
        public string Mensaje { get; set; }
        public int Prioridad { get; set; }
    }
}