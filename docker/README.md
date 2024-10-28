# Environment setup on local machine


Run bitcoin node

```bash
docker compose up -d bitcoin
```

Run lightning node

```bash
docker run -d --name lightning_node --rm -v $(pwd)/data/lnd:/app_data:Z -v $(pwd)/conf/lnd.conf:/app_data/lnd.conf:ro lightninglabs/lnd:v0.18.3-beta lnd --lnddir=/app_data
```

Create lightning wallet

```bash
docker exec -it lightning_node lncli  -n regtest --lnddir=/app_data  --rpcserver=localhost:11009 create
```

Provide password from `conf/lnd/password.txt` when asked.

Stop lightning node

```bash
docker stop lightning_node
```

Start lightning node from compose

```bash
docker compose up -d lightning_node
```


Create btc test wallet

```bash
docker exec -it bitcoin bitcoin-cli -datadir=/app_data createwallet "testwallet"
```


Mine 10 blocks to synchronize the Lightning Node

```bash
docker exec -it bitcoin bitcoin-cli -datadir=/app_data -generate 10
```


Run rest of the services

```bash
docker compose up -d
```
