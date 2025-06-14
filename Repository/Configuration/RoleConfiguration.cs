using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole() { Id = "d1205a38-eb19-4b90-ad5d-bc7d9de7d61b", Name = "Manager", NormalizedName = "MANAGER" },
            new() { Id = "69aed902-5a65-4fec-923f-4ab8b92842c9", Name = "Administrator", NormalizedName = "ADMINISTRATOR" }
        );
    }
}