using FilmForumModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.Database;

public class UsersDatabaseContext : DbContext
{
    public UsersDatabaseContext(DbContextOptions<UsersDatabaseContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserToRole> UsersToRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserDiagnostics> UserDiagnostics { get; set; }
    public DbSet<RequestLog> RequestLogs { get; set; }
}