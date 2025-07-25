﻿using ApiBtc;
using ApiBtc;
using GigDebugLoggerAPIClient;
using LNDClient;
using LNDWallet;
using Lnrpc;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using NetworkToolkit;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using TraceExColor;
using Enum = System.Enum;
using Type = System.Type;

ConsoleLoggerFactory.Initialize(true);

TraceEx.TraceInformation("[[lime]]Starting[[/]] ...");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.UseAllOfToExtendReferenceSchemas();
    c.DocumentFilter<CustomModelDocumentFilter<PaymentStatusChanged>>();
    c.DocumentFilter<CustomModelDocumentFilter<InvoiceStateChange>>();
    c.DocumentFilter<CustomModelDocumentFilter<NewTransactionFound>>();
    c.DocumentFilter<CustomModelDocumentFilter<PayoutStateChanged>>();
    c.SchemaFilter<NSwagEnumExtensionSchemaFilter>();
    c.SchemaFilter<EnumFilter>();
});
builder.Services.AddSignalR();
builder.Services.AddProblemDetails();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHsts();


IConfigurationRoot GetConfigurationRoot(string defaultFolder, string iniName)
{
    var basePath = Environment.GetEnvironmentVariable("WALLET_BASEDIR");
    if (basePath == null)
        basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), defaultFolder);
    foreach (var arg in args)
        if (arg.StartsWith("--basedir"))
            basePath = arg.Substring(arg.IndexOf('=') + 1).Trim().Replace("\"", "").Replace("\'", "");
        else if (arg.StartsWith("--cfg"))
            iniName = arg.Substring(arg.IndexOf('=') + 1).Trim().Replace("\"", "").Replace("\'", "");


    var builder = new ConfigurationBuilder();
    builder.SetBasePath(basePath)
           .AddIniFile(iniName)
           .AddEnvironmentVariables()
           .AddCommandLine(args);

    return builder.Build();
}

var config = GetConfigurationRoot(".conf", "wallet.conf");
var walletSettings = config.GetSection("wallet").Get<WalletSettings>();
var lndConf = config.GetSection("lnd").Get<LndSettings>();
BitcoinSettings btcConf = config.GetSection("bitcoin").Get<BitcoinSettings>();
BitcoinNode bitcoinNode = new BitcoinNode(btcConf.AuthenticationString, btcConf.HostOrUri, btcConf.Network, btcConf.WalletName);

while (true)
{
    var nd1 = LND.GetNodeInfo(lndConf);
    if (nd1.SyncedToChain)
        break;

    TraceEx.TraceWarning("Node not synced to chain");
    if (bitcoinNode.IsRegTest)
    {
        TraceEx.TraceWarning("Mining 101");
        bitcoinNode.Mine101Blocks();
    }
    Thread.Sleep(1000);
}


Singlethon.LNDWalletManager = new LNDWalletManager(
    Enum.Parse<DBProvider>(walletSettings.DBProvider),
    walletSettings.ConnectionString.Replace("$HOME", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)),
    bitcoinNode,
    lndConf,
    walletSettings.AdminPublicKey,
    walletSettings.EnforceTwoFactorAuthentication);

Singlethon.LNDWalletManager.OnInvoiceStateChanged += (sender, e) =>
{
    if (Singlethon.InvoiceAsyncComQueue4ConnectionId.TryGetValue(e.PublicKey, out var acomQue))
        foreach (var asyncCom in acomQue.Values)
            asyncCom.Enqueue(e);

    if (Singlethon.InvoiceWebhookAsyncComQueue.TryGetValue(new(e.PublicKey, e.InvoiceStateChange.PaymentHash), out var webhook))
        webhook.que.Enqueue(e);
};

