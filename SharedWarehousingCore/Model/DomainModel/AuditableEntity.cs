namespace SharedWarehousingCore.Model.DomainModel;

public abstract class AuditableEntity
{
    public ICollection<RecordHistory> History { get; set; }
}