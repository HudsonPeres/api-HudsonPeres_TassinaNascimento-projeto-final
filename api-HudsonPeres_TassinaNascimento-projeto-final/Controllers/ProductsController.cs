using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_HudsonPeres_TassinaNascimento_projeto_final.Data;
using api_HudsonPeres_TassinaNascimento_projeto_final.Models;
using api_HudsonPeres_TassinaNascimento_projeto_final.Services;
using Microsoft.AspNetCore.Authorization;

namespace api_HudsonPeres_TassinaNascimento_projeto_final.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HybridCacheService _hybridCache;
    

    // Injeção do HybridCacheService
    public ProductsController(AppDbContext context, HybridCacheService hybridCache)
    {
        _context = context;
        _hybridCache = hybridCache;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _hybridCache.GetOrSetAsync(
            "products_all",
            async () => await _context.Products.ToListAsync(),
            memoryExpiry: TimeSpan.FromSeconds(30),
            redisExpiry: TimeSpan.FromMinutes(5));

        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        string cacheKey = $"product_{id}";
        var product = await _hybridCache.GetOrSetAsync(
            cacheKey,
            async () => await _context.Products.FindAsync(id),
            memoryExpiry: TimeSpan.FromSeconds(30),
            redisExpiry: TimeSpan.FromMinutes(5));

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Invalida o cache da lista
        await _hybridCache.RemoveAsync("products_all");

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest();

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // Invalida os caches da lista e do produto específico
        await _hybridCache.RemoveAsync("products_all");
        await _hybridCache.RemoveAsync($"product_{id}");

        return NoContent();
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        // Invalida os caches
        await _hybridCache.RemoveAsync("products_all");
        await _hybridCache.RemoveAsync($"product_{id}");

        return NoContent();
    }
}