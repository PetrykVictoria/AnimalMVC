using Microsoft.EntityFrameworkCore;

namespace AnimalMVC.Models;

public partial class DbAnimalContext : DbContext
{
    public DbSet<Animal> Animals { get; set; }
    public DbSet<Exhibition> Exhibitions { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Species> Species { get; set; }
    public DbSet<AnimalExhibition> AnimalExhibitions { get; set; }
    public DbAnimalContext(DbContextOptions<DbAnimalContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseSqlServer("Server=LAPTOP-8CSE747M\\SQLEXPRESS; Database=AnimalMVC; Trusted_Connection=True; Trust Server Certificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OwnerId).HasMaxLength(50);
            entity.Property(e => e.SpeciesId).HasMaxLength(50);
            entity.Property(e => e.Breed).HasMaxLength(50);
            entity.Property(e => e.Age).HasColumnType("smallint");
            entity.Property(e => e.Image).HasColumnType("varbinary(max)");

            entity.HasOne(d => d.Owner)
                .WithMany(p => p.Animals)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Animals_Owners");

            entity.HasOne(d => d.Species)
                .WithMany(p => p.Animals)
                .HasForeignKey(d => d.SpeciesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Animals_Species");
        });

        modelBuilder.Entity<Exhibition>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Date).HasColumnType("smalldatetime");
            entity.Property(e => e.Location).HasMaxLength(100);
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(50);
        });

        modelBuilder.Entity<Species>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<AnimalExhibition>(entity =>
        {
            entity.HasKey(ae => new { ae.AnimalId, ae.ExhibitionId }).HasName("PK_AnimalExhibitions");

            entity.HasOne(ae => ae.Animal)
                .WithMany(a => a.AnimalExhibitions)
                .HasForeignKey(ae => ae.AnimalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnimalExhibitions_Animals");

            entity.HasOne(ae => ae.Exhibition)
                .WithMany(e => e.AnimalExhibitions)
                .HasForeignKey(ae => ae.ExhibitionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnimalExhibitions_Exhibitions");

            entity.Property(e => e.RegistrationDate).HasColumnType("smalldatetime");
        });

    }
}
