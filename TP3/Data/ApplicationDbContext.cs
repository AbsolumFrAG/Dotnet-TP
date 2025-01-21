using Microsoft.EntityFrameworkCore;
using TP3.Models;

namespace TP3.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
}