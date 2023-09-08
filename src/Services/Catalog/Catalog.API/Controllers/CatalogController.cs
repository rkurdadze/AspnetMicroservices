﻿using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repository.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"Product with id: {id} not found.");
                return NotFound();
            }
            else
            {
                return Ok(product);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [Route("[action]/{category}", Name = "GetProductByCategory")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products = await _repository.GetProductByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct ([FromBody] Product product)
        {
            await _repository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new {id = product.Id}, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof (Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]  
        public async Task<ActionResult<Product>> UpdateProduct(string id)
        {
            bool result = await _repository.DeleteProduct(id);
            if (result)
            {
                return Ok(result);
            } else { 
                return BadRequest($"Product with id: {id} not found."); 
            }
        }
    }
}