Thread webhookThread = new Thread(async () =>
{
TraceEx.TraceInformation("Webhook Thread Starting");
    while (true)
    {
        //TraceEx.TraceInformation("Webhook Loop Starting");
        try
        {
            foreach (var entry in Singlethon.InvoiceWebhookAsyncComQueue.ToList())
            {
                var (webhookUrl, asyncComQueue) = entry.Value;

                while (asyncComQueue.TryDequeue(out var invoiceStateChangedEventArgs))
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        var payload = JsonSerializer.Serialize(invoiceStateChangedEventArgs);
                        var content = new StringContent(payload, Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync(webhookUrl, content);
                        if (!response.IsSuccessStatusCode)
                        {
                            TraceEx.TraceWarning($"Failed to execute webhook for {entry.Key}. Status: {response.StatusCode}");
                        }
                        else
                        {
                            if (invoiceStateChangedEventArgs.InvoiceStateChange.NewState == InvoiceState.Cancelled || invoiceStateChangedEventArgs.InvoiceStateChange.NewState == InvoiceState.Settled)
                                Singlethon.InvoiceWebhookAsyncComQueue.TryRemove(entry.Key, out _);
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceEx.TraceException(ex);
                        TraceEx.TraceWarning($"Error executing webhook for {entry.Key}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TraceEx.TraceException(ex);
            TraceEx.TraceWarning($"Error in InvoiceWebhookAsyncComQueue processing: {ex.Message}");
        }

        try
        {
            foreach (var entry in Singlethon.InvoiceWebhookAsyncComQueue.ToList())
            {
                try
                {
                    var inv = await Singlethon.LNDWalletManager.GetAccount(entry.Key.pubkey.AsECXOnlyPubKey()).GetInvoiceAsync(entry.Key.paymentHash);
                    if (inv.State == InvoiceState.Cancelled)
                        entry.Value.que.Enqueue(new InvoiceStateChangedEventArgs() { PublicKey = entry.Key.pubkey, InvoiceStateChange = new InvoiceStateChange() { NewState = inv.State, PaymentHash = inv.PaymentHash } });
                }
                catch (Exception ex)
                {
                    TraceEx.TraceException(ex);
                    TraceEx.TraceWarning($"Error executing webhook for {entry.Key}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            TraceEx.TraceException(ex);
            TraceEx.TraceWarning($"Error in InvoiceWebhookAsyncComQueue processing: {ex.Message}");
        }

        await Task.Delay(1000); // Avoid busy looping

    }
    TraceEx.TraceInformation("TrackPayments Thread Joining");
});


Singlethon.LNDWalletManager.OnPaymentStatusChanged += (sender, e) =>
{
    if (Singlethon.PaymentAsyncComQueue4ConnectionId.TryGetValue(e.PublicKey, out var acomQue))
        foreach (var asyncCom in acomQue.Values)
            asyncCom.Enqueue(e);
};

Singlethon.LNDWalletManager.OnNewTransactionFound += (sender, e) =>
{
    if (Singlethon.TransactionAsyncComQueue4ConnectionId.TryGetValue(e.PublicKey, out var acomQue))
        foreach (var asyncCom in acomQue.Values)
            asyncCom.Enqueue(e);
};

Singlethon.LNDWalletManager.OnPayoutStateChanged += (sender, e) =>
{
    if (Singlethon.PayoutAsyncComQueue4ConnectionId.TryGetValue(e.PublicKey, out var acomQue))
        foreach (var asyncCom in acomQue.Values)
            asyncCom.Enqueue(e);
};


void SetInvoiceStateWebhook(string pubkey, string paymentHash, string webhook)
{
    var hook = Singlethon.InvoiceWebhookAsyncComQueue.GetOrAdd(new(pubkey, paymentHash), (key) => (webhook, new ()));
}

Singlethon.LNDWalletManager.Start();

LNDChannelManager channelManager = new LNDChannelManager(
    Singlethon.LNDWalletManager,
    lndConf.GetFriendNodes(),
    lndConf.MinSatoshisPerChannel,
    lndConf.MaxSatoshisPerChannel,
    walletSettings.MaxChannelCloseFeePerVByte);
channelManager.Start();

webhookThread.Start();


TraceEx.TraceInformation("... Running");


app.MapGet("/gettoken", (string pubkey) =>
{
    try
    {
        return new Result<Guid>(Singlethon.LNDWalletManager.GetTokenGuid(pubkey));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<Guid>(ex);
    }
})
.WithName("GetToken")
.WithSummary("Generates a unique authorization token GUID for API access")
.WithDescription("Returns a session-specific GUID associated with the provided public key. This GUID is used by the client to generate an authorization token for the current session. The actual token generation process occurs on the client side for enhanced security. This GUID serves as a unique identifier for the client's session and is linked to the public key for authentication purposes.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "64-character hex-encoded (32 bytes) Schnorr public key (secp256k1) that identifies the API user";
    return g;
})
.DisableAntiforgery();

app.MapGet("/validate", (string authToken) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken);
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("Validate")
.WithSummary("Validate Authorization Token")
.WithDescription("Validates the provided authorization token for authentication and access control. Returns a success result if the token is valid, or an error result if the token is invalid or expired.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/preparetwofactor", (string authToken, string issuer) =>
{
    try
    {
        return new Result<TwoFactorAuthSetup>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).PrepareTwoFactor(issuer));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<TwoFactorAuthSetup>(ex);
    }
})
.WithName("PrepareTwoFactor")
.WithSummary("Prepare Two-Factor Authentication")
.WithDescription("Prepares Two-Factor Authentication (2FA) for the user. This function generates a setup object containing a secret key that the user can use to configure their TOTP application. The user must scan the QR code with their 2FA app to complete the setup process. This endpoint is available only to users who have not yet enabled 2FA.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function.";
    g.Parameters[1].Description = "The issuer name for the TOTP application. This is typically the name of the application or service that the user is setting up 2FA for.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/enabletwofactor", (string authToken, string otp) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).EnableTwoFactor(otp);
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("EnableTwoFactor")
.WithSummary("Enable Two-Factor Authentication")
.WithDescription("Enables Two-Factor Authentication (2FA) for the user. This function requires a valid otp code for authentication. Once 2FA is enabled, the user will need to provide a TOTP code for authentication in addition to their regular credentials.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function.";
    g.Parameters[1].Description = "A valid OTP code to authenticate the enable request.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/istwofactorenabled", (string authToken) =>
{
    try
    {
        return new Result<bool>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).IsTwoFactorEnabled());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<bool>(ex);
    }
})
.WithName("IsTwoFactorEnabled")
.WithSummary("Check if Two-Factor Authentication is enabled")
.WithDescription("Checks if Two-Factor Authentication (2FA) is enabled for the user. This function returns a boolean value indicating whether 2FA is currently active for the user's account. It is useful for determining if the user needs to set up or disable 2FA.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/disabletwofactor", (string authToken, string code) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).DisableTwoFactor(code);
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("DisableTwoFactor")
.WithSummary("Disable Two-Factor Authentication")
.WithDescription("Disables Two-Factor Authentication (2FA) for the user. This function requires a valid single-use code for authentication. Once 2FA is disabled, the user will no longer need to provide a TOTP code for authentication.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function.";
    g.Parameters[1].Description = "A valid single-use code to authenticate the disable request.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/regeneratesingleusecodes", (string authToken, string otp) =>
{
    try
    {
        return new Result<string[]>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).RegenerateSingleUseCodes(otp));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<string[]>(ex);
    }
})
.WithName("RegenerateSingleUseCodes")
.WithSummary("Regenerate Single-Use Codes")
.WithDescription("Regenerates a set of single-use codes for Two-Factor Authentication (2FA). This function is useful if the user needs to generate new ones. The old codes are invalidated. The endpoint returns an array of new single-use codes.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function.";
    g.Parameters[1].Description = "A valid OTP code to authenticate the enable request.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/resettwofactor", (string authToken, string code, string issuer) =>
{
    try
    {
        return new Result<TwoFactorAuthSetup>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).ResetTwoFactor(code, issuer));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<TwoFactorAuthSetup>(ex);
    }
})
.WithName("ResetTwoFactor")
.WithSummary("Reset Two-Factor Authentication")
.WithDescription("Resets Two-Factor Authentication TOTP for the user. This function is used to generate a new secret key that the user can use to configure their TOTP application. The endpoint requires a valid single-use code for authentication.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function.";
    g.Parameters[1].Description = "A valid single-use code to authenticate the reset request.";
    g.Parameters[2].Description = "The issuer name for the TOTP application. This is typically the name of the application or service that the user is setting up 2FA for.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/topupandmine6blocks", (string authToken, string bitcoinAddr, long satoshis) =>
{
    try
    {
        if(satoshis<=0)
            throw new InvalidOperationException("Satoshis must be greater than 0");
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken);
        if (Singlethon.LNDWalletManager.BitcoinNode.IsRegTest)
            Singlethon.LNDWalletManager.BitcoinNode.TopUpAndMine6Blocks(bitcoinAddr, satoshis);
        else
            throw new InvalidOperationException("Bitcoin node is not in RegTest");
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("TopUpAndMine6Blocks")
.WithSummary("Sends specified amount of satoshis to a Bitcoin address and mines 6 blocks for confirmation (RegTest mode only)")
.WithDescription("In RegTest mode only: Sends the specified amount of satoshis from the local Bitcoin wallet to the provided Bitcoin address, then automatically mines 6 blocks to ensure transaction confirmation. This function is useful for testing and development purposes in a controlled environment.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token generated by the client using Schnorr Signatures for secp256k1. It encodes the user's public key and the session identifier returned by the GetToken function.";
    g.Parameters[1].Description = "Bitcoin address to receive the funds";
    g.Parameters[2].Description = "Amount of satoshis to send";
    return g;
})
.DisableAntiforgery();

app.MapGet("/sendtoaddress", (string authToken, string bitcoinAddr, long satoshis, string otp) =>
{
    try
    {
        if (satoshis <= 0)
            throw new InvalidOperationException("Satoshis must be greater than 0");
        var pubkey= Singlethon.LNDWalletManager.ValidateAuthToken(authToken, Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal:TokenType.Admin);
        Singlethon.LNDWalletManager.VerifyTotp(pubkey, otp);
        Singlethon.LNDWalletManager.BitcoinNode.SendToAddress(bitcoinAddr, satoshis);
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("SendToAddress")
.WithSummary("Sends Bitcoin from local wallet to a specified address (Admin-only in non-RegTest modes)")
.WithDescription("Transfers the specified amount of satoshis from the local Bitcoin wallet to the provided Bitcoin address. In RegTest mode, this function is available to all users. In other modes (TestNet, MainNet), only administrators can use this function.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    g.Parameters[1].Description = "Bitcoin address";
    g.Parameters[2].Description = "Number of satoshis";
    g.Parameters[3].Description = "One-Time Password (OTP) for Two-Factor Authentication. This is a time-based code generated by the user's TOTP application, used to verify the user's identity before executing the transaction.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/generateblocks", (string authToken, int blocknum) =>
{
    try
    {
        if (blocknum <= 0)
            throw new InvalidOperationException("Blucknum must be greater than 0");
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken);
        if (Singlethon.LNDWalletManager.BitcoinNode.IsRegTest)
            Singlethon.LNDWalletManager.BitcoinNode.GenerateBlocks(blocknum);
        else
            throw new InvalidOperationException("Bitcoin node is not in RegTest");

        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("GenerateBlocks")
.WithSummary("Generate new blocks in RegTest mode")
.WithDescription("Mines a specified number of new blocks in the Bitcoin network. This operation is only available in RegTest mode, which is used for testing and development purposes.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token generated by the client using Schnorr Signatures for secp256k1. It encodes the user's public key and the session identifier returned by the GetToken function.";
    g.Parameters[1].Description = "The number of new blocks to generate. Must be a positive integer.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/newbitcoinaddress", (string authToken) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken, Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal : TokenType.Admin);
        return new Result<string>(Singlethon.LNDWalletManager.BitcoinNode.NewAddress());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<string>(ex);
    }
})
.WithName("NewBitcoinAddress")
.WithSummary("Generate a new Bitcoin address for the local wallet")
.WithDescription("Creates and returns a new Bitcoin address associated with the local Bitcoin wallet. This endpoint provides different access levels based on the network mode: in RegTest mode, it's accessible to all users, while in TestNet and MainNet modes, it's restricted to administrators only. This feature enables secure fund management and testing in various network environments.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/getbitcoinwalletbalance", (string authToken, int minConf) =>
{
    try
    {
        if (minConf < 0)
            throw new InvalidOperationException("Blucknum must be greater equal 0");
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken,Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal : TokenType.Admin);
        return new Result<long>(Singlethon.LNDWalletManager.BitcoinNode.WalletBalance(minConf));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<long>(ex);
    }
})
.WithName("GetBitcoinWalletBalance")
.WithSummary("Retrieve the current balance of the Bitcoin wallet")
.WithDescription("Fetches and returns the current balance of the Bitcoin wallet in satoshis. The balance returned is based on the specified minimum number of confirmations. This endpoint has different access levels: in RegTest mode, it's accessible to all users, while in TestNet and MainNet modes, it's restricted to administrators only.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    g.Parameters[1].Description = "The minimum number of confirmations required for transactions to be included in the balance calculation. This parameter allows for flexibility in determining the level of certainty for the reported balance.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/getlndwalletbalance", (string authToken) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken,Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal : TokenType.Admin);
        var ret = Singlethon.LNDWalletManager.GetWalletBalance();
        return new Result<LndWalletBalanceRet>(
            new LndWalletBalanceRet
            {
                ConfirmedBalance = ret.ConfirmedBalance,
                UnconfirmedBalance = ret.UnconfirmedBalance,
                TotalBalance = ret.TotalBalance,
                ReservedBalanceAnchorChan = ret.ReservedBalanceAnchorChan,
                LockedBalance = ret.LockedBalance,
            });
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<LndWalletBalanceRet>(ex);
    }
})
.WithName("GetLndWalletBalance")
.WithSummary("Retrieve the current balance of the LND wallet")
.WithDescription("Fetches and returns the current balance of the LND (Lightning Network Daemon) wallet, including confirmed, unconfirmed, total, reserved, and locked balances. This endpoint provides different access levels based on the network mode: in RegTest mode, it's accessible to all users, while in TestNet and MainNet modes, it's restricted to administrators only.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/openreserve", (string authToken, long satoshis, string otp) =>
{
    try
    {
        if (satoshis <= 0)
            throw new InvalidOperationException("Satoshis must be greater than 0");
        var pubkey = Singlethon.LNDWalletManager.ValidateAuthToken(authToken,Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal : TokenType.Admin);
        Singlethon.LNDWalletManager.VerifyTotp(pubkey, otp);
        return new Result<Guid>(Singlethon.LNDWalletManager.OpenReserve(satoshis));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<Guid>(ex);
    }
})
.WithName("OpenReserve")
.WithSummary("Open a new reserve in the LND wallet")
.WithDescription("Creates a new reserve in the LND wallet, allocating a specified amount of satoshis. This operation is useful for setting aside funds for future transactions or channel openings. The endpoint returns a unique identifier (GUID) for the newly created reserve. Access to this endpoint varies based on the network mode: in RegTest mode, it's accessible to all users, while in TestNet and MainNet modes, it's restricted to administrators only.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    g.Parameters[1].Description = "The amount of satoshis to allocate to the new reserve. This value must be a positive integer representing the number of satoshis (1 satoshi = 0.00000001 BTC).";
    g.Parameters[2].Description = "One-Time Password (OTP) for Two-Factor Authentication. This is a time-based code generated by the user's TOTP application, used to verify the user's identity before executing the reserve opening.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/closereserve", (string authToken, Guid reserveId, string otp) =>
{
    try
    {
        var pubkey=Singlethon.LNDWalletManager.ValidateAuthToken(authToken,Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal : TokenType.Admin);
        Singlethon.LNDWalletManager.VerifyTotp(pubkey, otp);
        Singlethon.LNDWalletManager.CloseReserve(reserveId);
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("CloseReserve")
.WithSummary("Close a specific reserve in the LND wallet")
.WithDescription("Closes a previously opened reserve in the LND wallet, identified by its unique GUID. This operation releases the allocated funds back to the main wallet balance. Access to this endpoint varies based on the network mode: in RegTest mode, it's accessible to all users, while in TestNet and MainNet modes, it's restricted to administrators only.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    g.Parameters[1].Description = "The unique identifier (GUID) of the reserve to be closed. This GUID was returned when the reserve was initially opened using the OpenReserve endpoint.";
    g.Parameters[2].Description = "One-Time Password (OTP) for Two-Factor Authentication. This is a time-based code generated by the user's TOTP application, used to verify the user's identity before executing the reserve closing.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/listorphanedreserves", (string authToken) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken, Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal : TokenType.Admin);
        return new Result<Guid[]>(Singlethon.LNDWalletManager.ListOrphanedReserves().ToArray());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<Guid[]>(ex);
    }
})
.WithName("ListOrphanedReserves")
.WithSummary("Retrieve a list of orphaned reserves in the LND wallet")
.WithDescription("Returns an array of unique identifiers (GUIDs) for orphaned reserves in the LND wallet. Orphaned reserves are those that are not associated with any payouts, whether active or not active. This endpoint is useful for identifying and managing unused allocated funds. Access to this endpoint varies based on the network mode: in RegTest mode, it's accessible to all users, while in TestNet and MainNet modes, it's restricted to administrators only.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/listchannels", (string authToken, bool activeOnly) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken, Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal : TokenType.Admin);
        return new Result<Lnrpc.Channel[]>(Singlethon.LNDWalletManager.ListChannels(activeOnly).Channels.ToArray());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<Lnrpc.Channel[]>(ex);
    }
})
.WithName("ListChannels")
.WithSummary("List Lightning Network Channels")
.WithDescription("Retrieves a list of Lightning Network channels associated with the LND node. This endpoint provides detailed information about each channel, including its capacity, balance, and current state. It can be used to monitor the node's connectivity and liquidity within the Lightning Network. The response includes both active and inactive channels, depending on the 'activeOnly' parameter.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key and session identifier. Admin-level token required for TestNet and MainNet modes.";
    g.Parameters[1].Description = "If true, returns only active channels. If false, returns all channels including inactive ones.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/listclosedchannels", (string authToken) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthToken(authToken, Singlethon.LNDWalletManager.BitcoinNode.IsRegTest ? TokenType.Normal : TokenType.Admin);
        return new Result<Lnrpc.ChannelCloseSummary[]>(Singlethon.LNDWalletManager.ClosedChannels().Channels.ToArray());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<Lnrpc.ChannelCloseSummary[]>(ex);
    }
})
.WithName("ListClosedChannels")
.WithSummary("List Closed Lightning Network Channels")
.WithDescription("Retrieves a list of closed Lightning Network channels associated with the LND node. This endpoint provides detailed information about each closed channel, including its capacity, closing transaction, and settlement details. It can be used to review the history of closed channels and analyze past network connections.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/estimatefee", (string authToken, string address, long satoshis) =>
{
    bool isAdmin = false;
    try
    {
        if (satoshis < 0)
            throw new InvalidOperationException("Satoshis must be greater equal 0");
        var pubKey = Singlethon.LNDWalletManager.ValidateAuthToken(authToken);
        isAdmin = Singlethon.LNDWalletManager.HasAdminRights(pubKey); 
        var (feesat, satspervbyte) = Singlethon.LNDWalletManager.EstimateFee(address, satoshis);
        var closingChannelFeeSat = Singlethon.LNDWalletManager.EstimateChannelClosingFee();
        return new Result<FeeEstimateRet>(new FeeEstimateRet { TxFeeSat = feesat, SatPerVbyte = satspervbyte, ChannelClosingFeeSat = closingChannelFeeSat});
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        if (isAdmin)
            return new Result<FeeEstimateRet>(ex);
        else
            return new Result<FeeEstimateRet>(new FeeEstimateRet { TxFeeSat = -1, SatPerVbyte = 0 });
    }
})
.WithName("EstimateFee")
.WithSummary("Estimate total fees for potential payout including channel closing and Bitcoin transaction fees")
.WithDescription("Provides a comprehensive fee estimation for a potential payout that includes both the Bitcoin transaction fee and the Lightning Network channel closing fee. This endpoint calculates the estimated on-chain transaction fee based on the provided destination address and amount, as well as the potential fee for closing a Lightning Network channel. It returns the total Bitcoin transaction fee in satoshis, the fee rate in satoshis per virtual byte, and the estimated channel closing fee. This information is crucial for users to understand the full cost implications of their payout, including the possibility of needing to close a channel to fulfill the payout. It helps users make informed decisions about their fund management and transaction strategies. In case of any issues or errors, only administrators will receive detailed error information to maintain system security and privacy. Non-admin users will receive a generic response indicating an estimation problem without specific details.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function. It determines the user's access level and the level of detail in the response. Admin tokens provide access to detailed error information if issues occur.";
    g.Parameters[1].Description = "The destination Bitcoin address for the potential payout transaction. This should be a valid Bitcoin address where the funds would be sent if the payout is executed.";
    g.Parameters[2].Description = "The amount of the potential payout, specified in satoshis (1 satoshi = 0.00000001 BTC). This value must be a positive integer representing the exact amount of the intended payout, which will be used to calculate the appropriate fees.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/getbalance",(string authToken) =>
{
    try
    {
        return new Result<AccountBalanceDetails>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).GetBalance());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<AccountBalanceDetails>(ex);
    }
})
.WithName("GetBalance")
.WithSummary("Retrieve the current balance of the user's account")
.WithDescription("This endpoint provides detailed information about the user's account balance. The balance is returned as an AccountBalanceDetails object, which includes the total balance, available balance, and any pending transactions. All amounts are in satoshis (1 BTC = 100,000,000 satoshis). ")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/newaddress", (string authToken) =>
{
    try
    {
        return new Result<string>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).CreateNewTopupAddress());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<string>(ex);
    }
})
.WithName("NewAddress")
.WithSummary("Generate a new Bitcoin address for account top-up")
.WithDescription("Creates and returns a new Bitcoin address associated with the user's Lightning Network account. This address can be used to receive on-chain Bitcoin payments, which will then be credited to the user's Lightning Network balance. This feature enables seamless integration between on-chain and off-chain funds management.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/listtransactions", (string authToken) =>
{
    try
    {
        return new Result<TransactionRecord[]>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).ListTransactions().ToArray());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<TransactionRecord[]>(ex);
    }
})
.WithName("ListTransactions")
.WithSummary("List Topup transaction")
.WithDescription("List Topup transaction")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/registerpayout", (string authToken, long satoshis, string btcAddress, string otp) =>
{
    try
    {
        if (satoshis < 0)
            throw new InvalidOperationException("Satoshis must be greater equal 0");
        return new Result<Guid>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).VerifyTotp(otp).RegisterNewPayoutForExecution(satoshis, btcAddress));
    }
    catch(Exception ex) 
    {
        TraceEx.TraceException(ex);
        return new Result<Guid>(ex);
    }
})
.WithName("RegisterPayout")
.WithSummary("Register a new payout request to the Bitcoin blockchain")
.WithDescription("Initiates a new payout request from the user's Lightning Network wallet to a specified Bitcoin address on the blockchain. This operation registers the payout for execution, which may involve closing Lightning channels if necessary to fulfill the requested amount. The method returns a unique identifier (GUID) for tracking the payout request.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function.";
    g.Parameters[1].Description = "The amount to be paid out, specified in satoshis (1 satoshi = 0.00000001 BTC). Must be a positive integer representing the exact payout amount.";
    g.Parameters[2].Description = "The destination Bitcoin address where the payout will be sent. This should be a valid Bitcoin address on the blockchain.";
    g.Parameters[3].Description = "One-Time Password (OTP) for Two-Factor Authentication. This is a time-based code generated by the user's TOTP application, used to verify the user's identity before executing the payout registration.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/listpayouts", (string authToken) =>
{
    try
    {
        return new Result<PayoutRecord[]>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).ListPayouts().ToArray());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<PayoutRecord[]>(ex);
    }
})
.WithName("ListPayouts")
.WithSummary("List registered payouts")
.WithDescription("List registered payouts")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/getpayout", (string authToken, Guid payoutId) =>
{
    try
    {
        return new Result<PayoutRecord>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).GetPayout(payoutId));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<PayoutRecord>(ex);
    }
})
.WithName("GetPayout")
.WithSummary("Get registered payouts")
.WithDescription("Get registered payouts")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token is generated using Schnorr Signatures for secp256k1 and encodes the user's public key along with the session identifier obtained from the GetToken function. For TestNet and MainNet modes, an admin-level token is required.";
    g.Parameters[1].Description = "Payout id";
    return g;
})
.DisableAntiforgery();


