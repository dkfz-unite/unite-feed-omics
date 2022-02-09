using System;
using System.Linq;
using Unite.Data.Entities.Genome.Mutations;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Genome.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Genome.Annotations.Clients.Vep.Configuration.Options;
using Unite.Genome.Annotations.Data;
using Unite.Genome.Annotations.Data.Models.Audit;

namespace Unite.Genome.Annotations.Services
{
    public class AnnotationService
    {
        private readonly DomainDbContext _dbContext;
        private readonly AnnotationsDataLoader _dataLoader;
        private readonly AnnotationsDataWriter _dataWriter;


        public AnnotationService(
            DomainDbContext dbContext,
            IVepOptions vepOptions,
            IEnsemblOptions ensemblOptions)
        {
            _dbContext = dbContext;
            _dataLoader = new AnnotationsDataLoader(vepOptions, ensemblOptions);
            _dataWriter = new AnnotationsDataWriter(dbContext);
        }


        public void Annotate(long[] mutationIds, out AnnotationsUploadAudit audit)
        {
            var codes = GetVepCodes(mutationIds);

            var annotations = _dataLoader.LoadData(codes).Result;

            _dataWriter.SaveData(annotations, out audit);
        }


        private string[] GetVepCodes(long[] mutationIds)
        {
            var codes = _dbContext.Set<Mutation>()
                .Where(entity => mutationIds.Contains(entity.Id))
                .Select(entity => GetVepCode(entity))
                .ToArray();

            return codes;
        }

        private static string GetVepCode(Mutation mutation)
        {
            var chromosome = mutation.ChromosomeId.ToDefinitionString();
            var start = mutation.ReferenceBase != null ? mutation.Start : mutation.End + 1;
            var end = mutation.End;
            var referenceBase = mutation.ReferenceBase ?? "-";
            var alternateBase = mutation.AlternateBase ?? "-";

            return $"{chromosome} {start} {end} {referenceBase}/{alternateBase}";
        }
    }
}
