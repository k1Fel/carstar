using System.Security.Claims;
using api.Data;
using api.Models;
using api.DTO.Product;
using api.Mappers.ProductMappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/favorites")]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetAccountId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var accountId = GetAccountId();
            var favorites = await _context.Favorites
                .Where(f => f.AccountId == accountId)
                .Include(f => f.Product!)
                    .ThenInclude(p => p.ProductCategories!)
                        .ThenInclude(pc => pc.Category)
                .Select(f => f.Product!)
                .ToListAsync();

            return Ok(favorites.Select(p => p.ToProductResponseDto()));
        }

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> AddFavorite(int productId)
        {
            var accountId = GetAccountId();

            var exists = await _context.Favorites
                .AnyAsync(f => f.AccountId == accountId && f.ProductId == productId);

            if (exists)
                return Ok(new { message = "Вже в улюблених" });

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound(new { message = "Товар не знайдено" });

            _context.Favorites.Add(new Favorite
            {
                AccountId = accountId,
                ProductId = productId
            });
            await _context.SaveChangesAsync();

            return Ok(new { message = "Додано до улюблених" });
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> RemoveFavorite(int productId)
        {
            var accountId = GetAccountId();
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.AccountId == accountId && f.ProductId == productId);

            if (favorite == null)
                return NotFound(new { message = "Не знайдено в улюблених" });

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Видалено з улюблених" });
        }

        [HttpGet("ids")]
        public async Task<IActionResult> GetFavoriteIds()
        {
            var accountId = GetAccountId();
            var ids = await _context.Favorites
                .Where(f => f.AccountId == accountId)
                .Select(f => f.ProductId)
                .ToListAsync();

            return Ok(ids);
        }
    }
}