app.MapGet("/addinvoice", (string authToken, long satoshis, string memo, long expiry) =>
{
    try
    {
        if (satoshis < 0)
            throw new InvalidOperationException("Satoshis must be greater equal 0");
        if (expiry < 0)
            throw new InvalidOperationException("Expiry must be greater equal 0");
        return new Result<InvoiceRecord>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken, TokenType.LongTerm).CreateNewClassicInvoice(satoshis, memo, expiry));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<InvoiceRecord>(ex);
    }
})
.WithName("AddInvoice")
.WithSummary("Generate a new Lightning Network invoice")
.WithDescription("Creates and returns a new Lightning Network invoice for receiving payments. This endpoint allows users to generate payment requests with customizable amount, memo, and expiration time. The created invoice can be shared with payers to facilitate Lightning Network transactions.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function. This call supports long living tokens.";
    g.Parameters[1].Description = "The amount of the invoice in satoshis (1 BTC = 100,000,000 satoshis). Must be a positive integer representing the exact payment amount requested.";
    g.Parameters[2].Description = "An optional memo or description for the invoice. This can be used to provide additional context or details about the payment to the payer. The memo will be included in the encoded payment request.";
    g.Parameters[3].Description = "The expiration time for the payment request, in seconds. After this duration, the invoice will no longer be valid for payment.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/addhodlinvoice", (string authToken, long satoshis, string hash, string memo, long expiry) =>
{
    try
    {
        if (satoshis < 0)
            throw new InvalidOperationException("Satoshis must be greater equal 0");
        if (expiry < 0)
            throw new InvalidOperationException("Expiry must be greater equal 0");
        return new Result<InvoiceRecord>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken, TokenType.LongTerm ).CreateNewHodlInvoice(satoshis, memo, hash.AsBytes(), expiry));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<InvoiceRecord>(ex);
    }
})
.WithName("AddHodlInvoice")
.WithSummary("Generate a new Lightning Network HODL invoice for escrow-like functionality")
.WithDescription("Creates and returns a new Lightning Network HODL invoice. HODL invoices enable escrow-like functionalities by allowing the recipient to claim the payment only when a specific preimage is revealed using the SettleInvoice method. This preimage must be provided by the payer or a trusted third party. This mechanism provides an additional layer of security and enables conditional payments in the Lightning Network, making it suitable for implementing escrow accounts and other advanced payment scenarios.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function. This call supports long living tokens.";
    g.Parameters[1].Description = "The amount of the invoice in satoshis (1 BTC = 100,000,000 satoshis). Must be a positive integer representing the exact payment amount requested.";
    g.Parameters[2].Description = "The SHA-256 hash of the preimage. The payer or a trusted third party must provide the corresponding preimage, which will be used with the SettleInvoice method to claim the payment, enabling escrow-like functionality.";
    g.Parameters[3].Description = "An optional memo or description for the invoice. This can be used to provide additional context or details about the payment or escrow conditions to the payer. The memo will be included in the encoded payment request.";
    g.Parameters[4].Description = "The expiration time for the payment request, in seconds. After this duration, the HODL invoice will no longer be valid for payment. Consider setting an appropriate duration based on the expected escrow period.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/decodeinvoice", (string authToken, string paymentRequest) =>
{
    try
    {
        return new Result<PaymentRequestRecord>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken, TokenType.LongTerm).DecodeInvoice(paymentRequest));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<PaymentRequestRecord>(ex);
    }
})
.WithName("DecodeInvoice")
.WithSummary("Decode a Lightning Network invoice")
.WithDescription("This endpoint decodes a Lightning Network invoice (also known as a payment request) and returns detailed information about its contents. It provides insights into the payment amount, recipient, expiry time, and other relevant metadata encoded in the invoice.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function. This call supports long living tokens.";
    g.Parameters[1].Description = "The Lightning Network invoice string to be decoded. This is typically a long string starting with 'lnbc' for mainnet or 'lntb' for testnet.";
    return g;
})
.DisableAntiforgery();


