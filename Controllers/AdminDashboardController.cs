#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using boseapp.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using boseapp.Data;
using boseapp.Models;
using System.Dynamic;

namespace boseapp.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ILogger<AdminDashboardController> _logger;
        private readonly ProductoService _productoService;


        public AdminDashboardController(ILogger<AdminDashboardController> logger, ProductoService productoService)
        {
            _logger = logger;
            _productoService = productoService;

        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Inicio));
        }

        public IActionResult Inicio()
        {
            ViewData["Title"] = "Inicio - Dashboard Bose Store";
            return View();
        }

        public IActionResult Estadistica()
        {
            ViewData["Title"] = "Estadísticas - Dashboard Bose Store";
            return View();
        }

        public IActionResult Mensajes()
        {
            ViewData["Title"] = "Mensajes - Dashboard Bose Store";
            return View();
        }

        public async Task<IActionResult> Productos()
        {
            ViewData["Title"] = "Productos - Dashboard Bose Store";
            var productos = await _productoService.GetAll();
            ViewBag.Categorias = await _productoService.GetAllCategorias();
            return View("Productos", productos); // Make sure to use the correct view name
        }

        [HttpGet]
        public async Task<IActionResult> CrearProducto()
        {
            ViewBag.Categorias = await _productoService.GetAllCategorias();
            return PartialView("CrearProducto", new Producto());
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.Categoria = await _productoService.GetCategoria(producto.Categoria.Id);
                await _productoService.Create(producto);
                return RedirectToAction(nameof(Productos));
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> EditarProducto(int id)
        {
            var producto = await _productoService.Get(id);

            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }
            ViewBag.Categorias = await _productoService.GetAllCategorias();
            return PartialView("EditarProducto", producto);
        }

        [HttpPost]
        public async Task<IActionResult> EditarProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                var existingProducto = await _productoService.Get(producto.Id);
                if (existingProducto == null)
                {
                    return NotFound();
                }

                existingProducto.Nombre = producto.Nombre;
                existingProducto.Descripcion = producto.Descripcion;
                existingProducto.SKU = producto.SKU;
                existingProducto.Precio = producto.Precio;
                existingProducto.Stock = producto.Stock;
                existingProducto.ImagenURL = producto.ImagenURL;
                existingProducto.Categoria = await _productoService.GetCategoria(producto.Categoria.Id);

                await _productoService.Update(existingProducto);

                return RedirectToAction(nameof(Productos));
            }
            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> EliminarPorId(int id)
        {
            var producto = await _productoService.Get(id);
            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            await _productoService.Delete(id);

            return RedirectToAction(nameof(Productos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}