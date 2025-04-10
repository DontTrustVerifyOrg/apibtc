import logging
import urllib.parse

from signalrcore.hub_connection_builder import HubConnectionBuilder
        
class WalletUpdateStream:
    def __init__(self, wallet, hub_connection):
        self.hub_connection = hub_connection
        self.wallet = wallet

    def stop(self):
        """
        Stops the invoice update stream and closes the connection.
        """
        self.hub_connection.stop()

    def stream(self,next,bye):
        self.hub_connection.stream(
            "StreamAsync",
            [self.wallet._create_authtoken().decode('utf-8')]).subscribe({
                "next": next,
                "complete": lambda x: bye(False, x),
                "error": lambda x: bye(True, x)
            })
        

class WalletStreaming:
    def __init__(self, wallet, debug=False):
        self.wallet = wallet
        self.base_url = wallet.base_url
        self.pubkey = wallet.pubkey
        self.debug = debug

    def _start_hub(self, method):
        """
        Initiates a stream of updates related to the state of invoices. This allows users to receive real-time notifications about changes in invoice status, such as payments received or cancellations.

        Returns:
            HubConnection object for managing the connection and receiving updates
        """
        if self.debug:
            handler = logging.StreamHandler()
            handler.setLevel(logging.DEBUG)
        hub_connection = HubConnectionBuilder()\
            .with_url(self.base_url + "/" + method + "?authtoken="+urllib.parse.quote(self.wallet._create_authtoken().decode('utf-8')), options={"verify_ssl": True}) \
            .configure_logging(logging.DEBUG if self.debug else logging.CRITICAL, socket_trace=True, handler=handler if self.debug else None) \
            .with_automatic_reconnect({
                    "type": "interval",
                    "keep_alive_interval": 10,
                    "intervals": [1, 3, 5, 6, 7, 87, 3]
                }).build()
        hub_connection.start()
        hub_connection.on_open(lambda: print("connection opened and handshake received ready to send messages"))
        hub_connection.on_close(lambda: print("connection closed"))
        return hub_connection

    def invoicestateupdates(self):
        """
        Starts a stream of updates related to the state of invoices. This allows users to receive real-time notifications about changes in invoice status, such as payments received or cancellations.

        Returns:
            HubConnection object for managing the connection and receiving updates
        """
        return WalletUpdateStream(self.wallet,self._start_hub("invoicestateupdates"))

    def paymentstatusupdates(self):
        """
        Starts a stream of updates related to the state of invoices. This allows users to receive real-time notifications about changes in invoice status, such as payments received or cancellations.

        Returns:
            HubConnection object for managing the connection and receiving updates
        """
        return WalletUpdateStream(self.wallet,self._start_hub("paymentstatusupdates"))    

    def transactionupdates(self):
        """
        Starts a stream of updates related to the state of invoices. This allows users to receive real-time notifications about changes in invoice status, such as payments received or cancellations.

        Returns:
            HubConnection object for managing the connection and receiving updates
        """
        return WalletUpdateStream(self.wallet,self._start_hub("transactionupdates"))    
    
    def payoutstateupdates(self):
        """
        Starts a stream of updates related to the state of invoices. This allows users to receive real-time notifications about changes in invoice status, such as payments received or cancellations.

        Returns:
            HubConnection object for managing the connection and receiving updates
        """
        return WalletUpdateStream(self.wallet,self._start_hub("payoutstateupdates"))    
