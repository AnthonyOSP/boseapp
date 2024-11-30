using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using boseapp.Data;
using boseapp.Models;
using Microsoft.EntityFrameworkCore;

namespace boseapp.Service
{
    public class ProductoService
    {
        private readonly ILogger<ProductoService> _logger;
        private readonly ApplicationDbContext _context;

        public ProductoService(ILogger<ProductoService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Create(Producto producto)
        {
            _context.DataProducto.Add(producto);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Producto>?> GetAll()
        {
            if (_context.DataProducto == null)
                return null;
            var productos = await _context.DataProducto
            .Include(p => p.Categoria)
            .ToListAsync();
            _logger.LogInformation("Productos: {0}", productos);
            return productos;
        }

        public async Task<Producto?> Get(long? id)
        {
            if (id == null || _context.DataProducto == null)
            {
                return null;
            }

            var producto = await _context.DataProducto
                .Include(p => p.Categoria) // Incluir la categoría
                .FirstOrDefaultAsync(p => p.Id == id);
            _logger.LogInformation("Producto: {0}", producto);
            if (producto == null)
            {
                return null;
            }
            return producto;
        }

        public async Task Update(Producto producto)
        {
            _context.DataProducto.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var producto = await _context.DataProducto.FindAsync(id);
            if (producto != null)
            {
                _context.DataProducto.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Categoria>> GetAllCategorias()
        {
            return await _context.DataCategoria.ToListAsync();
        }

        public async Task<Categoria?> GetCategoria(long id)
        {
            return await _context.DataCategoria.FindAsync(id);
        }
    }
}