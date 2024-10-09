using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using boseapp.Models;


namespace boseapp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICompositeViewEngine _viewEngine;

    public HomeController(ILogger<HomeController> logger, ICompositeViewEngine viewEngine)
    {
        _logger = logger;
        _viewEngine = viewEngine;
    }

    public IActionResult Index()
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddMinutes(30), // Duración de la cookie: 30 minutos
            HttpOnly = true, // La cookie solo puede ser accedida mediante HTTP, no vía JavaScript
            Secure = true, // La cookie solo se enviará a través de conexiones HTTPS
            SameSite = SameSiteMode.Strict // Evita que la cookie sea enviada en solicitudes cross-site
        };
        Response.Cookies.Append("MyCookieApp", "app2game", cookieOptions);

        ViewData["HomeScript"] = RenderPartialViewToString("_HomeScript");

        return View();
    }

    private string RenderPartialViewToString(string viewName)
    {
        try
        {
            using (var writer = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (!viewResult.Success)
                {
                    throw new FileNotFoundException($"View {viewName} not found.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
                return writer.GetStringBuilder().ToString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering view to string");
            return string.Empty;
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error(int statusCode)
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
