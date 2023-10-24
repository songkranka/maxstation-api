namespace Vatno.Worker.Domain.Models.Repositories
{
    public interface IUnitOfWork
    {
        void Dispose();
        bool Commit();
        Task<bool> CommitAsync();
    }
}
