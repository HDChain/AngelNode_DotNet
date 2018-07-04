docker rm $(docker ps -a -q -f ancestor=pn:ipfs)
docker rmi pn:ipfs
docker build -t pn:ipfs .