using System;
using System.Linq;
using Unite.Data.Extensions;
using Unite.Data.Services;
using Unite.Mutations.Annotations.Vep.Client;
using Unite.Mutations.Annotations.Vep.Configuration.Options;
using Unite.Mutations.Annotations.Vep.Data;
using Unite.Mutations.Annotations.Vep.Data.Models.Audit;
using Unite.Mutations.Annotations.Vep.Services.Converters;

namespace Unite.Mutations.Annotations.Vep.Services
{
    public class VepAnnotationService
    {
        private readonly UniteDbContext _dbContext;
        private readonly VepAnnotationApiClient _apiClient;
        private readonly VepAnnotationDataWriter _dataWriter;
        private readonly AnnotationsModelConverter _modelConverter;

        public VepAnnotationService(
            IVepOptions options,
            UniteDbContext dbContext)
        {
            _dbContext = dbContext;
            _apiClient = new VepAnnotationApiClient(options);
            _dataWriter = new VepAnnotationDataWriter(dbContext);
            _modelConverter = new AnnotationsModelConverter();
        }


        public void Annotate(long[] mutationIds, out AnnotationsUploadAudit audit)
        {
            var codes = GetVepCodes(mutationIds);

            var resources = _apiClient.LoadAnnotations(codes);

            var models = resources.Select(resource => _modelConverter.Convert(resource)).ToArray();

            _dataWriter.SaveData(models, out audit);
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
