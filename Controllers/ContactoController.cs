#nullable disable
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
using TextClasification;

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
                ListContacto = miscontactos.OrderBy(c => c.Id).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Enviar(ContactoViewModel viewModel)
        {
            _logger.LogDebug("Ingreso a enviar mensaje");

            MLModelTextClasif.ModelInput sampleData = new MLModelTextClasif.ModelInput()
            {
                Comentario = viewModel.FormContacto.Message
            };

            MLModelTextClasif.ModelOutput output = MLModelTextClasif.Predict(sampleData);


            var sortedScoresWithLabel = MLModelTextClasif.PredictAllLabels(sampleData);
            var scoreKeyFirst = sortedScoresWithLabel.ToList()[0].Key;
            var scoreValueFirst = sortedScoresWithLabel.ToList()[0].Value;
            var scoreKeySecond = sortedScoresWithLabel.ToList()[1].Key;
            var scoreValueSecond = sortedScoresWithLabel.ToList()[1].Value;

            if (scoreKeyFirst == "1")
            {
                if (scoreValueFirst > 0.5)
                {
                    viewModel.FormContacto.Category = "Positivo";
                }
                else
                {
                    viewModel.FormContacto.Category = "Negativo";
                }
            }
            else
            {
                if (scoreValueFirst > 0.5)
                {
                    viewModel.FormContacto.Category = "Negativo";
                }
                else
                {
                    viewModel.FormContacto.Category = "Positivo";
                }
            }


            Console.WriteLine($"{scoreKeyFirst,-40}{scoreValueFirst,-20}");
            Console.WriteLine($"{scoreKeySecond,-40}{scoreValueSecond,-20}");

            var contacto = new Contacto
            {
                Nombre = viewModel.FormContacto.Nombre,
                Email = viewModel.FormContacto.Email,
                Asunto = viewModel.FormContacto.Asunto,
                Message = viewModel.FormContacto.Message,
                Category = viewModel.FormContacto.Category
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
                    await _sendMail.EnviarCorreoAsync(viewModel.FormContacto.Email, viewModel.FormContacto.Asunto, viewModel.FormContacto.Message);
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

            var miscontactos = from o in _context.DataContacto select o;
            viewModel.ListContacto = miscontactos.OrderBy(c => c.Id).ToList();

            return RedirectToAction("Index");
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