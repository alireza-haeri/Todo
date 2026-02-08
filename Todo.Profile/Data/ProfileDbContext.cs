using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todo.Profile.Models;

namespace Todo.Profile.Data;

public class ProfileDbContext : IdentityDbContext<ApplicationUser>
{
    public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var user = builder.Entity<ApplicationUser>();
        user.ToTable("Users");
        user.Property(u => u.Email).HasMaxLength(256);
        user.Property(u => u.UserName).HasMaxLength(256);
    }
}