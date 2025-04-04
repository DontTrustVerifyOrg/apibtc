﻿using Microsoft.AspNetCore.SignalR;
using LNDWallet;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using NetworkToolkit;

namespace ApiBtc;

public class InvoiceStateUpdatesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var authToken = Context?.GetHttpContext()?.Request.Query["authtoken"].First();
        var account = Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken, false);
        Context.Items["publicKey"] = account.PublicKey;
        Singlethon.InvoiceAsyncComQueue4ConnectionId.TryAdd(Context.ConnectionId, new AsyncComQueue<InvoiceStateChangedEventArgs>());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Singlethon.InvoiceAsyncComQueue4ConnectionId.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async IAsyncEnumerable<InvoiceStateChange> StreamAsync(string authToken, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        LNDAccountManager account;
        lock (Singlethon.LNDWalletManager)
            account = Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken);

        AsyncComQueue<InvoiceStateChangedEventArgs> asyncCom;
        if (Singlethon.InvoiceAsyncComQueue4ConnectionId.TryGetValue(Context.ConnectionId, out asyncCom))
        {
            await foreach (var ic in asyncCom.DequeueAsync(cancellationToken))
            {
                if (ic.PublicKey == account.PublicKey)
                {
                    Trace.TraceInformation(ic.PublicKey + "|" + ic.InvoiceStateChange.PaymentHash + "|" + ic.InvoiceStateChange.NewState.ToString());
                    yield return ic.InvoiceStateChange;
                }
            }
        }
    }
}
