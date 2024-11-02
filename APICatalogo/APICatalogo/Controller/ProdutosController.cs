using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace APICatalogo.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uof = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("primeiro/{valor:alpha:length(5)}")]
        public ActionResult<ProdutoDTO> GetPrimeiro()
        {
            try
            {
                var produto = _uof.ProdutoRepository.Get().FirstOrDefault();

                if (produto is null)                
                    return NotFound("Produto não encontrado.");

                var produtoDto = _mapper.Map<ProdutoDTO>(produto);

                return produtoDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("produto/{id}")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutoPorCategoria(int id)
        {
            try
            {
                var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);                

                if (produtos is null  || !produtos.Any())
                    return NotFound("Produtos não encontrados");

                var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

                return Ok(produtosDto);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            try
            {                        
                var produtos = _uof.ProdutoRepository.Get().ToList();

                if (produtos is null)                
                    return NotFound("Produtos não encontrados.");

                var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

                return Ok(produtosDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> Get([FromQuery] int id) 
        {
            try
            {
                var produto = _uof.ProdutoRepository.Get(p => p.Id == id);

                if (produto is null)                
                    return NotFound("Produto não encontrado.");

                var produtoDto = _mapper.Map<ProdutoDTO>(produto);

                return Ok(produtoDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto) 
        {
            try
            {
                if (!ModelState.IsValid)                
                    return BadRequest(ModelState);                

                if (produtoDto is null)                
                    return BadRequest();

                var produto = _mapper.Map<Produto>(produtoDto);

                var produtoCriado = _uof.ProdutoRepository.Create(produto);
                _uof.Commit();

                return new CreatedAtRouteResult("ObterProduto",
                    new { id = produtoCriado.Id }, produtoDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }


        [HttpPost("teste")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<ProdutoDTO>> Post(IEnumerable<ProdutoDTO> produtosDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produtos = _mapper.Map<IEnumerable<Produto>>(produtosDto);

            _uof.ProdutoRepository.Create(produtos);
            _uof.Commit();

            return Ok(produtosDto);
        }

        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
        {
            try
            {
                if (id != produtoDto.Id)                
                    return BadRequest();

                var produto = _mapper.Map<Produto>(produtoDto);

                _uof.ProdutoRepository.Update(produto);
                _uof.Commit();

                return Ok(produtoDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            try
            {
                var produto = _uof.ProdutoRepository.Get(p => p.Id == id);
                _uof.ProdutoRepository.Delete(produto);
                _uof.Commit();

                var produtoDto = _mapper.Map<ProdutoDTO>(produto);

                return Ok(produtoDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }
    }
}
