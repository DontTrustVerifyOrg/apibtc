//console.log( 'Content script loaded');

window.addEventListener('message', (event) => {
 // console.log('Received message:', event.data);

  if (event.data.type === 'APIBTC_EXTENSION_PING') {
      window.postMessage({ type: 'APIBTC_EXTENSION_PONG'}, event.origin);
/*    chrome.runtime.sendMessage({ action: 'setup' }, (response) => {
      window.postMessage({ type: 'APIBTC_EXTENSION_PONG'}, event.origin);
    });*/
  }

  if (event.data.type === 'APIBTC_GET_PUBLIC_KEY') {
    chrome.runtime.sendMessage({ action: 'getPublicKey' }, (response) => {
      if (response.success) {
        window.postMessage({ type: 'APIBTC_PUBLIC_KEY', publicKey: response.publicKey }, event.origin);
      }
    });
  }

  if (event.data.type === 'APIBTC_REQUEST_AUTHTOKEN') {
    chrome.runtime.sendMessage({ action: 'createAuthToken', token: event.data.token }, (response) => {
      if (response.success) {
        window.postMessage({ type: 'APIBTC_AUTH_TOKEN', authToken: response.authToken }, event.origin);
      }
    });
  }
});


window.postMessage({ type: "APIBTC_READY" }, "*");