using Examples.Data.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Examples.Data {
    public class ApplicationDbContext : DbContext {
        public virtual DbSet<Phone> Phones { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Phone>(entity => {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedOnAdd();

                entity.Property(m => m.Name).IsRequired();
                entity.Property(m => m.Manufacturer).IsRequired();
            });
        }
    }
}
