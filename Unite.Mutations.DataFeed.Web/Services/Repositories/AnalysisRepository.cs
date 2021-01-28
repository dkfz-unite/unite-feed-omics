using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unite.Data.Entities;
using Unite.Data.Entities.Mutations;
using Unite.Data.Services;

namespace Unite.Mutations.DataFeed.Web.Services.Repositories
{
    public class AnalysisRepository : Repository<Analysis>
    {
        private readonly Repository<File> _fileRepository;

        public AnalysisRepository(UniteDbContext database, ILogger logger) : base(database, logger)
        {
            _fileRepository = new FileRepository(database, logger);
        }

        public override Analysis Add(in Analysis model)
        {
            if(model.File != null)
            {
                model.File = _fileRepository.Add(model.File);
            }

            return base.Add(model);
        }

        public override void Update(ref Analysis entity, in Analysis model)
        {
            if(model.File != null)
            {
                if(entity.File == null)
                {
                    model.File = _fileRepository.Add(model.File);
                }
                else
                {
                    var file = entity.File;

                    _fileRepository.Update(ref file, model.File);

                    model.File = file;
                }
            }

            base.Update(ref entity, model);
        }

        protected override void Map(in Analysis source, ref Analysis target)
        {
            target.DonorId = source.DonorId;
            target.Name = source.Name;
            target.TypeId = source.TypeId;
            target.Date = source.Date;
            target.File = source.File;
        }

        protected override IQueryable<Analysis> Include(IQueryable<Analysis> query)
        {
            var includeQuery = query
                .Include(analysis => analysis.File);

            return includeQuery;
        }
    }
}
