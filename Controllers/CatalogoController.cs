using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using boseapp.Data;
using boseapp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Dynamic;

namespace boseapp.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;

        public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var catalogos = from o in _context.DataProducto select o;
            var categoria = from o in _context.DataCategoria select o;
            dynamic model = new ExpandoObject();
            model.itemCategoria = categoria;
            model.itemCatalogos = catalogos;
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}