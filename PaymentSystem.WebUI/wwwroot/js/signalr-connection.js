/**
 * SignalR Connection Manager
 * Manages SignalR hub connections for real-time updates
 * Handles PaymentHub, WalletHub, and TransactionHub
 */

class SignalRConnectionManager {
    constructor() {
        this.connections = {};
        this.isInitialized = false;
    }

    /**
     * Initialize SignalR connection
     * @param {string} hubName - Hub name (payment, wallet, transaction)
     * @param {string} baseUrl - API base URL
     * @returns {Promise<signalR.HubConnection>}
     */
    async initializeConnection(hubName, baseUrl) {
        const hubKey = hubName.toLowerCase();
        
        if (this.connections[hubKey]) {
            console.log(`[SignalR] Connection to ${hubName} already exists`);
            return this.connections[hubKey];
        }

        const hubUrl = `${baseUrl}/hubs/${hubKey}`;
        
        try {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl, {
                    skipNegotiation: false,
                    transport: signalR.HttpTransportType.WebSockets
                })
                .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
                .configureLogging(signalR.LogLevel.Information)
                .build();

            this.connections[hubKey] = connection;

            // Connection event handlers
            connection.onreconnecting((error) => {
                console.warn(`[SignalR][${hubName}] Reconnecting...`, error);
            });

            connection.onreconnected((connectionId) => {
                console.log(`[SignalR][${hubName}] Reconnected. ConnectionId: ${connectionId}`);
            });

            connection.onclose((error) => {
                console.error(`[SignalR][${hubName}] Connection closed.`, error);
            });

            await connection.start();
            console.log(`[SignalR][${hubName}] Connected successfully`);

            return connection;
        } catch (error) {
            console.error(`[SignalR][${hubName}] Connection failed:`, error);
            throw error;
        }
    }

    /**
     * Join a group in the hub
     * @param {string} hubName - Hub name
     * @param {string} methodName - Method name (e.g., JoinPaymentGroup)
     * @param {string} groupId - Group ID
     */
    async joinGroup(hubName, methodName, groupId) {
        const hubKey = hubName.toLowerCase();
        const connection = this.connections[hubKey];
        
        if (!connection) {
            console.error(`[SignalR][${hubName}] No connection found`);
            return;
        }

        try {
            await connection.invoke(methodName, groupId);
            console.log(`[SignalR][${hubName}] Joined group: ${groupId}`);
        } catch (error) {
            console.error(`[SignalR][${hubName}] Failed to join group:`, error);
        }
    }

    /**
     * Leave a group in the hub
     * @param {string} hubName - Hub name
     * @param {string} methodName - Method name (e.g., LeavePaymentGroup)
     * @param {string} groupId - Group ID
     */
    async leaveGroup(hubName, methodName, groupId) {
        const hubKey = hubName.toLowerCase();
        const connection = this.connections[hubKey];
        
        if (!connection) {
            console.error(`[SignalR][${hubName}] No connection found`);
            return;
        }

        try {
            await connection.invoke(methodName, groupId);
            console.log(`[SignalR][${hubName}] Left group: ${groupId}`);
        } catch (error) {
            console.error(`[SignalR][${hubName}] Failed to leave group:`, error);
        }
    }

    /**
     * Register event handler for hub notifications
     * @param {string} hubName - Hub name
     * @param {string} eventName - Event name (e.g., PaymentStatusChanged)
     * @param {Function} callback - Callback function
     */
    on(hubName, eventName, callback) {
        const hubKey = hubName.toLowerCase();
        const connection = this.connections[hubKey];
        
        if (!connection) {
            console.error(`[SignalR][${hubName}] No connection found`);
            return;
        }

        connection.on(eventName, callback);
        console.log(`[SignalR][${hubName}] Registered handler for: ${eventName}`);
    }

    /**
     * Stop connection
     * @param {string} hubName - Hub name
     */
    async stopConnection(hubName) {
        const hubKey = hubName.toLowerCase();
        const connection = this.connections[hubKey];
        
        if (connection) {
            await connection.stop();
            delete this.connections[hubKey];
            console.log(`[SignalR][${hubName}] Connection stopped`);
        }
    }

    /**
     * Stop all connections
     */
    async stopAll() {
        for (const hubName in this.connections) {
            await this.stopConnection(hubName);
        }
    }
}

// Singleton instance
window.signalRConnectionManager = new SignalRConnectionManager();
