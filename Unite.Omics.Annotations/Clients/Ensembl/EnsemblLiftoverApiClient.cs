using Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Omics.Annotations.Clients.Ensembl.Resources.Liftover;

namespace Unite.Omics.Annotations.Clients.Ensembl;

internal class EnsemblLiftoverApiClient
{
    private readonly IEnsemblDataOptions _options;

    public EnsemblLiftoverApiClient(IEnsemblDataOptions options)
    {
        _options = options;
    }

    public Task<LiftoverResultResource<SmResource>[]> LiftSms(byte sourceGenome, byte targetGenome, SmResource[] variants)
    {
        return Lift(sourceGenome, targetGenome, "sms", variants);
    }

    public Task<LiftoverResultResource<CnvResource>[]> LiftCnvs(byte sourceGenome, byte targetGenome, CnvResource[] variants)
    {
        return Lift(sourceGenome, targetGenome, "cnvs", variants);
    }

    public Task<LiftoverResultResource<SvResource>[]> LiftSvs(byte sourceGenome, byte targetGenome, SvResource[] variants)
    {
        return Lift(sourceGenome, targetGenome, "svs", variants);
    }

    private async Task<LiftoverResultResource<TVariant>[]> Lift<TVariant>(
        byte sourceGenome,
        byte targetGenome,
        string variantType,
        TVariant[] variants)
    {
        using var httpClient = new JsonHttpClient(_options.Host);
        var url = $"/api/grch{sourceGenome}/liftover/grch{targetGenome}/{variantType}";

        return await httpClient.PostAsync<LiftoverResultResource<TVariant>[], TVariant[]>(url, variants);
    }
}
