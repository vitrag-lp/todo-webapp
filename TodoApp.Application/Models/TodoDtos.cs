namespace ToDoApp.Application.Models
{
    public class Result<T>
    {
        public string? Message { get; set; }
        public T? Data { get; set; }
        
        public string? Error { get; set; }
    }

    public class CreateTodoRequestModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateTodoRequestModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
    }

    public class TodoIteResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
