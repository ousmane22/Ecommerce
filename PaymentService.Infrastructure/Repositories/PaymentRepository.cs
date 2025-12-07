using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Repositories;
using Ecommerce.Common.Repositories.EFCore;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Infrastructure.Repositories;

public class PaymentRepository : RepositoryBase<Payment>, IPaymentRepository
{
    public PaymentRepository(PaymentDbContext context) : base(context)
    {
    }

    public async Task<Payment> GetByOrderIdAsync(string orderId)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(string customerId)
    {
        return await DbSet.Where(p => p.CustomerId == customerId).ToListAsync();
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        await DbSet.AddAsync(payment);
        await Context.SaveChangesAsync();
        return payment;
    }
}
