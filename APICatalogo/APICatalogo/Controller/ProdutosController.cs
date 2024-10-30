﻿using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var produtos = _context.Produto?.AsNoTracking().ToList();

            if (produtos is null)
            {
                return NotFound("Produtos não encontrados.");
            }

            return produtos;
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id) 
        {
            var produto = _context.Produto?.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (produto is null)
            {
                return NotFound("Produto não encontrado.");
            }

            return Ok(produto);
        }

        [HttpPost]
        public ActionResult Post(Produto produto) 
        {
            if (produto is null)
            {
                return BadRequest();
            }

            _context.Produto.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", 
                new { id =  produto.Id }, produto);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.Id) 
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
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
    }
}
