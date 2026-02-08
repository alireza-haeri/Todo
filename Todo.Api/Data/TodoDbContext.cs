using Microsoft.EntityFrameworkCore;
using Todo.Api.Models;

namespace Todo.Api.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var todo = modelBuilder.Entity<TodoItem>();
        todo.ToTable("Todos");
        todo.HasKey(t => t.Id);
        todo.Property(t => t.Title).HasMaxLength(200).IsRequired();
        todo.Property(t => t.IsCompleted).HasDefaultValue(false);
        todo.Property(t => t.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        todo.Property(t => t.UserId).HasMaxLength(450);
    }
}