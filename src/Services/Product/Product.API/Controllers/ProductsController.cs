using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interfaces;
using Shared.DTOs.Product;
using System.ComponentModel.DataAnnotations;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _productRepository.GetProducts();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<ProductDto>>(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            var product = _mapper.Map<CatalogProduct>(productDto);
            await _productRepository.CreateProduct(product);
            await _productRepository.SaveChangesAsync();

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateProduct([Required] long id, [FromBody] UpdateProductDto productDto)
        {
            var product = await _productRepository.GetProduct(id);
            if(product == null)
            {
                return NotFound();
            }

            var updateProduct = _mapper.Map(productDto, product);
            await _productRepository.UpdateProduct(updateProduct);
            await _productRepository.SaveChangesAsync();

            return Ok(_mapper.Map<ProductDto>(product));
        }
    }
}
