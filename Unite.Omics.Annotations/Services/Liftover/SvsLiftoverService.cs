using Unite.Omics.Annotations.Clients.Ensembl;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;

namespace Unite.Omics.Annotations.Services.Liftover;

public class SvsLiftoverService
{
    private readonly EnsemblLiftoverApiClient _client;

    public SvsLiftoverService(IEnsemblDataOptions options)
    {
        _client = new EnsemblLiftoverApiClient(options);
    }

    public Task<LiftoverResultResource<SvResource>[]> Lift(byte sourceGenome, byte targetGenome, SvResource[] variants)
    {
        return _client.LiftSvs(sourceGenome, targetGenome, variants);
    }
}
