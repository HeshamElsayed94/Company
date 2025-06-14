using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasData
             (
             new Company
             {
                 Id = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                 Name = "IT_Solutions Ltd",
                 Address = "583 Wall Dr. Gwynn Oak, MD 21207",
                 Country = "USA"
             },
             new Company
             {
                 Id = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                 Name = "Admin_Solutions Ltd",
                 Address = "312 Forest Avenue, BF 923",
                 Country = "USA"
             }
             );
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole() { Id = "d1205a38-eb19-4b90-ad5d-bc7d9de7d61b", Name = "Manager", NormalizedName = "MANAGER" },
            new() { Id = "69aed902-5a65-4fec-923f-4ab8b92842c9", Name = "Administrator", NormalizedName = "ADMINISTRATOR" }
        );
    }
}