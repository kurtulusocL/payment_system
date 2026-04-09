using Microsoft.AspNetCore.SignalR;

namespace PaymentSystem.Application.Hubs
{
    public class PaymentHub : Hub
    {
        public async Task JoinPaymentGroup(string paymentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, paymentId);
        }

        public async Task LeavePaymentGroup(string paymentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, paymentId);
        }

        public async Task NotifyPaymentStatusChanged(string paymentId, object statusData)
        {
            await Clients.Group(paymentId).SendAsync("PaymentStatusChanged", paymentId, statusData);
        }

        public async Task BroadcastPaymentUpdate(object updateData)
        {
            await Clients.All.SendAsync("PaymentUpdate", updateData);
        }
    }
}
