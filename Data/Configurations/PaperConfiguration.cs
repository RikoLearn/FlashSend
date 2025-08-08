using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    class PaperConfiguration : IEntityTypeConfiguration<Paper>
    {
        public void Configure(EntityTypeBuilder<Paper> builder)
        {
            builder.ToTable("Papers");
            builder.HasKey(P => P.Id);
            builder.Property(P => P.Id).ValueGeneratedOnAdd();

            builder.HasIndex(e => e.UniqueNumber)
                   .IsUnique();
        }
    }
}
