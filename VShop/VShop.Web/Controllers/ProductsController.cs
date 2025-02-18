using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VShop.Web.Models;
using VShop.Web.Roles;
using VShop.Web.Services.Interfaces;

namespace VShop.Web.Controllers;
[Authorize(Roles = Role.Admin)]
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
        var token = await GetAccessToken();
        return new SelectList(await _categoryService.GetAllCategories(token), "Id", "Name");
    }

    private async Task<string> GetAccessToken()
    {
        return await HttpContext.GetTokenAsync("access_token");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductViewModel>>> Index()
    {
        var token = await GetAccessToken();
        var result = await _productService.GetAllProducts(token);

        return result is null
            ? View("Error")
            : View(result);
    }

    [HttpGet]
    public async Task<ActionResult> Create()
    {
        var token = await GetAccessToken();
        ViewBag.Categories = await ReturnCategoriesSelectList();

        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProductViewModel>> Create(ProductViewModel productVM)
    {
        var token = await GetAccessToken();
        if (ModelState.IsValid)
        {
            var result = await _productService.CreateProduct(productVM, token);

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
        var token = await GetAccessToken();
        ViewBag.Categories = await ReturnCategoriesSelectList();

        var result = await _productService.GetProductById(id, token);

        return result is null
            ? View("Error")
            : View(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Update(ProductViewModel productVM)
    {
        var token = await GetAccessToken();
        if (!ModelState.IsValid) return View(productVM);

        var result = await _productService.UpdateProduct(productVM, token);

        return result is null
            ? View(productVM)
            : RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ProductViewModel>> Delete(int id)
    {
        var token = await GetAccessToken();
        var result = await _productService.GetProductById(id, token);

        return result is null
            ? View("Error")
            : View(result);
    }

    [HttpPost, ActionName("DeleteProduct")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
        var token = await GetAccessToken();
        var result = await _productService.DeleteProduct(id, token);

        return !result
            ? View("Error")
            : RedirectToAction(nameof(Index));
    }
}
