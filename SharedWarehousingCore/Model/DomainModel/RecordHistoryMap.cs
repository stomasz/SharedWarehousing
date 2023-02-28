using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharedWarehousingCore.Model.DomainModel;

public class RecordHistoryMap : IEntityTypeConfiguration<RecordHistory>
{
    public void Configure(EntityTypeBuilder<RecordHistory> builder)
    {
        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.Created)
            .HasForeignKey(x => x.CreatedByUserId)
            .IsRequired(false);
        
        builder.HasOne(x => x.UpdatedByUser)
            .WithMany(x => x.Updated)
            .HasForeignKey(x => x.UpdatedByUserId)
            .IsRequired(false);
        
        // builder.HasOne(x => x.Offer)
        //     .WithMany(x => x.History)
        //     .HasForeignKey(x => x.OfferId)
        //     .IsRequired(false);
        //
    }
}