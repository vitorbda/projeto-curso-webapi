using CategoriasMvc.Models;
using CategoriasMvc.Services;
using Microsoft.AspNetCore.Mvc;
using CategoriasMvc.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CategoriasMvc.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly IProdutoService _produtoService;
        private readonly ICategoriaService _categoriaService;
        private string token { get => HttpContext.ObtemTokenJwt(); }

        public ProdutosController(IProdutoService produtoService, ICategoriaService categoriaService)
        {
            _produtoService = produtoService;
            _categoriaService = categoriaService;
        }

        public async Task<ActionResult<IEnumerable<ProdutoViewModel>>> Index()
        {
            var result = await _produtoService.GetProdutos(token);

            if (result is null)
                return View("Error");

            return View(result);
        }

        public async Task<IActionResult> CriarNovoProduto()
        {
            ViewBag.CategoriaId = new SelectList(await _categoriaService.GetCategorias(), "Id", "Nome");

            return View();
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> CriarNovoProduto(ProdutoViewModel produtoVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _produtoService.CriaProduto(produtoVM, token);

                if (result is not null)
                    return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.CategoriaId = new SelectList(await _categoriaService.GetCategorias(), "Id", "Nome");
            }

            return View(produtoVM);
        }
    }
}
