﻿using ApiBtc;
using Grpc.Core;
using LNDClient;
using Lnrpc;
using Microsoft.EntityFrameworkCore;
using NBitcoin;
using NBitcoin.Secp256k1;
using OtpNet;
using Spectre.Console;
using System.Collections.Concurrent;
using System.Data;
using System.Formats.Tar;
using TraceExColor;
using Walletrpc;

namespace LNDWallet;

[Serializable]
public enum InvoiceState
{
    Open = 0,
    Settled = 1,
    Cancelled = 2,
    Accepted = 3,
}

[Serializable]
public enum PaymentStatus
{
    InFlight = 1,
    Succeeded = 2,
    Failed = 3,
    Initiated = 4,
}

[Serializable]
public enum PayoutState
{
    Open = 0,
    Sending = 1,
    Sent = 2,
    Failure = 3,
}

[Serializable]
public enum PaymentFailureReason
{
    None = 0,
    Timeout = 1,
    NoRoute = 2,
    Error = 3,
    IncorrectPaymentDetails = 4,
    InsufficientBalance = 5,
    Canceled = 6,
    EmptyReturnStream = 101,
    InvoiceAlreadySettled = 102,
    InvoiceAlreadyCancelled = 103,
    InvoiceAlreadyAccepted = 104,
    FeeLimitTooSmall = 105,
}

[Serializable]
public class RouteFeeRecord
{
    public required long RoutingFeeMsat { get; set; }
    public required long TimeLockDelay { get; set; }
    public required PaymentFailureReason FailureReason { get; set; }
}

[Serializable]
public class PaymentRequestRecord
{
    public required string PaymentHash { get; set; }
    public required long Satoshis { get; set; }

    public required string PaymentAddr { get; set; }
    public required string Memo { get; set; }
    public required DateTime CreationTime { get; set; }
    public required DateTime ExpiryTime { get; set; }
}


[Serializable]
public class InvoiceRecord : PaymentRequestRecord
{
    public required string PaymentRequest { get; set; }
    public required InvoiceState State { get; set; }
    public required bool IsHodl { get; set; }

    public DateTime? SettleTime { get; set; }

    public static InvoiceRecord FromPaymentRequestRecord(PaymentRequestRecord payreq, string paymentRequest, InvoiceState state, bool isHodl, DateTime? settleTime = null)
    {
        return new InvoiceRecord
        {
            PaymentHash = payreq.PaymentHash,
            Satoshis = payreq.Satoshis,
            PaymentAddr = payreq.PaymentAddr,
            Memo = payreq.Memo,
            CreationTime = payreq.CreationTime,
            ExpiryTime = payreq.ExpiryTime,
            IsHodl = isHodl,
            PaymentRequest = paymentRequest,
            State = state,
            SettleTime = settleTime,
        };
    }
}

[Serializable]
public class PaymentRecord
{
    public required string PaymentHash { get; set; }
    public required PaymentStatus Status { get; set; }
    public required PaymentFailureReason FailureReason { get; set; }
    public required long Satoshis { get; set; }
    public required long FeeMsat { get; set; }
	public required DateTime CreationTime { get; set; }
    public static PaymentRecord FromPaymentRequestRecord(PaymentRequestRecord payreq, PaymentStatus status, PaymentFailureReason failureReason, long feeMsat)
    {
        return new PaymentRecord
        {
            PaymentHash = payreq.PaymentHash,
            Satoshis = payreq.Satoshis,
            CreationTime = payreq.CreationTime,
            Status = status,
            FailureReason = failureReason,
            FeeMsat = feeMsat,
        };
    }
}

[Serializable]
public class PayoutRecord
{
    public required Guid PayoutId { get; set; }
    public required string BitcoinAddress { get; set; }
    public required PayoutState State { get; set; }
    public required long Satoshis { get; set; }
    public required long PayoutFee { get; set; }
    public required string Tx { get; set; }
    public required int NumConfirmations { get; set; }
    public required DateTime CreationTime { get; set; }
}

[Serializable]
public class TransactionRecord
{
    public required string BitcoinAddress { get; set; }
    public required long Satoshis { get; set; }
    public required string Tx { get; set; }
    public required int NumConfirmations { get; set; }
    public required DateTime CreationTime { get; set; }
}

[Serializable]
public class TwoFactorAuthSetup
{
    public required string SecretKey { get; set; }
    public required string TwoFactorAuthUri { get; set; }
    public string[] SingleUseCodes { get; set; }
}

[Serializable]
public class AccountBalanceDetails
{
    /// <summary>
    /// Amount that is available for the user at the time
    /// </summary>
    public required long AvailableAmount { get; set; }

    /// <summary>
    /// Amount that might include inprogress and inflight payments
    /// </summary>
    public required long TotalAmount { get; set; }

    /// <summary>
    /// Total topup amount including not confirmed
    /// </summary>
    public required long TotalTopups { get; set; }

    /// <summary>
    /// Amount of topped up by sending to the NewAddress but not yet confirmed (less than 6 confirmations)
    /// </summary>
    public required long NotConfirmedTopups { get; set; }

    /// <summary>
    /// Amount of earning from settled invoices
    /// </summary>
    public required long SettledEarnings { get; set; }

    /// <summary>
    /// Amount on accepted invoices including these that are still not Settled (they can be Cancelled or Settled in the future)
    /// </summary>
    public required long TotalEarnings { get; set; }


    /// <summary>
    /// Amount that of payments that are executed including these that are still inflight
    /// </summary>
    public required long TotalPayments { get; set; }

    /// <summary>
    /// Amount that of payments that are not yet Successful (still can Fail and be given back)
    /// </summary>
    public required long InFlightPayments { get; set; }

    /// <summary>
    /// Amount of locked payouts that including these that are in progress
    /// </summary>
    public required long TotalPayouts { get; set; }

    /// <summary>
    /// Amount that is locked in the system for the payouts that are still in progress 
    /// </summary>
    public required long InProgressPayouts { get; set; }

    /// <summary>
    /// Total amount on payment fees (incuding those in InFlight state)
    /// </summary>
    public required long TotalPaymentFees { get; set; }

    /// <summary>
    /// Amount of inflight payment fees
    /// </summary>
    public required long InFlightPaymentFees { get; set; }

    /// <summary>
    /// Total amount on onchain payment fees (incuding those in in progress state)
    /// </summary>
    public required long TotalPayoutOnChainFees { get; set; }

    /// <summary>
    /// Amount of inprogress onchain fees)
    /// </summary>
    public required long InProgressPayoutOnChainFees { get; set; }

    /// <summary>
    /// Total amount on payout fees (including these that are in progress)
    /// </summary>
    public required long TotalPayoutFees { get; set; }

    /// <summary>
    /// Total amount of inprogress payout fees
    /// </summary>
    public required long InProgressPayoutFees { get; set; }
}

[Serializable]
public class InvoiceStateChange
{
    public required string PaymentHash { get; set; }
    public required InvoiceState NewState { get; set; }
}

[Serializable]
public class PaymentStatusChanged
{
    public required string PaymentHash { get; set; }
    public required PaymentStatus NewStatus { get; set; }
    public required PaymentFailureReason FailureReason { get; set; }
}

[Serializable]
public class NewTransactionFound
{
    public required string TxHash { get; set; }
    public required int NumConfirmations { get; set; }
    public required string Address { get; set; }
    public required long AmountSat { get; set; }
}

[Serializable]
public class PayoutStateChanged
{
    public required Guid PayoutId { get; set; }
    public required PayoutState NewState { get; set; }
    public required long PayoutFee { get; set; }
    public string? Tx { get; set; }
}

public class InvoiceStateChangedEventArgs : EventArgs
{
    public required string PublicKey { get; set; }
    public required InvoiceStateChange InvoiceStateChange { get; set; }
}

public delegate void InvoiceStateChangedEventHandler(object sender, InvoiceStateChangedEventArgs e);

public class PaymentStatusChangedEventArgs : EventArgs
{
    public required string PublicKey { get; set; }
    public required PaymentStatusChanged PaymentStatusChanged { get; set; }
}

public delegate void PaymentStatusChangedEventHandler(object sender, PaymentStatusChangedEventArgs e);

public class NewTransactionFoundEventArgs : EventArgs
{
    public required string PublicKey { get; set; }
    public required NewTransactionFound NewTransactionFound { get; set; }
}

public delegate void NewTransactionFoundEventHandler(object sender, NewTransactionFoundEventArgs e);

public class PayoutStateChangedEventArgs : EventArgs
{
    public required string PublicKey { get; set; }
    public required PayoutStateChanged PayoutStateChanged { get; set; }
}

public delegate void PayoutStatusChangedEventHandler(object sender, PayoutStateChangedEventArgs e);

public abstract class LNDEventSource
{
    public event InvoiceStateChangedEventHandler OnInvoiceStateChanged;
    public event PaymentStatusChangedEventHandler OnPaymentStatusChanged;
    public event NewTransactionFoundEventHandler OnNewTransactionFound;
    public event PayoutStatusChangedEventHandler OnPayoutStateChanged;

    public void FireOnInvoiceStateChanged(string pubkey, string paymentHash, InvoiceState invstate)
    {
        OnInvoiceStateChanged?.Invoke(this, new InvoiceStateChangedEventArgs()
        {
            PublicKey = pubkey,
             InvoiceStateChange = new InvoiceStateChange
             {
                 PaymentHash = paymentHash,
                 NewState = invstate
             }
        });
    }

    public void FireOnPaymentStatusChanged(string pubkey, string paymentHash, PaymentStatus paystatus, PaymentFailureReason failureReason)
    {
        OnPaymentStatusChanged?.Invoke(this, new PaymentStatusChangedEventArgs()
        {
                PublicKey = pubkey,
             PaymentStatusChanged= new PaymentStatusChanged
             {
                 PaymentHash = paymentHash,
                 NewStatus = paystatus,
                 FailureReason = failureReason,
             }
        });
    }

    public void FireOnNewTransactionFound(string pubkey, string txHash, int numConfirmations, string address, long amountSat)
    {
        OnNewTransactionFound?.Invoke(this, new NewTransactionFoundEventArgs()
        {
            PublicKey = pubkey,
            NewTransactionFound = new NewTransactionFound
            {
                TxHash = txHash,
                NumConfirmations = numConfirmations,
                Address = address,
                AmountSat = amountSat
            }
        });

    }

    public void FireOnPayoutStateChanged(string pubkey, Guid payoutId, PayoutState paystatus, long payoutFee, string tx)
    {
        OnPayoutStateChanged?.Invoke(this, new PayoutStateChangedEventArgs()
        {
            PublicKey = pubkey,
            PayoutStateChanged = new PayoutStateChanged
            {
                PayoutId = payoutId,
                NewState = paystatus,
                PayoutFee = payoutFee,
                Tx = tx,
            }
        });
    }

