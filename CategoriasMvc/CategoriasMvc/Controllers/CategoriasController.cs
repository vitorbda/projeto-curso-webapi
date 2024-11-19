using CategoriasMvc.Models;
using CategoriasMvc.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CategoriasMvc.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public async Task<ActionResult<IEnumerable<CategoriaViewModel>>> Index()
        {
            var result = await _categoriaService.GetCategorias();

            if (result is null)
                return View("Error");

            return View(result);
        }

        public IActionResult CriarNovaCategoria()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaViewModel>> CriarNovaCategoria(CategoriaViewModel categoriaVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoriaService.CriaCategoria(categoriaVM);

                if (result is not null)
                    return RedirectToAction(nameof(Index));
            }

            ViewBag.Erro = "Erro ao criar categoria";
            return View(categoriaVM);            
        }

        public async Task<IActionResult> AtualizarCategoria(int id)
        {
            var result = await _categoriaService.GetById(id);

            if (result is null)
                return View("Error");

            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaViewModel>> AtualizarCategoria(int id, CategoriaViewModel categoriaVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoriaService.AtualizaCategoria(id, categoriaVM);

                if (result)
                    return RedirectToAction(nameof(Index));
            }

            ViewBag.Erro = "Erro ao atualizar categoria";
            return View(categoriaVM);
        }

        public async Task<ActionResult> DeletarCategoria(int id)
        {
            var result = await _categoriaService.GetById(id);

            if (result is null)
                return View("Error");

            return View(result);
        }

        [HttpPost(), ActionName("DeletarCategoria")]
        public async Task<IActionResult> DeletaConfirmado(int id)
        {
            var result = await _categoriaService.DeletaCategoria(id);

            if (result)
                return RedirectToAction("Index");

            return View("Error");
        }
    }
}
