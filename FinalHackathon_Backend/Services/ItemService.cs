using FinalHackathon_Backend.Data;
using FinalHackathon_Backend.DTOs.Menu;
using FinalHackathon_Backend.Models;
using FOBackend.DTOs.Menu;
using Microsoft.EntityFrameworkCore;

namespace FinalHackathon_Backend.Services
{
    public class ItemService
    {
        private readonly AppDbContext _context;

        public ItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ItemDto>> GetAllAsync(int? categoryId = null, string? search = null)
        {
            var query = _context.Items
                .Include(i => i.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(i => i.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.Name.Contains(search) || i.Description.Contains(search));

            return await query
                .OrderBy(i => i.Name)
                .Select(i => MapToDto(i))
                .ToListAsync();
        }

        public async Task<ItemDto?> GetByIdAsync(int itemId)
        {
            var item = await _context.Items
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.ItemId == itemId);

            return item == null ? null : MapToDto(item);
        }

        public async Task<ItemDto?> CreateAsync(CreateItemDto dto)
        {
            // Validate category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists) return null;

            var item = new Item
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl.Trim(),
                IsAvailable = true,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            // Reload with navigation
            await _context.Entry(item).Reference(i => i.Category).LoadAsync();
            return MapToDto(item);
        }

        public async Task<ItemDto?> UpdateAsync(int itemId, UpdateItemDto dto)
        {
            var item = await _context.Items
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.ItemId == itemId);

            if (item == null) return null;

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists) return null;

            item.Name = dto.Name.Trim();
            item.Description = dto.Description.Trim();
            item.Price = dto.Price;
            item.StockQuantity = dto.StockQuantity;
            item.ImageUrl = dto.ImageUrl.Trim();
            item.IsAvailable = dto.IsAvailable;
            item.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            // Refresh category nav property if changed
            await _context.Entry(item).Reference(i => i.Category).LoadAsync();
            return MapToDto(item);
        }

        public async Task<bool> DeleteAsync(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null) return false;

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ItemDto MapToDto(Item i) => new ItemDto
        {
            ItemId = i.ItemId,
            Name = i.Name,
            Description = i.Description,
            Price = i.Price,
            StockQuantity = i.StockQuantity,
            ImageUrl = i.ImageUrl,
            IsAvailable = i.IsAvailable,
            CategoryId = i.CategoryId,
            CategoryName = i.Category?.Name ?? string.Empty,
            CreatedAt = i.CreatedAt
        };
    }
}
