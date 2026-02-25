namespace Unite.Omics.Feed.Web.Handlers;

public abstract class Handler(HandlerPriority priority)
{
    public HandlerPriority Priority => priority;
    public abstract Task Handle();

    public virtual Task Prepare()
    {
        return Task.CompletedTask;
    }
}