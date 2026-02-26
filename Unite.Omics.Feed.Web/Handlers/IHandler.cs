namespace Unite.Omics.Feed.Web.Handlers;

public interface IHandler
{
    Task Prepare();
    Task Handle();
}