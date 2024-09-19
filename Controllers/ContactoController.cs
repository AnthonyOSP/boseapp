using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using boseapp.Data;
using boseapp.Models;

namespace boseapp.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ILogger<ContactoController> _logger;
        private readonly ApplicationDbContext _context;

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new Contacto());
        }

        [HttpPost]
        public IActionResult Enviar(Contacto objcontacto)
        {
            _logger.LogDebug("Ingreso a enviar mensaje");
            _context.Add(objcontacto);
            _context.SaveChanges();

            ViewData["Message"] = "Se registro el contacto correctamente";

            return View("Index", new Contacto());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}