app.MapGet("/sendpayment", async (string authToken, string paymentrequest, int timeout, long feelimit, string otp) =>
{
    try
    {
        if (timeout < 0)
            throw new InvalidOperationException("Timeout must be greater equal 0");
        if (feelimit < 0)
            throw new InvalidOperationException("Feelimit must be greater equal 0");
        return new Result<PaymentRecord>(await Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).VerifyTotp(otp).SendPaymentAsync(paymentrequest, timeout, walletSettings.SendPaymentFee, feelimit));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<PaymentRecord>(ex);
    }
})
.WithName("SendPayment")
.WithSummary("Send a Lightning Network payment")
.WithDescription("Initiates a Lightning Network payment based on the provided payment request. This endpoint attempts to route the payment to its final destination, handling all necessary channel operations and routing decisions.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function.";
    g.Parameters[1].Description = "The Lightning Network payment request (invoice) to be paid. This encoded string contains all necessary details for the payment, including amount and recipient.";
    g.Parameters[2].Description = "Maximum time (in seconds) allowed for finding a route for the payment. If a route isn't found within this time, the payment attempt will be aborted.";
    g.Parameters[3].Description = "Maximum fee (in millisatoshis) that the user is willing to pay for this transaction. If the calculated fee exceeds this limit, the payment will not be sent.";
    g.Parameters[4].Description = "One-Time Password (OTP) for Two-Factor Authentication. This is a time-based code generated by the user's TOTP application, used to verify the user's identity before executing the payment.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/estimateroutefee", (string authToken, string paymentrequest, int timeout) =>
{
    try
    {
        if (timeout < 0)
            throw new InvalidOperationException("Timeout must be greater equal 0");
        return new Result<RouteFeeRecord>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken, TokenType.LongTerm).EstimateRouteFee(paymentrequest, walletSettings.SendPaymentFee, timeout));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<RouteFeeRecord>(ex);
    }
})
.WithName("EstimateRouteFee")
.WithSummary("Estimate Lightning Network payment route fee")
.WithDescription("This endpoint calculates and returns an estimated fee for routing a Lightning Network payment based on the provided payment request. It helps users anticipate the cost of sending a payment before actually initiating the transaction.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function. This call supports long living tokens.";
    g.Parameters[1].Description = "The Lightning Network payment request (invoice) for which the route fee is to be estimated. This encoded string contains necessary details such as the payment amount and recipient.";
    g.Parameters[2].Description = "Maximum probing time (in seconds) allowed for finding a routing fee for the payment.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/settleinvoice", (string authToken, string preimage) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken, TokenType.LongTerm).SettleInvoice(preimage.AsBytes());
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("SettleInvoice")
.WithSummary("Settle a Lightning Network Hold Invoice")
.WithDescription("Settles a previously accepted hold invoice using the provided preimage. This action finalizes the payment process for a hold invoice, releasing the funds to the invoice creator.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function. This call supports long living tokens.";
    g.Parameters[1].Description = "The preimage (32-byte hash preimage) that corresponds to the payment hash of the hold invoice to be settled. This preimage serves as proof of payment.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/cancelinvoice", (string authToken, string paymenthash) =>
{
    try
    {
        Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken, TokenType.LongTerm).CancelInvoice(paymenthash);
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("CancelInvoice")
.WithSummary("Cancel a Lightning Network Invoice")
.WithDescription("Cancels an open Lightning Network invoice. This endpoint allows users to cancel an invoice that hasn't been paid yet. If the invoice is already canceled, the operation succeeds. However, if the invoice has been settled, the cancellation will fail.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function. This call supports long living tokens.";
    g.Parameters[1].Description = "The payment hash of the invoice to be canceled. This unique identifier is used to locate the specific invoice in the system.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/getinvoice", async (string authToken, string paymenthash) =>
{
    try
    {
        return new Result<InvoiceRecord>(await Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken, TokenType.LongTerm).GetInvoiceAsync(paymenthash));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<InvoiceRecord>(ex);
    }
})
.WithName("GetInvoice")
.WithSummary("Retrieve a specific Lightning Network invoice")
.WithDescription("Fetches and returns detailed information about a specific Lightning Network invoice identified by its payment hash. This endpoint allows users to access invoice details such as amount, status, and creation date.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function. This call supports long living tokens.";
    g.Parameters[1].Description = "The payment hash of the invoice to retrieve. This unique identifier is used to locate the specific invoice in the system.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/listinvoices", (string authToken) =>
{
    try
    {
        return new Result<InvoiceRecord[]>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).ListInvoices(true,true));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<InvoiceRecord[]>(ex);
    }
})
.WithName("ListInvoices")
.WithSummary("Retrieve all invoices for the authenticated account")
.WithDescription("This endpoint returns a comprehensive list of all invoices associated with the authenticated user's account. It includes both paid and unpaid invoices, providing a complete overview of the account's invoice history.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/listpayments", (string authToken) =>
{
    try
    {
        return new Result<PaymentRecord[]>(Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).ListNotFailedPayments());
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<PaymentRecord[]>(ex);
    }
})
.WithName("ListPayments")
.WithSummary("Retrieve all successful and in-progress payments for the account")
.WithDescription("This endpoint provides a list of all payments associated with the authenticated user's account that have not failed. This includes successful payments and those that are still in progress, offering a clear view of the account's payment activity.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/getpayment", async (string authToken, string paymenthash) =>
{
    try
    {
        return new Result<PaymentRecord>(await Singlethon.LNDWalletManager.ValidateAuthTokenAndGetAccount(authToken).GetPaymentAsync(paymenthash));
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result<PaymentRecord>(ex);
    }
})
.WithName("GetPayment")
.WithSummary("Retrieve details of a specific payment")
.WithDescription("This endpoint fetches and returns detailed information about a specific payment identified by its payment hash. It provides comprehensive data about the payment, including its status, amount, and other relevant details.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function.";
    g.Parameters[1].Description = "Unique identifier (hash) of the payment to be retrieved. This hash is used to locate the specific payment record in the system.";
    return g;
})
.DisableAntiforgery();

