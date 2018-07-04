#!/bin/sh

echo start

export IPFS_PATH=/chain

start() {
	setup_chain_dir
	node_start
	
}

node_start() {
	ipfs daemon
}



setup_chain_dir() {
  if [ ! -d /chain ]; then
    mkdir /chain
    ipfs init -p test
	cp /swarm.key /chain/swarm.key
	ipfs bootstrap rm --all
	ipfs config Addresses.API /ip4/0.0.0.0/tcp/5001
	ipfs config Addresses.Gateway /ip4/0.0.0.0/tcp/8081
	ipfs config --json Addresses.Swarm [\"/ip4/0.0.0.0/tcp/4001\",\"/ip6/::/tcp/4001\"]

	ipfs bootstrap add /ip4/115.159.119.167/tcp/4001/ipfs/QmXK23HP6qV4rdMbkMTjZPR4CfBTxB2jy8nXBGkSupyQNM
	
  else
    echo 'ok'
  fi
}

start