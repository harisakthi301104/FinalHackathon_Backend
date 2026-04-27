using FOBackend.Data;
using FOBackend.DTOs.Inventory;
using Microsoft.EntityFrameworkCore;

namespace FOBackend.Services;

public interface IInventoryService
{
    Task<List<StockDto>> GetAllStockAsync();
    Task<StockDto> GetStockByItemIdAsync(int itemId);
}

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _context;

    public InventoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockDto>> GetAllStockAsync()
    {
        return await _context.Items
            .Include(i => i.Category)
            .Select(i => new StockDto
            {
                ItemId = i.ItemId,
                ItemName = i.Name,
                Category = i.Category.Name,
                Price = i.Price,
                StockQuantity = i.StockQuantity,
                IsAvailable = i.IsAvailable,
                ImageUrl = i.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<StockDto> GetStockByItemIdAsync(int itemId)
    {
        return await _context.Items
            .Include(i => i.Category)
            .Where(i => i.ItemId == itemId)
            .Select(i => new StockDto
            {
                ItemId = i.ItemId,
                ItemName = i.Name,
                Category = i.Category.Name,
                Price = i.Price,
                StockQuantity = i.StockQuantity,
                IsAvailable = i.IsAvailable,
                ImageUrl = i.ImageUrl
            })
            .FirstOrDefaultAsync();
    }
}
