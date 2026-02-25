namespace Unite.Omics.Feed.Web.Handlers;

public abstract class Handler(HandlerPriority priority)
{
    public HandlerPriority Priority => priority;
    public abstract Task Handle();
    public abstract Task Prepare();
}