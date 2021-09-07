using System;
using System.Linq;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Clients.Ensembl.Configuration.Options;
using Unite.Mutations.Annotations.Clients.Vep.Configuration.Options;
using Unite.Mutations.Annotations.Data;
using Unite.Mutations.Annotations.Data.Models.Audit;

namespace Unite.Mutations.Annotations.Services
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
            var codes = _dbContext.Mutations
                .Where(mutation => mutationIds.Contains(mutation.Id))
                .Select(mutation => GetVepCode(mutation))
                .ToArray();

            return codes;
        }

        private static string GetVepCode(Unite.Data.Entities.Mutations.Mutation mutation)
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
