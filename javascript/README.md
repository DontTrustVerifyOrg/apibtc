# APIBTC - Current Status

**Please Note:** Currently, our publicly hosted API endpoint is available **exclusively** on the Bitcoin **`regtest`** network for development and testing purposes.

*   **API Documentation (Swagger):** The `regtest` API documentation is available via Swagger [here](https://regtest.apibtc.org/apibtc/swagger/index.html).
*   **Mainnet Environment:** We do not provide a publicly hosted `mainnet` API endpoint. You can run your own `mainnet` instance using the provided Docker image.
*   **More Information:** For further details about the project, Docker images, and setup instructions, please visit the main [project website](https://apibtc.org/).

**⚠️ Important:** The public API operates on `regtest`. Do **not** send real Bitcoin (BTC) to any addresses generated or used via this public `regtest` endpoint.

```javascript
const { Wallet } = require('@thehyperlabs/apibtc');
const bip39 = require('bip39');
const hdkey = require('hdkey');

// Declare API url
const BASE_URL = "API_BASE_URL";

// Create two wallets
// Wallet 1 - Invoice Creator
const mnemonic1 = bip39.generateMnemonic(128);
const seed1 = bip39.mnemonicToSeedSync(mnemonic1);
const privateKey1 = hdkey.fromMasterSeed(seed1).privateKey.toString('hex');
const wallet1 = new Wallet(BASE_URL, privateKey1);

// Wallet 2 - Invoice Payer
const mnemonic2 = bip39.generateMnemonic(128);
const seed2 = bip39.mnemonicToSeedSync(mnemonic2);
const privateKey2 = hdkey.fromMasterSeed(seed2).privateKey.toString('hex');
const wallet2 = new Wallet(BASE_URL, privateKey2);

// Payment flow
// Create invoice with wallet1
const invoice = await wallet1.addinvoice(1000, "Payment from wallet2", 3600);
      
// Pay invoice with wallet2
await wallet2.sendpayment(invoice.paymentRequest, 30, 100);
      
// Check balances after payment
console.log("Wallet1 balance:", await wallet1.getbalance());
console.log("Wallet2 balance:", await wallet2.getbalance());
```