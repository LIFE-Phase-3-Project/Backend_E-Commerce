using Domain.Entities;
using System.Linq.Expressions;

namespace Application;

public interface ILifeEcommerceRepository<Tentity> where Tentity : class
{
    IQueryable<Tentity> GetByCondition(Expression<Func<Tentity, bool>> expression);
    IQueryable<Tentity> GetById(Expression<Func<Tentity, bool>> expression);
    IQueryable<Tentity> GetAll();
    void Create(Tentity entity);
    void CreateRange(List<Tentity> entity);
    void Update(Tentity entity);
    void UpdateRange(List<Tentity> entity);
    void Delete(Tentity entity);
    void DeleteRange(List<Tentity> entity);
    Task SaveChangesAsync();
    Task GetById(int id);
    Task<Tentity> GetByIdAsync(int productId);

    Task<List<Tentity>> ToListAsync();

}