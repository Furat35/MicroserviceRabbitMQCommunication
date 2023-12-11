namespace Shared.Events
{
    public class StockNotReservedEvent
    {
        public Guid BuyerId { get; set; }

        public Guid OrderId { get; set; }

        public string Message { get; set; }
    }
}
