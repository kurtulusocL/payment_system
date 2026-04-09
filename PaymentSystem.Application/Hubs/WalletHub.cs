using Microsoft.AspNetCore.SignalR;

namespace PaymentSystem.Application.Hubs
{
    public class WalletHub : Hub
    {
        public async Task JoinWalletGroup(string walletId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, walletId);
        }

        public async Task LeaveWalletGroup(string walletId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, walletId);
        }

        public async Task NotifyBalanceChanged(string walletId, object balanceData)
        {
            await Clients.Group(walletId).SendAsync("BalanceChanged", walletId, balanceData);
        }

        public async Task NotifyTransactionCompleted(string walletId, object transactionData)
        {
            await Clients.Group(walletId).SendAsync("TransactionCompleted", walletId, transactionData);
        }

        public async Task BroadcastWalletUpdate(object updateData)
        {
            await Clients.All.SendAsync("WalletUpdate", updateData);
        }
    }
}
