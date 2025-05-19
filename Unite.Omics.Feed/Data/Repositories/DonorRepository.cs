using Microsoft.EntityFrameworkCore;
using Unite.Data.Context;
using Unite.Data.Entities.Donors;
using Unite.Omics.Feed.Data.Models;

namespace Unite.Omics.Feed.Data.Repositories;

internal class DonorRepository
{
    private readonly DomainDbContext _dbContext;


    public DonorRepository(DomainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public Donor FindOrCreate(DonorModel model)
    {
        return Find(model) ?? Create(model);
    }

    public Donor Find(DonorModel model)
    {
        var entity = _dbContext.Set<Donor>()
            .FirstOrDefault(entity =>
                entity.ReferenceId == model.ReferenceId
            );

        return entity;
    }

    public Donor Create(DonorModel model)
    {
        var project = FindOrCreateProject();
        var projectDonor = new ProjectDonor { ProjectId = project.Id };
        var entity = new Donor
        {
            ReferenceId = model.ReferenceId,
            DonorProjects = [projectDonor]
        };

        _dbContext.Add(entity);
        _dbContext.SaveChanges();

        return entity;
    }


    private Project FindOrCreateProject()
    {
        var name = Project.DefaultName;
        var project = _dbContext.Set<Project>()
            .AsNoTracking()
            .FirstOrDefault(entity => entity.Name == name);

        if (project == null)
        {
            project = new Project() { Name = name };

            _dbContext.Add(project);
            _dbContext.SaveChanges();
        }

        return project;
    }
}
