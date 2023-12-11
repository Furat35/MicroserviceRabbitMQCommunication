using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.ViewModels;
using Shared.Events;
using Shared.Messages;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderApiDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(OrderApiDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderVM createOrder)
        {
            Order.API.Models.Entities.Order order = new()
            {
                OrderId = Guid.NewGuid(),
                BuyerId = createOrder.BuyerId,
                CreatedDate = DateTime.Now,
                OrderStatus = Models.Enums.OrderStatus.Suspend,
                OrderItems = createOrder.OrderItems.Select(oi => new OrderItem
                {
                    Count = oi.Count,
                    Price = oi.Price,
                    ProductId = oi.ProductId
                }).ToList(),
                TotalPrice = createOrder.OrderItems.Sum(oi => oi.Price * oi.Count)
            };
            _context.Add(order);
            await _context.SaveChangesAsync();
            OrderCreatedEvent orderCreated = new()
            {
                BuyerId = order.BuyerId,
                OrderId = order.OrderId,
                OrderItems = order.OrderItems.Select(orderItem => new OrderItemMessage { ProductId = orderItem.ProductId, Count = orderItem.Count }).ToList(),
                TotalPrice = order.TotalPrice
            };
            await _publishEndpoint.Publish(orderCreated);

            return Ok();
        }
    }
}
