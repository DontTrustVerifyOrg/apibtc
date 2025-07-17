// Convenience function
const $ = (id) => document.getElementById(id);

let pinVerified = false;

// Update UI state
function updateUI() {
  $('setPin').hidden = pinVerified || localStorage.getItem('pinSet');
  $('verifyPin').hidden = !$('setPin').hidden;
  ['generateKey', 'importKey']
    .forEach(id => $(id).disabled = !pinVerified);
    $('pin-box').hidden = pinVerified;
    $('login-box').hidden = !pinVerified;
}

// Initial UI setup on load
document.addEventListener('DOMContentLoaded', () => {
  chrome.storage.local.get('hashedPIN', ({hashedPIN}) => {
    if (hashedPIN) localStorage.setItem('pinSet', 'true');
    $('pin').focus();
    updateUI();
  });
});

$('pin').addEventListener('keydown', function(e) {
   if (e.key === 'Enter' && !e.shiftKey) {
         e.preventDefault();
         if(!$('setPin').hidden)
            $('setPin').onclick();
         else
            $('verifyPin').onclick();
   }
});

// Set PIN
$('setPin').onclick = () => {
  const pin = $('pin').value.trim();
  if (!pin) return alert('Enter a PIN.');

  chrome.runtime.sendMessage({ action: 'setPIN', pin }, res => {
    if (res.success) {
      localStorage.setItem('pinSet', 'true');
      alert('PIN successfully set.');
      pinVerified = false;
    } else {
      alert(`Failed: ${res.error}`);
    }
    updateUI();
  });
};

// Verify PIN
$('verifyPin').onclick = () => {
  const pin = $('pin').value.trim();
  if (!pin) return alert('Enter PIN to verify.');

  chrome.runtime.sendMessage({ action: 'verifyPIN', pin }, res => {
    pinVerified = res.isValid;
    if(!pinVerified) {
      alert(`PIN verification failed`);
    }
    updateUI();
    chrome.runtime.sendMessage({ action: 'getPublicKey' }, res => {
        if (res.success) {
        $('publicKey').value = res.publicKey;
        } else {
        alert(`Error: ${res.error}`);
        }
    });
  });
};

// Save (Import) Private Key
$('importKey').onclick = () => {
  const privateKey = $('privateKey').value.trim();
  if (!privateKey) return alert('Enter a private key.');

  chrome.runtime.sendMessage({ action: 'saveKey', privateKey }, res => {
    if (res.success) {
        chrome.runtime.sendMessage({ action: 'getPublicKey' }, res => {
            if (res.success) {
                $('publicKey').value = res.publicKey;
            } else {
                alert(`Error: ${res.error}`);
            }
        });
    } else {
      alert(`Error: ${res.error}`);
    }
  });
};

// Generate a New Key (Example)
$('generateKey').onclick = async () => {
  const { generatePrivateKey } = await import('./schnorr.js');
  const { privateKey, publicKey, mnemonic } = await generatePrivateKey();

  chrome.runtime.sendMessage({ action: 'saveKey', privateKey }, async res => {
    if (res.success) {
        document.getElementById('mnemonic-words').textContent = mnemonic;
        document.getElementById('mnemonic-box').style.display = 'block';
        document.getElementById('private-key').textContent = privateKey;
        $('privateKey').value = privateKey;
        $('importKey').click();
    } else {
      alert(`Error: ${res.error}`);
    }
  });
};

/*
// Get Stored Public Key
$('getPublicKey').onclick = () => {
  chrome.runtime.sendMessage({ action: 'getPublicKey' }, res => {
    if (res.success) {
      $('output').textContent = `Public Key:\n${res.publicKey}`;
    } else {
      alert(`Error: ${res.error}`);
    }
  });
};

// Create Auth Token
$('createAuthToken').onclick = () => {
  const token = $('token').value.trim();
  if (!token) return alert('Enter token ID.');

  chrome.runtime.sendMessage({ action: 'createAuthToken', token }, res => {
    if (res.success) {
      $('output').textContent = `Auth Token:\n${res.authToken}`;
    } else {
      alert(`Error: ${res.error}`);
    }
  });
};
*/