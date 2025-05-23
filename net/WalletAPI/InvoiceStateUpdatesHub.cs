using Microsoft.AspNetCore.SignalR;
using LNDWallet;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using NetworkToolkit;
using Walletrpc;

namespace ApiBtc;

public class InvoiceStateUpdatesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var authToken = Context?.GetHttpContext()?.Request.Query["authtoken"].First();
        var account = Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken);
        Context.Items["publicKey"] = account.PublicKey;
        var comq = Singlethon.InvoiceAsyncComQueue4ConnectionId.GetOrAdd(account.PublicKey, (key) => new());
        comq.TryAdd(Context.ConnectionId, new AsyncComQueue<InvoiceStateChangedEventArgs>());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Singlethon.InvoiceAsyncComQueue4ConnectionId.TryGetValue((string)Context.Items["publicKey"], out var comq))
            comq.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async IAsyncEnumerable<InvoiceStateChange> StreamAsync(string authToken, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        LNDAccountManager account;
        lock (Singlethon.LNDWalletManager)
            account = Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken);

        if (Singlethon.InvoiceAsyncComQueue4ConnectionId.TryGetValue(account.PublicKey, out var comq))
            if (comq.TryGetValue(Context.ConnectionId, out var asyncCom))
            {
                await foreach (var ic in asyncCom.DequeueAsync(cancellationToken))
                {
                    Trace.TraceInformation(ic.PublicKey + "|" + ic.InvoiceStateChange.PaymentHash + "|" + ic.InvoiceStateChange.NewState.ToString());
                    yield return ic.InvoiceStateChange;
                }
            }
    }
}
