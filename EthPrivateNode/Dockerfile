FROM ethereum/client-go:latest

RUN apk update && apk add bash curl
	
COPY genesis.json /genesis.json
COPY password.txt /password.txt
ADD keys /keys

WORKDIR /
COPY start.sh /start.sh
RUN chmod +x /start.sh
ENTRYPOINT [ "/start.sh" ]