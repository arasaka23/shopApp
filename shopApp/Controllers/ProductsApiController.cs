using Microsoft.AspNetCore.Mvc;
using shopApp.DTOs;
using shopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace shopApp.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsApiController : ControllerBase
{
    private readonly ShopContext _context;

    public ProductsApiController(ShopContext context)
    {
        _context = context;
    }

    // Метод для преобразования Entity в DTO
    private static ProductDto ToDto(Product product) => new ProductDto
    {
        ProductId = product.ProductId,
        ProductName = product.ProductName,
        Description = product.Description,
        Price = product.Price,
        StockQuantity = product.StockQuantity,
        CategoryId = product.CategoryId,
        ImageUrl = product.ImageUrl,
        //IsActive = product.IsActive,
        //CreatedAt = product.CreatedDate,
    };

    // 1. GET api/products
    [HttpGet]
    public IActionResult GetAll(int page = 1, int pageSize = 10)
    {
        var products = _context.Products
            .OrderBy(p => p.ProductId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .Select(ToDto)
            .ToList();

        return Ok(products);
    }

    // 2. GET api/products/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (product is null) return NotFound();
        return Ok(ToDto(product));
    }

    // 3. POST api/products
    [HttpPost]
    public IActionResult Create(CreateProductDto dto)
    {
        var product = new Product
        {
            ProductName = dto.ProductName,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, ToDto(product));
    }

    // 4. PUT api/products/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateProductDto dto)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (product is null) return NotFound();

        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.CategoryId = dto.CategoryId;
        product.ImageUrl = dto.ImageUrl;
        product.IsActive = dto.IsActive;

        _context.SaveChanges();
        return NoContent();
    }

    // 5. PATCH api/products/{id}
    [HttpPatch("{id}")]
    public IActionResult Patch(int id, PatchProductDto dto)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (product is null) return NotFound();

        if (dto.ProductName is not null) product.ProductName = dto.ProductName;
        if (dto.Description is not null) product.Description = dto.Description;
        if (dto.Price is not null) product.Price = dto.Price.Value;
        if (dto.StockQuantity is not null) product.StockQuantity = dto.StockQuantity.Value;
        if (dto.CategoryId is not null) product.CategoryId = dto.CategoryId.Value;
        if (dto.ImageUrl is not null) product.ImageUrl = dto.ImageUrl;
        if (dto.IsActive is not null) product.IsActive = dto.IsActive.Value;

        _context.SaveChanges();
        return NoContent();
    }

    // 6. DELETE api/products/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (product is null) return NotFound();

        _context.Products.Remove(product);
        _context.SaveChanges();
        return NoContent();
    }
}