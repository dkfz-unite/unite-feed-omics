using Unite.Omics.Annotations.Clients.Ensembl;
using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;

namespace Unite.Omics.Annotations.Services.Liftover;

public class SmsLiftoverService
{
    private readonly EnsemblLiftoverApiClient _client;

    public SmsLiftoverService(IEnsemblDataOptions options)
    {
        _client = new EnsemblLiftoverApiClient(options);
    }

    public Task<LiftoverResultResource<SmResource>[]> Lift(byte sourceGenome, byte targetGenome, SmResource[] variants)
    {
        return _client.LiftSms(sourceGenome, targetGenome, variants);
    }
}
