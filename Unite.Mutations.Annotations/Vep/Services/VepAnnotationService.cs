using System;
using System.Linq;
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
            var mutationCodes = GetMutationCodes(mutationIds);

            var resources = _apiClient.LoadAnnotations(mutationCodes);

            var models = resources.Select(resource => _modelConverter.Convert(resource)).ToArray();

            _dataWriter.SaveData(models, out audit);
        }


        private string[] GetMutationCodes(long[] mutationIds)
        {
            var mutationCodes = _dbContext.Mutations
                .Where(mutation => mutationIds.Contains(mutation.Id))
                .Select(mutation => mutation.Code)
                .ToArray();

            return mutationCodes;
        }
    }
}
