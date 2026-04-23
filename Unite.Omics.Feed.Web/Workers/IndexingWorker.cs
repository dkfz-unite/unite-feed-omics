using Unite.Omics.Feed.Web.Handlers.Indexing;

namespace Unite.Omics.Feed.Web.Workers;

public class IndexingWorker : Worker
{
    public IndexingWorker(
        GenesIndexingHandler genesIndexingHandler,
        ProteinsIndexingHandler proteinsIndexingHandler,
        SmsIndexingHandler smsIndexingHandler,
        CnvsIndexingHandler cnvsIndexingHandler,
        SvsIndexingHandler svsIndexingHandler,
        CnvProfilesIndexingHandler cnvProfilesIndexingHandler,
        ILogger<IndexingWorker> logger) : base(logger)
    {
        _handlers = [
            genesIndexingHandler,
            proteinsIndexingHandler,
            smsIndexingHandler,
            cnvsIndexingHandler,
            svsIndexingHandler,
            cnvProfilesIndexingHandler
        ];
    }
}
