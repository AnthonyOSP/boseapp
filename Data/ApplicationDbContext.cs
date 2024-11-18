#nullable disable
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using boseapp.Models;

namespace boseapp.Data;

public class ApplicationDbContext : IdentityDbContext<UserCliente>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<boseapp.Models.Cliente> DataCliente { get; set; }
    public DbSet<boseapp.Models.Contacto> DataContacto { get; set; }
    public DbSet<boseapp.Models.Producto> DataProducto { get; set; }
    public DbSet<boseapp.Models.Categoria> DataCategoria { get; set; }
}
