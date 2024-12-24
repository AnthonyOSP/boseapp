#nullable disable
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
using boseapp.Service;
using Microsoft.EntityFrameworkCore;

namespace boseapp.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ILogger<CarritoController> _logger;
        private readonly UserManager<UserCliente> _userManager;
        private readonly ApplicationDbContext _context;

        private readonly ProductoService _productoService;

        public CarritoController(ILogger<CarritoController> logger, UserManager<UserCliente> userManager, ApplicationDbContext context, ProductoService productoService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _productoService = productoService;
        }

        public IActionResult Index()
        {
            List<Carrito> carrito = Helper.SessionExtensions.Get<List<Carrito>>(HttpContext.Session, "carritoSesion") ?? new List<Carrito>();
            if (carrito == null)
            {
                carrito = new List<Carrito>();
            }
            decimal total = carrito.Sum(item => item.Precio * item.Cantidad);
            var deliveryFee = HttpContext.Request.Query["delivery"].ToString();
            decimal selectedDeliveryFee = !string.IsNullOrEmpty(deliveryFee) ?
                decimal.Parse(deliveryFee) : 10.00m; // Default 10 soles

            ViewData["DeliveryFee"] = selectedDeliveryFee;
            ViewData["Total"] = total + selectedDeliveryFee;
            ViewData["SubTotal"] = total;

            var deliveryOptions = new Dictionary<string, decimal>
            {
                { "Envío en Lima", 10.00m },
                { "Envío a Provincia", 20.00m },
            };
            ViewData["DeliveryOptions"] = deliveryOptions;
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
                var producto = await _context.DataProducto
            .Include(p => p.Categoria) // Incluir la categoría
            .FirstOrDefaultAsync(p => p.Id == id);
                if (producto == null)
                {
                    _logger.LogInformation("No existe producto");
                    ViewData["Message"] = "No existe el producto";
                    return RedirectToAction("Index", "Catalogo");
                }
                List<Carrito> carrito = Helper.SessionExtensions.Get<List<Carrito>>(HttpContext.Session, "carritoSesion") ?? new List<Carrito>();
                if (carrito == null)
                {
                    carrito = new List<Carrito>();
                }
                var item = carrito.FirstOrDefault(c => c.Producto.Id == id && c.UserName == userName);
                if (item == null)
                {
                    item = new Carrito
                    {
                        Producto = producto,
                        Cantidad = 1,
                        Precio = producto.Precio ?? 0,
                        UserName = userName,
                        Categoria = producto.Categoria.Nombre, // Asignar la categoría
                        ImagenURL = producto.ImagenURL // Asignar la URL de la imagen
                    };
                    carrito.Add(item);
                    Helper.SessionExtensions.Set<List<Carrito>>(HttpContext.Session, "carritoSesion", carrito);
                    ViewData["Message"] = "Se agregó el producto al carrito";
                    _logger.LogInformation("Se agregó un producto al carrito");
                }
                else
                {
                    item.Cantidad++;
                    Helper.SessionExtensions.Set<List<Carrito>>(HttpContext.Session, "carritoSesion", carrito);
                    ViewData["Message"] = "Se agregó una unidad del producto al carrito";
                    _logger.LogInformation("Se agregó una unidad del producto al carrito");
                }
                return RedirectToAction("Index");
            }
        }

        public IActionResult Delete(long? id)
        {
            var userName = _userManager.GetUserName(User);
            if (userName == null)
            {
                _logger.LogInformation("No existe usuario");
                ViewData["Message"] = "Por favor debe loguearse antes de eliminar un producto";
                return RedirectToAction("Index", "Catalogo");
            }
            else
            {
                List<Carrito> carrito = Helper.SessionExtensions.Get<List<Carrito>>(HttpContext.Session, "carritoSesion") ?? new List<Carrito>();
                if (carrito == null)
                {
                    carrito = new List<Carrito>();
                }
                var itemToRemove = carrito.FirstOrDefault(c => c.Producto.Id == id && c.UserName == userName);
                if (itemToRemove != null)
                {
                    carrito.Remove(itemToRemove);
                    Helper.SessionExtensions.Set<List<Carrito>>(HttpContext.Session, "carritoSesion", carrito);
                    ViewData["Message"] = "Se eliminó el producto del carrito";
                    _logger.LogInformation("Se eliminó un producto del carrito");
                }
                return RedirectToAction("Index");
            }
        }

        public IActionResult Increment(long? id)
        {
            var userName = _userManager.GetUserName(User);
            if (userName == null)
            {
                return RedirectToAction("Index", "Catalogo");
            }

            List<Carrito> carrito = Helper.SessionExtensions.Get<List<Carrito>>(HttpContext.Session, "carritoSesion");
            var item = carrito.FirstOrDefault(c => c.Producto.Id == id && c.UserName == userName);
            if (item != null)
            {
                item.Cantidad++;
                Helper.SessionExtensions.Set<List<Carrito>>(HttpContext.Session, "carritoSesion", carrito);
                _logger.LogInformation($"Aumentó la cantidad del producto {item.Producto.Nombre} a {item.Cantidad}");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Decrement(long? id)
        {
            var userName = _userManager.GetUserName(User);
            if (userName == null)
            {
                return RedirectToAction("Index", "Catalogo");
            }

            List<Carrito> carrito = Helper.SessionExtensions.Get<List<Carrito>>(HttpContext.Session, "carritoSesion");
            var item = carrito.FirstOrDefault(c => c.Producto.Id == id && c.UserName == userName);
            if (item != null && item.Cantidad > 1)
            {
                item.Cantidad--;
                Helper.SessionExtensions.Set<List<Carrito>>(HttpContext.Session, "carritoSesion", carrito);
                _logger.LogInformation($"Disminuyó la cantidad del producto {item.Producto.Nombre} a {item.Cantidad}");
            }
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}