app.MapGet("/health", () =>
{
    return Results.Ok("ok");
})
.WithName("Health")
.WithSummary("Health check endpoint")
.WithDescription("This endpoint returns a status 200 and 'ok' to indicate that the service is running properly.")
.DisableAntiforgery();


app.MapGet("/setinvoicestatewebhook", async (string authToken, string paymenthash, string webhook) =>
{
    try
    {
        var pubkey = Singlethon.LNDWalletManager.ValidateAuthToken(authToken, TokenType.LongTerm);
        SetInvoiceStateWebhook(pubkey, paymenthash, webhook);
        return new Result();
    }
    catch (Exception ex)
    {
        TraceEx.TraceException(ex);
        return new Result(ex);
    }
})
.WithName("SetInvoiceStateWebhook")
.WithSummary("Set webhook for invoice state changes")
.WithDescription("This endpoint allows setting a webhook URL to receive real-time updates about the state changes of a specific invoice. The webhook will be triggered whenever the state of the invoice changes, such as when it is paid, expired, or canceled.")
.WithOpenApi(g =>
{
    g.Parameters[0].Description = "Authorization token for authentication and access control. This token, generated using Schnorr Signatures for secp256k1, encodes the user's public key and session identifier from the GetToken function.This call supports long living tokens.";
    g.Parameters[1].Description = "The payment hash of the invoice for which the webhook is being set. This unique identifier is used to locate the specific invoice in the system.";
    g.Parameters[2].Description = "The webhook URL to be called when the state of the invoice changes. This URL should be accessible and capable of handling POST requests with the state change details.";
    return g;
})
.DisableAntiforgery();


