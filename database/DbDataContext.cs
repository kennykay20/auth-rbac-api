using crud_api.Models;
using Microsoft.EntityFrameworkCore;

namespace crud_api.database;

public class DbDataContext(DbContextOptions<DbDataContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<ImageFile> Images { get; set; }
}