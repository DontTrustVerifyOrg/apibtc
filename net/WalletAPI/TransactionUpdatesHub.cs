using Microsoft.AspNetCore.SignalR;
using LNDWallet;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using NetworkToolkit;

namespace ApiBtc;

public class TransactionUpdatesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var authToken = Context?.GetHttpContext()?.Request.Query["authtoken"].First();
        var account = Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken);
        Context.Items["publicKey"] = account.PublicKey;
        var comq = Singlethon.TransactionAsyncComQueue4ConnectionId.GetOrAdd(account.PublicKey, (key) => new());
        comq.TryAdd(Context.ConnectionId, new AsyncComQueue<NewTransactionFoundEventArgs>());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Singlethon.TransactionAsyncComQueue4ConnectionId.TryGetValue((string)Context.Items["publicKey"], out var comq))
            comq.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async IAsyncEnumerable<NewTransactionFound> StreamAsync(string authToken, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        LNDAccountManager account;
        lock (Singlethon.LNDWalletManager)
            account = Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken);

        if (Singlethon.TransactionAsyncComQueue4ConnectionId.TryGetValue(account.PublicKey, out var comq))
            if (comq.TryGetValue(Context.ConnectionId, out var asyncCom))
            {
                await foreach (var ic in asyncCom.DequeueAsync(cancellationToken))
                {
                    Trace.TraceInformation(ic.PublicKey + "|" + ic.NewTransactionFound.TxHash + "|" + ic.NewTransactionFound.NumConfirmations.ToString() + "|" + ic.NewTransactionFound.Address + "|" + ic.NewTransactionFound.AmountSat.ToString());
                    yield return ic.NewTransactionFound;
                }
            }
    }
}