/**
 * Transaction Hub SignalR Handler
 * Handles real-time transaction status updates and notifications
 */

class TransactionSignalRHandler {
    constructor(baseUrl, options = {}) {
        this.baseUrl = baseUrl;
        this.hubName = 'transaction';
        this.onTransactionStatusChanged = options.onTransactionStatusChanged || this.defaultTransactionStatusChanged;
        this.onTransactionCompleted = options.onTransactionCompleted || this.defaultTransactionCompleted;
        this.onTransactionFailed = options.onTransactionFailed || this.defaultTransactionFailed;
        this.onTransactionUpdate = options.onTransactionUpdate || this.defaultTransactionUpdate;
    }

    /**
     * Initialize Transaction Hub connection
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
                'TransactionStatusChanged',
                this.onTransactionStatusChanged
            );

            window.signalRConnectionManager.on(
                this.hubName,
                'TransactionCompleted',
                this.onTransactionCompleted
            );

            window.signalRConnectionManager.on(
                this.hubName,
                'TransactionFailed',
                this.onTransactionFailed
            );

            window.signalRConnectionManager.on(
                this.hubName,
                'TransactionUpdate',
                this.onTransactionUpdate
            );

            console.log('[TransactionSignalR] Initialized successfully');
        } catch (error) {
            console.error('[TransactionSignalR] Initialization failed:', error);
        }
    }

    /**
     * Join transaction group for specific transaction
     * @param {string} transactionId - Transaction ID
     */
    async joinTransactionGroup(transactionId) {
        await window.signalRConnectionManager.joinGroup(
            this.hubName,
            'JoinTransactionGroup',
            transactionId
        );
    }

    /**
     * Leave transaction group
     * @param {string} transactionId - Transaction ID
     */
    async leaveTransactionGroup(transactionId) {
        await window.signalRConnectionManager.leaveGroup(
            this.hubName,
            'LeaveTransactionGroup',
            transactionId
        );
    }

    /**
     * Default handler for TransactionStatusChanged
     */
    defaultTransactionStatusChanged(transactionId, statusData) {
        console.log('[Transaction] Status changed:', { transactionId, statusData });
        
        // Update transaction row if exists
        const transactionRow = document.querySelector(`[data-transaction-id="${transactionId}"]`);
        if (transactionRow) {
            const statusElement = transactionRow.querySelector('.transaction-status');
            if (statusElement) {
                statusElement.textContent = statusData.status;
                statusElement.className = `transaction-status status-${statusData.status.toLowerCase().replace(' ', '-')}`;
            }
        }
    }

    /**
     * Default handler for TransactionCompleted
     */
    defaultTransactionCompleted(transactionId, transactionData) {
        console.log('[Transaction] Completed:', { transactionId, transactionData });
        
        this.showToast(
            'Transaction Completed',
            `Transaction ${transactionId} completed successfully`
        );
    }

    /**
     * Default handler for TransactionFailed
     */
    defaultTransactionFailed(transactionId, errorData) {
        console.error('[Transaction] Failed:', { transactionId, errorData });
        
        this.showToast(
            'Transaction Failed',
            errorData.message || `Transaction ${transactionId} failed`
        );

        // Update transaction row if exists
        const transactionRow = document.querySelector(`[data-transaction-id="${transactionId}"]`);
        if (transactionRow) {
            transactionRow.classList.add('transaction-failed');
        }
    }

    /**
     * Default handler for TransactionUpdate (broadcast)
     */
    defaultTransactionUpdate(updateData) {
        console.log('[Transaction] Update received:', updateData);
        this.showToast('Transaction Updated', updateData.message || 'A transaction has been updated');
    }

    /**
     * Show toast notification
     */
    showToast(title, message) {
        const toastContainer = document.getElementById('toast-container');
        if (!toastContainer) return;

        const toast = document.createElement('div');
        toast.className = 'toast toast-warning';
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
window.TransactionSignalRHandler = TransactionSignalRHandler;
