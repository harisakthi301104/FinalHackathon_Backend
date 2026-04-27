using FOBackend.DTOs.Inventory;
using FOBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Get all items with current stock levels
    /// </summary>
    [HttpGet("stock")]
    [AllowAnonymous]
    public async Task<ActionResult<List<StockDto>>> GetAllStock()
    {
        var stock = await _inventoryService.GetAllStockAsync();
        return Ok(stock);
    }

    /// <summary>
    /// Get stock for a specific item
    /// </summary>
    [HttpGet("stock/{itemId}")]
    [AllowAnonymous]
    public async Task<ActionResult<StockDto>> GetStockByItemId(int itemId)
    {
        var stock = await _inventoryService.GetStockByItemIdAsync(itemId);
        if (stock == null)
            return NotFound(new { message = "Item not found" });

        return Ok(stock);
    }
}
