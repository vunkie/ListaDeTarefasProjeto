using ListaDeTarefasProjeto.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ListaDeTarefasProjeto.Data;
using Microsoft.EntityFrameworkCore;

namespace ListaDeTarefasProjeto.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index(string id)
        {
            var filtros = new Filtros(id);

            ViewBag.Filtros = filtros;
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Status = _context.Statuses.ToList();
            ViewBag.Vencimento = Filtros.VencimentoValoresFiltro;

            IQueryable<Tarefa> consulta = _context.Tarefas
                .Include(c => c.Categoria)
                .Include(s => s.Status);


            if (filtros.TemCategoria)
            {
                consulta = consulta.Where(c => c.CategoriaId == filtros.CategoriaId);
            }
            if (filtros.TemStatus)
            {
                consulta = consulta.Where(s => s.StatusId == filtros.StatusId);
            }
            if (filtros.TemVencimento)
            {
                var hoje = DateTime.Today;
                if (filtros.EhHoje)
                {
                    consulta = consulta.Where(v => v.DataDeVencimento == hoje);
                }
                if (filtros.EhFuturo)
                {
                    consulta = consulta.Where(v => v.DataDeVencimento > hoje);
                }
                if (filtros.EhPassado)
                {
                    consulta = consulta.Where(v => v.DataDeVencimento < hoje);
                }
            }

            var tarefas = consulta.OrderBy(t => t.DataDeVencimento).ToList();



            return View(tarefas);
        }


        public IActionResult Adicionar()
        {
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewBag.Status = _context.Statuses.ToList();

            var tarefa = new Tarefa{StatusId = "aberto"};
            return View(tarefa);
        }


        [HttpPost]
        public IActionResult Filtrar(string[] filtro)
        {
            string id = string.Join('-', filtro);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarcarCompleto([FromRoute] string id, Tarefa tarefaSelecionada){
            tarefaSelecionada = _context.Tarefas.Find(tarefaSelecionada.Id);
            if (tarefaSelecionada != null)
            {
                tarefaSelecionada.StatusId = "completo";
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult Adicionar(Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                _context.Tarefas.Add(tarefa);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categorias = _context.Categorias.ToList();
                ViewBag.Status = _context.Statuses.ToList();

                return View(tarefa);
            }
        }


        [HttpPost]
        public IActionResult DeletarCompletos(string id)
        {
            var paraDeletar = _context.Tarefas.Where(t => t.StatusId == "completo").ToList();

            foreach (var tarefa in paraDeletar)
            {
                _context.Tarefas.Remove(tarefa);
            }
            _context.SaveChanges();

            return RedirectToAction("Index", new {ID = id });
        }

    }
}
