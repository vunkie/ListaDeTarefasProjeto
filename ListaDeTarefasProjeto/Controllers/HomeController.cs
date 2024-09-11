using ListaDeTarefasProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ListaDeTarefasProjeto.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

    }
}
