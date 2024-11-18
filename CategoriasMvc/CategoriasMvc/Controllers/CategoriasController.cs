using CategoriasMvc.Models;
using CategoriasMvc.Services;
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
    }
}
