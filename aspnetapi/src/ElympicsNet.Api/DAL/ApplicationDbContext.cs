using ElympicsNet.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace ElympicsNet.Api.DAL;

internal sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // here can be entity configuration
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Podcast> Podcasts { get; set; }
}