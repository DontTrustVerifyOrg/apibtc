using System.Collections.Concurrent;
using LNDWallet;
using NetworkToolkit;

namespace ApiBtc;

public static class Singlethon
{
    public static LNDWalletManager LNDWalletManager = null;
    public static ConcurrentDictionary<string, ConcurrentDictionary<string, AsyncComQueue<PaymentStatusChangedEventArgs>>> PaymentAsyncComQueue4ConnectionId = new();
    public static ConcurrentDictionary<string, ConcurrentDictionary<string, AsyncComQueue<InvoiceStateChangedEventArgs>>> InvoiceAsyncComQueue4ConnectionId = new();
    public static ConcurrentDictionary<string, ConcurrentDictionary<string, AsyncComQueue<NewTransactionFoundEventArgs>>> TransactionAsyncComQueue4ConnectionId = new();
    public static ConcurrentDictionary<string, ConcurrentDictionary<string, AsyncComQueue<PayoutStateChangedEventArgs>>> PayoutAsyncComQueue4ConnectionId = new();

    public static ConcurrentDictionary<(string pubkey, string paymentHash), (string webhook, ConcurrentQueue<InvoiceStateChangedEventArgs> que)> InvoiceWebhookAsyncComQueue = new();
}

