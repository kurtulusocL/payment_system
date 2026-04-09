/**
 * Payment Hub SignalR Handler
 * Handles real-time payment notifications
 */

class PaymentSignalRHandler {
    constructor(baseUrl, options = {}) {
        this.baseUrl = baseUrl;
        this.hubName = 'payment';
        this.onPaymentStatusChanged = options.onPaymentStatusChanged || this.defaultPaymentStatusChanged;
        this.onPaymentUpdate = options.onPaymentUpdate || this.defaultPaymentUpdate;
    }

    /**
     * Initialize Payment Hub connection
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
                'PaymentStatusChanged',
                this.onPaymentStatusChanged
            );

            window.signalRConnectionManager.on(
                this.hubName,
                'PaymentUpdate',
                this.onPaymentUpdate
            );

            console.log('[PaymentSignalR] Initialized successfully');
        } catch (error) {
            console.error('[PaymentSignalR] Initialization failed:', error);
        }
    }

    /**
     * Join payment group for specific payment
     * @param {string} paymentId - Payment ID
     */
    async joinPaymentGroup(paymentId) {
        await window.signalRConnectionManager.joinGroup(
            this.hubName,
            'JoinPaymentGroup',
            paymentId
        );
    }

    /**
     * Leave payment group
     * @param {string} paymentId - Payment ID
     */
    async leavePaymentGroup(paymentId) {
        await window.signalRConnectionManager.leaveGroup(
            this.hubName,
            'LeavePaymentGroup',
            paymentId
        );
    }

    /**
     * Default handler for PaymentStatusChanged
     */
    defaultPaymentStatusChanged(paymentId, statusData) {
        console.log('[Payment] Status changed:', { paymentId, statusData });
        
        // Update UI if payment row exists
        const paymentRow = document.querySelector(`[data-payment-id="${paymentId}"]`);
        if (paymentRow) {
            const statusElement = paymentRow.querySelector('.payment-status');
            if (statusElement) {
                statusElement.textContent = statusData.status;
                statusElement.className = `payment-status status-${statusData.status.toLowerCase()}`;
            }
        }
    }

    /**
     * Default handler for PaymentUpdate (broadcast)
     */
    defaultPaymentUpdate(updateData) {
        console.log('[Payment] Update received:', updateData);
        
        // Refresh payment list if on list page
        const paymentTable = document.getElementById('payment-table');
        if (paymentTable) {
            // Show toast notification
            this.showToast('Payment Updated', updateData.message || 'A payment has been updated');
        }
    }

    /**
     * Show toast notification
     */
    showToast(title, message) {
        const toastContainer = document.getElementById('toast-container');
        if (!toastContainer) return;

        const toast = document.createElement('div');
        toast.className = 'toast toast-info';
        toast.innerHTML = `
            <div class="toast-header">
                <strong>${title}</strong>
                <button type="button" class="toast-close">&times;</button>
            </div>
            <div class="toast-body">${message}</div>
        `;

        toastContainer.appendChild(toast);

        // Auto remove after 5 seconds
        setTimeout(() => toast.remove(), 5000);

        // Close button handler
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
window.PaymentSignalRHandler = PaymentSignalRHandler;
