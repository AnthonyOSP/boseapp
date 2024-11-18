#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace boseapp.Models
{
    public class UserCliente : IdentityUser
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
    }
}