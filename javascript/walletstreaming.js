import * as signalR from "@microsoft/signalr";

class WalletUpdateStream {
    constructor(wallet, hubConnection) {
        this.hubConnection = hubConnection;
        this.wallet = wallet;
    }

    stop() {
        /**
         * Stops the invoice update stream and closes the connection.
         */
        this.hubConnection.stop();
    }

    stream(next, bye) {
        this.hubConnection.stream("StreamAsync", [this.wallet.create_authtoken()])
            .subscribe({
                next: next,
                complete: (x) => bye(false, x),
                error: (x) => bye(true, x)
            });
    }
}

class WalletStreaming {
    constructor(wallet, debug = false) {
        this.wallet = wallet;
        this.baseUrl = wallet.base_url;
        this.pubkey = wallet.pubkey;
        this.debug = debug;
    }

    _startHub(method) {
        /**
         * Initiates a stream of updates related to the state of invoices.
         */
        const authToken = encodeURIComponent(this.wallet.create_authtoken());
        const url = `${this.baseUrl}/${method}?authtoken=${authToken}`;

        const hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(url, {
                skipNegotiation: false,
                transport: signalR.HttpTransportType.WebSockets
            })
            .withAutomaticReconnect([1000, 3000, 5000, 6000, 7000, 87000, 3000])
            .configureLogging(this.debug ? signalR.LogLevel.Debug : signalR.LogLevel.Critical)
            .build();

        hubConnection.start()
            .then(() => console.log("connection opened and handshake received ready to send messages"))
            .catch(err => console.error("Error while establishing connection:", err));

        hubConnection.onclose(() => console.log("connection closed"));

        return hubConnection;
    }

    invoicestateupdates() {
        /**
         * Starts a stream of updates related to the state of invoices.
         */
        return new WalletUpdateStream(this.wallet, this._startHub("invoicestateupdates"));
    }

    paymentstatusupdates() {
        /**
         * Starts a stream of updates related to payment status.
         */
        return new WalletUpdateStream(this.wallet, this._startHub("paymentstatusupdates"));
    }

    transactionupdates() {
        /**
         * Starts a stream of updates related to transactions.
         */
        return new WalletUpdateStream(this.wallet, this._startHub("transactionupdates"));
    }

    payoutstateupdates() {
        /**
         * Starts a stream of updates related to payouts.
         */
        return new WalletUpdateStream(this.wallet, this._startHub("payoutstateupdates"));
    }
}

export { WalletUpdateStream, WalletStreaming };