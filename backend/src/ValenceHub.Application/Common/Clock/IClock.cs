namespace ValenceHub.Application.Common.Clock;

public interface IClock
{
    DateTime UtcNow { get; }
}
