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

Run bitcoin node

```bash
docker compose up -d
```

Run lightning node

    Create lightning wallet, afterwards provide password from `conf.local/lnd/password.txt` when asked, then generate or provide a mnemonic according to the instructions.

- On linux:

```bash
docker exec -it apibtc-aio-1 bash -c "lncli -n regtest --lnddir=/app/lnd --rpcserver=localhost:11009 create && pkill lnd"
```

- On Windows:

    Make sure to adjust the paths according to your local setup.


[Regtest only] Mine 10 blocks to synchronize the Lightning Node

```bash
docker exec -it api_btc-btc-1 bitcoin-cli -datadir=/app/btc -generate 10
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
