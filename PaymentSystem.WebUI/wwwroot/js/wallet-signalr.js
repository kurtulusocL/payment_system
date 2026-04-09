/**
 * Wallet Hub SignalR Handler
 * Handles real-time wallet balance updates and transaction notifications
 */

class WalletSignalRHandler {
    constructor(baseUrl, options = {}) {
        this.baseUrl = baseUrl;
        this.hubName = 'wallet';
        this.onBalanceChanged = options.onBalanceChanged || this.defaultBalanceChanged;
        this.onTransactionCompleted = options.onTransactionCompleted || this.defaultTransactionCompleted;
        this.onWalletUpdate = options.onWalletUpdate || this.defaultWalletUpdate;
    }

    /**
     * Initialize Wallet Hub connection
     */
    async initialize() {
        try {
            const connection = await window.signalRConnectionManager.initializeConnection(
                this.hubName,
                this.baseUrl
            );

            // Register event handlers
            window.signalRConnectionManager.on(
                this.hubName,
                'BalanceChanged',
                this.onBalanceChanged
            );

            window.signalRConnectionManager.on(
                this.hubName,
                'TransactionCompleted',
                this.onTransactionCompleted
            );

            window.signalRConnectionManager.on(
                this.hubName,
                'WalletUpdate',
                this.onWalletUpdate
            );

            console.log('[WalletSignalR] Initialized successfully');
        } catch (error) {
            console.error('[WalletSignalR] Initialization failed:', error);
        }
    }

    /**
     * Join wallet group for specific wallet
     * @param {string} walletId - Wallet ID
     */
    async joinWalletGroup(walletId) {
        await window.signalRConnectionManager.joinGroup(
            this.hubName,
            'JoinWalletGroup',
            walletId
        );
    }

    /**
     * Leave wallet group
     * @param {string} walletId - Wallet ID
     */
    async leaveWalletGroup(walletId) {
        await window.signalRConnectionManager.leaveGroup(
            this.hubName,
            'LeaveWalletGroup',
            walletId
        );
    }

    /**
     * Default handler for BalanceChanged
     */
    defaultBalanceChanged(walletId, balanceData) {
        console.log('[Wallet] Balance changed:', { walletId, balanceData });
        
        // Update balance if wallet card exists
        const walletCard = document.querySelector(`[data-wallet-id="${walletId}"]`);
        if (walletCard) {
            const balanceElement = walletCard.querySelector('.wallet-balance');
            if (balanceElement) {
                balanceElement.textContent = `${balanceData.currency} ${balanceData.balance.toFixed(2)}`;
                
                // Add highlight animation
                balanceElement.classList.add('balance-updated');
                setTimeout(() => balanceElement.classList.remove('balance-updated'), 2000);
            }
        }
    }

    /**
     * Default handler for TransactionCompleted
     */
    defaultTransactionCompleted(walletId, transactionData) {
        console.log('[Wallet] Transaction completed:', { walletId, transactionData });
        
        // Show notification
        this.showToast(
            'Transaction Completed',
            `Transaction ${transactionData.id} completed successfully`
        );

        // Refresh transactions table if exists
        const transactionsTable = document.getElementById('transactions-table');
        if (transactionsTable) {
            // Could trigger a reload here
        }
    }

    /**
     * Default handler for WalletUpdate (broadcast)
     */
    defaultWalletUpdate(updateData) {
        console.log('[Wallet] Update received:', updateData);
        this.showToast('Wallet Updated', updateData.message || 'A wallet has been updated');
    }

    /**
     * Show toast notification
     */
    showToast(title, message) {
        const toastContainer = document.getElementById('toast-container');
        if (!toastContainer) return;

        const toast = document.createElement('div');
        toast.className = 'toast toast-success';
        toast.innerHTML = `
            <div class="toast-header">
                <strong>${title}</strong>
                <button type="button" class="toast-close">&times;</button>
            </div>
            <div class="toast-body">${message}</div>
        `;

        toastContainer.appendChild(toast);
        setTimeout(() => toast.remove(), 5000);
        toast.querySelector('.toast-close').addEventListener('click', () => toast.remove());
    }

    /**
     * Stop connection
     */
    async stop() {
        await window.signalRConnectionManager.stopConnection(this.hubName);
    }
}

// Make it globally available
window.WalletSignalRHandler = WalletSignalRHandler;
