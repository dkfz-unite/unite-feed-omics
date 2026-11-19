using Unite.Omics.Annotations.Clients.Ensembl;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;

namespace Unite.Omics.Annotations.Services.Liftover;

public class CnvsLiftoverService
{
    private readonly EnsemblLiftoverApiClient _client;

    public CnvsLiftoverService(IEnsemblDataOptions options)
    {
        _client = new EnsemblLiftoverApiClient(options);
    }

    public Task<LiftoverResultResource<CnvResource>[]> Lift(byte sourceGenome, byte targetGenome, CnvResource[] variants)
    {
        return _client.LiftCnvs(sourceGenome, targetGenome, variants);
    }
}
