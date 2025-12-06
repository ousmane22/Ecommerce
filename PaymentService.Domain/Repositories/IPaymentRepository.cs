#nullable disable
using PaymentService.Domain.Entities;
using Ecommerce.Common.Repositories;

namespace PaymentService.Domain.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment> GetByOrderIdAsync(string orderId);
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(string customerId);
    Task<Payment> CreateAsync(Payment payment);
}
