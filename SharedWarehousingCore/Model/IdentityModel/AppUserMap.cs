using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharedWarehousingCore.Model.IdentityModel;

public class AppUserMap : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        // builder.HasOne(x => x)
        //     .WithMany(x => x)
    }
}