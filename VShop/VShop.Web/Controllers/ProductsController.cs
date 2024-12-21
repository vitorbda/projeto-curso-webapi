using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VShop.Web.Models;
using VShop.Web.Services.Interfaces;

namespace VShop.Web.Controllers;
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> Index()
    {
        var result = await _productService.GetAllProducts();

        return result is null
            ? View("Error")
            : View(result);
    }

    [HttpGet]
    public async Task<ActionResult> Create()
    {
        ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");

        return View();
    }

    [HttpPost]
    public async Task<ActionResult<ProductViewModel>> Create(ProductViewModel productVM)
    {
        if (ModelState.IsValid) 
        { 
            var result = await _productService.CreateProduct(productVM);

            if (result != null)
                return RedirectToAction(nameof(Index));
        }
        else
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
        }

        return View(productVM);
    }
}