// Map the InvoiceStateUpdatesHub to the "/invoicestateupdates" endpoint
// This hub allows real-time updates on invoice state changes
// Clients can connect to this hub to receive immediate notifications when an invoice's state changes
app.MapHub<InvoiceStateUpdatesHub>("/invoicestateupdates")
.WithSummary("Real-time invoice state update hub")
.WithDescription("This endpoint establishes a WebSocket connection for real-time updates on invoice state changes. Clients can subscribe to receive immediate notifications when the state of an invoice changes, such as when it becomes paid or expires.")
.DisableAntiforgery();

// Map the PaymentStatusUpdatesHub to the "/paymentstatusupdates" endpoint
// This hub provides real-time updates on payment status changes
// Clients can connect to this hub to receive immediate notifications about payment status changes
app.MapHub<PaymentStatusUpdatesHub>("/paymentstatusupdates")
.WithSummary("Real-time payment status update hub")
.WithDescription("This endpoint establishes a WebSocket connection for real-time updates on payment status changes. Clients can subscribe to receive immediate notifications when the status of a payment changes, such as when it becomes successful, fails, or is in progress.")
.DisableAntiforgery();

app.MapHub<TransactionUpdatesHub>("/transactionupdates")
.WithSummary("Real-time transaction status update hub")
.WithDescription("This endpoint establishes a WebSocket connection for real-time updates on transaction status changes. Clients can subscribe to receive immediate notifications when the status of a payment changes, such as when it becomes successful, fails, or is in progress.")
.DisableAntiforgery();

