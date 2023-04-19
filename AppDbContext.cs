using Bogus;
using Microsoft.EntityFrameworkCore;

namespace WriteQueryTos3;

public sealed class AppDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(databaseName: "AppDb");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cars = new Faker<Car>()
            .RuleFor(x => x.Id, _ => Guid.NewGuid())
            .RuleFor(x => x.Make, f => f.Vehicle.Manufacturer())
            .RuleFor(x => x.Model, f => f.Vehicle.Model());

        modelBuilder.Entity<Car>().HasData(cars.GenerateBetween(Consts.MinCars, Consts.MaxCars));
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Car> Cars { get; set; } = default!;
}