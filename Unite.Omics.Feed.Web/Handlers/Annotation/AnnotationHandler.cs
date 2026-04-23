namespace Unite.Omics.Feed.Web.Handlers.Annotation;

public abstract class AnnotationHandler : IHandler
{
    public void Prepare() {}
    public abstract void Handle();
}
