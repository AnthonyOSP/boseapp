#nullable disable
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using boseapp.Models;
using Microsoft.AspNetCore.Identity;

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
    public DbSet<boseapp.Models.Servicios> DataServicios { get; set; }

}


