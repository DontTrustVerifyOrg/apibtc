# APIBTC - Current Status

**Please Note:** Currently, our publicly hosted API endpoint is available **exclusively** on the Bitcoin **`regtest`** network for development and testing purposes.

*   **API Documentation (Swagger):** The `regtest` API documentation is available via Swagger [here](https://regtest.apibtc.org/apibtc/swagger/index.html).
*   **Mainnet Environment:** We do not provide a publicly hosted `mainnet` API endpoint. You can run your own `mainnet` instance using the provided Docker image.
*   **More Information:** For further details about the project, Docker images, and setup instructions, please visit the main [project website](https://apibtc.org/).

**⚠️ Important:** The public API operates on `regtest`. Do **not** send real Bitcoin (BTC) to any addresses generated or used via this public `regtest` endpoint.

```C#
using System;
using NBitcoin;
using ApiBtc.Client;
class Program
{
    static void Main()
    {
        // Declare API url
        const string BASE_URL = "API_BASE_URL";
        
        // Create two wallets
        // Wallet 1 - Invoice Creator
        Mnemonic mnemonic1 = new Mnemonic(Wordlist.English, WordCount.Twelve);
        ExtKey hdRoot1 = mnemonic1.DeriveExtKey();
        string privateKey1 = hdRoot1.PrivateKey.ToHex();
        var wallet1 = new Wallet(BASE_URL, privateKey1);
        
        // Wallet 2 - Invoice Payer
        Mnemonic mnemonic2 = new Mnemonic(Wordlist.English, WordCount.Twelve);
        ExtKey hdRoot2 = mnemonic2.DeriveExtKey();
        string privateKey2 = hdRoot2.PrivateKey.ToHex();
        var wallet2 = new Wallet(BASE_URL, privateKey2);
        
        // Payment flow
        // Create invoice with wallet1
        var invoice = await wallet1.AddInvoice(1000, "Payment from wallet2", 3600);
        
        // Pay invoice with wallet2
        await wallet2.SendPayment(invoice.PaymentRequest, 30, 100);
        
        // Check balances after payment
        Console.WriteLine($"Wallet1 balance: {await wallet1.GetBalance()}");
        Console.WriteLine($"Wallet2 balance: {await wallet2.GetBalance()}");
    }
}
```