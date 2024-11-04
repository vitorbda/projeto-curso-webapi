﻿using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;

namespace APICatalogo.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriaController(IUnitOfWork uof, IConfiguration configuration, ILogger<CategoriaController> logger, IMapper mapper)
        {
            _uof = uof;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("LerArquivoConfiguracao")]
        public ActionResult<string> GetValores()
        {
            _logger.LogInformation("====================== Categoria/GetValores ==============================");

            var valor1 = _configuration["chave1"];
            var valor2 = _configuration.GetValue<string>("chave2");

            var secao1 = _configuration.GetValue<string>("secao1:chave1");
            var secao1_ = _configuration["secao1:chave2"];

            return ConcatenaValores(valor1, valor2, secao1, secao1_);
        }

        [HttpGet("UsandoFromServices/{nome}")]
        public ActionResult<string> GetSaudacaoFromServices([FromServices] IMeuServico meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
        }

        [HttpGet("pagination")]
        public ActionResult<IEnumerable<CategoriaDTO>> Get([FromQuery] CategoriasParameters parameters)
        {
            var categorias = _uof.CategoriaRepository.GetCategorias(parameters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(_mapper.Map<IEnumerable<CategoriaDTO>>(categorias));
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
        {
            try
            {
                var categorias = _uof.CategoriaRepository.Get();

                var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

                return Ok(categorias);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }            
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            try
            {
                var categorias = _uof.CategoriaRepository.Get();

                var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

                return Ok(categoriasDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }                        
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id) 
        {
            try
            {
                var categoria = _uof.CategoriaRepository.Get(c => c.Id == id);

                if (categoria is null)                
                    return NotFound("Categoria não encontrada.");

                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return Ok(categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPost]
        public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
        {
            try
            {
                if (categoriaDto is null)                
                    return BadRequest();

                var categoria = _mapper.Map<Categoria>(categoriaDto);

                categoriaDto.Id = _uof.CategoriaRepository.Create(categoria).Id;
                _uof.Commit();

                return new CreatedAtRouteResult("ObterCategoria",
                    new { id = categoriaDto.Id }, categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
        {
            try
            {
                if (id != categoriaDto.Id)                
                    return BadRequest();

                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _uof.CategoriaRepository.Update(categoria);
                _uof.Commit();

                return Ok(categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a sua solicitação.");
            }

        }

        [HttpDelete]
        public ActionResult<CategoriaDTO> Delete(int id) 
        {
            try
            {
                var categoria = _uof.CategoriaRepository.Get(c => c.Id == id);
                if (categoria is null)
                    return NotFound("Categoria não encontrada");

                var categoriaDeletada = _uof.CategoriaRepository.Delete(categoria);
                _uof.Commit();

                var categoriaDeletadaDto = _mapper.Map<CategoriaDTO>(categoriaDeletada);

                return Ok(categoriaDeletadaDto);
            }
            catch 
            {
                throw;
            }            
        }


        private string ConcatenaValores(params string[] valores)
        {
            var valoresConcatenados = new StringBuilder();

            for (int i = 0; i < valores.Length; i++)
                valoresConcatenados.Append($"valor({i + 1}) = {valores[i]} \n");

            return valoresConcatenados.ToString();
        }
    }
}
