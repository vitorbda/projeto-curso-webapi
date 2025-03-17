using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VShop.Web.Models;
using VShop.Web.Services;
using VShop.Web.Services.Interfaces;

namespace VShop.Web.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public HomeController(ILogger<HomeController> logger,
        IProductService productService,
        ICartService cartService)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index() 
    {
        var result = await _productService.GetAllProducts(string.Empty);

        return result is null
            ? View("Error")
            : View(result);
    }

    public IActionResult Logout() => SignOut("Cookies", "oidc");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
   
    [Authorize]
    public async Task<IActionResult> Login()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ProductViewModel>> ProductDetails(int id)
    {
        var result = await _productService.GetProductById(id, string.Empty);

        return result is null
            ? View("Error")
            : View(result);
    }

    [HttpPost]
    [ActionName("ProductDetails")]
    [Authorize]
    public async Task<ActionResult<ProductViewModel>> ProductDetailsPost (ProductViewModel productVM)
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        CartViewModel cart = new()
        {
            CartHeader = new CartHeaderViewModel
            {
                UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
            }
        };

        CartItemViewModel cartItem = new()
        {
            Quantity = productVM.Quantity,
            ProductId = productVM.Id,
            Product = await _productService.GetProductById(productVM.Id, token)
        };

        IEnumerable<CartItemViewModel> cartItemsVM = [cartItem];
        cart.CartItems = cartItemsVM;

        var result = await _cartService.AddItemToCartAsync(cart, token);

        return result is null
            ? View(productVM)
            : RedirectToAction(nameof(Index));
    }
}
