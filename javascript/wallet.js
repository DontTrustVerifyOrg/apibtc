const { sign } = require('bip-schnorr');
const { frames } = require('./frames');
const { get } = require('axios');
const crypto = require('crypto');
const elliptic = require('elliptic');

class Wallet {
    constructor(base_url, privkey) {
        const ec = new elliptic.ec('secp256k1');
        this.base_url = base_url;
        this.privkey = privkey;
        this.pubkey = (() => {
            const privKeyBuffer = Buffer.from(privkey, 'hex');
            const keyPair = ec.keyFromPrivate(privKeyBuffer);
            const pubKey = keyPair.getPublic(true, 'hex');
            return pubKey.slice(2);
        })();
    }

    async get_token() {
        const apiUrl = `${this.base_url}/gettoken?pubkey=${this.pubkey}`;
        const response = await get(apiUrl);
        const uuidValue = response.data.value.replace(/-/g, '');
        return Buffer.from(uuidValue, 'hex');
    }

    async create_authtoken() {
        const token = await this.get_token();
        const header = {
            PublicKey: { Value: Buffer.from(this.pubkey, 'hex') },
            Timestamp: { Value: Math.floor(Date.now() / 1000) },
            TokenId: { Value: token }
        };
        const authTok = frames.AuthToken.create({ Header: header });
        const headerSerialized = frames.AuthTokenHeader.encode(authTok.Header).finish();
        const hash = crypto.createHash('sha256').update(headerSerialized).digest();
        authTok.Signature = {
            Value: sign(
                this.privkey,
                hash,
                crypto.randomBytes(32)
            )
        };
        return Buffer.from(frames.AuthToken.encode(authTok).finish()).toString('base64');
    }

    async topupandmine6blocks(bitcoinAddr, satoshis) {
        const apiUrl = `${this.base_url}/topupandmine6blocks`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, bitcoinAddr, satoshis }
        });
        return response.data;
    }

    async sendtoaddress(bitcoinAddr, satoshis) {
        const apiUrl = `${this.base_url}/sendtoaddress`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, bitcoinAddr, satoshis }
        });
        return response.data;
    }

    async generateblocks(blockNum) {
        const apiUrl = `${this.base_url}/generateblocks`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, blocknum: blockNum }
        });
        return response.data;
    }

    async newbitcoinaddress() {
        const apiUrl = `${this.base_url}/newbitcoinaddress`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken }
        });
        return response.data;
    }

    async getbitcoinwalletballance(minConf) {
        const apiUrl = `${this.base_url}/getbitcoinwalletballance`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, minConf }
        });
        return response.data;
    }

    async getlndwalletballance() {
        const apiUrl = `${this.base_url}/getlndwalletballance`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken }
        });
        return response.data;
    }

    async openreserve(satoshis) {
        const apiUrl = `${this.base_url}/openreserve`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, satoshis }
        });
        return response.data;
    }

    async closereserve(reserveId) {
        const apiUrl = `${this.base_url}/closereserve`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, reserveId }
        });
        return response.data;
    }

    async estimatefee(address, satoshis) {
        const apiUrl = `${this.base_url}/estimatefee`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, address, satoshis }
        });
        return response.data;
    }

    async getbalance() {
        const apiUrl = `${this.base_url}/getbalance`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken }
        });
        return response.data;
    }

    async newaddress() {
        const apiUrl = `${this.base_url}/newaddress`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken }
        });
        return response.data;
    }

    async listtransactions() {
        const apiUrl = `${this.base_url}/listtransactions`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken }
        });
        return response.data;
    }

    async registerpayout(satoshis, btcAddress) {
        const apiUrl = `${this.base_url}/registerpayout`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, satoshis, btcAddress }
        });
        return response.data;
    }

    async listpayouts() {
        const apiUrl = `${this.base_url}/listpayouts`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken }
        });
        return response.data;
    }

    async addinvoice(satoshis, memo, expiry) {
        const apiUrl = `${this.base_url}/addinvoice`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, satoshis, memo, expiry }
        });
        return response.data;
    }

    async addhodlinvoice(satoshis, hash, memo, expiry) {
        const apiUrl = `${this.base_url}/addhodlinvoice`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, satoshis, hash, memo, expiry }
        });
        return response.data;
    }

    async decodeinvoice(paymentrequest) {
        const apiUrl = `${this.base_url}/decodeinvoice`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, paymentrequest }
        });
        return response.data;
    }

    async sendpayment(paymentrequest, timeout, feelimit) {
        const apiUrl = `${this.base_url}/sendpayment`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, paymentrequest, timeout, feelimit }
        });
        return response.data;
    }

    async estimateroutefee(paymentrequest, timeout) {
        const apiUrl = `${this.base_url}/estimateroutefee`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, paymentrequest, timeout }
        });
        return response.data;
    }

    async settleinvoice(preimage) {
        const apiUrl = `${this.base_url}/settleinvoice`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, preimage }
        });
        return response.data;
    }

    async cancelinvoice(paymenthash) {
        const apiUrl = `${this.base_url}/cancelinvoice`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, paymenthash }
        });
        return response.data;
    }

    async getinvoice(paymenthash) {
        const apiUrl = `${this.base_url}/getinvoice`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, paymenthash }
        });
        return response.data;
    }

    async listinvoices() {
        const apiUrl = `${this.base_url}/listinvoices`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken }
        });
        return response.data;
    }

    async listpayments() {
        const apiUrl = `${this.base_url}/listpayments`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken }
        });
        return response.data;
    }

    async getpayment(paymenthash) {
        const apiUrl = `${this.base_url}/getpayment`;
        const authToken = await this.create_authtoken();
        const response = await get(apiUrl, {
            params: { authToken, paymenthash }
        });
        return response.data;
    }
}

module.exports = Wallet;