using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unite.Genome.Annotations.Clients.Ensembl;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Ensembl.Models.Constants;
using Unite.Genome.Annotations.Clients.Ensembl.Resources;

namespace Unite.Genome.Annotations.Tests.Integration.Clients
{
    [TestClass]
    public class EnsemblApiClientTests
    {
        private EnsemblApiClient _apiClient;


        [TestInitialize]
        public void InitializeTest()
        {
            var options = new EnsemblOptions();

            _apiClient = new EnsemblApiClient(options);
        }

        [TestMethod]
        public void Lookup_ShouldAnnotateOneGene()
        {
            var ensemblId = "ENSG00000141510";

            var resource = _apiClient.Lookup<GeneResource>(ensemblId).Result;

            Assert.IsNotNull(resource);
        }

        [TestMethod]
        public void Lookup_ShouldAnnotateMultipleGenes()
        {
            var ensemblIds = new string[] { "ENSG00000141510", "ENSG00000146648" };

            var resources = _apiClient.Lookup<GeneResource>(ensemblIds).Result;

            Assert.IsNotNull(resources);
            Assert.AreEqual(resources.Length, 2);
        }

        [TestMethod]
        public void Lookup_ShouldAnnotateOneTranscript()
        {
            var ensemblId = "ENST00000275493";

            var resource = _apiClient.Lookup<TranscriptResource>(ensemblId, expand: true).Result;

            Assert.IsNotNull(resource);
        }

        [TestMethod]
        public void Lookup_ShouldAnnotateMultipleTranscripts()
        {
            var ensemblIds = new string[] { "ENST00000275493", "ENST00000330909" };

            var resources = _apiClient.Lookup<TranscriptResource>(ensemblIds, expand: true).Result;

            Assert.IsNotNull(resources);
            Assert.AreEqual(resources.Length, 2);
            Assert.IsNotNull(resources[0].Protein);
            Assert.IsNotNull(resources[1].Protein);
        }

        [TestMethod]
        public void Xrefs_ShouldFindReferencesOfProtein()
        {
            var ensemblId = "ENSP00000275493.2";

            var resources = _apiClient.Xrefs(ensemblId).Result;

            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.Length > 0);
            Assert.IsTrue(resources.Any(resource => resource.Database == ProteinDataSources.Uniprot));
        }

        [TestMethod]
        public void Xrefs_ShouldFindReferencesOfProteinInUniprotDatabase()
        {
            var ensemblId = "ENSP00000275493.2";

            var resources = _apiClient.Xrefs(ensemblId, ProteinDataSources.Uniprot).Result;

            Assert.IsNotNull(resources);
            Assert.IsTrue(resources.Length > 0);
            Assert.IsTrue(resources.Any(resource => resource.Database == ProteinDataSources.Uniprot));
        }


        private class EnsemblOptions : IEnsemblOptions
        {
            public string Host => @"https://grch37.rest.ensembl.org";
        }
    }
}
