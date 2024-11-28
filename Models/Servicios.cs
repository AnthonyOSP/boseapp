
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;


namespace boseapp.Models
{
    [Table("t_servicios")]
    public class Servicios
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string? Nombre { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string? SerialNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string? Descripcion { get; set; }

        [Required]
        public DateTime FechaDeCreacion { get; set; } = DateTime.UtcNow;
        public decimal Costo { get; set; }
        [Required]
        public bool Estado { get; set; } = false;

        // Foreign key for UserCliente
        [ForeignKey("UserCliente")]
        public string? UserClienteId { get; set; }
        public UserCliente? UserCliente { get; set; }
    }
}