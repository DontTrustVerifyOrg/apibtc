
import { Buffer } from 'buffer';
import {apibtc} from './frames';
const { sign } = require('bip-schnorr');
const elliptic = require('elliptic');
const bip39 = require('bip39');
const QRCode = require('qrcode');

export function createHash(algorithm) {
   if (algorithm !== 'sha256') {
      throw new Error('Only SHA-256 is supported in this implementation');
   }
   
   let hashBuffer = null;
   let data = null;
   
   return {
      update: function(d) {
         data = d;
         return this;
      },
      digest: async function() {
         // Convert the data to ArrayBuffer if it's a Buffer
         const dataBuffer = data instanceof Buffer ? 
            data.buffer.slice(data.byteOffset, data.byteOffset + data.byteLength) : 
            new TextEncoder().encode(data);
            
         // Use the subtle crypto API to hash the data
         hashBuffer = await crypto.subtle.digest('SHA-256', dataBuffer);
         return Buffer.from(hashBuffer);
      }
   };
};

export async function generatePrivateKey() {
   const mnemonic1 = bip39.generateMnemonic(128);
   const seed1 = bip39.mnemonicToSeedSync(mnemonic1);
   const privkey = (await fromMasterSeed(seed1)).privateKey.toString('hex');
   return { privateKey: privkey, publicKey: await getPublicKey(privkey), mnemonic: mnemonic1 };
}

async function fromMasterSeed(seedBuffer) {
   const MASTER_SECRET = 'Bitcoin seed';
   
   // Import the key for HMAC
   const keyData = new TextEncoder().encode(MASTER_SECRET);
   const key = await crypto.subtle.importKey(
      'raw',
      keyData,
      { name: 'HMAC', hash: { name: 'SHA-512' } },
      false,
      ['sign']
   );
   
   // Convert seed buffer to proper format
   const dataBuffer = seedBuffer instanceof Buffer ? 
      seedBuffer.buffer.slice(seedBuffer.byteOffset, seedBuffer.byteOffset + seedBuffer.byteLength) : 
      seedBuffer;
   
   // Perform the HMAC operation
   const signature = await crypto.subtle.sign('HMAC', key, dataBuffer);
   const I = Buffer.from(signature);
   console.log('I:', I.toString('hex'));
   
   // Split the result
   return {
      chainCode: I.slice(32),
      privateKey: I.slice(0, 32)
   };
}

function randomBytes(size) {
   const bytes = new Uint8Array(size);
   crypto.getRandomValues(bytes);
   return Buffer.from(bytes);
};

export async function getPublicKey(privkey) {
   const ec = new elliptic.ec('secp256k1');
   const privKeyBuffer = Buffer.from(privkey, 'hex');
   const keyPair = ec.keyFromPrivate(privKeyBuffer);
   return keyPair.getPublic(true, 'hex').slice(2);
}

export async function createAuthToken(token,pubkey,privkey) {
   const dt = Math.floor(Date.now() / 1000); // Current timestamp in seconds
   console.log('Current Timestamp:', dt);
   const header = {
      PublicKey: { Value: Buffer.from(pubkey, 'hex') },
      Timestamp: { Value: dt },
      TokenId: { Value: Buffer.from(token, 'hex') }
   };
   const authTok = apibtc.AuthToken.create({ Header: header });
   const headerSerialized = Buffer.from(apibtc.AuthTokenHeader.encode(authTok.Header).finish());
   const hash = await createHash('sha256').update(headerSerialized).digest();
   console.log('Token ID:', token.toString('hex'));
   console.log('Header Serialized:', headerSerialized.toString('hex'));
   console.log('Hash:', hash.toString('hex'));
   const rnd = randomBytes(32);
   console.log('Random Bytes:', rnd.toString('hex'));
   const sgn = sign(privkey,hash,rnd);
   console.log('Signature:', sgn.toString('hex'));
   authTok.Signature = {Value: sgn};
   
   return Buffer.from(apibtc.AuthToken.encode(authTok).finish()).toString('base64');
}