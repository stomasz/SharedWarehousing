using SharedWarehousingCore.Model.IdentityModel;

namespace SharedWarehousingCore.Model.DomainModel;

public class RecordHistory
{
    public int Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public int? CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public int? UpdatedByUserId { get; set; }
    public AppUser UpdatedByUser { get; set; }
    public string LegacyRecord { get; set; }
}