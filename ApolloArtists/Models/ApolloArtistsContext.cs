using Microsoft.EntityFrameworkCore;

namespace ApolloArtists.Models
{
    public class ApolloArtistsContext : DbContext
    {
        public ApolloArtistsContext(DbContextOptions<ApolloArtistsContext> options) : base(options) {}

        public virtual DbSet<Artists> Artists { get; set; }
        public virtual DbSet<LikedArtists> LikedArtists { get; set; }
        public virtual DbSet<PassedArtists> PassedArtists { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artists>(entity =>
                                         {
                                             entity.HasKey(e => e.ArtistId);

                                             entity.Property(e => e.Href)
                                                   .IsRequired()
                                                   .HasMaxLength(500);

                                             entity.Property(e => e.Image)
                                                   .IsRequired()
                                                   .HasMaxLength(500);

                                             entity.Property(e => e.Name)
                                                   .IsRequired()
                                                   .HasMaxLength(500);

                                             entity.HasIndex(e => e.Href)
                                                   .HasName("UC_Artists_Href")
                                                   .IsUnique();
                                         });


            modelBuilder.Entity<Users>(entity =>
                                       {
                                           entity.HasKey(e => e.UserId);

                                           entity.Property(e => e.Email)
                                                 .IsRequired()
                                                 .HasMaxLength(500);

                                           entity.Property(e => e.Password)
                                                 .IsRequired()
                                                 .HasMaxLength(500);

                                           entity.HasIndex(e => e.Email)
                                                 .HasName("UC_Users_Email")
                                                 .IsUnique();
                                       });

            modelBuilder.Entity<LikedArtists>(entity =>
                                              {
                                                  entity.HasKey(e => new {e.UserId, e.ArtistId});

                                                  entity.HasIndex(e => e.ArtistId);

                                                  entity.HasIndex(e => e.UserId);

                                                  entity.HasOne(d => d.Artist)
                                                        .WithMany(p => p.LikedArtists)
                                                        .HasForeignKey(d => d.ArtistId);

                                                  entity.HasOne(d => d.User)
                                                        .WithMany(p => p.LikedArtists)
                                                        .HasForeignKey(d => d.UserId);
                                              });

            modelBuilder.Entity<PassedArtists>(entity =>
                                               {
                                                   entity.HasKey(e => new {e.UserId, e.ArtistId});

                                                   entity.HasIndex(e => e.ArtistId);

                                                   entity.HasIndex(e => e.UserId);

                                                   entity.HasOne(d => d.Artist)
                                                         .WithMany(p => p.PassedArtists)
                                                         .HasForeignKey(d => d.ArtistId);

                                                   entity.HasOne(d => d.User)
                                                         .WithMany(p => p.PassedArtists)
                                                         .HasForeignKey(d => d.UserId);
                                               });
        }
    }
}