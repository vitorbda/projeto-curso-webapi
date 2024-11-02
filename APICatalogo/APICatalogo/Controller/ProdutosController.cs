using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;

        public ProdutosController(IUnitOfWork unitOfWork)
        {
            _uof = unitOfWork;
        }

        [HttpGet("primeiro/{valor:alpha:length(5)}")]
        public ActionResult<Produto> GetPrimeiro()
        {
            try
            {
                var produto = _uof.ProdutoRepository.Get().FirstOrDefault();

                if (produto is null)                
                    return NotFound("Produto não encontrado.");
                
                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("produto/{id}")]
        public ActionResult<IEnumerable<Produto>> GetProdutoPorCategoria(int id)
        {
            try
            {
                var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);                

                if (produtos is null  || !produtos.Any())
                    return NotFound("Produtos não encontrados");

                return Ok(produtos);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            try
            {                        
                var produtos = _uof.ProdutoRepository.Get().ToList();

                if (produtos is null)                
                    return NotFound("Produtos não encontrados.");
                
                return produtos;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<Produto> Get([FromQuery] int id) 
        {
            try
            {
                var produto = _uof.ProdutoRepository.Get(p => p.Id == id);

                if (produto is null)                
                    return NotFound("Produto não encontrado.");                

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult Post(Produto produto) 
        {
            try
            {
                if (!ModelState.IsValid)                
                    return BadRequest(ModelState);                

                if (produto is null)                
                    return BadRequest();

                var produtoCriado = _uof.ProdutoRepository.Create(produto);
                _uof.Commit();

                return new CreatedAtRouteResult("ObterProduto",
                    new { id = produtoCriado.Id }, produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }


        [HttpPost("teste")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Produto>> Post(IEnumerable<Produto> produtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _uof.ProdutoRepository.Create(produtos);
            _uof.Commit();

            return Ok(produtos);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            try
            {
                if (id != produto.Id)                
                    return BadRequest();                

                _uof.ProdutoRepository.Update(produto);
                _uof.Commit();

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult<Produto> Delete(int id)
        {
            try
            {
                var produto = _uof.ProdutoRepository.Get(p => p.Id == id);
                _uof.ProdutoRepository.Delete(produto);
                _uof.Commit();

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }
    }
}
