using System.Collections.Generic;
using System.Linq;

namespace Unite.Mutations.DataFeed.Domain.Resources.Mutations.Extensions
{
    public static class ResourceExtensions
    {
        public static IEnumerable<Samples.Sample> AsSamples(this IEnumerable<Mutation> source)
        {
            var pidGroups = source.GroupBy(mutation => mutation.Pid);

            foreach (var pidGroup in pidGroups)
            {
                var sampleGroups = pidGroup.GroupBy(mutation => mutation.Sample);

                foreach (var sampleGroup in sampleGroups)
                {
                    var sample = new Samples.Sample();

                    sample.Pid = pidGroup.Key;
                    sample.Name = sampleGroup.Key?.Name;
                    sample.Link = sampleGroup.Key?.Link;
                    sample.Type = sampleGroup.Key?.Type;
                    sample.Subtype = sampleGroup.Key?.Subtype;

                    sample.Mutations = sampleGroup.Select(source =>
                    {
                        var mutation = new Samples.Mutation();

                        mutation.Id = source.Id;
                        mutation.Chromosome = source.Chromosome;
                        mutation.Contig = source.Contig;
                        mutation.SequenceType = source.SequenceType;
                        mutation.Position = source.Position;
                        mutation.Type = source.Type;
                        mutation.Ref = source.Ref;
                        mutation.Alt = source.Alt;
                        mutation.Quality = source.Quality;
                        mutation.Filter = source.Filter;
                        mutation.Info = source.Info;

                        mutation.Gene = Convert(source.Gene);

                        return mutation;

                    }).ToArray();

                    yield return sample;
                }
            }
        }

        private static Samples.Gene Convert(Gene source)
        {
            if(source == null)
            {
                return null;
            }

            var gene = new Samples.Gene();
            gene.Name = source.Name;

            return gene;
        }
    }
}
