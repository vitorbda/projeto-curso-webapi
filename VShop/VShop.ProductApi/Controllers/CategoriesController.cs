using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VShop.ProductApi.DTOs;
using VShop.ProductApi.Services;

namespace VShop.ProductApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
    {
        var categoriesDto = await _service.GetCategories();

        if (categoriesDto is null) 
            return NotFound();

        return Ok(categoriesDto);
    }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesProducts()
    {
        var categoriesDto = await _service.GetCategoriesProducts();

        if (categoriesDto is null)
            return NotFound();

        return Ok(categoriesDto);
    }

    [HttpGet("{id:int}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDTO>> Get(int id)
    {
        var categoryDto = await _service.GetCategoryById(id);

        if (categoryDto is null)
            return NotFound();

        return Ok(categoryDto);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CategoryDTO categoryDTO)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _service.AddCategory(categoryDTO);

        return new CreatedAtRouteResult("GetCategory", new { id = categoryDTO.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, CategoryDTO categoryDTO)
    {
        if (id != categoryDTO.Id) return BadRequest("Ids não conferem");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _service.UpdateCategory(categoryDTO);

        return Ok(categoryDTO);
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.RemoveCategory(id);

        return NoContent();
    }
}
