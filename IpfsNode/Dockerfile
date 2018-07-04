FROM ipfs/go-ipfs:latest
	
COPY swarm.key /swarm.key

WORKDIR /

COPY start.sh /start.sh
RUN chmod +x /start.sh
ENTRYPOINT [ "/start.sh" ]