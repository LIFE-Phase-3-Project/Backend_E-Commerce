namespace Application;

public interface IUnitOfWork
{
    public ILifeEccommerceRepository<TEntity> Repository<TEntity>() where TEntity : class;

    bool Complete();
    Task<bool> CompleteAsync();
}