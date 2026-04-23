namespace Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;

public interface IEnsemblVepOptions : IEnsemblGenomeOptions
{
    string Host { get; }
}
