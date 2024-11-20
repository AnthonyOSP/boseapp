#nullable disable
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
using boseapp.ViewModel;
using boseapp.Integration.CurrencyExchange;
using System.Drawing;

namespace boseapp.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CurrencyExchangeIntegration _currencyExchangeIntegration;

        public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context, CurrencyExchangeIntegration currencyExchangeIntegration)
        {
            _logger = logger;
            _context = context;
            _currencyExchangeIntegration = currencyExchangeIntegration;
        }

        public async Task<IActionResult> Index(string currency = "PEN")
        {
            var catalogos = from o in _context.DataProducto select o;
            var categoria = from o in _context.DataCategoria select o;
            var currencySymbol = "S/";

            if (currency == "USD")
            {
                var exchangeRate = await _currencyExchangeIntegration.GetExchangeRate("PEN", "USD", 1);
                currencySymbol = "$";
                catalogos = catalogos.Select(p => new Producto
                {
                    // Clone all properties
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    ImagenURL = p.ImagenURL,
                    Precio = p.Precio * (decimal)exchangeRate.info.rate
                });
            }
            else if (currency == "EUR")
            {
                var exchangeRate = await _currencyExchangeIntegration.GetExchangeRate("PEN", "EUR", 1);
                currencySymbol = "€";
                catalogos = catalogos.Select(p => new Producto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    ImagenURL = p.ImagenURL,
                    Precio = p.Precio * (decimal)exchangeRate.info.rate
                });
            }
            dynamic model = new ExpandoObject();
            model.itemCategoria = categoria;
            model.itemCatalogos = catalogos;
            model.CurrentCurrency = currency;
            model.CurrencySymbol = currencySymbol;
            return View(model);

        }

        public async Task<IActionResult> Exchange(TipoCambioViewModel viewmodel)
        {
            var monedaBase = "PEN";
            var tipoCambio = await _currencyExchangeIntegration.GetExchangeRate(monedaBase, viewmodel.To, viewmodel.Amount);
            ViewData["result"] = tipoCambio.result;
            ViewData["rate"] = tipoCambio.info.rate;
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}