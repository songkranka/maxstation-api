using Microsoft.EntityFrameworkCore;
using Vatno.Worker.Context;
using Vatno.Worker.Domain.Models.Repositories;

namespace Vatno.Worker.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IPTMaxStationContext _context;

    public UnitOfWork(IPTMaxStationContext context)
    {
        _context = context;
    }

    public void Dispose()
        => _context?.Dispose();

    public bool Commit()
    {
        bool returnValue = true;
        using var transaction = _context.Database.BeginTransaction();


        try
        {
            _context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            returnValue = false;
            transaction.Rollback();
        }

        return returnValue;
    }

    public async Task<bool> CommitAsync()
    {
        bool returnValue = true;
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                returnValue = false;
                await transaction.RollbackAsync();
            }
        });
        return returnValue;
    }
}