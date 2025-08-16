using Microsoft.EntityFrameworkCore;
using MyGames.Models;

namespace MyGames.Data;

public class MyGamesDbContext : DbContext{
    public MyGamesDbContext(DbContextOptions<MyGamesDbContext> options)
        : base(options) { }

    // EF Core will use these to perform CRUD operations on the database tables for you on the .
    public virtual DbSet<Game>? Games { get; set; }
    public virtual DbSet<Publisher>? Publishers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        // here we've overridden the method so that we can choose the name of our tables
        // since database tables are supposed to be singular, we name that way here
        // otherwise they'd be plural (e.g. Courses) in the database.
        modelBuilder.Entity<Game>().ToTable("Game");
        modelBuilder.Entity<Publisher>().ToTable("Publisher");
    }
}

