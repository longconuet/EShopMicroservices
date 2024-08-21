using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ordering.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(
            customerId => customerId.Value,
            dbId => CustomerId.Of(dbId));

        builder
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(1000);

        builder
            .Property(c => c.Email)
            .HasMaxLength(255);

        builder
            .HasIndex(x => x.Email)
            .IsUnique();
    }
}