app.MapHub<PayoutStateUpdatesHub>("/payoutstateupdates")
.WithSummary("Real-time payout state update hub")
.WithDescription("This endpoint establishes a WebSocket connection for real-time updates on payout state changes. Clients can subscribe to receive immediate notifications when the status of a payment changes, such as when it becomes successful, fails, or is in progress.")
.DisableAntiforgery();


app.Run(walletSettings.ListenHost.AbsoluteUri);

[Serializable]
public class Result 
{
    public Result() { }
    public Result(Exception exception) {
        ErrorCode = LNDWalletErrorCode.OperationFailed;
        if (exception is LNDWalletException ex)
            ErrorCode = ex.ErrorCode;
        ErrorMessage = exception.Message;
    }
    public LNDWalletErrorCode ErrorCode { get; set; } = LNDWalletErrorCode.Ok;
    public string ErrorMessage { get; set; } = "";
}

[Serializable]
public class Result<T> 
{
    public Result(T value) { Value = value; }
    public Result(Exception exception) {
        ErrorCode = LNDWalletErrorCode.OperationFailed;
        if (exception is LNDWalletException ex)
            ErrorCode = ex.ErrorCode;
        ErrorMessage = exception.Message;
    }
    public T? Value { get; set; } = default;
    public LNDWalletErrorCode ErrorCode { get; set; } = LNDWalletErrorCode.Ok;
    public string ErrorMessage { get; set; } = "";
}


