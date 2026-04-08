namespace Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;

public interface IEnsemblDataOptions : IEnsemblGenomeOptions
{
    string Host { get; }
}
