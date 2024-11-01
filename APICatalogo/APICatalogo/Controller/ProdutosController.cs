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
        private readonly IProdutoRepository _repository;

        public ProdutosController(IProdutoRepository produtoRepository)
        {
            _repository = produtoRepository;
        }

        [HttpGet("primeiro/{valor:alpha:length(5)}")]
        public ActionResult<Produto> GetPrimeiro()
        {
            try
            {
                var produto = _repository.GetProdutos().FirstOrDefault();

                if (produto is null)                
                    return NotFound("Produto não encontrado.");
                
                return produto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            try
            {                        
                var produtos = _repository.GetProdutos().ToList();

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
                var produto = _repository.GetProduto(id);

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

                var produtoCriado = _repository.Create(produto);

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

            _repository.Create(produtos);

            return Ok(produtos);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            try
            {
                if (id != produto.Id)                
                    return BadRequest();                

                var produtoAtualizado = _repository.Update(produto);

                if (!produtoAtualizado)
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Falha ao atualizar o produto [Id = {id}]");

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
                var acaoExecutada = _repository.Delete(id);

                if (!acaoExecutada)
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Falha ao excluir o produto [Id = {id}]");

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }
    }
}
