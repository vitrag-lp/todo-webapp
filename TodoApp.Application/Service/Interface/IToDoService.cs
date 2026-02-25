using ToDoApp.Application.Models;

namespace TodoApp.Application.Interface
{
    public interface IToDoService
    {
        Task<Result<IReadOnlyCollection<TodoIteResponseModel>>> GetToDosByUserIdAsync(int userId);
        Task<Result<TodoIteResponseModel>> GetToDosBydAsync(int id, int userId);

        Task<Result<TodoIteResponseModel>> CreatToDoAsync(CreateTodoRequestModel createTodoRequest, int userId);
        Task<Result<TodoIteResponseModel?>> UpdateToDoAsync(UpdateTodoRequestModel updateTodoRequest, int id, int userId);

        Task<Result<bool>> DeleteTodoItemByIdAsync(int id, int userId);
        Task<Result<bool>> DeleteTodoItemsAsync(int userId);

    }
}
