using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharedWarehousingCore.Model.IdentityModel;

public class AppRoleMap : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.HasData(new AppRole
        {
            Id = 1,
            Name = "USER",
        });
    }
}