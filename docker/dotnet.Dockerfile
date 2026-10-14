FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine

RUN apk add --no-cache bash

WORKDIR /home/package

CMD ["bash"]
