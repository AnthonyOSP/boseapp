using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace boseapp.Models
{
    [Table("t_categoria")]
    public class Categoria
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Nombre { get; set; }

        // Navigation property
        [JsonIgnore]
        public ICollection<Producto>? Productos { get; set; }
    }
}