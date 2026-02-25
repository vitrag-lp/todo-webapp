using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApp.Application.Interface;
using ToDoApp.Application.Models;


namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodoItemsController : ControllerBase
    {
        private readonly IToDoService _todoService;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(IToDoService todoService, ILogger<TodoItemsController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoIteResponseModel>>> GetTodoItems()
        {
            var userId = GetUserId();

            var todoItem = await _todoService.GetToDosByUserIdAsync(userId);
            if (todoItem.Data == null && !string.IsNullOrEmpty(todoItem.Message))
            {
                return NotFound(new { message = todoItem.Message });
            }
            if (todoItem.Data == null && !string.IsNullOrEmpty(todoItem.Error))
            {
                return StatusCode(500, "An error occurred while retrieving the todo item");
            }
            return Ok(todoItem.Data);
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoIteResponseModel>> GetTodoItem(int id)
        {
            var userId = GetUserId();
            var todoItem = await _todoService.GetToDosBydAsync(id, userId);

            if (todoItem.Data == null && !string.IsNullOrEmpty(todoItem.Message))
            {
                return NotFound(new { message = todoItem.Message });
            }

            if (todoItem.Data == null && !string.IsNullOrEmpty(todoItem.Error))
            {
                return StatusCode(500, "An error occurred while retrieving the todo item");
            }

            return Ok(todoItem.Data);
        }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoIteResponseModel>> CreateTodoItem(CreateTodoRequestModel createTodoRequest)
        {
            if (string.IsNullOrWhiteSpace(createTodoRequest.Title))
            {
                return BadRequest(new { message = "Title is required" });
            }

            var userId = GetUserId();
            var response = await _todoService.CreatToDoAsync(createTodoRequest, userId);
            if (response.Data == null && !string.IsNullOrEmpty(response.Message))
            {
                return NotFound(new { message = response.Message });
            }

            if (response.Data == null && !string.IsNullOrEmpty(response.Error))
            {
                _logger.LogError("Error creating todo item");
                return StatusCode(500, "An error occurred while retrieving the todo item");
            }

            return CreatedAtAction(nameof(GetTodoItem), new { id = response.Data.Id }, response.Data);
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, UpdateTodoRequestModel updateTodoRequest)
        {
            if (updateTodoRequest.Title != null)
            {
                if (string.IsNullOrWhiteSpace(updateTodoRequest.Title))
                {
                    return BadRequest(new { message = "Title cannot be empty" });
                }
            }
            var userId = GetUserId();
            var response = await _todoService.UpdateToDoAsync(updateTodoRequest, id, userId);
            if (response.Data == null && !string.IsNullOrEmpty(response.Message))
            {
                return NotFound(new { message = response.Message });
            }

            if (response.Data == null && !string.IsNullOrEmpty(response.Error))
            {
                _logger.LogError("Error updating todo item with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the todo item");
            }

            return NoContent();
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var userId = GetUserId();
            var response = await _todoService.DeleteTodoItemByIdAsync(id, userId);
            if (response.Data)
            {
                return NoContent();
            }
            if (!string.IsNullOrEmpty(response.Message))
            {
                return NotFound(new { message = response.Message });
            }
            if (!string.IsNullOrEmpty(response.Error))
            {
                _logger.LogError("Error updating todo item with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the todo item");
            }

            return NoContent();
        }

        // DELETE: api/TodoItems
        [HttpDelete]
        public async Task<IActionResult> DeleteAllTodoItems()
        {
            var userId = GetUserId();
            var response = await _todoService.DeleteTodoItemsAsync(userId);

            if (response.Data)
            {
                return NoContent();
            }
            if (!string.IsNullOrEmpty(response.Error))
            {
                _logger.LogError("Error updating todo item with ID {userId}", userId);
                return StatusCode(500, "An error occurred while deleting the todo item");
            }

            return NoContent();
        }

    }
}
