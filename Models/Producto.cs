using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace boseapp.Models
{
    [Table("t_producto")]
    public class Producto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public decimal? Precio { get; set; }
        public string? Descripcion { get; set; }
        public int Calificacion { get; set; }
        public int Stock { get; set; }
        public string? SKU { get; set; }
        public string? ImagenURL { get; set; }
        public DateTime FechaDeCreacion { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;

        // Foreign key for Categoria
        public Categoria? Categoria { get; set; }
    }
}