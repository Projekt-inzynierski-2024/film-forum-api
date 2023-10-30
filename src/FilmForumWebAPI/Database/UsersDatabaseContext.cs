using FilmForumModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.Database;

public class UsersDatabaseContext : DbContext
{
    public UsersDatabaseContext(DbContextOptions<UsersDatabaseContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}