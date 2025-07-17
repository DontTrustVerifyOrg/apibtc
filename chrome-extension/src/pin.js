// Convenience function
const $ = (id) => document.getElementById(id);

// Initial UI setup on load
document.addEventListener('DOMContentLoaded', () => {
  chrome.storage.local.get('hashedPIN', ({hashedPIN}) => {
    if (hashedPIN) localStorage.setItem('pinSet', 'true');
    $('pin').focus();
  });
});

$('pin').addEventListener('keydown', function(e) {
   if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        $('verifyPin').onclick();
   }
});


// Verify PIN
$('verifyPin').onclick = () => {
  const pin = $('pin').value.trim();
  if (!pin) return alert('Enter PIN to verify.');

  chrome.runtime.sendMessage({ action: 'verifyPIN', pin }, res => {
    pinVerified = res.isValid;
    if(!pinVerified) {
      alert(`PIN verification failed`);
    }
    chrome.runtime.sendMessage({ action: 'getPublicKey' }, res => {
        if (res.success) {
          window.close();
        } else {
          alert(`Error: ${res.error}`);
        }
    });
  });
};

