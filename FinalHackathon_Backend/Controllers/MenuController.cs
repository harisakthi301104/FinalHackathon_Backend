using FinalHackathon_Backend.DTOs.Menu;
using FinalHackathon_Backend.Services;
using FOBackend.DTOs.Menu;
using Microsoft.AspNetCore.Mvc;

namespace FinalHackathon_Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class MenuController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ItemService _itemService;

        public MenuController(CategoryService categoryService, ItemService itemService)
        {
            _categoryService = categoryService;
            _itemService = itemService;
        }

        // ─── CATEGORY (Public) ────────────────────────────────────────────

        /// <summary>Get all categories</summary>
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        // ─── ITEMS (Public) ───────────────────────────────────────────────

        /// <summary>Get all items — supports optional ?categoryId=&amp;search= query params</summary>
        [HttpGet("items")]
        public async Task<ActionResult<List<ItemDto>>> GetItems(
            [FromQuery] int? categoryId = null,
            [FromQuery] string? search = null)
        {
            var items = await _itemService.GetAllAsync(categoryId, search);
            return Ok(items);
        }

        /// <summary>Get a single item by ID</summary>
        [HttpGet("items/{itemId}")]
        public async Task<ActionResult<ItemDto>> GetItem(int itemId)
        {
            var item = await _itemService.GetByIdAsync(itemId);
            if (item == null) return NotFound(new { message = $"Item {itemId} not found" });

            return Ok(item);
        }
    }
}
