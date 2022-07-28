FROM mcr.microsoft.com/dotnet/sdk:3.1-bullseye

RUN dotnet tool install -g dotnet-format
ENV PATH="$PATH:/root/.dotnet/tools"
