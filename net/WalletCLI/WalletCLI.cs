﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;

using ApiBtc;
using ApiBtc.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using NBitcoin;
using NBitcoin.RPC;
using NBitcoin.Secp256k1;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Sharprompt;
using Spectre.Console;

namespace GigLNDWalletCLI;

public class GigLNDWalletCLI
{

    public sealed class DefaultRetryPolicy : IRetryPolicy
    {
        private static TimeSpan?[] DefaultBackoffTimes = new TimeSpan?[]
        {
        TimeSpan.Zero,
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30),
        null
        };

        TimeSpan?[] backoffTimes;

        public DefaultRetryPolicy()
        {
            this.backoffTimes = DefaultBackoffTimes;
        }

        public DefaultRetryPolicy(TimeSpan?[] customBackoffTimes)
        {
            this.backoffTimes = customBackoffTimes;
        }

        public TimeSpan? NextRetryDelay(RetryContext context)
        {
            if (context.PreviousRetryCount >= this.backoffTimes.Length)
                return null;

            return this.backoffTimes[context.PreviousRetryCount];
        }
    }

    UserSettings userSettings;
    IWalletAPI walletClient;
    IInvoiceStateUpdatesClient invoiceStateUpdatesClient;
    IPaymentStatusUpdatesClient paymentStatusUpdatesClient;
    CancellationTokenSource CancellationTokenSource = new();

    static IConfigurationRoot GetConfigurationRoot(string? basePath, string[] args, string defaultFolder, string iniName)
    {
        if (basePath == null)
        {
            basePath = Environment.GetEnvironmentVariable("WALLET_BASEDIR");
            if (basePath == null)
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), defaultFolder);
        }
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(basePath)
               .AddIniFile(iniName)
               .AddEnvironmentVariables()
               .AddCommandLine(args);

        return builder.Build();
    }

    public GigLNDWalletCLI(string[] args, string baseDir, string sfx)
    {
        if (sfx == null)
            sfx = AnsiConsole.Prompt(new TextPrompt<string>("Enter the [orange1]config suffix[/]?").AllowEmpty());

        sfx = (string.IsNullOrWhiteSpace(sfx)) ? "" : "_" + sfx;

        IConfigurationRoot config = GetConfigurationRoot(baseDir, args, ".conf", "walletcli" + sfx + ".conf");

        this.userSettings = config.GetSection("user").Get<UserSettings>();

        var baseUrl = userSettings.GigWalletOpenApi;
        walletClient = new WalletAPIRetryWrapper(baseUrl, new HttpClient(), new DefaultRetryPolicy());

        invoiceStateUpdatesClient = walletClient.CreateInvoiceStateUpdatesClient();
        paymentStatusUpdatesClient = walletClient.CreatePaymentStatusUpdatesClient();
    }

    public enum CommandEnum
    {
        [Display(Name = "Exit App")]
        Exit,
        [Display(Name = "Refresh")]
        Refresh,
        [Display(Name = "Estimate Fee")]
        EstimateFee,
        [Display(Name = "New Address")]
        NewAddress,
        [Display(Name = "New Bitcoin Address")]
        NewBitcoinAddress,
        [Display(Name = "Send To Address")]
        SendToAddress,
        [Display(Name = "Generate Blocks")]
        GenerateBlocks,
        [Display(Name = "Bitcoin Wallet Balance")]
        BitcoinWalletBalance,
        [Display(Name = "Lnd Node Wallet Balance")]
        LndWalletBalance,
        [Display(Name = "Add Invoice")]
        AddInvoice,
        [Display(Name = "Add Hodl Invoice")]
        AddHodlInvoice,
        [Display(Name = "Estimate Route Fee")]
        EstimateRouteFee,
        [Display(Name = "Accept Invoice")]
        AcceptInvoice,
        [Display(Name = "Cancel Invoice")]
        CancelInvoice,
        [Display(Name = "Settle Invoice")]
        SettleInvoice,
        [Display(Name = "Get Invoice State")]
        GetInvoice,
        [Display(Name = "Get Payment Status")]
        GetPayment,
        [Display(Name = "List Invoices")]
        ListInvoices,
        [Display(Name = "List Payments")]
        ListPayments,
        [Display(Name = "Payout")]
        Payout,
        [Display(Name = "Open Reserve")]
        OpenReserve,
        [Display(Name = "Close Reserve")]
        CloseReserve,
    }

    public async Task<string> MakeToken()
    {
        var ecpriv = userSettings.UserPrivateKey.AsECPrivKey();
        string pubkey = ecpriv.CreateXOnlyPubKey().AsHex();
        var guid = WalletAPIResult.Get<Guid>(await walletClient.GetTokenAsync(pubkey, CancellationToken.None));
        return AuthToken.Create(ecpriv, DateTime.UtcNow, guid);
    }

    private async Task WriteBalance()
    {
        var balanceOfCustomer = WalletAPIResult.Get<AccountBalanceDetails>(await walletClient.GetBalanceAsync(await MakeToken(), CancellationToken.None));
        AnsiConsole.WriteLine("Current amout in satoshis:");
        PrintObjectAsTree (balanceOfCustomer);
    }


    enum ClipType
    {
        Invoice = 0,
        PaymentHash = 1,
        Preimage = 2,
        BitcoinAddr = 3,
        ReserveId = 4,
    }

    private void ToClipboard(ClipType clipType, string value)
    {
        var clip = TextCopy.ClipboardService.GetText();
        if (string.IsNullOrWhiteSpace(clip) || !clip.StartsWith("GigGossipClipboard\n"))
        {
            var ini = new List<string>() { "GigGossipClipboard" };
            for (var i = 0; i <= Enum.GetValues(typeof(ClipType)).Cast<int>().Max(); i++)
                ini.Add("");
            clip = string.Join("\n", ini);
        }
        var clarr = clip.Split("\n");
        clarr[((int)clipType) + 1] = value;
        TextCopy.ClipboardService.SetText(string.Join("\n", clarr));
    }

    private string FromClipboard(ClipType clipType)
    {
        var clip = TextCopy.ClipboardService.GetText();
        if (string.IsNullOrWhiteSpace(clip) || !clip.StartsWith("GigGossipClipboard\n"))
            return "";
        var clarr = clip.Split("\n");
        return clarr[((int)clipType) + 1];
    }

    Thread invoiceMonitorThread;
    Thread paymentMonitorThread;
    public async Task RunAsync()
    {
        invoiceMonitorThread = new Thread(async () =>
        {
            await invoiceStateUpdatesClient.ConnectAsync(await MakeToken(), CancellationToken.None);
            try
            {
                await foreach (var invstateupd in invoiceStateUpdatesClient.StreamAsync(await MakeToken(), CancellationTokenSource.Token))
                {
                    AnsiConsole.WriteLine("InvoiceStateUpdate");
                    PrintObjectAsTree(invstateupd);
                    await WriteBalance();
                }
            }
            catch (OperationCanceledException)
            {
                //stream closed
                return;
            };
        });
        invoiceMonitorThread.Start();

        paymentMonitorThread = new Thread(async () =>
        {
            await paymentStatusUpdatesClient.ConnectAsync(await MakeToken(), CancellationToken.None);
            try
            {
                await foreach (var paystateupd in paymentStatusUpdatesClient.StreamAsync(await MakeToken(), CancellationTokenSource.Token))
                {
                    AnsiConsole.WriteLine("PaymentStatusUpdate");
                    PrintObjectAsTree(paystateupd);
                    await WriteBalance();
                }
            }
            catch (OperationCanceledException)
            {
                //stream closed
                return;
            };
        });
        paymentMonitorThread.Start();

        while (true)
        {
            try
            {
                await WriteBalance();
                var cmd = Prompt.Select<CommandEnum>("Select command", pageSize: 6);
                if (cmd == CommandEnum.Exit)
                {
                    if (cmd == CommandEnum.Exit)
                        break;
                }
                else if (cmd == CommandEnum.NewAddress)
                {
                    var newBitcoinAddressOfCustomer = WalletAPIResult.Get<string>(await walletClient.NewAddressAsync(await MakeToken(), CancellationToken.None));
                    ToClipboard(ClipType.BitcoinAddr, newBitcoinAddressOfCustomer);
                    AnsiConsole.WriteLine(newBitcoinAddressOfCustomer);
                }
                else if (cmd == CommandEnum.NewBitcoinAddress)
                {
                    var newBitcoinAddressOfBitcoinWallet = WalletAPIResult.Get<string>(await walletClient.NewBitcoinAddressAsync(await MakeToken(), CancellationToken.None));
                    ToClipboard(ClipType.BitcoinAddr, newBitcoinAddressOfBitcoinWallet);
                    AnsiConsole.WriteLine(newBitcoinAddressOfBitcoinWallet);
                }
                else if (cmd == CommandEnum.SendToAddress)
                {
                    var satoshis = Prompt.Input<int>("How much to send", 100000);
                    if (satoshis > 0)
                    {
                        var bitcoinAddressOfCustomer = Prompt.Input<string>("Bitcoin Address", FromClipboard(ClipType.BitcoinAddr));
                        AnsiConsole.WriteLine(bitcoinAddressOfCustomer);
                        WalletAPIResult.Check(await walletClient.SendToAddressAsync(await MakeToken(), bitcoinAddressOfCustomer, satoshis, "000000", CancellationToken.None));
                    }
                }
                else if (cmd == CommandEnum.GenerateBlocks)
                {
                    var numblocks = Prompt.Input<int>("How many blocks", 6);
                    if (numblocks > 0)
                    {
                        WalletAPIResult.Check(await walletClient.GenerateBlocksAsync(await MakeToken(), numblocks, CancellationToken.None));
                    }
                }
                else if (cmd == CommandEnum.BitcoinWalletBalance)
                {
                    var minconf = Prompt.Input<int>("How many confirtmations", 6);
                    var balance = WalletAPIResult.Get<long>(await walletClient.GetBitcoinWalletBalanceAsync(await MakeToken(), minconf, CancellationToken.None));
                    AnsiConsole.WriteLine($"Balance={balance}");
                }
                else if (cmd == CommandEnum.LndWalletBalance)
                {
                    var balance = WalletAPIResult.Get<LndWalletBalanceRet>(await walletClient.GetLndWalletBalanceAsync(await MakeToken(), CancellationToken.None));
                    PrintObjectAsTree(balance);
                }
                else if (cmd == CommandEnum.EstimateFee)
                {
                    var satoshis = Prompt.Input<int>("How much satoshis to send", 100000);
                    if (satoshis > 0)
                    {
                        var bitcoinAddressOfCustomer = Prompt.Input<string>("Bitcoin Address", FromClipboard(ClipType.BitcoinAddr));
                        var feeEr = WalletAPIResult.Get<FeeEstimateRet>(await walletClient.EstimateFeeAsync(await MakeToken(), bitcoinAddressOfCustomer, satoshis, CancellationToken.None));
                        PrintObjectAsTree(feeEr);
                    }
                }
                else if (cmd == CommandEnum.Payout)
                {
                    var payoutAmount = Prompt.Input<int>("How much to payout");
                    if (payoutAmount > 0)
                    {
                        var payoutId = WalletAPIResult.Get<Guid>(await walletClient.RegisterPayoutAsync(await MakeToken(), payoutAmount, FromClipboard(ClipType.BitcoinAddr), "000000", CancellationToken.None));
                        AnsiConsole.WriteLine(payoutId.ToString());
                    }
                }
                else if (cmd == CommandEnum.OpenReserve)
                {
                    var satoshis = Prompt.Input<int>("How much satoshis to reserve", 100000);
                    if (satoshis > 0)
                    {
                        var reserveId = WalletAPIResult.Get<Guid>(await walletClient.OpenReserveAsync(await MakeToken(), satoshis, "000000", CancellationToken.None));
                        ToClipboard(ClipType.ReserveId, reserveId.ToString());
                        AnsiConsole.WriteLine(reserveId.ToString());
                    }
                }
                else if (cmd == CommandEnum.CloseReserve)
                {
                    var reserveId = Prompt.Input<Guid>("ReserveId", FromClipboard(ClipType.ReserveId));
                    WalletAPIResult.Check(await walletClient.CloseReserveAsync(await MakeToken(), reserveId,"000000", CancellationToken.None));
                }
                else if (cmd == CommandEnum.AddInvoice)
                {
                    var satoshis = Prompt.Input<long>("Satoshis", 1000L);
                    var memo = Prompt.Input<string>("Memo", "test");
                    var expiry = Prompt.Input<long>("Expiry", 1000L);
                    var inv = WalletAPIResult.Get<InvoiceRecord>(await walletClient.AddInvoiceAsync(await MakeToken(), satoshis, memo, expiry, CancellationToken.None));
                    ToClipboard(ClipType.Invoice, inv.PaymentRequest);
                    ToClipboard(ClipType.PaymentHash, inv.PaymentHash);
                    PrintObjectAsTree(inv);
                }
                else if (cmd == CommandEnum.AddHodlInvoice)
                {
                    var satoshis = Prompt.Input<long>("Satoshis", 1000L);
                    var memo = Prompt.Input<string>("Memo", "hodl");
                    var expiry = Prompt.Input<long>("Expiry", 1000L);
                    var preimage = Crypto.GenerateRandomPreimage();
                    AnsiConsole.WriteLine(preimage.AsHex());
                    var hash = Crypto.ComputePaymentHash(preimage).AsHex();
                    var inv = WalletAPIResult.Get<InvoiceRecord>(await walletClient.AddHodlInvoiceAsync(await MakeToken(), satoshis, hash, memo, expiry, CancellationToken.None));
                    ToClipboard(ClipType.Preimage, preimage.AsHex());
                    ToClipboard(ClipType.Invoice, inv.PaymentRequest);
                    ToClipboard(ClipType.PaymentHash, inv.PaymentHash);
                    PrintObjectAsTree(inv);
                }
                else if (cmd == CommandEnum.AcceptInvoice)
                {
                    var paymentreq = Prompt.Input<string>("Payment Request", FromClipboard(ClipType.Invoice));
                    AnsiConsole.WriteLine(paymentreq);
                    var pay = WalletAPIResult.Get<PaymentRequestRecord>(await walletClient.DecodeInvoiceAsync(await MakeToken(), paymentreq, CancellationToken.None));
                    AnsiConsole.WriteLine($"Amount:{pay.Satoshis}");
                    AnsiConsole.WriteLine($"Memo:{pay.Memo}");
                    AnsiConsole.WriteLine($"Creation:{pay.CreationTime}");
                    AnsiConsole.WriteLine($"Expiry:{pay.ExpiryTime}");
                    AnsiConsole.WriteLine($"Payment Hash:{pay.PaymentHash}");
                    ToClipboard(ClipType.PaymentHash, pay.PaymentHash);
                    var timeout = Prompt.Input<int>("Timeout", 1000);
                    var feeLimit = Prompt.Input<int>("FeeLimit", 10000);
                    var payment = WalletAPIResult.Get<PaymentRecord>(await walletClient.SendPaymentAsync(await MakeToken(), paymentreq, timeout, feeLimit, "000000", CancellationToken.None));
                    PrintObjectAsTree(payment);
                }
                else if (cmd == CommandEnum.CancelInvoice)
                {
                    var paymenthash = Prompt.Input<string>("Payment Hash", FromClipboard(ClipType.PaymentHash));
                    AnsiConsole.WriteLine(paymenthash);
                    WalletAPIResult.Check(await walletClient.CancelInvoiceAsync(await MakeToken(), paymenthash, CancellationToken.None));
                }
                else if (cmd == CommandEnum.SettleInvoice)
                {
                    var preimage = Prompt.Input<string>("Preimage", FromClipboard(ClipType.Preimage));
                    AnsiConsole.WriteLine(preimage);
                    var paymenthash = Crypto.ComputePaymentHash(preimage.AsBytes()).AsHex();
                    AnsiConsole.WriteLine(paymenthash);
                    WalletAPIResult.Check(await walletClient.SettleInvoiceAsync(await MakeToken(), preimage, CancellationToken.None));
                    ToClipboard(ClipType.PaymentHash, paymenthash);
                }
                else if (cmd == CommandEnum.GetPayment)
                {
                    var paymenthash = Prompt.Input<string>("Payment Hash", FromClipboard(ClipType.PaymentHash));
                    AnsiConsole.WriteLine(paymenthash);
                    var payment = WalletAPIResult.Get<PaymentRecord>(await walletClient.GetPaymentAsync(await MakeToken(), paymenthash, CancellationToken.None));
                    PrintObjectAsTree(payment);
                }
                else if (cmd == CommandEnum.GetInvoice)
                {
                    var paymenthash = Prompt.Input<string>("Payment Hash");
                    if (string.IsNullOrWhiteSpace(paymenthash))
                    {
                        paymenthash = FromClipboard(ClipType.PaymentHash);
                        AnsiConsole.WriteLine(paymenthash);
                    }
                    var invState = WalletAPIResult.Get<InvoiceRecord>(await walletClient.GetInvoiceAsync(await MakeToken(), paymenthash, CancellationToken.None));
                    PrintObjectAsTree(invState);
                }
                else if (cmd == CommandEnum.EstimateRouteFee)
                {
                    var paymentreq = Prompt.Input<string>("Payment Request", FromClipboard(ClipType.Invoice));
                    var timeout = Prompt.Input<int>("Timeout", 1000);
                    AnsiConsole.WriteLine(paymentreq);
                    var fee = WalletAPIResult.Get<RouteFeeRecord>(await walletClient.EstimateRouteFeeAsync(await MakeToken(), paymentreq, timeout, CancellationToken.None));
                    PrintObjectAsTree(fee);
                }
                else if (cmd == CommandEnum.ListInvoices)
                {
                    string[] columns = {
                        "Payment Hash",
                        "State",
                        "Amount",
                        "Currency",
                        "Memo",
                        "Expiry"
                    };
                    var invoices = WalletAPIResult.Get<List<InvoiceRecord>>(await walletClient.ListInvoicesAsync(await MakeToken(), CancellationToken.None));
                    var rows = (from inv in invoices
                                select new string[] {
                        inv.PaymentHash,
                        inv.State.ToString(),
                        inv.Satoshis.ToString(),
                        inv.Memo,
                        inv.ExpiryTime.ToString(),
                    }).ToArray();
                    DrawTable(columns, rows);
                }
                else if (cmd == CommandEnum.ListPayments)
                {
                    string[] columns = {
                        "Payment Hash",
                        "Status",
                        "Amount",
                        "Currency",
                        "Fee"
                    };
                    var payments = WalletAPIResult.Get< List<PaymentRecord>>(await walletClient.ListPaymentsAsync(await MakeToken(), CancellationToken.None));
                    var rows = (from pay in payments
                                select new string[] {
                        pay.PaymentHash,
                        pay.Status.ToString(),
                        pay.Satoshis.ToString(),
                        (pay.FeeMsat/1000).ToString(),
                    }).ToArray();
                    DrawTable(columns, rows);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex,
                    ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes |
                    ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks);
            }
        }
        CancellationTokenSource.Cancel();
        invoiceMonitorThread.Join();
        paymentMonitorThread.Join();
    }

    private void DrawTable(string[] columnNames, string[][] rows)
    {
        var table = new Table()
            .Border(TableBorder.Rounded);
        foreach (var c in columnNames)
            table = table.AddColumn(c);
        foreach (var row in rows)
            table = table.AddRow(row);
        AnsiConsole.Write(table);
    }

    public static void PrintObjectAsTree(object obj, string intend="  - ")
    {
        AnsiConsole.MarkupLine($"[lime]--- [[{obj.GetType().Name}]][/]");

        // Serialize the object to JSON
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new StringEnumConverter());
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);

        // Parse the JSON into a JObject
        JObject jsonObject = JObject.Parse(json);

        // Recursively print the JSON object as a tree
        PrintJTokenAsTree(jsonObject, "  - ", CalculateMaxKeyLength(jsonObject));
        AnsiConsole.WriteLine();
    }


    private static int CalculateMaxKeyLength(JToken token)
    {
        int maxLength = 0;
        if (token.Type == JTokenType.Object)
        {
            foreach (var property in token.Children<JProperty>())
            {
                int keyLength = property.Name.Length;
                if (keyLength > maxLength)
                {
                    maxLength = keyLength;
                }

                // Recursively check nested objects
                int childMax = CalculateMaxKeyLength(property.Value);
                if (childMax > maxLength)
                {
                    maxLength = childMax;
                }
            }
        }
        return maxLength;
    }

    private static void PrintJTokenAsTree(JToken token, string indent, int keyWidth)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (var property in token.Children<JProperty>())
                {
                    // Format the key and apply colors
                    string formattedKey = $"{indent}{property.Name}:".PadRight(keyWidth + indent.Length + 2);
                    AnsiConsole.Markup($"[cyan]{formattedKey}[/]");

                    PrintJTokenAsTree(property.Value, indent + "  ", keyWidth);
                }
                break;

            case JTokenType.Array:
                var array = token as JArray;
                for (int i = 0; i < array.Count; i++)
                {
                    // Format arrays separately
                    string formattedKey = $"{indent}[{i}]:".PadRight(keyWidth + indent.Length + 2);
                    AnsiConsole.Markup($"[green]{formattedKey}[/]");
                    PrintJTokenAsTree(array[i], indent + "  ", keyWidth);
                }
                break;

            default:
                // For primitive types (leaf nodes), print the value on the same line
                AnsiConsole.MarkupLine($"[yellow]\t{token.ToString()}[/]");
                break;
        }
    }
}

public class UserSettings
{
    public required string GigWalletOpenApi { get; set; }
    public required string UserPrivateKey { get; set; }
    public required long FeeLimitSat { get; set; }
}