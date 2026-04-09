using Microsoft.AspNetCore.SignalR;

namespace PaymentSystem.Application.Hubs
{
    public class TransactionHub : Hub
    {
        public async Task JoinTransactionGroup(string transactionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, transactionId);
        }

        public async Task LeaveTransactionGroup(string transactionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, transactionId);
        }

        public async Task NotifyTransactionStatusChanged(string transactionId, object statusData)
        {
            await Clients.Group(transactionId).SendAsync("TransactionStatusChanged", transactionId, statusData);
        }

        public async Task NotifyTransactionCompleted(string transactionId, object transactionData)
        {
            await Clients.Group(transactionId).SendAsync("TransactionCompleted", transactionId, transactionData);
        }

        public async Task NotifyTransactionFailed(string transactionId, object errorData)
        {
            await Clients.Group(transactionId).SendAsync("TransactionFailed", transactionId, errorData);
        }

        public async Task BroadcastTransactionUpdate(object updateData)
        {
            await Clients.All.SendAsync("TransactionUpdate", updateData);
        }
    }
}
