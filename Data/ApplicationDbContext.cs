using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YouWeMovie.Models; // This is the namespace containing your models

namespace YouWeMovie.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    
    public DbSet<Content> Contents { get; set; }
    public DbSet<Review> Reviews { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}