using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using X.PagedList;

namespace APICatalogo.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uof = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetProdutosPagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutos([FromQuery] ProdutosParameters prodParams)
        {
            try
            {
                var produtos = await _uof.ProdutoRepository.GetProdutosAsync(prodParams);
                if (produtos is null)
                    return NotFound();

                return ObterProdutosPagination(produtos);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("FilterPagePagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFiltroPreco)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFiltroPreco);
            if (produtos is null)
                return NotFound();

            return ObterProdutosPagination(produtos);
        }

        [HttpPatch("{id}/UpdatePartial")]
        public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDto)
        {
            if (patchProdutoDto is null || id <= 0)
                return BadRequest();

            var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);

            if (produto is null)
                return NotFound();

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDto.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
                return BadRequest(ModelState);

            _mapper.Map(produtoUpdateRequest, produto);

            _uof.ProdutoRepository.Update(produto);
            await _uof.CommitAsync();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        [HttpGet("primeiro/{valor:alpha:length(5)}")]
        public async Task<ActionResult<ProdutoDTO>> GetPrimeiro()
        {
            try
            {
                var produto = await _uof.ProdutoRepository.GetAsync();

                if (produto is null)                
                    return NotFound("Produto não encontrado.");

                var produtoDto = _mapper.Map<ProdutoDTO>(produto.FirstOrDefault());

                return produtoDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpGet("produto/{id}")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutoPorCategoria(int id)
        {
            try
            {
                var produtos = await _uof.ProdutoRepository.GetProdutosPorCategoriaAsync(id);                

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
        //[Authorize("UserOnly")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
        {
            try
            {
                var produtos = await _uof.ProdutoRepository.GetAsync();

                if (produtos is null)                
                    return NotFound("Produtos não encontrados.");

                var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos.ToList());

                return Ok(produtosDto);
            }
            catch (Exception)
            {
                return BadRequest();
                //return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        /// <summary>
        /// Obtem um produto pelo seu Id
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Um objeto Produto</returns>
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get([FromQuery] int id) 
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id de produto inválido");

                var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);

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

        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <param name="produtoDto"></param>
        /// <returns>Retorna uma lista de objetos Produto</returns>
        [HttpPost]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto) 
        {
            try
            {
                if (!ModelState.IsValid)                
                    return BadRequest(ModelState);                

                if (produtoDto is null)                
                    return BadRequest();

                var produto = _mapper.Map<Produto>(produtoDto);

                var produtoCriado = _uof.ProdutoRepository.Create(produto);
                await _uof.CommitAsync();

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
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Post(IEnumerable<ProdutoDTO> produtosDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var produtos = _mapper.Map<IEnumerable<Produto>>(produtosDto);

            _uof.ProdutoRepository.Create(produtos);
            await _uof.CommitAsync();

            return Ok(produtosDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
        {
            try
            {
                if (id != produtoDto.Id)                
                    return BadRequest();

                var produto = _mapper.Map<Produto>(produtoDto);

                _uof.ProdutoRepository.Update(produto);
                await _uof.CommitAsync();

                return Ok(produtoDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            try
            {
                var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
                _uof.ProdutoRepository.Delete(produto);
                await _uof.CommitAsync();

                var produtoDto = _mapper.Map<ProdutoDTO>(produto);

                return Ok(produtoDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutosPagination(IPagedList<Produto> produtos)
        {
            var metadata = new
            {
                produtos.Count,
                produtos.PageSize,
                produtos.PageCount,
                produtos.TotalItemCount,
                produtos.HasNextPage,
                produtos.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(_mapper.Map<IEnumerable<ProdutoDTO>>(produtos));
        }
    }
}
