using Unite.Essentials.Extensions;
using Unite.Indices.Entities.Basic.Specimens;
using Unite.Indices.Entities.CnvProfiles;

namespace Unite.Omics.Indices.Services;

public class CnvpIndexEntityBuilder: IndexEntityBuilder<CnvProfileIndex, CnvpIndexingCache>
{
    public override CnvProfileIndex[] Create(int key, CnvpIndexingCache cache)
    {
        var cnvp = cache.Cnvps.FirstOrDefault(x => x.Id == key);
        if (cnvp == null)
            return null;

        var sample = cache.Samples.FirstOrDefault(x => x.Id == cnvp.SampleId);
        if (sample == null)
            return null;
        
        var specimen = cache.Specimens.FirstOrDefault(x => x.Id == sample.SpecimenId);
        if(specimen == null)
            return null;
        
        //TODO: convert Gain, Loss and Neutral to float
        return [new CnvProfileIndex
        {
            Chromosome = cnvp.ChromosomeId.ToDefinitionString(),
            ChromosomeArm = cnvp.ChromosomeArmId.ToDefinitionString(),
            Gain =  (float)cnvp.Gain,
            Loss =  (float)cnvp.Loss,
            Neutral = (float)cnvp.Neutral,
            Id =  cnvp.Id,
            Specimen = new SpecimenNavIndex
            {
                Id = specimen.Id,
                ReferenceId =  specimen.ReferenceId,
                Type = specimen.TypeId.ToDefinitionString()
            }
        }];
    }
}