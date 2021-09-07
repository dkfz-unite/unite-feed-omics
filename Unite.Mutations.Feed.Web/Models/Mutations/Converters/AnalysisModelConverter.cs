using System.Linq;

namespace Unite.Mutations.Feed.Web.Services.Mutations.Converters
{
    public class AnalysisModelConverter
    {
        public Data.Mutations.Models.AnalysisModel Convert(MutationsModel source)
        {
            var analysisModel = new Data.Mutations.Models.AnalysisModel();

            if (source.Analysis != null)
            {
                Map(source.Analysis, analysisModel);

                if (source.Analysis.File != null)
                {
                    analysisModel.File = new Data.Mutations.Models.FileModel();

                    Map(source.Analysis.File, analysisModel.File);
                }
            }

            analysisModel.AnalysedSamples = source.Samples.Select(analysedSample =>
            {
                var analysedSampleModel = new Data.Mutations.Models.AnalysedSampleModel();

                analysedSampleModel.AnalysedSample = new Data.Mutations.Models.SampleModel();

                Map(analysedSample, analysedSampleModel.AnalysedSample);


                if (!string.IsNullOrWhiteSpace(analysedSample.MatchedSampleId))
                {
                    var matchedSampleId = analysedSample.MatchedSampleId;

                    var matchedAnalysedSample = source.Samples.Single(analysedSample => analysedSample.Id == matchedSampleId);

                    analysedSampleModel.MatchedSample = new Data.Mutations.Models.SampleModel();

                    Map(matchedAnalysedSample, analysedSampleModel.MatchedSample);
                }


                if (analysedSample.Mutations != null)
                {
                    analysedSampleModel.Mutations = analysedSample.Mutations.Select(mutation =>
                    {
                        var mutationModel = new Data.Mutations.Models.MutationModel();

                        Map(mutation, mutationModel);

                        return mutationModel;

                    }).ToArray();
                }


                return analysedSampleModel;

            }).ToArray();

            return analysisModel;
        }


        private static void Map(in AnalysisModel source, Data.Mutations.Models.AnalysisModel target)
        {
            target.Type = source.Type;
        }

        private static void Map(in FileModel source, Data.Mutations.Models.FileModel target)
        {
            target.Name = source.Name;
            target.Link = source.Link;
        }

        private static void Map(in SampleModel source, Data.Mutations.Models.SampleModel target)
        {
            target.ReferenceId = source.Id;

            target.Specimen = new Data.Mutations.Models.SpecimenModel();
            target.Specimen.ReferenceId = source.SpecimenId;
            target.Specimen.Type = source.SpecimenType.Value;

            target.Specimen.Donor = new Data.Mutations.Models.DonorModel();
            target.Specimen.Donor.ReferenceId = source.DonorId;
        }

        private static void Map(in MutationModel source, Data.Mutations.Models.MutationModel target)
        {
            target.Code = source.GetCode();
            target.Chromosome = source.Chromosome.Value;
            target.SequenceType = source.SequenceType.Value;
            target.Start = Unite.Data.Utilities.Mutations.PositionParser.Parse(source.Position).Start;
            target.End = Unite.Data.Utilities.Mutations.PositionParser.Parse(source.Position).End;
            target.ReferenceBase = source.Ref;
            target.AlternateBase = source.Alt;
            target.Type = Unite.Data.Utilities.Mutations.MutationTypeDetector.Detect(source.Ref, source.Alt);
        }
    }
}
