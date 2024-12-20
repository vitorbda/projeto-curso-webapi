using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VShop.ProductApi.DTOs;
using VShop.ProductApi.Services;

namespace VShop.ProductApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
    {
        var productsDTO = await _service.GetProducts();

        if (productsDTO is null)
            return NotFound();

        return Ok(productsDTO);
    }

    [HttpGet("{id:int}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDTO>> Get(int id)
    {
        var productDTO = await _service.GetProductById(id);

        if (productDTO is null)
            return NotFound();

        return Ok(productDTO);
    }

    [HttpPost]
    public async Task<ActionResult> Post(ProductDTO productDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _service.AddProduct(productDTO);

        return new CreatedAtRouteResult("GetProduct", new { id = productDTO.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, ProductDTO productDTO)
    {
        if (id != productDTO.Id) return BadRequest("Ids não conferem");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _service.UpdateProduct(productDTO);

        return Ok(productDTO);
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.RemoveProduct(id);

        return NoContent();
    }
}
