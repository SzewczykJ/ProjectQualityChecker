FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic  as build
EXPOSE 5000/tcp
SHELL ["/bin/bash", "-c"]

RUN apt-get update && apt-get install -y openjdk-8-jdk-headless
RUN dotnet tool install --global dotnet-sonarscanner --version 4.8.0

ENV SONAR_SCANNER_VERSION 4.5.0.2216
RUN apt-get install -y wget unzip &&\
    wget https://binaries.sonarsource.com/Distribution/sonar-scanner-cli/sonar-scanner-cli-${SONAR_SCANNER_VERSION}-linux.zip &&\
    unzip sonar-scanner-cli-${SONAR_SCANNER_VERSION}-linux &&\
    apt-get remove -y wget unzip

RUN apt-get install -y maven nodejs
ENV JAVA_HOME="/usr/lib/jvm/java-8-openjdk-amd64"
ENV PATH="$PATH:$JAVA_HOME/jre/bin/java:/root/.dotnet/tools:/sonar-scanner-${SONAR_SCANNER_VERSION}-linux/bin:/opt/gradle/gradle-5.0/bin"

ENV ASPNETCORE_URLS="http://*:5000"
ENV ASPNETCORE_ENVIRONMENT="Development"
WORKDIR /app
COPY ./ProjectQualityChecker/ .
WORKDIR /app
RUN dotnet restore "ProjectQualityChecker.csproj"

RUN dotnet publish "ProjectQualityChecker.csproj" -c Release -o /app/publish --no-restore

WORKDIR /app/publish
ENTRYPOINT ["dotnet", "ProjectQualityChecker.dll"]
