using Microsoft.EntityFrameworkCore;
using Unite.Data.Entities.Tasks;
using Unite.Data.Services;

namespace Unite.Mutations.Feed.Data.Repositories
{
    public class TaskRepository : Repository<Task>
    {
        public TaskRepository(DbContext database) : base(database)
        {
        }

        protected override void Map(in Task source, ref Task target)
        {
            target.TypeId = source.TypeId;
            target.TargetTypeId = source.TargetTypeId;
            target.Target = source.Target;
            target.Data = source.Data;
            target.Date = source.Date;
        }
    }
}
