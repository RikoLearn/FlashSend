using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");
            builder.HasKey(P => P.Id);
            builder.Property(P => P.Id).ValueGeneratedOnAdd();

            builder.HasOne(p => p.Paper).WithMany(x => x.Documents).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
