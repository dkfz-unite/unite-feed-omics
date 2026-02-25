namespace Unite.Omics.Feed.Web.Handlers;

public interface IHandler
{
    HandlerPriority Priority { get; }
    Task Prepare();
    Task Handle();
}