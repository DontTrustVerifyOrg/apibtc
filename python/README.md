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
wallet2.sendpayment(paymentrequest=invoice['payment_request'], timeout=30, feelimit=100)

# Check balances after payment
print("Wallet1 balance:", wallet1.getbalance())
print("Wallet2 balance:", wallet2.getbalance())
```