import { getPublicKey, createAuthToken } from './schnorr';

let pinVerified = false; // tracks whether PIN was verified

async function hashPIN(pin) {
  const enc = new TextEncoder();
  const hashBuffer = await crypto.subtle.digest('SHA-256', enc.encode(pin));
  return Array.from(new Uint8Array(hashBuffer))
    .map(b => b.toString(16).padStart(2, '0')).join('');
}

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
  (async () => {
    try {
        console.log("Received request B:", request);
      switch (request.action) {
        
        case "setup": {
          const { hashedPIN } = await chrome.storage.local.get("hashedPIN");
          if (!hashedPIN) {
            chrome.windows.getCurrent((tabWindow) => {
                const w = 700;
                const h = 600;
 
                chrome.windows.create({
                    url: chrome.runtime.getURL("popup.html"),
                    type: "popup",
                    focused: true,
                    left: tabWindow.left + Math.round((tabWindow.width - w) / 2),
                    top: tabWindow.top + Math.round((tabWindow.height - h) / 2),
                    width: w,
                    height: h,
                }, (subWindow) => {});
            });
          } else if(!pinVerified) {

            chrome.windows.getCurrent((tabWindow) => {
                const w = 400;
                const h = 300;
 
                chrome.windows.create({
                    url: chrome.runtime.getURL("pin.html"),
                    type: "popup",
                    focused: true,
                    left: tabWindow.left + Math.round((tabWindow.width - w) / 2),
                    top: tabWindow.top + Math.round((tabWindow.height - h) / 2),
                    width: w,
                    height: h,
                }, (subWindow) => {});
            });
          }
          sendResponse({ success: true });
          break;
        }

        case "setPIN": {
          const { hashedPIN } = await chrome.storage.local.get("hashedPIN");

          // Only allow setting PIN if it's first-time or PIN was verified
          if (!hashedPIN || pinVerified) {
            const newHashedPIN = await hashPIN(request.pin);
            await chrome.storage.local.set({ hashedPIN: newHashedPIN });
            pinVerified = false; // Reset verification after changing PIN
            sendResponse({ success: true });
          } else {
            sendResponse({ success: false, error: "PIN verification required" });
          }
          break;
        }

        case "verifyPIN": {
          const { hashedPIN } = await chrome.storage.local.get("hashedPIN");
          if (!hashedPIN) {
            sendResponse({ isValid: false, error: "No PIN set" });
            break;
          }
          const hashedInput = await hashPIN(request.pin);
          pinVerified = hashedPIN === hashedInput;
          sendResponse({ isValid: pinVerified });
          break;
        }

        case "saveKey": {
          console.log('Private Key:', request.privateKey);
          if (!pinVerified) {
            sendResponse({ success: false, error: "PIN verification required" });
            break;
          }
          const publicKey = await getPublicKey(request.privateKey);
          console.log('Public Key:', publicKey);
          await chrome.storage.local.set({ privateKey: request.privateKey, publicKey });
          sendResponse({ success: true });
          break;
        }

        case "getPublicKey": {
          if (!pinVerified) {
            sendResponse({ success: false, publicKey: null, error: "PIN verification required" });
            break;
          }
          const { publicKey } = await chrome.storage.local.get("publicKey");
          if (!publicKey) {
            sendResponse({ success: false, publicKey: null, error: "No public key stored" });
          } else {
            sendResponse({ success: true, publicKey });
          }
          break;
        }

        case "createAuthToken": {
          if (!pinVerified) {
            sendResponse({ success: false, error: "PIN verification required" });
            break;
          }
          const { privateKey, publicKey } = await chrome.storage.local.get(["privateKey", "publicKey"]);

          if (!privateKey) {
            sendResponse({ success: false, error: "No key stored" });
          } else if (!publicKey) {
            sendResponse({ success: false, error: "No public key stored" });
          } else {
            const authToken = await createAuthToken(request.token, publicKey, privateKey);
            sendResponse({ success: true, authToken });
          }
          break;
        }

        case "ping": {
            console.log("Ping received");
          sendResponse({ success: true, message: "pong" });
          break;
        }

      }
    } catch (error) {
      console.error("Error:", error);
      sendResponse({ success: false, error: error.message });
    }
  })();

  return true;  // Mandatory for async
});

