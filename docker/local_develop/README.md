# Requiements for local runner

Minimal hardware requirements:

- 2 CPU
- 4 GB RAM
- x86_64 platform

Software:

- docker
- docker compose

# Environment setup on local machine

Setup commands are based on Linux, on Windows some of the commandline commands might need to be adjusted. Generally the services itself work the same on both platforms.

Build docker images

```bash
docker compose build
```

Run bitcoin node

```bash
docker compose up -d btc
```

Run lightning node

    Create lightning wallet, afterwards provide password from `conf.local/lnd/password.txt` when asked, then generate or provide a mnemonic according to the instructions.

- On linux:

```bash
docker run -it --rm --name api_btc_lnd -e INIT=1 -v $(pwd)/data/lnd:/app/lnd:Z -v $(pwd)/../conf.local/lnd/lnd.conf:/app/lnd/lnd.conf:ro --network apibtc apibtc-lnd:latest
```

- On Windows:

    Make sure to adjust the paths according to your local setup.

Create btc test wallet

```bash
docker exec -it apibtc-btc-1 bitcoin-cli -datadir=/app/data createwallet "testwallet"
```

[Regtest only] Mine 10 blocks to synchronize the Lightning Node

```bash
docker exec -it apibtc-btc-1 bitcoin-cli -datadir=/app/data -generate 10
```


Start rest of the services from compose

```bash
docker compose up -d
```

# Stop services

```bash
docker compose down
```

# Synchronize the Lightning Node

```bash
docker exec -it bitcoin bitcoin-cli -datadir=/app/data -generate 10
```

# Bitcon node management

```bash
docker exec -it bitcoin bitcoin-cli -datadir=/app/data <command>
```

#### List of commands

```bash
docker exec -it bitcoin bitcoin-cli -datadir=/app/data help
```

#### More efficient way to interact with bitcoin management

```bash
alias bcli="docker exec -it bitcoin bitcoin-cli -datadir=/app/data"
bcli <command>
```


# Lightning node management

```bash
docker exec -it lightning_node lncli -n regtest --lnddir=/app/data --rpcserver=localhost:11009 <command>
```

#### List of commands

```bash
docker exec -it lightning_node lncli -n regtest --lnddir=/app/data --rpcserver=localhost:11009 help
```

#### More efficient way to interact with lightning node management

```bash
alias lcli="docker exec -it lightning_node lncli -n regtest --lnddir=/app/data --rpcserver=localhost:11009"
lcli <command>
```

# Mnemonic, private key, public key generator

```bash
docker run --rm -it awazcognitum/keypairgen
```
