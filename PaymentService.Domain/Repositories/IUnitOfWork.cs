namespace PaymentService.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IPaymentRepository Payments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
