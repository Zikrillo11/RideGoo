namespace RideGoo.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

    public void SoftDelete()
    {
        IsDeleted = true;
        MarkAsUpdated();
    }

    public void Restore()
    {
        IsDeleted = false;
        MarkAsUpdated();
    }
}