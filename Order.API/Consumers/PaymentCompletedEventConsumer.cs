using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly OrderApiDbContext _orderApiDbContext;

        public PaymentCompletedEventConsumer(OrderApiDbContext orderApiDbContext)
        {
            _orderApiDbContext = orderApiDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            Order.API.Models.Entities.Order order = await _orderApiDbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == context.Message.OrderId);
            order.OrderStatus = Models.Enums.OrderStatus.Completed;
            await _orderApiDbContext.SaveChangesAsync();
        }
    }
}
