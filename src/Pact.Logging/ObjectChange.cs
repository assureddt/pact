namespace Pact.Logging;

public record ObjectChange(object OriginalValue, object NewValue, ObjectChange.ChangeType Change)
{
    public ObjectChange(object originalValue) : this(originalValue, originalValue, ChangeType.None)
    {
    }

    public enum ChangeType
    {
        None,
        Edit,
        New,
        Removed
    }
}