[Serializable]
public struct FeeEstimateRet
{
    public long TxFeeSat { get; set; }
    public long ChannelClosingFeeSat { get; set; }
    public ulong SatPerVbyte { get; set; }
}

[Serializable]
public struct PaymentRequestAndHashRet
{
    public string PaymentRequest { get; set; }
    public string PaymentHash { get; set; }
}

[Serializable]
public struct LndWalletBalanceRet
{
    public long ConfirmedBalance { get; set; }
    public long LockedBalance { get; set; }
    public long ReservedBalanceAnchorChan { get; set; }
    public long TotalBalance { get; set; }
    public long UnconfirmedBalance { get; set; }
}

public class WalletSettings
{
    public required string AdminPublicKey { get; set; }
    public required Uri ListenHost { get; set; }
    public required Uri ServiceUri { get; set; }
    public required string DBProvider { get; set; }
    public required string ConnectionString { get; set; }
    public required long SendPaymentFee { get; set; }
    public required long MaxChannelCloseFeePerVByte { get; set; }
    public required bool EnforceTwoFactorAuthentication { get; set; }

}

public class LndSettings : LND.NodeSettings
{
    public string FriendNodes { get; set; }
    public long MinSatoshisPerChannel { get; set; }
    public long MaxSatoshisPerChannel { get; set; }

    public List<string> GetFriendNodes()
    {
        return (from s in JsonArray.Parse(FriendNodes).AsArray() select s.GetValue<string>()).ToList();
    }
}

public class BitcoinSettings
{
    public required string AuthenticationString { get; set; }
    public required string HostOrUri { get; set; }
    public required string Network { get; set; }
    public required string WalletName { get; set; }

}

public class CustomModelDocumentFilter<T> : IDocumentFilter where T : class
{
    public void Apply(OpenApiDocument openapiDoc, DocumentFilterContext context)
    {
        context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
    }
}

/// <summary>
/// Add enum value descriptions to Swagger
/// https://stackoverflow.com/a/49941775/1910735
/// </summary>
public class EnumDocumentFilter : IDocumentFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (KeyValuePair<string, OpenApiPathItem> schemaDictionaryItem in swaggerDoc.Paths)
        {
            OpenApiPathItem schema = schemaDictionaryItem.Value;
            foreach (OpenApiParameter property in schema.Parameters)
            {
                IList<IOpenApiAny> propertyEnums = property.Schema.Enum;
                if (propertyEnums.Count > 0)
                    property.Description += DescribeEnum(propertyEnums);
            }
        }

        if (swaggerDoc.Paths.Count == 0)
            return;

        // add enum descriptions to input parameters
        foreach (OpenApiPathItem pathItem in swaggerDoc.Paths.Values)
        {
            DescribeEnumParameters(pathItem.Parameters);

            foreach (KeyValuePair<OperationType, OpenApiOperation> operation in pathItem.Operations)
                DescribeEnumParameters(operation.Value.Parameters);
        }
    }

    private static void DescribeEnumParameters(IList<OpenApiParameter> parameters)
    {
        if (parameters == null)
            return;

        foreach (OpenApiParameter param in parameters)
        {
            if (param.Schema.Enum?.Any() == true)
            {
                param.Description += DescribeEnum(param.Schema.Enum);
            }
            else if (param.Extensions.ContainsKey("enum") &&
                     param.Extensions["enum"] is IList<object> paramEnums &&
                     paramEnums.Count > 0)
            {
                param.Description += DescribeEnum(paramEnums);
            }
        }
    }

    private static string DescribeEnum(IEnumerable<object> enums)
    {
        List<string> enumDescriptions = new();
        Type? type = null;
        foreach (object enumOption in enums)
        {
            if (type == null)
                type = enumOption.GetType();

            enumDescriptions.Add($"{Convert.ChangeType(enumOption, type.GetEnumUnderlyingType())} = {Enum.GetName(type, enumOption)}");
        }

        return Environment.NewLine + string.Join(Environment.NewLine, enumDescriptions);
    }
}

//https://stackoverflow.com/a/60276722/4390133
public class EnumFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is null)
            throw new ArgumentNullException(nameof(schema));

        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (context.Type.IsEnum is false)
            return;

        schema.Extensions.Add("x-ms-enum", new EnumFilterOpenApiExtension(context));
    }
}

public class EnumFilterOpenApiExtension : IOpenApiExtension
{
    private readonly SchemaFilterContext _context;
    public EnumFilterOpenApiExtension(SchemaFilterContext context)
    {
        _context = context;
    }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        JsonSerializerOptions options = new() { WriteIndented = true };

        var obj = new
        {
            name = _context.Type.Name,
            modelAsString = false,
            values = _context.Type
                            .GetEnumValues()
                            .Cast<object>()
                            .Distinct()
                            .Select(value => new { value, name = value.ToString() })
                            .ToArray()
        };
        writer.WriteRaw(JsonSerializer.Serialize(obj, options));
    }
}

/// <summary>
/// Adds extra schema details for an enum in the swagger.json i.e. x-enumNames (used by NSwag to generate Enums for C# client)
/// https://github.com/RicoSuter/NSwag/issues/1234
/// </summary>
public class NSwagEnumExtensionSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is null)
            throw new ArgumentNullException(nameof(schema));

        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (context.Type.IsEnum)
            schema.Extensions.Add("x-enumNames", new NSwagEnumOpenApiExtension(context));
    }
}

public class NSwagEnumOpenApiExtension : IOpenApiExtension
{
    private readonly SchemaFilterContext _context;
    public NSwagEnumOpenApiExtension(SchemaFilterContext context)
    {
        _context = context;
    }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        string[] enums = Enum.GetNames(_context.Type);
        JsonSerializerOptions options = new() { WriteIndented = true };
        string value = JsonSerializer.Serialize(enums, options);
        writer.WriteRaw(value);
    }
}