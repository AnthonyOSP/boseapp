using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using boseapp.Models;

namespace boseapp.ViewModel
{
    public class ContactoViewModel
    {
        public Contacto? FormContacto { get; set; }
        public IEnumerable<Contacto>? ListContacto { get; set; }
    }

    public class FormContacto
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
    }
}