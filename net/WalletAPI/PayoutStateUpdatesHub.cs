using Microsoft.AspNetCore.SignalR;
using LNDWallet;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using NetworkToolkit;

namespace ApiBtc;

public class PayoutStateUpdatesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var authToken = Context?.GetHttpContext()?.Request.Query["authtoken"].First();
        var account = Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken);
        Context.Items["publicKey"] = account.PublicKey;
        var comq = Singlethon.PayoutAsyncComQueue4ConnectionId.GetOrAdd(account.PublicKey, (key) => new());
        comq.TryAdd(Context.ConnectionId, new AsyncComQueue<PayoutStateChangedEventArgs>());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Singlethon.InvoiceAsyncComQueue4ConnectionId.TryGetValue((string)Context.Items["publicKey"], out var comq))
            comq.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async IAsyncEnumerable<PayoutStateChanged> StreamAsync(string authToken, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        LNDAccountManager account;
        lock (Singlethon.LNDWalletManager)
            account = Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken);

        if (Singlethon.PayoutAsyncComQueue4ConnectionId.TryGetValue(account.PublicKey, out var comq))
            if (comq.TryGetValue(Context.ConnectionId, out var asyncCom))
            {
                await foreach (var ic in asyncCom.DequeueAsync(cancellationToken))
                {
                    Trace.TraceInformation(ic.PublicKey + "|" + ic.PayoutStateChanged.PayoutId + "|" + ic.PayoutStateChanged.NewState.ToString() + "|" + ic.PayoutStateChanged.PayoutFee.ToString() + (ic.PayoutStateChanged.Tx == null ? "" : ("|" + ic.PayoutStateChanged.Tx)));
                    yield return ic.PayoutStateChanged;
                }
            }
    }
}
