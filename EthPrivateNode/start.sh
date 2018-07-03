#!/bin/bash 

RPCPORT=8545
WSPORT=8546
if [[ "${ROOT}" == "" ]] ; then ROOT="." ; fi


start() {
	setup_chain_dir
	node_start
	
	node_detect_ready &
	wait $!

	if ! eth_running ; 
	then
		>&2 echo "Failed to start Ethereum Node, exiting"
		RESULT=1
	else
		echo -e "\e[32mEthereum Node up and running!\e[0m"
		node_wait
		RESULT=0
	fi

	node_cleanup
	exit $RESULT
}

node_start() {
  /usr/local/bin/geth \
    --datadir "${ROOT}/chain" \
    --rpc --rpcapi admin,eth,net,web3,personal,miner,txpool --rpcaddr 0.0.0.0 --rpcport $RPCPORT --rpccorsdomain '*' --rpcvhosts '*' \
  NODE_PID=$!
}

node_cleanup() {
  kill $NODE_PID > /dev/null 2>&1
}

node_wait() {
  wait $NODE_PID
}

trap node_cleanup INT TERM

eth_call() {
  local response
  response=$(curl --silent --show-error localhost:$RPCPORT -H "Content-Type: application/json" -X POST --data "${response}" 2>&1)
  if [[ \
    "${response}" == *'"error":'* || \
    "${response}" == *'Connection refused'* || \
    "${response}" == *'bad method'* \
  ]] ; then
    echo "not ready"
  else
    echo "ready"
  fi
}

eth_running() {
  kill -0 $NODE_PID > /dev/null 2>&1
}

node_detect_ready() {
  while eth_running && [[ $(eth_call $INITIAL_TX_DATA) == "not ready" ]] ; do sleep 1; done
}

setup_chain_template(){
	if [ ! -d "${ROOT}/chain-template" ]; then
		echo "Setting up Genesis with Network ID: ${NETWORK_ID:-12346}"
		sed -i'' -r "s/NETWORK_ID/${NETWORK_ID:-12346}/" ${ROOT}/genesis.json
		/usr/local/bin/geth --datadir "${ROOT}/chain-template" --keystore "${ROOT}/keys" init "${ROOT}/genesis.json"

		echo ${NETWORK_ID:-12346} > "${ROOT}/networkid"
	fi
}

setup_chain_dir() {
  if [ ! -d ${ROOT}/chain ]; then
    setup_chain_template
    echo "${ROOT}/chain not mounted, transactions will be ephemeral"
    mv ${ROOT}/chain-template ${ROOT}/chain
  else
    # Chain dir exists
    if [ -d ${ROOT}/chain/geth/chaindata ]; then
      if [ ! -f "${ROOT}/networkid" ]; then
        echo ${NETWORK_ID:-12346} > "${ROOT}/networkid"
      fi
      echo "${ROOT}/chain-template mounted and has prior blockchain state, restored"
    else
      setup_chain_template
      echo "${ROOT}/chain-template mounted, but uninitialized. Copying chaindata template"
      mv ${ROOT}/chain-template/* ${ROOT}/chain
    fi
  fi
}

start