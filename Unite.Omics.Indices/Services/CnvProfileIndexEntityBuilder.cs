using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Specimens;
using Unite.Indices.Entities.CnvProfiles;

namespace Unite.Omics.Indices.Services;

public class CnvProfileIndexEntityBuilder: IndexEntityBuilder<CnvProfileIndex, CnvProfileIndexingCache>
{
    public override CnvProfileIndex Create(int key, CnvProfileIndexingCache cache)
    {
        var cnvProfile = cache.CnvProfiles.FirstOrDefault(x => x.Id == key);
        if (cnvProfile == null)
            return null;

        var sample = cache.Samples.FirstOrDefault(x => x.Id == cnvProfile.SampleId);
        if (sample == null)
            return null;
        
        var specimen = cache.Specimens.FirstOrDefault(x => x.Id == sample.SpecimenId);
        if(specimen == null)
            return null;
        
        return new CnvProfileIndex
        {
            Chromosome = cnvProfile.Chromosome.ToString(),
            ChromosomeArm = cnvProfile.ChromosomeArm.ToString(),
            Gain =  cnvProfile.Gain,
            Loss =  cnvProfile.Loss,
            Neutral = cnvProfile.Neutral,
            Id =  cnvProfile.Id,
            Specimen = new SpecimenNavIndex
            {
                Id = specimen.Id,
                ReferenceId =  specimen.ReferenceId,
                Type = specimen.TypeId.ToDefinitionString()
            }
        };
    }
}