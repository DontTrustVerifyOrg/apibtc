# Requiements for local runner

Minimal hardware requirements:

- 2 CPU
- 4 GB RAM
- x86_64 platform

Software:

- docker
- docker compose

# Environment setup on local machine

1. Build services

```bash
docker compose build
```

2. Run stack (do not worry about the `apibtc-api-1` container restarting, it is expected. It will start up successfully after finishing the step 4.)

```bash
docker compose up -d
```

3. Wait for the stack to be ready, you can look at the logs to see the progress. You can check for the message in logs `Make sure the lightning node is initialized ...`

```bash
docker compose logs -f
```

4. Create lightning wallet, provided password has to be the same as in `conf.local/lnd/password.txt`, then generate or provide a mnemonic according to the instructions

```bash
docker exec -it apibtc-lnd-1 lncli -n regtest --lnddir=/app/lnd --rpcserver=localhost:11009 create
```

![caution] Make sure to save the mnemonic, it will be needed to restore the wallet later. 



# Stop services

```bash
docker compose down
```

# Synchronize the Lightning Node

```bash
docker exec -it apibtc-btc-1 bitcoin-cli -datadir=/app/data -generate 10
```

# Bitcon node management

```bash
docker exec -it apibtc-btc-1 bitcoin-cli -datadir=/app/data <command>
```

#### List of commands

```bash
docker exec -it apibtc-btc-1 bitcoin-cli -datadir=/app/data help
```

#### More efficient way to interact with bitcoin management

```bash
alias bcli="docker exec -it apibtc-btc-1 bitcoin-cli -datadir=/app/data"
bcli <command>
```


# Lightning node management

```bash
docker exec -it apibtc-lnd-1 lncli -n regtest --lnddir=/app/data --rpcserver=localhost:11009 <command>
```

#### List of commands

```bash
docker exec -it apibtc-lnd-1 lncli -n regtest --lnddir=/app/data --rpcserver=localhost:11009 help
```

#### More efficient way to interact with lightning node management

```bash
alias lcli="docker exec -it apibtc-lnd-1 lncli -n regtest --lnddir=/app/data --rpcserver=localhost:11009"
lcli <command>
```

# Mnemonic, private key, public key generator

```bash
docker run --rm -it awazcognitum/keypairgen
```
