using Domain.Entities;
using System.Threading.Tasks;

namespace Application;

public interface IUnitOfWork
{

   
    public ILifeEcommerceRepository<TEntity> Repository<TEntity>() where TEntity : class;

    bool Complete();
    Task<bool> CompleteAsync();
    Task SaveChangesAsync();
    
    void Dispose();


}