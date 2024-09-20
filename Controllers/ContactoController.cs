using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using boseapp.Data;
using boseapp.Models;
using boseapp.ViewModel;

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
            var miscontactos = from o in _context.DataContacto select o;
            _logger.LogDebug("contactos {miscontactos}", miscontactos);
            var viewModel = new ContactoViewModel
            {
                FormContacto = new Contacto(),
                ListContacto = miscontactos
            };
            _logger.LogDebug("viewModel {viewModel}", viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Enviar(ContactoViewModel viewModel)
        {
            _logger.LogDebug("Ingreso a enviar mensaje");

            if (viewModel.FormContacto == null)
            {
                _logger.LogError("FormContacto is null");
                return BadRequest("FormContacto cannot be null");
            }

            var contacto = new Contacto
            {
                Nombre = viewModel.FormContacto.Nombre,
                Email = viewModel.FormContacto.Email,
                Message = viewModel.FormContacto.Message
            };

            _context.Add(contacto);
            _context.SaveChanges();

            ViewData["Message"] = "Se registro el contacto correctamente";

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}