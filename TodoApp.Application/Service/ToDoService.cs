using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Interface;
using ToDoApp.Application.Models;
using ToDoApp.Domain;

namespace TodoApp.Application.Service
{
    public class ToDoService : IToDoService
    {
        private readonly ITodoContext _context;
        private readonly ILogger<ToDoService> _logger;

        public ToDoService(ITodoContext context, ILogger<ToDoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<TodoIteResponseModel>> CreatToDoAsync(CreateTodoRequestModel createTodoRequest, int userId)
        {
            try
            {
                var todoItem = new TodoItem
                {
                    Title = createTodoRequest.Title.Trim(),
                    Description = createTodoRequest.Description?.Trim(),
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId
                };

                _context.TodoItems.Add(todoItem);
                var dbResult = await _context.SaveChangesAsync();

                var todoDto = new TodoIteResponseModel
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    IsCompleted = todoItem.IsCompleted,
                    CreatedAt = todoItem.CreatedAt,
                    CompletedAt = todoItem.CompletedAt
                };

                return new Result<TodoIteResponseModel>
                {
                    Message = dbResult > 0 ? "Created" : "Not Created",
                    Data = todoDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo item");

                return new Result<TodoIteResponseModel>
                {
                    Data = null,
                    Error = "An error occurred while creating the todo item"
                };
            }
        }

        public async Task<Result<bool>> DeleteTodoItemByIdAsync(int id, int userId)
        {
            try
            {
                var todoItem = await _context.TodoItems
                    .Where(t => t.UserId == userId && t.Id == id)
                    .FirstOrDefaultAsync();

                if (todoItem == null)
                {
                    return new Result<bool>()
                    {
                        Message = $"Todo item with ID {id} not found"
                    };
                }

                _context.TodoItems.Remove(todoItem);
                var dbResult = await _context.SaveChangesAsync();
                return new Result<bool>()
                {
                    Message = dbResult > 0 ? "Deleted" : "Not Deleted",
                    Data = dbResult > 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo item by id" + id + "UserId:" + userId);
                return new Result<bool>
                {
                    Data = false,
                    Error = "An error occurred while deleting the todo item"
                };
            }
        }

        public async Task<Result<bool>> DeleteTodoItemsAsync(int userId)
        {
            try
            {
                var allItems = await _context.TodoItems
                    .Where(t => t.UserId == userId)
                    .ToListAsync();

                if (!allItems.Any())
                {
                    return new Result<bool>()
                    {
                        Data = true,
                        Message = $"Todos item with UserID {userId} not found"
                    };
                }

                _context.TodoItems.RemoveRange(allItems);

                var dbResult = await _context.SaveChangesAsync();
                return new Result<bool>()
                {
                    Message = dbResult > 0 ? "Deleted" : "Not Deleted",
                    Data = dbResult > 0
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo item by UserId:" + userId);
                return new Result<bool>
                {
                    Data = false,
                    Error = "An error occurred while deleting the todo item"
                };
            }
        }

        public async Task<Result<TodoIteResponseModel>> GetToDosBydAsync(int id, int userId)
        {
            try
            {
                var todoItem = await _context.TodoItems
                    .Where(t => t.UserId == userId && t.Id == id)
                    .FirstOrDefaultAsync();

                if (todoItem == null)
                {
                    return new Result<TodoIteResponseModel>()
                    {
                        Message = $"Todo item with ID {id} not found"
                    };
                }

                var todoDto = new TodoIteResponseModel
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    IsCompleted = todoItem.IsCompleted,
                    CreatedAt = todoItem.CreatedAt,
                    CompletedAt = todoItem.CompletedAt
                };

                return new Result<TodoIteResponseModel>()
                {
                    Data = todoDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo item by UserId:" + userId);
                return new Result<TodoIteResponseModel>
                {
                    Data = null,
                    Error = "An error occurred while deleting the todo item"
                };
            }
        }

        public async Task<Result<IReadOnlyCollection<TodoIteResponseModel>>> GetToDosByUserIdAsync(int userId)
        {
            try
            {
                var todoItem = await _context.TodoItems
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => new TodoIteResponseModel
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        IsCompleted = t.IsCompleted,
                        CreatedAt = t.CreatedAt,
                        CompletedAt = t.CompletedAt
                    })
                    .ToListAsync();

                if (todoItem == null)
                {
                    return new Result<IReadOnlyCollection<TodoIteResponseModel>>()
                    {
                        Data = new List<TodoIteResponseModel>(),
                        Message = $"No Todo item found"
                    };
                }

                return new Result<IReadOnlyCollection<TodoIteResponseModel>>()
                {
                    Data = todoItem
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo item by UserId:" + userId);
                return new Result<IReadOnlyCollection<TodoIteResponseModel>>
                {
                    Data = null,
                    Error = "An error occurred while deleting the todo item"
                };
            }
        }

        public async Task<Result<TodoIteResponseModel?>> UpdateToDoAsync(UpdateTodoRequestModel updateTodoRequest, int id, int userId)
        {
            try
            {
                var todoItem = await _context.TodoItems
                    .Where(t => t.UserId == userId && t.Id == id)
                    .FirstOrDefaultAsync();

                if (todoItem == null)
                {
                    return new Result<TodoIteResponseModel?>()
                    {
                        Message = $"Todo item with ID {id} not found"
                    };
                }

                if (updateTodoRequest.Title != null)
                {
                    todoItem.Title = updateTodoRequest.Title.Trim();
                }

                if (updateTodoRequest.Description != null)
                {
                    todoItem.Description = updateTodoRequest.Description.Trim();
                }

                if (updateTodoRequest.IsCompleted.HasValue)
                {
                    todoItem.IsCompleted = updateTodoRequest.IsCompleted.Value;
                    todoItem.CompletedAt = updateTodoRequest.IsCompleted.Value ? DateTime.UtcNow : null;
                }

                var dbUpdated = await _context.SaveChangesAsync();

                var updatedTodoDto = new TodoIteResponseModel
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    IsCompleted = todoItem.IsCompleted,
                    CreatedAt = todoItem.CreatedAt,
                    CompletedAt = todoItem.CompletedAt
                };

                return new Result<TodoIteResponseModel?>()
                {
                    Message = dbUpdated > 0 ? "Created" : "Not Created",
                    Data = updatedTodoDto
                };
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!await TodoItemExists(id, userId))
                {
                    return new Result<TodoIteResponseModel?>()
                    {
                        Message = $"Todo item with ID {id} not found"
                    };
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo item by UserId:" + userId);
                return new Result<TodoIteResponseModel?>
                {
                    Data = null,
                    Error = "An error occurred while deleting the todo item"
                };
            }
        }

        private async Task<bool> TodoItemExists(int id, int userId)
        {
            return await _context.TodoItems.AnyAsync(e => e.Id == id && e.UserId == userId);
        }
    }
}
