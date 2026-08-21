using Microsoft.EntityFrameworkCore;
using Project.DbModels;

namespace Folivora.Scaffold;

public class CartridgeDbContext : DbContext
{
    private readonly IConfiguration _conf;

    public CartridgeDbContext()
    {
    }

    public CartridgeDbContext(IConfiguration conf) => _conf = conf;

    public DbSet<Office> Offices { get; set; }
    public DbSet<Cartridge> Cartridges { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Printer> Printers { get; set; }
    public DbSet<Changes> Change { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<ModelCompatibility> ModelCompatibilities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_conf["DBConnectionString"]);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Office>()
            .ToTable("Office", "public").HasKey(e => e.Id);

        modelBuilder.Entity<Cartridge>()
        .ToTable("Cartridge", "public")
        .HasKey(c => c.IdCr);

        modelBuilder.Entity<Cartridge>()
       .Property(e => e.StatusCr)
       .HasConversion<string>();

        modelBuilder.Entity<Request>().ToTable("Request", "public").HasKey(e => e.IdZv);
        modelBuilder.Entity<Request>().Property(e => e.StatusZv).HasConversion<string>();

        modelBuilder.Entity<Printer>()
            .ToTable("Printer", "public").HasKey(e => e.IdPrint);

        modelBuilder.Entity<Changes>()
            .ToTable("Changes", "public").HasKey(e => e.IdChanges);

        modelBuilder.Entity<Vendor>()
            .ToTable("Vendor", "public").HasKey(e => e.Id);

        modelBuilder.Entity<Model>()
            .ToTable("Model", "public").HasKey(m => m.Id);
        modelBuilder.Entity<Model>().Property(e => e.Type).HasConversion<int>();

        modelBuilder.Entity<ModelCompatibility>()
            .ToTable("ModelCompatibility", "public").HasKey(x => new { x.PrinterModelId, x.CartridgeModelId });

    }
}
