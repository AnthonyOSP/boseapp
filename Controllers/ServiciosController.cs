#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using boseapp.Models;
using boseapp.Data;

namespace boseapp.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly ILogger<ServiciosController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserCliente> _userManager;

        public ServiciosController(ILogger<ServiciosController> logger, ApplicationDbContext context, UserManager<UserCliente> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var servicios = _context.DataServicios.Where(s => s.UserClienteId == user.Id).ToList();
            return View(servicios);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddServicio(Servicios servicio)
        {
            var user = await _userManager.GetUserAsync(User);
            servicio.UserClienteId = user.Id;
            servicio.Nombre = user.Nombre + " " + user.Apellido;
            servicio.Costo = 0;
            servicio.FechaDeCreacion = DateTime.UtcNow;
            servicio.Estado = false;
            _context.DataServicios.Add(servicio);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}