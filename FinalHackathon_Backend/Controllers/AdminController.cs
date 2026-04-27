using FinalHackathon_Backend.DTOs.Menu;
using FinalHackathon_Backend.Services;
using FOBackend.DTOs.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOBackend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ItemService _itemService;

        public AdminController(CategoryService categoryService, ItemService itemService)
        {
            _categoryService = categoryService;
            _itemService = itemService;
        }

    
        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _categoryService.CreateAsync(req.Name, req.Description);
            return CreatedAtAction(nameof(CreateCategory), new { id = result.CategoryId }, result);
        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] CreateCategoryRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _categoryService.UpdateAsync(id, req.Name, req.Description);
            if (result == null) return NotFound(new { message = $"Category {id} not found" });

            return Ok(result);
        }

        /// <summary>Delete a category</summary>
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Category {id} not found" });

            return NoContent();
        }

        // ─── ITEM ENDPOINTS ───────────────────────────────────────────────

        /// <summary>Add a new menu item</summary>
        [HttpPost("items")]
        public async Task<ActionResult<ItemDto>> CreateItem([FromBody] CreateItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _itemService.CreateAsync(dto);
            if (result == null) return BadRequest(new { message = "Invalid CategoryId — category not found" });

            return CreatedAtAction(nameof(CreateItem), new { itemId = result.ItemId }, result);
        }

        /// <summary>Update an existing menu item</summary>
        [HttpPut("items/{itemId}")]
        public async Task<ActionResult<ItemDto>> UpdateItem(int itemId, [FromBody] UpdateItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _itemService.UpdateAsync(itemId, dto);
            if (result == null) return NotFound(new { message = $"Item {itemId} not found or invalid CategoryId" });

            return Ok(result);
        }

        /// <summary>Delete a menu item</summary>
        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            var deleted = await _itemService.DeleteAsync(itemId);
            if (!deleted) return NotFound(new { message = $"Item {itemId} not found" });

            return NoContent();
        }
    }

    // ─── Inline request DTOs for category (avoids creating extra DTO file) ──
    public class CreateCategoryRequest
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}
