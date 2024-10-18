using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using boseapp.Data;
using boseapp.Models;
using boseapp.Helper;

namespace boseapp.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ILogger<CarritoController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CarritoController(ILogger<CarritoController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            List<Carrito> carrito = Helper.SessionExtensions.Get<List<Carrito>>(HttpContext.Session, "carritoSesion") ?? new List<Carrito>();
            if (carrito == null)
            {
                carrito = new List<Carrito>();
            }
            return View(carrito);
        }

        public async Task<IActionResult> Add(long? id)
        {
            var userName = _userManager.GetUserName(User);
            if (userName == null)
            {
                _logger.LogInformation("No existe usuario");
                ViewData["Message"] = "Por favor debe loguearse antes de agregar un producto";
                return RedirectToAction("Index", "Catalogo");
            }
            else
            {
                //obtengo el carrito de memoria
                List<Carrito> carrito = Helper.SessionExtensions.Get<List<Carrito>>(HttpContext.Session, "carritoSesion") ?? new List<Carrito>();
                if (carrito == null)
                {
                    carrito = new List<Carrito>();
                }
                //obtengo los datos del producto
                var producto = await _context.DataProducto.FindAsync(id);
                Carrito itemCarrito = new Carrito();
                itemCarrito.Producto = producto;
                itemCarrito.UserName = userName;
                itemCarrito.Cantidad = 1;
                carrito.Add(itemCarrito);
                //seteo el carrito en memoria
                Helper.SessionExtensions.Set<List<Carrito>>(HttpContext.Session, "carritoSesion", carrito);
                ViewData["Message"] = "Se Agrego al carrito";
                _logger.LogInformation("Se agrego un producto al carrito");
                return RedirectToAction("Index", "Catalogo");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}