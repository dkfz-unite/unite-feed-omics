using System.Linq;
using Unite.Data.Entities.Specimens;
using Unite.Data.Entities.Specimens.Cells;
using Unite.Data.Services;
using Unite.Mutations.Feed.Data.Mutations.Models;

namespace Unite.Mutations.Feed.Data.Mutations.Repositories
{
    public class CellLineRepository
    {
        private readonly UniteDbContext _dbContext;


        public CellLineRepository(UniteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public Specimen FindOrCreate(int donorId, CellLineModel cellLineModel)
        {
            return Find(donorId, cellLineModel) ?? Create(donorId, cellLineModel);
        }

        public Specimen Find(int donorId, CellLineModel cellLineModel)
        {
            var specimen = _dbContext.Specimens.FirstOrDefault(specimen =>
                specimen.DonorId == donorId &&
                specimen.CellLine.ReferenceId == cellLineModel.ReferenceId
            );

            return specimen;
        }

        public Specimen Create(int donorId, CellLineModel cellLineModel)
        {
            var specimen = new Specimen
            {
                DonorId = donorId
            };

            Map(cellLineModel, specimen);

            _dbContext.Specimens.Add(specimen);
            _dbContext.SaveChanges();

            return specimen;
        }


        private void Map(CellLineModel cellLineModel, Specimen specimen)
        {
            if (specimen.CellLine == null)
            {
                specimen.CellLine = new CellLine();
            }

            specimen.Tissue.ReferenceId = cellLineModel.ReferenceId;
        }
    }
}
