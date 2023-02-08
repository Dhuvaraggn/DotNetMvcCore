
using Microsoft.EntityFrameworkCore;
using SelfTraining.Models;

namespace SelfTraining.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Mail> Mails { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mail>().ToTable("mail");  
    }
}