    public virtual void CancelSingleInvoiceTracking(string paymentHash) { }
}

public enum TokenType
{
    Normal,
    LongTerm,
    Admin,
}

public class LNDAccountManager
{
    public GigDebugLoggerAPIClient.LogWrapper<LNDAccountManager> TRACE = GigDebugLoggerAPIClient.ConsoleLoggerFactory.Trace<LNDAccountManager>();

    private LND.NodeSettings lndConf;
    private WaletContextFactory walletContextFactory;
    public string PublicKey;
    private LNDEventSource eventSource;
    private CancellationTokenSource trackPaymentsCancallationTokenSource = new();
    private bool enforceTwoFactorAuthentication;


    internal LNDAccountManager(LND.NodeSettings conf, DBProvider provider, string connectionString, ECXOnlyPubKey pubKey, LNDEventSource eventSource, bool enforceTwoFactorAuthentication)
    {
        this.lndConf = conf;
        this.PublicKey = pubKey.AsHex();
        this.walletContextFactory = new WaletContextFactory(provider, connectionString);
        this.eventSource = eventSource;
        this.enforceTwoFactorAuthentication = enforceTwoFactorAuthentication;
    }

    static ConcurrentDictionary<string, TwoFactorAuthSetup> tempCodes = new();

    public TwoFactorAuthSetup PrepareTwoFactor(string issuer)
    {
        using var TL = TRACE.Log();
        try
        {
            using var walletContext = walletContextFactory.Create();

            if ((from tfa in walletContext.TwoFactorAuths where tfa.PublicKey == PublicKey select tfa).FirstOrDefault() != null)
                throw new LNDWalletException(LNDWalletErrorCode.TwoFactorAlreadyEnabled);

            var base32String = Base32Encoding.ToString(RandomUtils.GetBytes(20));

            var singleUseCodes = new HashSet<string>();
            while (singleUseCodes.Count < 16)
            {
                var code = RandomUtils.GetBytes(5).AsHex();
                if (!singleUseCodes.Contains(code))
                    singleUseCodes.Add(code);
            }
            var codes = new TwoFactorAuthSetup()
            {
                TwoFactorAuthUri = new OtpUri(OtpType.Totp, base32String, PublicKey, issuer).ToString(),
                SecretKey = base32String,
                SingleUseCodes = singleUseCodes.ToArray()
            };
            tempCodes.AddOrReplace(PublicKey, codes);
            return TL.Ret(codes);
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public void EnableTwoFactor(string code)
    {
        using var TL = TRACE.Log();
        try
        {
            using var walletContext = walletContextFactory.Create();

            if ((from tfa in walletContext.TwoFactorAuths where tfa.PublicKey == PublicKey select tfa).FirstOrDefault() != null)
                throw new LNDWalletException(LNDWalletErrorCode.TwoFactorAlreadyEnabled);

            if (!tempCodes.TryGetValue(PublicKey, out var preparedCodes))
                throw new LNDWalletException(LNDWalletErrorCode.TwoFactorNotPrepared);

            // Verify the TOTP code

            var base32Bytes = Base32Encoding.ToBytes(preparedCodes.SecretKey);
            var totp = new Totp(base32Bytes);
            if (!totp.VerifyTotp(code, out _))
                throw new LNDWalletException(LNDWalletErrorCode.InvalidTwoFactorCode);

            walletContext.INSERT(new TwoFactorAuth()
            {
                PublicKey = PublicKey,
                SecretKey = preparedCodes.SecretKey,
            });

            foreach (var scode in preparedCodes.SingleUseCodes)
            {
                walletContext.INSERT(new SingleUseCode()
                {
                    PublicKey = PublicKey,
                    Code = Crypto.ComputeSha256(scode.AsBytes()).AsHex(),
                });
            }
            walletContext.SAVE();
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public bool IsTwoFactorEnabled()
    {
        using var TL = TRACE.Log();
        try
        {
            using var walletContext = walletContextFactory.Create();
            return (from tfa in walletContext.TwoFactorAuths where tfa.PublicKey == PublicKey select tfa).FirstOrDefault() != null;
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public LNDAccountManager VerifyTotp(string code)
    {         
        using var TL = TRACE.Log().Args(code);
        try
        {
            using var walletContext = walletContextFactory.Create();
            // Find the TFA entry for this user
            var tfaEntry = (from tfa in walletContext.TwoFactorAuths
                            where tfa.PublicKey == PublicKey
                            select tfa).FirstOrDefault();
            if (tfaEntry == null)
                if (enforceTwoFactorAuthentication)
                    throw new LNDWalletException(LNDWalletErrorCode.TwoFactorNotEnabled);
                else
                    return this; // If not enforced, we can skip verification
            // Verify the TOTP code

            var base32Bytes = Base32Encoding.ToBytes(tfaEntry.SecretKey);
            var totp = new Totp(base32Bytes);
            if(!totp.VerifyTotp(code, out _))
                throw new LNDWalletException(LNDWalletErrorCode.InvalidTwoFactorCode);
            return this;
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public string[] RegenerateSingleUseCodes(string code)
    {
        using var TL = TRACE.Log().Args(code);
        try
        {
            using var walletContext = walletContextFactory.Create();

            // Find the TFA entry for this user
            var tfaEntry = (from tfa in walletContext.TwoFactorAuths
                            where tfa.PublicKey == PublicKey
                            select tfa).FirstOrDefault();
            if (tfaEntry == null)
                    throw new LNDWalletException(LNDWalletErrorCode.TwoFactorNotEnabled);

            // Verify the TOTP code

            var base32Bytes = Base32Encoding.ToBytes(tfaEntry.SecretKey);
            var totp = new Totp(base32Bytes);
            if (!totp.VerifyTotp(code, out _))
                throw new LNDWalletException(LNDWalletErrorCode.InvalidTwoFactorCode);

            var oldCodes = from c in walletContext.SingleUseCodes
                           where c.PublicKey == PublicKey
                           select c;
            foreach (var c in oldCodes)
                walletContext.DELETE(c);

            var singleUseCodes = new HashSet<string>();
            while (singleUseCodes.Count < 16)
            {
                var scode = RandomUtils.GetBytes(5).AsHex();
                if (!singleUseCodes.Contains(scode))
                {
                    singleUseCodes.Add(scode);
                    walletContext.INSERT(new SingleUseCode()
                    {
                        PublicKey = PublicKey,
                        Code = Crypto.ComputeSha256(scode.AsBytes()).AsHex(),
                    });
                }
            }
            walletContext.SAVE();
            return singleUseCodes.ToArray();
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public TwoFactorAuthSetup ResetTwoFactor(string providedCode, string issuer)
    {
        using var TL = TRACE.Log().Args(providedCode);
        try
        {
            using var walletContext = walletContextFactory.Create();

            var shaCode = Crypto.ComputeSha256(providedCode.AsBytes()).AsHex();

            // Find the code and ensure it belongs to this user and is not used
            var codeEntry = (from c in walletContext.SingleUseCodes
                             where c.PublicKey == PublicKey && c.Code == shaCode
                             select c).FirstOrDefault();

            if (codeEntry == null)
                throw new LNDWalletException(LNDWalletErrorCode.InvalidSingleUseToken);

            walletContext.DELETE(codeEntry);

            // Update the secret to a new one
            var tfaEntry = (from tfa in walletContext.TwoFactorAuths
                            where tfa.PublicKey == PublicKey
                            select tfa).FirstOrDefault();

            if (tfaEntry == null)
                throw new LNDWalletException(LNDWalletErrorCode.TwoFactorNotEnabled);

            var base32String = Base32Encoding.ToString(RandomUtils.GetBytes(20));
            tfaEntry.SecretKey = base32String;
            walletContext.UPDATE(tfaEntry);

            walletContext.SAVE();
            return TL.Ret(new TwoFactorAuthSetup()
            {
                TwoFactorAuthUri = new OtpUri(OtpType.Totp, base32String, PublicKey, issuer).ToString(),
                SecretKey = base32String,
            });
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public void DisableTwoFactor(string providedCode)
    {
        using var TL = TRACE.Log().Args(providedCode);
        try
        {

            using var walletContext = walletContextFactory.Create();

            // Find the TFA entry for this user
            var tfaEntry = (from tfa in walletContext.TwoFactorAuths
                            where tfa.PublicKey == PublicKey
                            select tfa).FirstOrDefault();

            if (tfaEntry == null)
                return; // 2FA already disabled

            if (enforceTwoFactorAuthentication)
                throw new LNDWalletException(LNDWalletErrorCode.AccessDenied);

            var shaCode = Crypto.ComputeSha256(providedCode.AsBytes()).AsHex();
            // Find the code and ensure it belongs to this user and is not used
            var codeEntry = (from c in walletContext.SingleUseCodes
                             where c.PublicKey == PublicKey && c.Code == shaCode
                             select c).FirstOrDefault();

            if (codeEntry == null)
                throw new LNDWalletException(LNDWalletErrorCode.InvalidSingleUseToken);

            // Remove the TFA secret
            walletContext.DELETE(from tfa in walletContext.TwoFactorAuths
                                           where tfa.PublicKey == PublicKey
                                           select tfa);

            // Invalidate all old single use codes
            var codes = from c in walletContext.SingleUseCodes
                        where c.PublicKey == PublicKey
                        select c;

            foreach (var c in codes)
                walletContext.DELETE(c);

            walletContext.SAVE();
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public string CreateNewTopupAddress()
    {
        using var TL = TRACE.Log();
        try
        {
            var newaddress = LND.NewAddress(lndConf);

            using var walletContext = walletContextFactory.Create();
            walletContext
                .INSERT(new TopupAddress() { BitcoinAddress = newaddress, PublicKey = PublicKey })
                .SAVE();
            return newaddress;
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public Guid RegisterNewPayoutForExecution(long satoshis, string btcAddress)
    {
        using var TL = TRACE.Log().Args(satoshis, btcAddress);
        try
        {
            var myid = Guid.NewGuid();
            using var walletContext = walletContextFactory.Create();

            using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

            var availableAmount = GetAccountBalance().AvailableAmount;
            if (availableAmount < satoshis)
                throw new LNDWalletException(LNDWalletErrorCode.NotEnoughFunds); ;



            walletContext
                .INSERT(new Payout()
                {
                    PayoutId = myid,
                    BitcoinAddress = btcAddress,
                    PublicKey = PublicKey,
                    State = PayoutState.Open,
                    PayoutFee = 0,
                    Satoshis = satoshis,
                    CreationTime = DateTime.UtcNow,
                })
                .INSERT(new Reserve()
                {
                    ReserveId = myid,
                    Satoshis = satoshis
                })
                .SAVE();

            TX.Commit();
            return myid;
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    private long GetExecutedTopupTotalAmount(int minConf)
    {
        using var TL = TRACE.Log().Args(minConf);
        try
        {
            using var walletContext = walletContextFactory.Create();

            var myaddrs = new HashSet<string>(
            from a in walletContext.TopupAddresses
            where a.PublicKey == PublicKey
            select a.BitcoinAddress);

            var transactuinsResp = LND.GetTransactions(lndConf);
            long balance = 0;
            foreach (var transation in transactuinsResp.Transactions)
                if (transation.NumConfirmations >= minConf)
                    foreach (var outp in transation.OutputDetails)
                        if (outp.IsOurAddress)
                            if (myaddrs.Contains(outp.Address))
                                balance += outp.Amount;

            return balance;
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public PayoutRecord GetPayout(Guid payoutId)
    {
        using var TL = TRACE.Log().Args(payoutId);
        try
        {
            using var walletContext = walletContextFactory.Create();

            var payout = (
                from a in walletContext.Payouts
                where a.PublicKey == PublicKey
                && a.PayoutId == payoutId
                select a).FirstOrDefault();
            if (payout == null)
                throw new LNDWalletException(LNDWalletErrorCode.PayoutNotOpened);

            return new PayoutRecord
            {
                BitcoinAddress = payout.BitcoinAddress,
                PayoutFee = payout.PayoutFee,
                PayoutId = payout.PayoutId,
                Satoshis = payout.Satoshis,
                State = payout.State,
                Tx = payout.Tx == null ? "" : payout.Tx,
                NumConfirmations = payout.Tx == null ? 0 : LND.GetTransaction(lndConf, payout.Tx).NumConfirmations,
                CreationTime = payout.CreationTime,
            };

        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public List<PayoutRecord> ListPayouts()
    {
        using var TL = TRACE.Log();
        try
        {
            var ret = new List<PayoutRecord>();

            using var walletContext = walletContextFactory.Create();

            var mypayouts = (
                from a in walletContext.Payouts
                where a.PublicKey == PublicKey
                select a).ToList();

            var runningtransactions = new Dictionary<string, Lnrpc.Transaction>(
                from a in LND.GetTransactions(lndConf).Transactions
                select new KeyValuePair<string, Lnrpc.Transaction>(a.TxHash, a));

            foreach (var payout in mypayouts)
            {
                if (payout.Tx != null && runningtransactions.ContainsKey(payout.Tx))
                {
                    ret.Add(new PayoutRecord
                    {
                        BitcoinAddress = payout.BitcoinAddress,
                        PayoutFee = payout.PayoutFee,
                        PayoutId = payout.PayoutId,
                        Satoshis = payout.Satoshis,
                        State = payout.State,
                        Tx = payout.Tx,
                        NumConfirmations = runningtransactions[payout.Tx].NumConfirmations,
                        CreationTime = payout.CreationTime,
                    });
                }
                else
                {
                    ret.Add(new PayoutRecord
                    {
                        BitcoinAddress = payout.BitcoinAddress,
                        PayoutFee = payout.PayoutFee,
                        PayoutId = payout.PayoutId,
                        Satoshis = payout.Satoshis,
                        State = payout.State,
                        Tx = payout.Tx == null ? "" : payout.Tx,
                        NumConfirmations = 0,
                        CreationTime = payout.CreationTime,
                    });
                }
            }
            return ret;

        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public List<TransactionRecord> ListTransactions()
    {
        using var TL = TRACE.Log();
        try
        {
            var ret = new List<TransactionRecord>();

            using var walletContext = walletContextFactory.Create();

            var myaddrs = new HashSet<string>(
                from a in walletContext.TopupAddresses
                where a.PublicKey == PublicKey
                select a.BitcoinAddress);

            var transactuinsResp = LND.GetTransactions(lndConf);
            foreach (var transation in transactuinsResp.Transactions)
                foreach (var outp in transation.OutputDetails)
                    if (outp.IsOurAddress)
                        if (myaddrs.Contains(outp.Address))
                        {
                            ret.Add(new TransactionRecord
                            {
                                BitcoinAddress = outp.Address,
                                Satoshis = outp.Amount,
                                Tx = transation.TxHash,
                                NumConfirmations = transation.NumConfirmations,
                                CreationTime = DateTimeOffset.FromUnixTimeSeconds(transation.TimeStamp).UtcDateTime,
                            });
                        }
            return ret;
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    /*
    public (long all, long allTxFee, long confirmed, long confirmedTxFee) GetExecutedPayoutTotalAmount(int minConf)
    {
        using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

        Dictionary<string, LNDWallet.Payout> mypayouts;
        mypayouts = new Dictionary<string, LNDWallet.Payout>(
            from a in walletContext.Payouts
            where a.PublicKey == PublicKey && a.Tx != null
            select new KeyValuePair<string, LNDWallet.Payout>(a.Tx, a));

        var transactuinsResp = LND.GetTransactions(lndConf);
        long confirmed = 0;
        long confirmedTxFee = 0;
        long all = 0;
        long allTxFee = 0;
        foreach (var transation in transactuinsResp.Transactions)
        {
            if (mypayouts.ContainsKey(transation.TxHash))
            {
                foreach (var outp in transation.OutputDetails)
                {
                    if (!outp.IsOurAddress)
                    {
                        if (transation.NumConfirmations >= minConf)
                        {
                            confirmed += outp.Amount;
                            confirmedTxFee += (long)mypayouts[outp.Address].PayoutFee;
                        }
                        all += outp.Amount;
                        allTxFee += (long)mypayouts[outp.Address].PayoutFee;
                    }
                }
            }
        }

        TX.Commit();

        return (all, allTxFee, confirmed, confirmedTxFee);
    }
    */

    public InvoiceRecord CreateNewHodlInvoice(long satoshis, string memo, byte[] hash, long expiry)
    {
        using var TL = TRACE.Log().Args(satoshis, memo, hash, expiry);
        try
        {
            var inv = LND.AddHodlInvoice(lndConf, satoshis, memo, hash, expiry);

            using var walletContext = walletContextFactory.Create();

            walletContext
                .INSERT(new HodlInvoice()
                {
                    PaymentHash = hash.AsHex(),
                    PublicKey = PublicKey,
                })
                .SAVE();
            var payreq = DecodeInvoice(inv.PaymentRequest);
            return TL.Ret(InvoiceRecord.FromPaymentRequestRecord(payreq,
                paymentRequest: inv.PaymentRequest,
                state: InvoiceState.Open,
                isHodl: true));
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public InvoiceRecord CreateNewClassicInvoice(long satoshis, string memo, long expiry)
    {
        using var TL = TRACE.Log().Args(satoshis, memo, expiry);
        try
        {
            var inv = LND.AddInvoice(lndConf, satoshis, memo, expiry);
            using var walletContext = walletContextFactory.Create();

            walletContext
                .INSERT(new ClassicInvoice()
                {
                    PaymentHash = inv.RHash.ToArray().AsHex(),
                    PublicKey = PublicKey,
                })
                .SAVE();
            var payreq = DecodeInvoice(inv.PaymentRequest);
            return InvoiceRecord.FromPaymentRequestRecord(payreq,
                paymentRequest: inv.PaymentRequest,
                state: InvoiceState.Open,
                isHodl: false);
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public PaymentRequestRecord DecodeInvoice(string paymentRequest)
    {
        using var TL = TRACE.Log().Args(paymentRequest);
        try
        {
            var dec = LND.DecodeInvoice(lndConf, paymentRequest);
            return ParsePayReqToPaymentRequestRecord(dec);
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public RouteFeeRecord EstimateRouteFee(string paymentRequest, long ourRouteFeeSat, int timeout)
    {
        using var TL = TRACE.Log().Args(paymentRequest, ourRouteFeeSat, timeout);
        try
        {
            var decinv = LND.DecodeInvoice(lndConf, paymentRequest);

            Invoice invoice = null;
            HodlInvoice? selfHodlInvoice = null;
            ClassicInvoice? selfClsInv = null;
            try
            {
                invoice = LND.LookupInvoiceV2(lndConf, decinv.PaymentHash.AsBytes());
            }
            catch (Exception)
            {
                /* cannot locate */
            }

            using var walletContext = walletContextFactory.Create();

            using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

            if (invoice != null)
            {
                if (invoice.State == Lnrpc.Invoice.Types.InvoiceState.Settled)
                    throw new LNDWalletException(LNDWalletErrorCode.InvoiceAlreadySettled);
                else if (invoice.State == Lnrpc.Invoice.Types.InvoiceState.Canceled)
                    throw new LNDWalletException(LNDWalletErrorCode.InvoiceAlreadyCancelled);
                else if (invoice.State == Lnrpc.Invoice.Types.InvoiceState.Accepted)
                    throw new LNDWalletException(LNDWalletErrorCode.InvoiceAlreadyAccepted);

                selfHodlInvoice = (from inv in walletContext.HodlInvoices
                                   where inv.PaymentHash == decinv.PaymentHash
                                   select inv).FirstOrDefault();

                if (selfHodlInvoice == null)
                    selfClsInv = (from inv in walletContext.ClassicInvoices
                                  where inv.PaymentHash == decinv.PaymentHash
                                  select inv).FirstOrDefault();
            }

            if (selfHodlInvoice != null || selfClsInv != null) // selfpayment
            {

                var payment = (from pay in walletContext.InternalPayments
                               where pay.PaymentHash == decinv.PaymentHash
                               select pay).FirstOrDefault();

                if (payment != null)
                {
                    if (payment.Status == InternalPaymentStatus.InFlight)
                        throw new LNDWalletException(LNDWalletErrorCode.InvoiceAlreadyAccepted);
                    else //if (payment.Status == InternalPaymentStatus.Succeeded)
                        throw new LNDWalletException(LNDWalletErrorCode.InvoiceAlreadySettled);
                }
                TX.Commit();

                return new RouteFeeRecord { RoutingFeeMsat = ourRouteFeeSat * 1000, TimeLockDelay = 0, FailureReason = PaymentFailureReason.None };
            }
            else
            {
                TX.Commit();
                var rsp = LND.EstimateRouteFee(lndConf, paymentRequest, timeout);
                return new RouteFeeRecord { RoutingFeeMsat = rsp.RoutingFeeMsat, TimeLockDelay = rsp.TimeLockDelay, FailureReason = (PaymentFailureReason)rsp.FailureReason };
            }
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    private void failedPaymentRecordStore(PaymentRequestRecord payReq, PaymentFailureReason reason, bool delete)
    {
        using var walletContext = walletContextFactory.Create();

        walletContext
            .INSERT(new FailedPayment
            {
                Id = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                PaymentHash = payReq.PaymentHash,
                PublicKey = PublicKey,
                FailureReason = reason,
            })
            .SAVE();

        if (delete)
        {
            walletContext
                .DELETE_IF_EXISTS(from pay in walletContext.InternalPayments
                                  where pay.PaymentHash == payReq.PaymentHash
                                  select pay)
                .DELETE_IF_EXISTS(
                                from pay in walletContext.ExternalPayments
                                where pay.PaymentHash == payReq.PaymentHash
                                select pay)
                .SAVE();
        }
    }

    private PaymentRecord failedPaymentRecordAndCommit(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction TX, PaymentRequestRecord payReq, PaymentFailureReason reason, bool delete)
    {
        failedPaymentRecordStore(payReq, reason, delete);
        if(TX != null)
            TX.Commit();
        return PaymentRecord.FromPaymentRequestRecord(payReq, PaymentStatus.Failed, reason, 0);
    }



    public async Task<PaymentRecord> SendPaymentAsync(string paymentRequest, int timeout, long ourRouteFeeSat, long feelimit)
    {
        using var TL = TRACE.Log().Args(paymentRequest, timeout, ourRouteFeeSat, feelimit);
        try
        {
            var decinv = LND.DecodeInvoice(lndConf, paymentRequest);
            var paymentRequestRecord = ParsePayReqToPaymentRequestRecord(decinv);

            Invoice invoice = null;
            HodlInvoice? selfHodlInvoice = null;
            ClassicInvoice? selfClsInv = null;
            try
            {
                invoice = LND.LookupInvoiceV2(lndConf, decinv.PaymentHash.AsBytes());
            }
            catch (Exception) {/* cannot locate */  }

            using var walletContext = walletContextFactory.Create();
            using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

            var availableAmount = GetAccountBalance().AvailableAmount;

            if (invoice != null)
            {

                if (invoice.State == Lnrpc.Invoice.Types.InvoiceState.Settled)
                    return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InvoiceAlreadySettled, false);
                else if (invoice.State == Lnrpc.Invoice.Types.InvoiceState.Canceled)
                {
                    var internalPayment = (from pay in walletContext.InternalPayments
                                           where pay.PaymentHash == decinv.PaymentHash
                                           select pay).FirstOrDefault();

                    if (internalPayment != null)
                        return failedPaymentRecordAndCommit(TX, paymentRequestRecord, internalPayment.Status == InternalPaymentStatus.InFlight ? PaymentFailureReason.InvoiceAlreadyAccepted : PaymentFailureReason.InvoiceAlreadySettled, false);
                    else
                        return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InvoiceAlreadyCancelled, false);
                }
                else if (invoice.State == Lnrpc.Invoice.Types.InvoiceState.Accepted)
                    return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InvoiceAlreadyAccepted, false);


                selfHodlInvoice = (from inv in walletContext.HodlInvoices
                                   where inv.PaymentHash == decinv.PaymentHash
                                   select inv).FirstOrDefault();

                selfClsInv = null;
                if (selfHodlInvoice == null)
                    selfClsInv = (from inv in walletContext.ClassicInvoices
                                  where inv.PaymentHash == decinv.PaymentHash
                                  select inv).FirstOrDefault();
            }

            if (selfHodlInvoice != null || selfClsInv != null) // selfpayment
            {
                var invPubKey = selfHodlInvoice != null ? selfHodlInvoice.PublicKey : selfClsInv.PublicKey;

                var noFees = (invPubKey == PublicKey);

                if (!noFees)
                {
                    if (feelimit < ourRouteFeeSat)
                        return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.FeeLimitTooSmall, false);

                    if (availableAmount < decinv.NumSatoshis + ourRouteFeeSat)
                        return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InsufficientBalance, false);
                }

                var internalPayment = (from pay in walletContext.InternalPayments
                                       where pay.PaymentHash == decinv.PaymentHash
                                       select pay).FirstOrDefault();

                if (internalPayment != null)
                {
                    if (internalPayment.Status == InternalPaymentStatus.InFlight)
                        return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InvoiceAlreadyAccepted, false);
                    else //if (internalPayment.Status == InternalPaymentStatus.Succeeded)
                        return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InvoiceAlreadySettled, false);
                }


                walletContext
                    .INSERT(new InternalPayment()
                    {
                        PaymentHash = decinv.PaymentHash,
                        PublicKey = PublicKey,
                        PaymentFee = noFees ? 0 : ourRouteFeeSat,
                        Satoshis = decinv.NumSatoshis,
                        Status = selfHodlInvoice != null ? InternalPaymentStatus.InFlight : InternalPaymentStatus.Succeeded,
                        CreationTime = DateTime.UtcNow,
                    })
                    .SAVE();

                TX.Commit();

                eventSource.CancelSingleInvoiceTracking(decinv.PaymentHash);

                var pubkey = selfHodlInvoice != null ? selfHodlInvoice.PublicKey : selfClsInv.PublicKey;

                eventSource.FireOnInvoiceStateChanged(pubkey, decinv.PaymentHash, InvoiceState.Accepted);

                if (selfClsInv != null)
                    eventSource.FireOnInvoiceStateChanged(pubkey, decinv.PaymentHash, InvoiceState.Settled);

                eventSource.FireOnPaymentStatusChanged(PublicKey, decinv.PaymentHash,
                    selfHodlInvoice != null ? PaymentStatus.InFlight : PaymentStatus.Succeeded,
                    PaymentFailureReason.None);


                return ParsePayReqToPaymentRecord(decinv,
                    selfHodlInvoice != null ? PaymentStatus.InFlight : PaymentStatus.Succeeded,
                    PaymentFailureReason.None,
                    noFees ? 0 : ourRouteFeeSat * 1000);
            }
            else
            {
                if (availableAmount < decinv.NumSatoshis + feelimit)
                    return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InsufficientBalance, false);

                var externalPayment = (from pay in walletContext.ExternalPayments
                                       where pay.PaymentHash == decinv.PaymentHash
                                       select pay).FirstOrDefault();

                if (externalPayment != null)
                {
                    var stream = LND.TrackPaymentV2(lndConf, decinv.PaymentHash.AsBytes(), false);
                    Payment cur = null;
                    while (await stream.ResponseStream.MoveNext(trackPaymentsCancallationTokenSource.Token))
                    {
                        cur = stream.ResponseStream.Current;
                        if (cur.Status == Payment.Types.PaymentStatus.InFlight)
                            return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InvoiceAlreadyAccepted, false);
                        else if (cur.Status == Payment.Types.PaymentStatus.Initiated)
                            return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InvoiceAlreadyAccepted, false);
                        else if (cur.Status == Payment.Types.PaymentStatus.Succeeded)
                            return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.InvoiceAlreadySettled, false);
                        else
                            break;
                    }

                    // previously failed - retrying with my publickey
                    failedPaymentRecordStore(paymentRequestRecord, cur == null ? PaymentFailureReason.EmptyReturnStream : (PaymentFailureReason)cur.FailureReason, false);
                }
                {
                    walletContext
                        .INSERT(new ExternalPayment()
                        {
                            PaymentHash = decinv.PaymentHash,
                            PublicKey = PublicKey,
                            Status = ExternalPaymentStatus.Initiated,
                        })
                        .SAVE();

                    var stream = LND.SendPaymentV2(lndConf, paymentRequest, timeout, feelimit);
                    Payment cur = null;
                    while (await stream.ResponseStream.MoveNext(trackPaymentsCancallationTokenSource.Token))
                    {
                        cur = stream.ResponseStream.Current;
                        if (cur.Status == Payment.Types.PaymentStatus.Initiated
                            || cur.Status == Payment.Types.PaymentStatus.InFlight
                            || cur.Status == Payment.Types.PaymentStatus.Succeeded)
                            break;
                        else if (cur.Status == Payment.Types.PaymentStatus.Failed)
                            return failedPaymentRecordAndCommit(TX, paymentRequestRecord, (PaymentFailureReason)cur.FailureReason, true);
                    }

                    if (cur == null)
                        return failedPaymentRecordAndCommit(TX, paymentRequestRecord, PaymentFailureReason.EmptyReturnStream, true);

                    var payment = (from pay in walletContext.ExternalPayments
                                   where pay.PaymentHash == decinv.PaymentHash && pay.PublicKey == PublicKey
                                   select pay).FirstOrDefault();

                    if (payment != null)
                    {
                        payment.Status = (ExternalPaymentStatus)cur.Status;
                        walletContext.UPDATE(payment);
                        walletContext.SAVE();
                    }

                    TX.Commit();
                    return ParsePayReqToPaymentRecord(decinv, (PaymentStatus)cur.Status, PaymentFailureReason.None, cur.FeeMsat);
                }
            }
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public void SettleInvoice(byte[] preimage)
    {
        using var TL = TRACE.Log();
        try
        {
            var paymentHashBytes = Crypto.ComputePaymentHash(preimage);
            var paymentHash = paymentHashBytes.AsHex();
            TL.Args(paymentHash);
            var invoice = LND.LookupInvoiceV2(lndConf, paymentHashBytes);

            var paymentRequestRecord = ParseInvoiceToInvoiceRecord(invoice, true);
            using var walletContext = walletContextFactory.Create();
            using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

            HodlInvoice? selfHodlInvoice = (from inv in walletContext.HodlInvoices
                                            where inv.PaymentHash == paymentHash && inv.PublicKey == PublicKey
                                            select inv).FirstOrDefault();

            if (selfHodlInvoice == null)
                throw new LNDWalletException(LNDWalletErrorCode.UnknownInvoice);

            var internalPayment = (from pay in walletContext.InternalPayments
                                   where pay.PaymentHash == paymentHash
                                   select pay).FirstOrDefault();

            if (internalPayment != null)
            {
                if (internalPayment.Status == InternalPaymentStatus.Succeeded)
                    throw new LNDWalletException(LNDWalletErrorCode.InvoiceAlreadySettled);
                else if (internalPayment.Status != InternalPaymentStatus.InFlight) //this should be always true
                    throw new LNDWalletException(LNDWalletErrorCode.InvoiceNotAccepted);

                internalPayment.Status = InternalPaymentStatus.Succeeded;
                walletContext
                    .UPDATE(internalPayment)
                    .SAVE();
                TX.Commit();
            }
            else
            {
                TX.Commit();
                LND.SettleInvoice(lndConf, preimage);
            }

            if (internalPayment != null)
            {
                eventSource.FireOnInvoiceStateChanged(selfHodlInvoice.PublicKey, paymentHash, InvoiceState.Settled);
                eventSource.FireOnPaymentStatusChanged(internalPayment.PublicKey, paymentHash, PaymentStatus.Succeeded, PaymentFailureReason.None);
            }
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public void CancelInvoice(string paymentHash)
    {
        using var TL = TRACE.Log().Args(paymentHash);
        try
        {
            var invoice = LND.LookupInvoiceV2(lndConf, paymentHash.AsBytes());

            using var walletContext = walletContextFactory.Create();

            using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

            HodlInvoice? selfHodlInvoice = (from inv in walletContext.HodlInvoices
                                            where inv.PaymentHash == paymentHash && inv.PublicKey == PublicKey
                                            select inv).FirstOrDefault();

            ClassicInvoice? selfClsInv = null;
            if (selfHodlInvoice == null)
                selfClsInv = (from inv in walletContext.ClassicInvoices
                              where inv.PaymentHash == paymentHash
                              select inv).FirstOrDefault();

            if (selfClsInv == null)
                throw new LNDWalletException(LNDWalletErrorCode.UnknownInvoice);

            var internalPayment = (from pay in walletContext.InternalPayments
                                   where pay.PaymentHash == paymentHash
                                   select pay).FirstOrDefault();

            if (internalPayment != null)
            {
                if (internalPayment.Status == InternalPaymentStatus.Succeeded)
                    throw new LNDWalletException(LNDWalletErrorCode.InvoiceAlreadySettled);
                else if (internalPayment.Status != InternalPaymentStatus.InFlight) // this should be always true
                    throw new LNDWalletException(LNDWalletErrorCode.InvoiceNotAccepted);

                walletContext
                    .DELETE_IF_EXISTS(from pay in walletContext.InternalPayments
                                      where pay.PaymentHash == paymentHash
                                      select pay)
                    .INSERT(new FailedPayment
                    {
                        Id = Guid.NewGuid(),
                        DateTime = DateTime.UtcNow,
                        PaymentHash = paymentHash,
                        PublicKey = PublicKey,
                        FailureReason = PaymentFailureReason.Canceled,
                    })
                    .SAVE();

                TX.Commit();

                eventSource.CancelSingleInvoiceTracking(paymentHash);
                eventSource.FireOnPaymentStatusChanged(internalPayment.PublicKey, paymentHash, PaymentStatus.Failed, PaymentFailureReason.Canceled);
            }
            else
                TX.Commit();

            LND.CancelInvoice(lndConf, paymentHash.AsBytes());
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }

    }


    public static InvoiceRecord ParseInvoiceToInvoiceRecord(Invoice invoice, bool isHodl)
    {
        return new InvoiceRecord
        {
            PaymentHash = invoice.RHash.ToArray().AsHex(),
            IsHodl = isHodl,
            PaymentRequest = invoice.PaymentRequest,
            Satoshis = invoice.Value,
            State = (InvoiceState)invoice.State,
            Memo = invoice.Memo,
            PaymentAddr = invoice.PaymentAddr.ToArray().AsHex(),
            CreationTime = DateTimeOffset.FromUnixTimeSeconds(invoice.CreationDate).UtcDateTime,
            ExpiryTime = DateTimeOffset.FromUnixTimeSeconds(invoice.CreationDate).UtcDateTime.AddSeconds(invoice.Expiry),
            SettleTime = DateTimeOffset.FromUnixTimeSeconds(invoice.SettleDate).UtcDateTime,
        };

    }

    public static PaymentRequestRecord ParsePayReqToPaymentRequestRecord(PayReq payReq)
    {
        return new PaymentRequestRecord
        {
            PaymentHash = payReq.PaymentHash,
            Satoshis = payReq.NumSatoshis,
            Memo = payReq.Description,
            PaymentAddr = payReq.PaymentAddr.ToArray().AsHex(),
            CreationTime = DateTimeOffset.FromUnixTimeSeconds(payReq.Timestamp).UtcDateTime,
            ExpiryTime = DateTimeOffset.FromUnixTimeSeconds(payReq.Timestamp).UtcDateTime.AddSeconds(payReq.Expiry),
        };

    }

    public static PaymentRecord ParsePayReqToPaymentRecord(PayReq payReq, PaymentStatus status, PaymentFailureReason reason, long feeMsat)
    {
        return new PaymentRecord
        {
            PaymentHash = payReq.PaymentHash,
            Satoshis = payReq.NumSatoshis,
            FailureReason = reason,
            FeeMsat = feeMsat,
            Status = status,
            CreationTime = DateTimeOffset.FromUnixTimeSeconds(payReq.Timestamp).UtcDateTime,
        };

    }
    public InvoiceRecord[] ListInvoices(bool includeClassic, bool includeHodl)
    {
        var allInvs = new Dictionary<string, InvoiceRecord>(
            (from inv in LND.ListInvoices(lndConf).Invoices 
             select KeyValuePair.Create(inv.RHash.ToArray().AsHex(),
             ParseInvoiceToInvoiceRecord(invoice: inv, isHodl: false))));

        var ret = new Dictionary<string, InvoiceRecord>();

        using var walletContext = walletContextFactory.Create();

        if (includeClassic)
        {
            var myInvoices = (from inv in walletContext.ClassicInvoices where inv.PublicKey == this.PublicKey select inv).ToList();
            foreach (var inv in myInvoices)
            {
                if (allInvs.ContainsKey(inv.PaymentHash))
                    ret.Add(inv.PaymentHash, allInvs[inv.PaymentHash]);
            }
        }

        if (includeHodl)
        {
            var myInvoices = (from inv in walletContext.HodlInvoices where inv.PublicKey == this.PublicKey select inv).ToList();
            foreach (var inv in myInvoices)
            {
                if (allInvs.ContainsKey(inv.PaymentHash))
                {
                    allInvs[inv.PaymentHash].IsHodl = true;
                    ret.Add(inv.PaymentHash, allInvs[inv.PaymentHash]);
                }
            }
        }

        {
            var internalPayments = (from pay in walletContext.InternalPayments
                                    select pay).ToList();
            foreach (var pay in internalPayments)
            {
                if (ret.ContainsKey(pay.PaymentHash))
                    ret[pay.PaymentHash].State = (pay.Status == InternalPaymentStatus.InFlight) ? InvoiceState.Accepted : (pay.Status == InternalPaymentStatus.Succeeded ? InvoiceState.Settled : InvoiceState.Cancelled);
            }
        }

        return ret.Values.ToArray();
    }

    public PaymentRecord[] ListNotFailedPayments()
    {
        var allPays = new Dictionary<string, PaymentRecord>(
            (from pay in LND.ListPayments(lndConf).Payments
             select KeyValuePair.Create(pay.PaymentHash, new PaymentRecord
             {
                 PaymentHash = pay.PaymentHash,
                 Satoshis = pay.ValueSat,
                 FeeMsat = pay.FeeMsat,
                 Status = (PaymentStatus)pay.Status,
                 FailureReason = (PaymentFailureReason)pay.FailureReason,
                 CreationTime = DateTimeOffset.FromUnixTimeMilliseconds(pay.CreationTimeNs / 1000000).UtcDateTime
             })));

        var ret = new Dictionary<string, PaymentRecord>();

        using var walletContext = walletContextFactory.Create();

        {
            var myPayments = (from pay in walletContext.ExternalPayments where pay.PublicKey == this.PublicKey select pay).ToList();
            foreach (var pay in myPayments)
            {
                if (allPays[pay.PaymentHash].Status != PaymentStatus.Failed)
                    if (allPays.ContainsKey(pay.PaymentHash))
                        ret.Add(pay.PaymentHash, allPays[pay.PaymentHash]);
            }
        }
        {
            var myPayments = (from pay in walletContext.InternalPayments where pay.PublicKey == this.PublicKey select pay).ToList();
            foreach (var pay in myPayments)
            {
                ret.Add(pay.PaymentHash, new PaymentRecord
                {
                    PaymentHash = pay.PaymentHash,
                    Satoshis = pay.Satoshis,
                    FeeMsat = pay.PaymentFee * 1000,
                    Status = (PaymentStatus)pay.Status,
                    FailureReason = PaymentFailureReason.None,
                    CreationTime = pay.CreationTime,
                });
            }
        }

        return ret.Values.ToArray();
    }

    public async Task<PaymentRecord> GetPaymentAsync(string paymentHash)
    {
        using var walletContext = walletContextFactory.Create();

        var internalPayment = (from pay in walletContext.InternalPayments
                               where pay.PaymentHash == paymentHash
                               select pay).FirstOrDefault();

        if (internalPayment != null)
        {
            if (internalPayment.PublicKey != PublicKey)
                throw new LNDWalletException(LNDWalletErrorCode.UnknownPayment);

            return new PaymentRecord
            {
                PaymentHash = internalPayment.PaymentHash,
                Satoshis = internalPayment.Satoshis,
                FeeMsat = internalPayment.PaymentFee * 1000,
                Status = (PaymentStatus)internalPayment.Status,
                FailureReason = PaymentFailureReason.None,
                CreationTime = internalPayment.CreationTime,
            };
        }
        else
        {
            var externalPayment = (from pay in walletContext.ExternalPayments
                                   where pay.PaymentHash == paymentHash
                                   && pay.PublicKey == PublicKey
                                   select pay).FirstOrDefault();

            if (externalPayment == null)
                throw new LNDWalletException(LNDWalletErrorCode.UnknownPayment);

            var stream = LND.TrackPaymentV2(lndConf, paymentHash.AsBytes(), false);
            while (await stream.ResponseStream.MoveNext(trackPaymentsCancallationTokenSource.Token))
            {
                var cur = stream.ResponseStream.Current;
                return new PaymentRecord
                {
                    PaymentHash = cur.PaymentHash,
                    Satoshis = cur.ValueSat,
                    FeeMsat = cur.FeeMsat,
                    Status = (PaymentStatus)cur.Status,
                    FailureReason = (PaymentFailureReason)cur.FailureReason,
                    CreationTime = DateTimeOffset.FromUnixTimeMilliseconds(cur.CreationTimeNs / 1000000).UtcDateTime
                };
            }
            return new PaymentRecord
            {
                PaymentHash = paymentHash,
                Satoshis = 0,
                FeeMsat = 0,
                Status = PaymentStatus.Failed,
                FailureReason = PaymentFailureReason.EmptyReturnStream,
                CreationTime = DateTime.UtcNow,
            };
        }
    }

    public async Task<InvoiceRecord> GetInvoiceAsync(string paymentHash)
    {

        var invoice = LND.LookupInvoiceV2(lndConf, paymentHash.AsBytes());

        using var walletContext = walletContextFactory.Create();

        using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);
        HodlInvoice? selfHodlInvoice = (from inv in walletContext.HodlInvoices
                                        where inv.PaymentHash == paymentHash
                                        select inv).FirstOrDefault();

        if ((invoice.State != Lnrpc.Invoice.Types.InvoiceState.Open) && (invoice.State != Lnrpc.Invoice.Types.InvoiceState.Canceled))
        {
            return ParseInvoiceToInvoiceRecord(invoice, selfHodlInvoice != null);
        }

        // if InvoiceState is Open or Cancelled

        ClassicInvoice? selfClsInv = null;
        if (selfHodlInvoice == null)
            selfClsInv = (from inv in walletContext.ClassicInvoices
                          where inv.PaymentHash == paymentHash
                          select inv).FirstOrDefault();

        if (selfHodlInvoice != null || selfClsInv != null) // our invoice
        {

            var payment = (from pay in walletContext.InternalPayments
                           where pay.PaymentHash == paymentHash
                           select pay).FirstOrDefault();

            TX.Commit();
            var ret = ParseInvoiceToInvoiceRecord(invoice, selfHodlInvoice != null);
            ret.State = payment == null ? (InvoiceState)invoice.State : (payment.Status == InternalPaymentStatus.InFlight ? InvoiceState.Accepted : InvoiceState.Settled);
            return ret;
        }
        throw new LNDWalletException(LNDWalletErrorCode.UnknownInvoice);
    }

    public AccountBalanceDetails GetBalance()
    {
        using var walletContext = walletContextFactory.Create();

        using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);
        var bal = GetAccountBalance();
        TX.Commit();
        return bal;
    }

    private AccountBalanceDetails GetAccountBalance()
    {

        var channelfunds = GetExecutedTopupTotalAmount(6);

        using var walletContext = walletContextFactory.Create();

        var payedout = (from a in walletContext.Payouts
                        where a.PublicKey == PublicKey
                        && a.State != PayoutState.Failure
                        select a.Satoshis).Sum();

        var payedoutfee = (from a in walletContext.Payouts
                           where a.PublicKey == PublicKey
                           && a.State != PayoutState.Failure
                           select a.PayoutFee).Sum();

        var invoices = ListInvoices(true, true);
        var payments = ListNotFailedPayments();

        var earnedFromSettledInvoices = (from inv in invoices
                                         where inv.State == InvoiceState.Settled
                                         select inv.Satoshis).Sum();

        var sendOrLockedPayments = (from pay in payments
                                    select pay.Satoshis).Sum();

        var sendOrLockedPaymentFeesMsat = (from pay in payments
                                           select pay.FeeMsat).Sum();


        var channelfundsAndNotConfirmed = GetExecutedTopupTotalAmount(0);

        var earnedFromAcceptedInvoices = (from inv in invoices
                                          where inv.State == InvoiceState.Accepted
                                          select inv.Satoshis).Sum();

        var lockedPayedout = (from a in walletContext.Payouts
                              where a.PublicKey == PublicKey
                              && a.State != PayoutState.Sent
                           && a.State != PayoutState.Failure
                              select a.Satoshis).Sum();

        var lockedPayedoutFee = (from a in walletContext.Payouts
                                 where a.PublicKey == PublicKey
                                 && a.State != PayoutState.Sent
                              && a.State != PayoutState.Failure
                                 select a.PayoutFee).Sum();

        var lockedPayments = (from pay in payments
                              where pay.Status != PaymentStatus.Succeeded
                              select pay.Satoshis).Sum();

        var lockedPaymentFeesMsat = (from pay in payments
                                     where pay.Status != PaymentStatus.Succeeded
                                     select pay.FeeMsat).Sum();

        var executedPayedous = (from a in walletContext.Payouts
                                where a.PublicKey == PublicKey
                             && a.State != PayoutState.Failure
                             && a.State != PayoutState.Open
                                select a);

        long totalTxFees = 0;
        long sentTxFees = 0;
        foreach (var pa in executedPayedous)
        {
            if (!string.IsNullOrEmpty(pa.Tx))
            {
                var tx = LND.GetTransaction(this.lndConf, pa.Tx);
                totalTxFees += tx.TotalFees;
                if (pa.State == PayoutState.Sent)
                    sentTxFees += tx.TotalFees;
            }
        }

        return new AccountBalanceDetails
        {
            AvailableAmount = channelfunds - payedout + earnedFromSettledInvoices - sendOrLockedPayments - sendOrLockedPaymentFeesMsat / 1000,

            TotalAmount = channelfundsAndNotConfirmed - payedout + earnedFromSettledInvoices + earnedFromAcceptedInvoices - sendOrLockedPayments - sendOrLockedPaymentFeesMsat / 1000,

            TotalTopups = channelfundsAndNotConfirmed,
            NotConfirmedTopups = channelfundsAndNotConfirmed - channelfunds,
            TotalEarnings = earnedFromAcceptedInvoices + earnedFromSettledInvoices,
            SettledEarnings = earnedFromSettledInvoices,

            TotalPayments = -sendOrLockedPayments,
            TotalPaymentFees = -sendOrLockedPaymentFeesMsat / 1000,

            InFlightPayments = -lockedPayments,
            InFlightPaymentFees = -lockedPaymentFeesMsat / 1000,

            TotalPayouts = -payedout,
            TotalPayoutFees = -payedoutfee,

            InProgressPayouts = -lockedPayedout,
            InProgressPayoutFees = -lockedPayedoutFee,

            InProgressPayoutOnChainFees = -(totalTxFees - sentTxFees),
            TotalPayoutOnChainFees = -(totalTxFees),
        };
    }

}

public class LNDWalletManager : LNDEventSource
{
    public GigDebugLoggerAPIClient.LogWrapper<LNDWalletManager> TRACE = GigDebugLoggerAPIClient.ConsoleLoggerFactory.Trace<LNDWalletManager>();

    public BitcoinNode BitcoinNode;
    private LND.NodeSettings lndConf;
    private WaletContextFactory walletContextFactory;
    private DBProvider provider;
    private string connectionString;
    private string adminPubkey;
    private CancellationTokenSource subscribeInvoicesCancallationTokenSource;
    private Thread subscribeInvoicesThread;
    private CancellationTokenSource trackPaymentsCancallationTokenSource;
    private Thread trackPaymentsThread;
    private CancellationTokenSource subscribeTransactionsCancallationTokenSource;
    private Thread subscribeTransactionsThread;
    private ConcurrentDictionary<string, bool> alreadySubscribedSingleInvoices = new();
    private ConcurrentDictionary<string, CancellationTokenSource> alreadySubscribedSingleInvoicesTokenSources = new();
    private bool enforceTwoFactorAuthentication;

    public LNDWalletManager(DBProvider provider, string connectionString, BitcoinNode bitcoinNode, LND.NodeSettings lndConf, string adminPubkey, bool enforceTwoFactorAuthentication)
    {
        this.provider = provider;
        this.connectionString = connectionString;
        this.walletContextFactory = new WaletContextFactory(provider, connectionString);
        this.BitcoinNode = bitcoinNode;
        this.lndConf = lndConf;
        this.adminPubkey = adminPubkey;
        this.enforceTwoFactorAuthentication = enforceTwoFactorAuthentication;
        using var walletContext = walletContextFactory.Create();
        walletContext.Database.EnsureCreated();
    }



    public ListPeersResponse ListPeers()
    {
        return LND.ListPeers(lndConf);
    }

    public void Connect(string friend)
    {
        var pr = friend.Split('@');
        LND.Connect(lndConf, pr[1], pr[0]);
    }

    private void SubscribeSingleInvoiceTracking(string pubkey, string paymentHash)
    {
        alreadySubscribedSingleInvoices.GetOrAdd(paymentHash, (paymentHash) =>
        {
            Task.Run(async () =>
            {

                try
                {
                    alreadySubscribedSingleInvoicesTokenSources[paymentHash] = new CancellationTokenSource();
                    while (alreadySubscribedSingleInvoicesTokenSources.ContainsKey(paymentHash))
                    {
                        try
                        {
                            var stream = LND.SubscribeSingleInvoice(lndConf, paymentHash.AsBytes(), cancellationToken: alreadySubscribedSingleInvoicesTokenSources[paymentHash].Token);
                            while (await stream.ResponseStream.MoveNext(alreadySubscribedSingleInvoicesTokenSources[paymentHash].Token))
                            {
                                var inv = stream.ResponseStream.Current;
                                this.FireOnInvoiceStateChanged(pubkey, inv.RHash.ToArray().AsHex(), (InvoiceState)inv.State);
                                if (inv.State == Invoice.Types.InvoiceState.Settled || inv.State == Invoice.Types.InvoiceState.Canceled)
                                {
                                    alreadySubscribedSingleInvoicesTokenSources.TryRemove(paymentHash, out _);
                                    break;
                                }
                            }
                        }
                        catch (RpcException e)
                        {
                            TraceEx.TraceInformation($"Streaming was {e.Status.StatusCode.ToString()} from the client!");
                            TraceEx.TraceException(e);
                        }
                        catch (Exception ex)
                        {
                            TraceEx.TraceException(ex);
                        }
                    }
                }
                finally
                {
                    alreadySubscribedSingleInvoices.TryRemove(paymentHash, out _);
                }
            });
            return true;
        });
    }

    public override void CancelSingleInvoiceTracking(string paymentHash)
    {
        if (alreadySubscribedSingleInvoicesTokenSources.TryRemove(paymentHash, out var cancellationTokenSource))
            cancellationTokenSource.Cancel();
    }

    public void Start()
    {
        alreadySubscribedSingleInvoicesTokenSources = new();
        {
            using var walletContext = walletContextFactory.Create();

            try
            {
                walletContext.INSERT_IF_NOT_EXISTS(from t in walletContext.TrackingIndexes where t.Id == TackingIndexId.StartTransactions select t, new TrackingIndex { Id = TackingIndexId.StartTransactions, Value = 0 }).SAVE();
                walletContext.INSERT_IF_NOT_EXISTS(from t in walletContext.TrackingIndexes where t.Id == TackingIndexId.AddInvoice select t, new TrackingIndex { Id = TackingIndexId.AddInvoice, Value = 0 }).SAVE();
                walletContext.INSERT_IF_NOT_EXISTS(from t in walletContext.TrackingIndexes where t.Id == TackingIndexId.SettleInvoice select t, new TrackingIndex { Id = TackingIndexId.SettleInvoice, Value = 0 }).SAVE();
            }
            catch (DbUpdateException)
            {
                //Ignore already inserted
            }
        }
        { 
            subscribeInvoicesCancallationTokenSource = new CancellationTokenSource();
            subscribeInvoicesThread = new Thread(async () =>
            {
                using var walletContext = walletContextFactory.Create();
                TraceEx.TraceInformation("SubscribeInvoices Thread Starting");
                var allInvs = new Dictionary<string, InvoiceRecord>(
                    from inv in LND.ListInvoices(lndConf).Invoices
                    select KeyValuePair.Create(inv.RHash.ToArray().AsHex(), LNDAccountManager.ParseInvoiceToInvoiceRecord(invoice: inv, isHodl: false)));

                var internalPayments = new HashSet<string>(from pay in walletContext.InternalPayments
                                                           where pay.Status == InternalPaymentStatus.InFlight
                                                           select pay.PaymentHash);

                foreach (var inv in allInvs.Values)
                {
                    if (!internalPayments.Contains(inv.PaymentHash) && inv.State == InvoiceState.Accepted)
                    {
                        var pubkey = (from i in walletContext.HodlInvoices
                                      where i.PaymentHash == inv.PaymentHash
                                      select i.PublicKey).FirstOrDefault();
                        if (pubkey == null)
                            pubkey = (from i in walletContext.ClassicInvoices
                                      where i.PaymentHash == inv.PaymentHash
                                      select i.PublicKey).FirstOrDefault();
                        if (pubkey == null)
                            continue;
                        SubscribeSingleInvoiceTracking(pubkey, inv.PaymentHash);
                    }
                }

                while (!Stopping)
                {
                    TraceEx.TraceInformation("SubscribeInvoices Loop Starting");
                    try
                    {
                        var trackIdxes = new Dictionary<TackingIndexId, ulong>(from idx in walletContext.TrackingIndexes
                                                                               select KeyValuePair.Create(idx.Id, idx.Value));

                        var stream = LND.SubscribeInvoices(lndConf, trackIdxes[TackingIndexId.AddInvoice], trackIdxes[TackingIndexId.SettleInvoice],
                            cancellationToken: subscribeInvoicesCancallationTokenSource.Token);

                        while (await stream.ResponseStream.MoveNext(subscribeInvoicesCancallationTokenSource.Token))
                        {
                            var inv = stream.ResponseStream.Current;

                            var pubkey = (from i in walletContext.HodlInvoices
                                          where i.PaymentHash == inv.RHash.ToArray().AsHex()
                                          select i.PublicKey).FirstOrDefault();
                            if (pubkey == null)
                                pubkey = (from i in walletContext.ClassicInvoices
                                          where i.PaymentHash == inv.RHash.ToArray().AsHex()
                                          select i.PublicKey).FirstOrDefault();

                            if (pubkey == null)
                                continue;

                            this.FireOnInvoiceStateChanged(pubkey, inv.RHash.ToArray().AsHex(), (InvoiceState)inv.State);
                            if (inv.State == Lnrpc.Invoice.Types.InvoiceState.Accepted)
                                SubscribeSingleInvoiceTracking(pubkey, inv.RHash.ToArray().AsHex());

                            walletContext.UPDATE(new TrackingIndex { Id = TackingIndexId.AddInvoice, Value = inv.AddIndex }).SAVE();
                            walletContext.UPDATE(new TrackingIndex { Id = TackingIndexId.SettleInvoice, Value = inv.SettleIndex }).SAVE();
                        }
                    }
                    catch (RpcException e)
                    {
                        TraceEx.TraceInformation($"Subscribe Invoices streaming was {e.Status.StatusCode.ToString()} from the client!");
                        TraceEx.TraceException(e);
                    }
                    catch (Exception ex)
                    {
                        TraceEx.TraceException(ex);
                    }
                }

                TraceEx.TraceInformation("SubscribeInvoices Thread Joining");

            });
        }

        trackPaymentsCancallationTokenSource = new CancellationTokenSource();
        trackPaymentsThread = new Thread(async () =>
        {
            TraceEx.TraceInformation("TrackPayments Thread Starting");
            while (!Stopping)
            {
                TraceEx.TraceInformation("TrackPayments Loop Starting");
                try
                {
                    using var walletContext = walletContextFactory.Create();
                    var stream = LND.TrackPayments(lndConf, cancellationToken: trackPaymentsCancallationTokenSource.Token);
                    while (await stream.ResponseStream.MoveNext(trackPaymentsCancallationTokenSource.Token))
                    {
                        var pm = stream.ResponseStream.Current;
                        var pay = (from i in walletContext.InternalPayments
                            where i.PaymentHash == pm.PaymentHash
                            select i).FirstOrDefault();

                        if (pay == null)
                            continue;

                        this.FireOnPaymentStatusChanged(pay.PublicKey, pm.PaymentHash, (PaymentStatus)pm.Status, (PaymentFailureReason)pm.FailureReason);
                    }
                }
                catch (RpcException e)
                {
                    TraceEx.TraceInformation($"TrackPayments streaming was {e.Status.StatusCode.ToString()} from the client!");
                    TraceEx.TraceException(e);
                }
                catch (Exception ex)
                {
                    TraceEx.TraceException(ex);
                }
            }
            TraceEx.TraceInformation("TrackPayments Thread Joining");
        });

        subscribeTransactionsCancallationTokenSource = new CancellationTokenSource();
        subscribeTransactionsThread = new Thread(async () =>
        {
            TraceEx.TraceInformation("SubscribeTransactions Thread Starting");
            while (!Stopping)
            {
                using var walletContext = walletContextFactory.Create();
                var trackIdxes = new Dictionary<TackingIndexId, ulong>(from idx in walletContext.TrackingIndexes.AsNoTracking()
                                                                       select KeyValuePair.Create(idx.Id, idx.Value));
                TraceEx.TraceInformation("SubscribeTransactions Loop Starting");
                try
                {
                    var stream = LND.SubscribeTransactions(lndConf, (int)trackIdxes[TackingIndexId.StartTransactions], cancellationToken: subscribeTransactionsCancallationTokenSource.Token);
                    while (await stream.ResponseStream.MoveNext(subscribeTransactionsCancallationTokenSource.Token))
                    {
                        var transation = stream.ResponseStream.Current;
                        foreach (var outp in transation.OutputDetails)
                            if (outp.IsOurAddress)
                            {
                                var pubkey = (from a in walletContext.TopupAddresses where a.BitcoinAddress == outp.Address select a.PublicKey).FirstOrDefault();
                                if (pubkey != null)
                                    this.FireOnNewTransactionFound(pubkey, transation.TxHash, transation.NumConfirmations, outp.Address, outp.Amount);
                            }
                        walletContext.UPDATE(new TrackingIndex { Id = TackingIndexId.StartTransactions, Value = (ulong)transation.BlockHeight}).SAVE();
                    }
                }
                catch (RpcException e)
                {
                    TraceEx.TraceInformation($"SubscribeTransactions streaming was {e.Status.StatusCode.ToString()} from the client!");
                    TraceEx.TraceException(e);
                }
                catch (Exception ex)
                {
                    TraceEx.TraceException(ex);
                }
            }
            TraceEx.TraceInformation("SubscribeTransactions Thread Joining");

        });

        subscribeInvoicesThread.Start();
        trackPaymentsThread.Start();
        subscribeTransactionsThread.Start();

    }

    public bool Stopping = false;

    public void Stop()
    {
        TraceEx.TraceInformation("Stopping...");
        Stopping = true;

        foreach (var s in alreadySubscribedSingleInvoicesTokenSources)
            s.Value.Cancel();

        subscribeTransactionsCancallationTokenSource.Cancel();
        subscribeInvoicesCancallationTokenSource.Cancel();
        trackPaymentsCancallationTokenSource.Cancel();
        subscribeInvoicesThread.Join();
        trackPaymentsThread.Join();
        TraceEx.TraceInformation("...Stopped");
    }

    private string ValidateAuthTokenNoTimeout(string authTokenBase64)
    {
        var timedToken = ApiBtc.AuthToken.Verify(authTokenBase64, -1);
        if (timedToken == null)
            throw new LNDWalletException(LNDWalletErrorCode.InvalidToken);

        using var walletContext = walletContextFactory.Create();

        var tk = (from token in walletContext.Tokens where token.PublicKey == ApiBtc.ProtoBufExtensions.AsHex(timedToken.Header.PublicKey) && token.Id == ApiBtc.ProtoBufExtensions.AsGuid(timedToken.Header.TokenId) select token).FirstOrDefault();
        if (tk == null)
            throw new LNDWalletException(LNDWalletErrorCode.InvalidToken);
        return tk.PublicKey;
    }

    private string ValidateAuthTokenTimeout(string authTokenBase64)
    {
        var timedToken = ApiBtc.AuthToken.Verify(authTokenBase64, 120.0);
        if (timedToken == null)
            throw new LNDWalletException(LNDWalletErrorCode.InvalidToken);

        using var walletContext = walletContextFactory.Create();

        var tk = (from token in walletContext.Tokens where token.PublicKey == ApiBtc.ProtoBufExtensions.AsHex(timedToken.Header.PublicKey) && token.Id == ApiBtc.ProtoBufExtensions.AsGuid(timedToken.Header.TokenId) select token).FirstOrDefault();
        if (tk == null)
            throw new LNDWalletException(LNDWalletErrorCode.InvalidToken);
        return tk.PublicKey;
    }


    public string ValidateAuthToken(string authTokenBase64, TokenType tokenType = TokenType.Normal)
    {
        var pubkey = (tokenType == TokenType.LongTerm) ?
            ValidateAuthTokenNoTimeout(authTokenBase64) :
            ValidateAuthTokenTimeout(authTokenBase64);
        if (tokenType == TokenType.Admin)
            if (!HasAdminRights(pubkey))
                throw new LNDWalletException(LNDWalletErrorCode.AccessDenied);
        return pubkey;
    }

    public void VerifyTotp(string pubkey, string code)
    {
        using var TL = TRACE.Log().Args(code);
        try
        {
            using var walletContext = walletContextFactory.Create();
            // Find the TFA entry for this user
            var tfaEntry = (from tfa in walletContext.TwoFactorAuths
                            where tfa.PublicKey == pubkey
                            select tfa).FirstOrDefault();
            if (tfaEntry == null)
                if (enforceTwoFactorAuthentication)
                    throw new LNDWalletException(LNDWalletErrorCode.TwoFactorNotEnabled);
                else
                    return; // If not enforced, we can skip verification
            // Verify the TOTP code

            var base32Bytes = Base32Encoding.ToBytes(tfaEntry.SecretKey);
            var totp = new Totp(base32Bytes);
            if (!totp.VerifyTotp(code, out _))
                throw new LNDWalletException(LNDWalletErrorCode.InvalidTwoFactorCode);
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public LNDAccountManager ValidateAuthTokenAndGetAccount(string authTokenBase64, TokenType tokenType = TokenType.Normal)
    {
        return GetAccount(ValidateAuthToken(authTokenBase64, tokenType).AsECXOnlyPubKey());
    }


    public LNDAccountManager GetAccount(ECXOnlyPubKey pubkey)
    {
        return new LNDAccountManager(lndConf, this.provider, this.connectionString, pubkey, this, this.enforceTwoFactorAuthentication);
    }

    public bool HasAdminRights(string pubkey)
    {
        return pubkey == this.adminPubkey;
    }

    public Guid GetTokenGuid(string pubkey)
    {
        using var walletContext = walletContextFactory.Create();

        var t = (from token in walletContext.Tokens where pubkey == token.PublicKey select token).FirstOrDefault();
        if (t == null)
        {
            t = new Token() { Id = Guid.NewGuid(), PublicKey = pubkey };
            walletContext
                .INSERT(t)
                .SAVE();
        }
        return t.Id;
    }


    public string SendCoins(string address, long satoshis, string memo)
    {
        return LND.SendCoins(lndConf, address, memo, satoshis);
    }

    /*
    public Lnrpc.Transaction GetTransaction(string txid)
    {
        return LND.GetTransaction(lndConf, txid);
    }
    */

    public Guid OpenReserve(long satoshis)
    {
        var myid = Guid.NewGuid();

        using var walletContext = walletContextFactory.Create();

        walletContext
            .INSERT(new LNDWallet.Reserve()
            {
                ReserveId = myid,
                Satoshis = satoshis
            })
            .SAVE();
        return myid;
    }

    public void CloseReserve(Guid id)
    {
        using var walletContext = walletContextFactory.Create();

        walletContext
            .DELETE_IF_EXISTS(from po in walletContext.Reserves where po.ReserveId == id select po)
            .SAVE();
    }

    public List<Guid> ListOrphanedReserves()
    {
        using var walletContext = walletContextFactory.Create();

        var allReserves = new HashSet<Guid>(from po in walletContext.Reserves select po.ReserveId);
        var allPayouts = new HashSet<Guid>(from po in walletContext.Payouts where po.State!= PayoutState.Failure && po.State != PayoutState.Sent select po.PayoutId);
        allReserves.ExceptWith(allPayouts);
        return allReserves.ToList();
    }

    internal bool MarkPayoutAsSending(Guid id, long fee)
    {
        using var walletContext = walletContextFactory.Create();

        using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

        var payout = (from po in walletContext.Payouts where po.PayoutId == id && po.State == PayoutState.Open select po).FirstOrDefault();
        if (payout == null)
            return false;
        payout.State = PayoutState.Sending;
        payout.PayoutFee = fee;
        walletContext
            .UPDATE(payout)
            .SAVE();
        TX.Commit();
        FireOnPayoutStateChanged(payout.PublicKey, payout.PayoutId, payout.State, payout.PayoutFee, payout.Tx);
        return true;
    }

    internal bool MarkPayoutAsFailure(Guid id, string tx)
    {
        using var walletContext = walletContextFactory.Create();

        using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

        var payout = (from po in walletContext.Payouts where po.PayoutId == id select po).FirstOrDefault();
        if (payout == null)
            return false;
        payout.State = PayoutState.Failure;
        payout.Tx = tx;
        walletContext
            .UPDATE(payout)
            .SAVE();

        CloseReserve(id);
        TX.Commit();
        FireOnPayoutStateChanged(payout.PublicKey, payout.PayoutId, payout.State, payout.PayoutFee, payout.Tx);

        return true;
    }

    internal void MarkPayoutAsSent(Guid id, string tx)
    {
        using var walletContext = walletContextFactory.Create();

        using var TX = walletContext.BEGIN_TRANSACTION(IsolationLevel.Serializable);

        var payout = (from po in walletContext.Payouts where po.PayoutId == id && po.State == PayoutState.Sending select po).FirstOrDefault();
        if (payout == null)
            throw new LNDWalletException(LNDWalletErrorCode.PayoutAlreadySent);
        payout.State = PayoutState.Sent;
        payout.Tx = tx;
        walletContext
            .UPDATE(payout)
            .SAVE();

        CloseReserve(id);
        TX.Commit();

        FireOnPayoutStateChanged(payout.PublicKey, payout.PayoutId, payout.State, payout.PayoutFee, payout.Tx);
    }

    public (long feeSat, ulong satpervbyte) EstimateFee(string addr, long satoshis)
    {
        try
        {
            var est = LND.FeeEstimate(lndConf, new List<(string, long)>() { (addr, satoshis) }, 6, 6);
            return (est.FeeSat, est.SatPerVbyte);
        }
        catch (RpcException ex)
        {
            throw new LNDWalletException(LNDWalletErrorCode.OperationFailed, ex.Status.Detail);
        }
    }

    /*
    public long GetTransactions(int minConf)
    {
        var myaddrs = new Dictionary<string, long>(
            from a in walletContext.FundingAddresses
            where a.PublicKey == PublicKey
            select new KeyValuePair<string, long>(a.BitcoinAddress, a.TxFee));

        var transactuinsResp = LND.GetTransactions(conf);
        long balance = 0;
        foreach (var transation in transactuinsResp.Transactions)
            if (transation.NumConfirmations >= minConf)
                foreach (var outp in transation.OutputDetails)
                    if (outp.IsOurAddress)
                        if (myaddrs.ContainsKey(outp.Address))
                        {
                            balance += outp.Amount;
                            balance -= (long)myaddrs[outp.Address];
                        }
        return balance;
    }
    */

    /*
    public long GetChannelFundingBalance(int minconf)
    {
        return (from utxo in LND.ListUnspent(lndConf, minconf).Utxos select utxo.AmountSat).Sum();
    }
    */

    public WalletBalanceResponse GetWalletBalance()
    {
        return LND.WalletBalance(lndConf);
    }

    public long GetRequiredReserve(uint additionalChannelsNum)
    {
        return LND.RequiredReserve(lndConf, additionalChannelsNum).RequiredReserve;
    }

    public long GetRequestedReserveAmount()
    {
        using var walletContext = walletContextFactory.Create();
        return (from r in walletContext.Reserves select r.Satoshis).Sum();
    }

    public List<Reserve> GetRequestedReserves()
    {
        using var walletContext = walletContextFactory.Create();
        return (from r in walletContext.Reserves select r).ToList();
    }

    public List<Payout> GetPendingPayouts()
    {
        using var walletContext = walletContextFactory.Create();
        return (from p in walletContext.Payouts where p.State == PayoutState.Open select p).ToList();
    }

    public void CompleteSendingPayouts()
    {
        using var TL = TRACE.Log();
        try
        {
            using var walletContext = walletContextFactory.Create();

            var sendingPayouts = (from p in walletContext.Payouts where p.State == PayoutState.Sending select KeyValuePair.Create(p.PayoutId.ToString(), p)).ToDictionary();
            if (sendingPayouts.Count == 0)
                return;

            var sendingPayoutsSet = new HashSet<string>(sendingPayouts.Keys);
            var transactions = LND.GetTransactions(lndConf).Transactions;
            foreach (var transation in transactions)
            {
                if (sendingPayouts.Keys.Contains(transation.Label))
                {
                    TL.Info("Completing payout");
                    TL.Iteration(transation);
                    TL.Iteration(sendingPayouts[transation.Label]);
                    MarkPayoutAsSent(sendingPayouts[transation.Label].PayoutId, transation.TxHash);
                    sendingPayoutsSet.Remove(transation.Label);
                }
            }
            foreach(var payout in sendingPayoutsSet)
            {
                TL.Info("Failed payout");
                TL.Iteration(sendingPayouts[payout]);
                MarkPayoutAsFailure(sendingPayouts[payout].PayoutId, "");
            }
        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }

    public AsyncServerStreamingCall<OpenStatusUpdate> OpenChannel(string nodePubKey, long fundingSatoshis)
    {
        return LND.OpenChannel(lndConf, nodePubKey, fundingSatoshis);
    }

    /*
    public BatchOpenChannelResponse BatchOpenChannel(List<(string, long)> amountsPerNode)
    {
        return LND.BatchOpenChannel(lndConf, amountsPerNode);
    }
    public string OpenChannelSync(string nodePubKey, long fundingSatoshis)
    {
        var channelpoint = LND.OpenChannelSync(lndConf, nodePubKey, fundingSatoshis);
        string channelTx;
        if (channelpoint.HasFundingTxidBytes)
            channelTx = channelpoint.FundingTxidBytes.ToByteArray().Reverse().ToArray().AsHex();
        else
            channelTx = channelpoint.FundingTxidStr;

        var chanpoint = channelTx + ":" + channelpoint.OutputIndex;

        return chanpoint;
    }
    */


    public ListChannelsResponse ListChannels(bool openOnly)
    {
        return LND.ListChannels(lndConf, openOnly);
    }

    public ClosedChannelsResponse ClosedChannels()
    {
        return LND.ClosedChannels(lndConf);
    }

    public AsyncServerStreamingCall<CloseStatusUpdate> CloseChannel(string chanpoint, ulong maxFeePerVByte)
    {
        return LND.CloseChannel(lndConf, chanpoint, maxFeePerVByte);
    }

    public long EstimateChannelClosingFee()
    {
        var chans = LND.ClosedChannels(lndConf);
        if (chans.Channels.Count == 0)
        {
            return (long)(EstimateFeeSatoshiPerByte() * 1024);
        }
        else
        {
            long sum = 0;
            long count = 0;
            foreach (var chan in chans.Channels)
            {
                try
                {
                    sum += LND.GetTransaction(lndConf, chan.ClosingTxHash).TotalFees;
                    count++;
                }
                catch
                {
                    //pass
                }
            }
            return sum / count;
        }
    }

    public decimal EstimateFeeSatoshiPerByte()
    {
        return BitcoinNode.EstimateFeeSatoshiPerByte(6);
    }


    public void GoForCancellingInternalInvoices()
    {
        using var TL = TRACE.Log();
        try
        {
            var allInvs = new Dictionary<string, InvoiceRecord>(
                       (from inv in LND.ListInvoices(lndConf).Invoices
                        select KeyValuePair.Create(inv.RHash.ToArray().AsHex(),
                        LNDAccountManager.ParseInvoiceToInvoiceRecord(invoice: inv, isHodl: false))));

            {
                using var walletContext = walletContextFactory.Create();

                var internalPayments = (from pay in walletContext.InternalPayments
                                        select pay).ToList();
                foreach (var pay in internalPayments)
                {
                    try
                    {
                        if (allInvs.ContainsKey(pay.PaymentHash))
                        {
                            var inv = allInvs[pay.PaymentHash];
                            if (pay.Status == InternalPaymentStatus.InFlight)
                            {
                                if (inv.ExpiryTime < DateTime.UtcNow)
                                {
                                    TL.Iteration(pay);

                                    walletContext
                                        .DELETE_IF_EXISTS(from p in walletContext.InternalPayments
                                                          where p.PaymentHash == pay.PaymentHash
                                                          select p)
                                        .INSERT(new FailedPayment
                                        {
                                            Id = Guid.NewGuid(),
                                            DateTime = DateTime.UtcNow,
                                            PaymentHash = pay.PaymentHash,
                                            PublicKey = pay.PublicKey,
                                            FailureReason = PaymentFailureReason.Canceled,
                                        })
                                        .SAVE();

                                    this.CancelSingleInvoiceTracking(pay.PaymentHash);
                                    this.FireOnPaymentStatusChanged(pay.PublicKey, pay.PaymentHash, PaymentStatus.Failed, PaymentFailureReason.Canceled);

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceEx.TraceException(ex);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            TL.Exception(ex);
            throw;
        }
    }
}

