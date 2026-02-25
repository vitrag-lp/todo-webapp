using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain;

namespace ToDoApp.Domain
{
    public interface ITodoContext
    {
        DbSet<TodoItem> TodoItems { get; set; }
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync();
    }
}
