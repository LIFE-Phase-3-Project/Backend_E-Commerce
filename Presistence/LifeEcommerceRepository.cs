using System.Linq.Expressions;
using Application;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Presistence;

public class LifeEcommerceRepository<Tentity> : ILifeEcommerceRepository<Tentity> where Tentity : class
{
    private readonly APIDbContext _dbContext;

    public LifeEcommerceRepository(APIDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Create(Tentity entity)
    {
        _dbContext.Set<Tentity>().Add(entity);
    }

    public void CreateRange(List<Tentity> entities)
    {
        _dbContext.Set<Tentity>().AddRange(entities);
    }

    public void Delete(Tentity entity)
    {
        _dbContext.Set<Tentity>().Remove(entity);
    }

    public void DeleteRange(List<Tentity> entities)
    {
        _dbContext.Set<Tentity>().RemoveRange(entities);
    }

    public IQueryable<Tentity> GetAll()
    {
        var result = _dbContext.Set<Tentity>();

        return result;
    }

    public IQueryable<Tentity> GetByCondition(Expression<Func<Tentity, bool>> expression)
    {
        return _dbContext.Set<Tentity>().Where(expression);
    }

    public IQueryable<Tentity> GetById(Expression<Func<Tentity, bool>> expression)
    {
        return _dbContext.Set<Tentity>().Where(expression);
    }

    public async Task GetById(int id)
    {
         await _dbContext.Set<Tentity>().FindAsync(id);
    }

    public async Task<Tentity> GetByIdAsync(int id)
    {
        return await _dbContext.Set<Tentity>().FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public void Update(Tentity entity)
    {
        _dbContext.Set<Tentity>().Update(entity);
    }

    public void UpdateRange(List<Tentity> entities)
    {
        _dbContext.Set<Tentity>().UpdateRange(entities);
    }

    public async Task<List<Tentity>> ToListAsync()
    {
        return await _dbContext.Set<Tentity>().ToListAsync();
    }
}