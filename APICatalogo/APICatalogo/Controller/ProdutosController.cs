using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("primeiro/{valor:alpha:length(5)}")]
        public ActionResult<Produto> GetPrimeiro()
        {
            try
            {
                var produto = _context.Produto?.AsNoTracking().FirstOrDefault();

                if (produto is null)
                {
                    return NotFound("Produto não encontrado.");
                }

                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            try
            {                        
                var produtos = await _context.Produto.AsNoTracking().ToListAsync();

                if (produtos is null)
                {
                    return NotFound("Produtos não encontrados.");
                }

                return produtos;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<Produto>> Get([FromQuery] int id) 
        {
            try
            {
                var produto = await _context.Produto?.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                if (produto is null)
                {
                    return NotFound("Produto não encontrado.");
                }

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        public ActionResult Post(Produto produto) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (produto is null)
                {
                    return BadRequest();
                }

                _context.Produto.Add(produto);
                _context.SaveChanges();

                return new CreatedAtRouteResult("ObterProduto",
                    new { id = produto.Id }, produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            try
            {
                if (id != produto.Id)
                {
                    return BadRequest();
                }

                _context.Entry(produto).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var produto = _context.Produto?.FirstOrDefault(x => x.Id == id);

                if (produto is null)
                {
                    return NotFound("Produto não localizado");
                }

                _context.Produto?.Remove(produto);
                _context.SaveChanges();

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }
    }
}
