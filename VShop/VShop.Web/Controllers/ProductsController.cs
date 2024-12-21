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

    private async Task<SelectList> ReturnCategoriesSelectList()
    {
        return new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
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
        ViewBag.Categories = await ReturnCategoriesSelectList();

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
            ViewBag.Categories = await ReturnCategoriesSelectList();
        }

        return View(productVM);
    }

    [HttpGet]
    public async Task<ActionResult<ProductViewModel>> Update(int id)
    {
        ViewBag.Categories = await ReturnCategoriesSelectList();

        var result = await _productService.GetProductById(id);

        return result is null
            ? View("Error") 
            : View(result);
    }

    [HttpPost]
    public async Task<ActionResult> Update(ProductViewModel productVM)
    {
        if (!ModelState.IsValid) return View(productVM);

        var result = await _productService.UpdateProduct(productVM);

        return result is null
            ? View(productVM)
            : RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<ActionResult<ProductViewModel>> Delete(int id)
    {
        var result = await _productService.GetProductById(id);

        return result is null
            ? View("Error")
            : View(result);
    }

    [HttpPost, ActionName("DeleteProduct")]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        var result = await _productService.DeleteProduct(id);

        return !result
            ? View("Error")
            : RedirectToAction(nameof(Index));
    }
}
