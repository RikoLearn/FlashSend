using Data.Configurations;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public partial class Context : DbContext
    {
        public Context() { }
        public Context(DbContextOptions<Context> options) : base(options) { }

        public virtual DbSet<Paper> Papers { get; set; }

        public virtual DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaperConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
