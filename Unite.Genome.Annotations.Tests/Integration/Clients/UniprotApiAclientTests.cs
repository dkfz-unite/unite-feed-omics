using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unite.Genome.Annotations.Clients.Uniprot;
using Unite.Genome.Annotations.Clients.Uniprot.Configuration.Options;

namespace Unite.Genome.Annotations.Tests.Integration.Clients;

[TestClass]
public class UniprotApiAclientTests
{
    private UniprotApiClient _apiClient;


    [TestInitialize]
    public void InitializeTest()
    {
        var options = new UniprotOptions();

        _apiClient = new UniprotApiClient(options);
    }


    [TestMethod]
    public void Protein_ShouldAnnotateOneProtein()
    {
        var accessionId = "P00533";

        var protein = _apiClient.Protein(accessionId).Result;

        Assert.IsNotNull(protein);
        Assert.IsNotNull(protein.Domains);
        Assert.IsTrue(protein.Domains.Length > 0);
    }

    [TestMethod]
    public void Proteins_ShouldAnnotateMultipleProteins()
    {
        var accessionIds = new string[]
        {
            "P00530",
            "P00531",
            "P00532",
            "P00533",
            "P00534"
        };

        var proteins = _apiClient.Proteins(accessionIds).Result;

        Assert.IsNotNull(proteins);
        Assert.IsTrue(proteins.Length == accessionIds.Length);
    }


    private class UniprotOptions : IUniprotOptions
    {
        public string Host => @"https://www.ebi.ac.uk";
    }
}
