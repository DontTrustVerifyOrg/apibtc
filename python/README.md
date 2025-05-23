# APIBTC - Current Status

**Please Note:** Currently, our publicly hosted API endpoint is available **exclusively** on the Bitcoin **`regtest`** network for development and testing purposes.

*   **API Documentation (Swagger):** The `regtest` API documentation is available via Swagger [here](https://regtest.apibtc.org/apibtc/swagger/index.html).
*   **Mainnet Environment:** We do not provide a publicly hosted `mainnet` API endpoint. You can run your own `mainnet` instance using the provided Docker image.
*   **More Information:** For further details about the project, Docker images, and setup instructions, please visit the main [project website](https://apibtc.org/).

**⚠️ Important:** The public API operates on `regtest`. Do **not** send real Bitcoin (BTC) to any addresses generated or used via this public `regtest` endpoint.

```python
from apibtc import Wallet
from mnemonic import Mnemonic
from bip32utils import BIP32Key

# Declare API url
BASE_URL = "API_BASE_URL"

# Create two wallets
# Wallet 1 - Invoice Creator
mnemon1 = Mnemonic('english')
words1 = mnemon1.generate(128)
private_key1 = BIP32Key.fromEntropy(mnemon1.to_seed(words1)).PrivateKey().hex()
wallet1 = Wallet(base_url=BASE_URL, privkey=private_key1)

# Wallet 2 - Invoice Payer
mnemon2 = Mnemonic('english')
words2 = mnemon2.generate(128)
private_key2 = BIP32Key.fromEntropy(mnemon2.to_seed(words2)).PrivateKey().hex()
wallet2 = Wallet(base_url=BASE_URL, privkey=private_key2)

# Payment flow
# Create invoice with wallet1
invoice = wallet1.addinvoice(satoshis=1000, memo="Payment from wallet2", expiry=3600)

# Pay invoice with wallet2
wallet2.sendpayment(paymentrequest=invoice['paymentRequest'], timeout=30, feelimit=100)

# Check balances after payment
print("Wallet1 balance:", wallet1.getbalance())
print("Wallet2 balance:", wallet2.getbalance())
```