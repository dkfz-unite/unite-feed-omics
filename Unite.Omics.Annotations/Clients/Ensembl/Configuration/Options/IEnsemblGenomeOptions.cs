namespace Unite.Omics.Annotations.Clients.Ensembl.Configuration.Options;

public interface IEnsemblGenomeOptions
{
    string Build { get; }
    byte Version { get; }
}
