```javascript
const { Wallet } = require('@thehyperlabs/apibtc');
const bip39 = require('bip39');
const hdkey = require('hdkey');

// Declare API url
const BASE_URL = "API_BASE_URL";

// Generate private key from mnemonic
const mnemonic = bip39.generateMnemonic(128);
const seed = bip39.mnemonicToSeedSync(mnemonic);
const hdNode = hdkey.fromMasterSeed(seed);
const privateKey = hdNode.privateKey.toString('hex');
console.log("Mnemonic:", mnemonic);
console.log("Private Key:", privateKey);

// Create wallet instance
const wallet = new Wallet(BASE_URL, privateKey);

// Check wallet balance
wallet.getBalance();

// Create new invoice
wallet.addInvoice(1000, "INVOICE_MEMO", 3600);

// Pay invoice
wallet.sendPayment("PAYMENT_REQUEST", 3600, 1000);
```