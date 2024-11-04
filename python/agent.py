import os
import json
import requests
from wallet import Wallet
from openai import OpenAI
from termcolor import colored

os.environ["OPENAI_API_KEY"] = ""
os.environ["BASE_URL"] = "http://localhost:7101"

class ChatAgent:
    client = OpenAI()
    GPT_MODEL = "gpt-4o"
    messages = [
        {"role": "system", "content": (
            "You are a Lightning Network wallet management agent. Your task is to assist users in managing their Lightning Network wallet and performing various operations. "
            "The following functionalities are available: "
            "1. List channels: Retrieve a list of Lightning Network channels associated with the node, including their capacity, balance, and state. "
            "2. List closed channels: Get information about previously closed channels, including capacity and settlement details. "
            "3. Estimate fee: Provide a fee estimation for potential payouts, including both Bitcoin transaction fees and Lightning Network channel closing fees. "
            "4. Get balance: Display detailed account balance information, including total and available balance. "
            "5. Create new address: Generate a new Bitcoin address for receiving on-chain payments. "
            "6. List transactions: Display top-up transactions associated with the account. "
            "7. Register payout: Initiate a new payout request to a specified Bitcoin address, which may involve closing channels as necessary. "
            "8. List payouts: Retrieve a list of registered payouts. "
            "9. Add invoice: Create a new Lightning Network invoice for receiving payments, with customizable amount, memo, and expiration time. "
            "10. Add HODL invoice: Create a HODL invoice, enabling conditional payments secured by a preimage. "
            "11. Decode invoice: Decode a Lightning Network invoice to reveal its contents, including amount and recipient. "
            "12. Send payment: Execute a payment on the Lightning Network using a provided payment request. "
            "13. Estimate route fee: Calculate the estimated fee for routing a payment based on the payment request. "
            "14. Settle invoice: Finalize a hold invoice by settling it with the provided preimage. "
            "15. Cancel invoice: Cancel an open invoice that has not yet been paid. "
            "16. Get invoice: Retrieve detailed information about a specific invoice. "
            "17. List invoices: Display a list of all invoices associated with the account. "
            "18. List payments: Show all payments associated with the account that have not failed. "
            "19. Get payment: Fetch detailed information about a specific payment by its payment hash. "
            "You should guide users through these operations, provide relevant information, and handle any errors or issues that may arise. "
            "For any unclear commands or additional information needed, you should ask the user for clarification. Ensure all operations maintain security and accuracy."
        )}
    ]
    tools = [
        {
            "type": "function",
            "function": {
                "name": "listchannels",
                "description": "Retrieves a list of Lightning Network channels associated with the LND node. This endpoint provides detailed information about each channel, including its capacity, balance, and current state. It can be used to monitor the node's connectivity and liquidity within the Lightning Network. The response includes both active and inactive channels, depending on the 'activeOnly' parameter.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "activeOnly": {
                            "type": "boolean",
                            "description": "If true, returns only active channels. If false, returns all channels including inactive ones.",
                        }
                    },
                    "required": ["activeOnly"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "listclosedchannels",
                "description": "Retrieves a list of closed Lightning Network channels associated with the LND node. This endpoint provides detailed information about each closed channel, including its capacity, closing transaction, and settlement details. It can be used to review the history of closed channels and analyze past network connections.",
            }
        },
        {
            "type": "function",
            "function": {
                "name": "estimatefee",
                "description": "Provides a comprehensive fee estimation for a potential payout that includes both the Bitcoin transaction fee and the Lightning Network channel closing fee. This endpoint calculates the estimated on-chain transaction fee based on the provided destination address and amount, as well as the potential fee for closing a Lightning Network channel. It returns the total Bitcoin transaction fee in satoshis, the fee rate in satoshis per virtual byte, and the estimated channel closing fee. This information is crucial for users to understand the full cost implications of their payout, including the possibility of needing to close a channel to fulfill the payout. It helps users make informed decisions about their fund management and transaction strategies. In case of any issues or errors, only administrators will receive detailed error information to maintain system security and privacy. Non-admin users will receive a generic response indicating an estimation problem without specific details.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "address": {
                            "type": "string",
                            "description": "The destination Bitcoin address for the potential payout transaction. This should be a valid Bitcoin address where the funds would be sent if the payout is executed.",
                        },
                        "satoshis": {
                            "type": "integer",
                            "description": "The amount of the potential payout, specified in satoshis (1 satoshi = 0.00000001 BTC). This value must be a positive integer representing the exact amount of the intended payout, which will be used to calculate the appropriate fees.",
                        }
                    },
                    "required": ["address", "satoshis"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "getbalance",
                "description": "This endpoint provides detailed information about the user's account balance. The balance is returned as an AccountBalanceDetails object, which includes the total balance, available balance, and any pending transactions. All amounts are in satoshis (1 BTC = 100,000,000 satoshis).",
            }
        },
        {
            "type": "function",
            "function": {
                "name": "newaddress",
                "description": "Creates and returns a new Bitcoin address associated with the user's Lightning Network account. This address can be used to receive on-chain Bitcoin payments, which will then be credited to the user's Lightning Network balance. This feature enables seamless integration between on-chain and off-chain funds management.",
            }
        },
        {
            "type": "function",
            "function": {
                "name": "listtransactions",
                "description": "List top-up transactions."
            }
        },
        {
            "type": "function",
            "function": {
                "name": "registerpayout",
                "description": "Initiates a new payout request from the user's Lightning Network wallet to a specified Bitcoin address on the blockchain. This operation registers the payout for execution, which may involve closing Lightning channels if necessary to fulfill the requested amount. The method returns a unique identifier (GUID) for tracking the payout request.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "satoshis": {
                            "type": "integer",
                            "description": "The amount to be paid out, specified in satoshis (1 satoshi = 0.00000001 BTC). Must be a positive integer representing the exact payout amount.",
                        },
                        "btcAddress": {
                            "type": "string",
                            "description": "The destination Bitcoin address where the payout will be sent. This should be a valid Bitcoin address on the blockchain.",
                        }
                    },
                    "required": ["satoshis", "btcAddress"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "listpayouts",
                "description": "List registered payouts.",
            }
        },
        {
            "type": "function",
            "function": {
                "name": "addinvoice",
                "description": "Creates and returns a new Lightning Network invoice for receiving payments. This endpoint allows users to generate payment requests with customizable amount, memo, and expiration time. The created invoice can be shared with payers to facilitate Lightning Network transactions.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "satoshis": {
                            "type": "integer",
                            "description": "The amount of the invoice in satoshis (1 BTC = 100,000,000 satoshis). Must be a positive integer representing the exact payment amount requested.",
                        },
                        "memo": {
                            "type": "string",
                            "description": "An optional memo or description for the invoice. This can be used to provide additional context or details about the payment to the payer. The memo will be included in the encoded payment request.",
                        },
                        "expiry": {
                            "type": "integer",
                            "description": "The expiration time for the payment request, in seconds. After this duration, the invoice will no longer be valid for payment.",
                        }
                    },
                    "required": ["satoshis", "memo", "expiry"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "addhodlinvoice",
                "description": "Creates and returns a new Lightning Network HODL invoice. HODL invoices enable escrow-like functionalities by allowing the recipient to claim the payment only when a specific preimage is revealed using the SettleInvoice method. This preimage must be provided by the payer or a trusted third party. This mechanism provides an additional layer of security and enables conditional payments in the Lightning Network, making it suitable for implementing escrow accounts and other advanced payment scenarios.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "satoshis": {
                            "type": "integer",
                            "description": "The amount of the invoice in satoshis (1 BTC = 100,000,000 satoshis). Must be a positive integer representing the exact payment amount requested.",
                        },
                        "hash": {
                            "type": "string",
                            "description": "The SHA-256 hash of the preimage. The payer or a trusted third party must provide the corresponding preimage, which will be used with the SettleInvoice method to claim the payment, enabling escrow-like functionality.",
                        },
                        "memo": {
                            "type": "string",
                            "description": "An optional memo or description for the invoice. This can be used to provide additional context or details about the payment or escrow conditions to the payer. The memo will be included in the encoded payment request.",
                        },
                        "expiry": {
                            "type": "integer",
                            "description": "The expiration time for the payment request, in seconds. After this duration, the HODL invoice will no longer be valid for payment. Consider setting an appropriate duration based on the expected escrow period.",
                        }
                    },
                    "required": ["satoshis", "hash", "memo", "expiry"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "decodeinvoice",
                "description": "This endpoint decodes a Lightning Network invoice (also known as a payment request) and returns detailed information about its contents. It provides insights into the payment amount, recipient, expiry time, and other relevant metadata encoded in the invoice.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "paymentrequest": {
                            "type": "string",
                            "description": "This endpoint decodes a Lightning Network invoice (also known as a payment request) and returns detailed information about its contents. It provides insights into the payment amount, recipient, expiry time, and other relevant metadata encoded in the invoice.",
                        }
                    },
                    "required": ["paymentrequest"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "sendpayment",
                "description": "Send a Lightning Network payment",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "paymentrequest": {
                            "type": "string",
                            "description": "The Lightning Network payment request (invoice) to be paid. This encoded string contains all necessary details for the payment, including amount and recipient.",
                        },
                        "timeout": {
                            "type": "integer",
                            "description": "Maximum time (in seconds) allowed for finding a route for the payment. If a route isn't found within this time, the payment attempt will be aborted.",
                        },
                        "feelimit": {
                            "type": "integer",
                            "description": "Maximum fee (in millisatoshis) that the user is willing to pay for this transaction. If the calculated fee exceeds this limit, the payment will not be sent.",
                        }
                    },
                    "required": ["paymentrequest", "timeout", "feelimit"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "estimateroutefee",
                "description": "This endpoint calculates and returns an estimated fee for routing a Lightning Network payment based on the provided payment request. It helps users anticipate the cost of sending a payment before actually initiating the transaction.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "paymentrequest": {
                            "type": "string",
                            "description": "The Lightning Network payment request (invoice) for which the route fee is to be estimated. This encoded string contains necessary details such as the payment amount and recipient.",
                        },
                        "timeout": {
                            "type": "integer",
                            "description": "Maximum probing time (in seconds) allowed for finding a routing fee for the payment.",
                        }
                    },
                    "required": ["paymentrequest", "timeout"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "settleinvoice",
                "description": "Settles a previously accepted hold invoice using the provided preimage. This action finalizes the payment process for a hold invoice, releasing the funds to the invoice creator.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "preimage": {
                            "type": "string",
                            "description": "The preimage (32-byte hash preimage) that corresponds to the payment hash of the hold invoice to be settled. This preimage serves as proof of payment.",
                        }
                    },
                    "required": ["preimage"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "cancelinvoice",
                "description": "Cancels an open Lightning Network invoice. This endpoint allows users to cancel an invoice that hasn't been paid yet. If the invoice is already canceled, the operation succeeds. However, if the invoice has been settled, the cancellation will fail.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "paymenthash": {
                            "type": "string",
                            "description": "The payment hash of the invoice to be canceled. This unique identifier is used to locate the specific invoice in the system.",
                        }
                    },
                    "required": ["paymenthash"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "getinvoice",
                "description": "Fetches and returns detailed information about a specific Lightning Network invoice identified by its payment hash. This endpoint allows users to access invoice details such as amount, status, and creation date.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "paymenthash": {
                            "type": "string",
                            "description": "The payment hash of the invoice to retrieve. This unique identifier is used to locate the specific invoice in the system.",
                        }
                    },
                    "required": ["paymenthash"],
                },
            }
        },
        {
            "type": "function",
            "function": {
                "name": "listinvoices",
                "description": "This endpoint returns a comprehensive list of all invoices associated with the authenticated user's account. It includes both paid and unpaid invoices, providing a complete overview of the account's invoice history.",
            }
        },
        {
            "type": "function",
            "function": {
                "name": "listpayments",
                "description": "This endpoint provides a list of all payments associated with the authenticated user's account that have not failed. This includes successful payments and those that are still in progress, offering a clear view of the account's payment activity.",
            }
        },
        {
            "type": "function",
            "function": {
                "name": "getpayment",
                "description": "This endpoint fetches and returns detailed information about a specific payment identified by its payment hash. It provides comprehensive data about the payment, including its status, amount, and other relevant details.",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "paymenthash": {
                            "type": "string",
                            "description": "Unique identifier (hash) of the payment to be retrieved. This hash is used to locate the specific payment record in the system.",
                        }
                    },
                    "required": ["paymenthash"],
                },
            }
        }
    ]

    def __init__(self, privkey):
        self.theard = self.client.beta.threads.create()
        self.wallet = Wallet(os.environ["BASE_URL"], privkey)

    def prompt(self, prompt):
        self.messages.append({"role": "user", "content": prompt})

        response = self.client.chat.completions.create(
            model = self.GPT_MODEL,
            messages = self.messages,
            tools = self.tools
        )

        response_message = response.choices[0].message 
        self.messages.append(response_message)
        tool_calls = response_message.tool_calls

        if tool_calls:
            tool_call_id = tool_calls[0].id
            tool_function_name = tool_calls[0].function.name
            tool_query_args = json.loads(tool_calls[0].function.arguments)

            if tool_function_name == 'listchannels':
                function_result = self.wallet.listchannels(
                    activeOnly = tool_query_args['activeOnly']
                )
            elif tool_function_name == 'listclosedchannels':
                function_result = self.wallet.listclosedchannels()
            elif tool_function_name == 'estimatefee':
                function_result = self.wallet.estimatefee(
                    address = tool_query_args['address'], 
                    satoshis = tool_query_args['satoshis']
                )
            elif tool_function_name == 'getbalance':
                function_result = self.wallet.getbalance()
            elif tool_function_name == 'newaddress':
                function_result = self.wallet.newaddress()
            elif tool_function_name == 'listtransactions':
                function_result = self.wallet.listtransactions()
            elif tool_function_name == 'registerpayout':
                function_result = self.wallet.registerpayout(
                    satoshis = tool_query_args['satoshis'], 
                    btcAddress = tool_query_args['btcAddress']
                )
            elif tool_function_name == 'listpayouts':
                function_result = self.wallet.listpayouts()
            elif tool_function_name == 'addinvoice':
                function_result = self.wallet.addinvoice(
                    satoshis = tool_query_args['satoshis'], 
                    memo = tool_query_args['memo'], 
                    expiry = tool_query_args['expiry']
                )
            elif tool_function_name == 'addhodlinvoice':
                function_result = self.wallet.addhodlinvoice(
                    satoshis = tool_query_args['satoshis'],
                    hash = tool_query_args['hash'], 
                    memo = tool_query_args['memo'], 
                    expiry = tool_query_args['expiry']
                )
            elif tool_function_name == 'decodeinvoice':
                function_result = self.wallet.decodeinvoice(
                    paymentrequest = tool_query_args['paymentrequest']
                )
            elif tool_function_name == 'sendpayment':
                function_result = self.wallet.sendpayment(
                    paymentrequest = tool_query_args['paymentrequest'], 
                    timeout = tool_query_args['timeout'], 
                    feelimit = tool_query_args['feelimit']
                )
            elif tool_function_name == 'estimateroutefee':
                function_result = self.wallet.estimateroutefee(
                    paymentrequest = tool_query_args['paymentrequest'], 
                    timeout = tool_query_args['timeout']
                )
            elif tool_function_name == 'settleinvoice':
                function_result = self.wallet.settleinvoice(
                    preimage = tool_query_args['preimage']
                )
            elif tool_function_name == 'cancelinvoice':
                function_result = self.wallet.cancelinvoice(
                    paymenthash = tool_query_args['paymenthash']
                )
            elif tool_function_name == 'getinvoice':
                function_result = self.wallet.getinvoice(
                    paymenthash = tool_query_args['paymenthash']
                )
            elif tool_function_name == 'listinvoices':
                function_result = self.wallet.listinvoices()
            elif tool_function_name == 'listpayments':
                function_result = self.wallet.listpayments()
            elif tool_function_name == 'getpayment':
                function_result = self.wallet.getpayment(
                    paymenthash = tool_query_args['paymenthash']
                )
            else:
                function_result = 'Unknown function.'

            self.messages.append({
                "role": "tool", 
                "tool_call_id": tool_call_id, 
                "name": tool_function_name, 
                "content": str(function_result)
            })
            
            response_with_function_call = self.client.chat.completions.create(
                model = self.GPT_MODEL,
                messages = self.messages
            )
            
            answer = response_with_function_call.choices[0].message.content
        else: 
            answer = response_message.content
                
        self.messages.append({"role": "assistant", "content": answer})
        return answer
    
def main():
    try:
        healthcheck = requests.get(url=f"{os.environ["BASE_URL"]}/health")
        healthcheck.raise_for_status()

        print(colored(f"\nWelcome to the Cryptocurrency Wallet Management Console!\n", "cyan"))
        privkey = input(colored("Enter your private key: ", "cyan"))
        chat_agent = ChatAgent(privkey)

        while True:
            user_input = input(colored("\nPrompt:\n", "magenta"))
            
            if user_input.lower() in ['exit', 'quit']:
                print(colored("Exiting the chat. Goodbye!", "red"))
                break
            
            response = chat_agent.prompt(user_input)
            print(colored("\nAgent:", "green"))
            print(response)
            
    except Exception as e:
        print(colored(f"Could not connect to the server. Please make sure the server is running and accessible.\n{e}", "red"))
    

if __name__ == "__main__":
    main()