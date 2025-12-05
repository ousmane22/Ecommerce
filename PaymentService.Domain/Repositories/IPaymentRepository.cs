using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<Payment?> GetByOrderIdAsync(string orderId);
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment> CreateAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task<bool> ExistsAsync(int id);
}
