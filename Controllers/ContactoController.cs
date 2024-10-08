using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using boseapp.Data;
using boseapp.Models;
using boseapp.ViewModel;
using boseapp.Helper;

namespace boseapp.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ILogger<ContactoController> _logger;
        private readonly ApplicationDbContext _context;

        private readonly SendMail _sendMail;

        public ContactoController(ILogger<ContactoController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _sendMail = new SendMail();
        }

        public IActionResult Index()
        {
            var miscontactos = from o in _context.DataContacto select o;
            var viewModel = new ContactoViewModel
            {
                FormContacto = new Contacto(),
                ListContacto = [.. miscontactos.OrderBy(c => c.Id)]
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Enviar(ContactoViewModel viewModel)
        {
            _logger.LogDebug("Ingreso a enviar mensaje");

            if (viewModel.FormContacto == null)
            {
                _logger.LogError("FormContacto is null");
                return BadRequest("FormContacto cannot be null");
            }

            if (viewModel.FormContacto.Id == 0)
            {
                var contacto = new Contacto
                {
                    Nombre = viewModel.FormContacto.Nombre,
                    Email = viewModel.FormContacto.Email,
                    Message = viewModel.FormContacto.Message
                };


                _context.Add(contacto);
                _context.SaveChanges();

                TempData["Message"] = "Se registró el contacto correctamente";

                var __apikey = Environment.GetEnvironmentVariable("SMTP_PASS");
                _logger.LogDebug($"SMTP_PASS: {__apikey}");

                if (!string.IsNullOrEmpty(viewModel.FormContacto.Email))
                {
                    if (!string.IsNullOrEmpty(viewModel.FormContacto.Message))
                    {
                        await _sendMail.EnviarCorreoAsync(viewModel.FormContacto.Email, "Nuevo Contacto", viewModel.FormContacto.Message);
                    }
                    else
                    {
                        _logger.LogError("Message is null or empty");
                    }
                }
                else
                {
                    _logger.LogError("Email is null or empty");
                }
            }
            else
            {
                var contacto = _context.DataContacto.FirstOrDefault(c => c.Id == viewModel.FormContacto.Id);
                if (contacto == null)
                {
                    return NotFound("Contacto no encontrado");
                }

                contacto.Nombre = viewModel.FormContacto.Nombre;
                contacto.Email = viewModel.FormContacto.Email;
                contacto.Message = viewModel.FormContacto.Message;

                _context.Update(contacto);
                _context.SaveChanges();

                TempData["Message"] = "Se actualizó el contacto correctamente";
            }

            var miscontactos = from o in _context.DataContacto select o;
            viewModel.ListContacto = [.. miscontactos.OrderBy(c => c.Id)];

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult EliminarPorId(int id)
        {
            var contacto = _context.DataContacto.FirstOrDefault(c => c.Id == id);
            if (contacto == null)
            {
                return NotFound("Contacto no encontrado");
            }

            _context.DataContacto.Remove(contacto);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}