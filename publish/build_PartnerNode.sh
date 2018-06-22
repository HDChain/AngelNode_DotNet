docker rm $(docker ps -a -f ancestor=pn:pn)
docker rmi pn:pn
docker build -t pn:pn .