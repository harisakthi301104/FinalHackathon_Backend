

using FinalHackathon_Backend.Data;
using FinalHackathon_Backend.DTOs.Menu;
using FinalHackathon_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalHackathon_Backend.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .Select(c => MapToDto(c))
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category == null ? null : MapToDto(category);
        }

        public async Task<CategoryDto> CreateAsync(string name, string description)
        {
            var category = new Category
            {
                Name = name.Trim(),
                Description = description.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateAsync(int id, string name, string description)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            category.Name = name.Trim();
            category.Description = description.Trim();

            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        private static CategoryDto MapToDto(Category c) => new CategoryDto
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            Description = c.Description,
            CreatedAt = c.CreatedAt
        };
    }
}
