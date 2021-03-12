FROM mcr.microsoft.com/dotnet/sdk:5.0-focal  as build
EXPOSE 5000/tcp
SHELL ["/bin/bash", "-c"]

ENV SONAR_SCANNER_VERSION 4.5.0.2216

RUN apt-get update -y &&\
    apt-get install -y openjdk-8-jdk \
    maven \
    nodejs \
    wget \
    unzip \
    apt-transport-https &&\
    wget https://binaries.sonarsource.com/Distribution/sonar-scanner-cli/sonar-scanner-cli-${SONAR_SCANNER_VERSION}-linux.zip &&\
    unzip sonar-scanner-cli-${SONAR_SCANNER_VERSION}-linux &&\
    wget https://packages.microsoft.com/config/ubuntu/20.10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb &&\
    dpkg -i packages-microsoft-prod.deb

RUN  apt-get update && apt-get install -y dotnet-sdk-3.1 &&\
    apt-get remove -y wget unzip

ENV JAVA_HOME="/usr/lib/jvm/java-8-openjdk-amd64"
ENV PATH="$PATH:$JAVA_HOME/jre/bin/java:/root/.dotnet/tools:/sonar-scanner-${SONAR_SCANNER_VERSION}-linux/bin:/opt/gradle/gradle-5.0/bin"

RUN dotnet tool install --global dotnet-sonarscanner --version 4.8.0

ENV JAVA_HOME="/usr/lib/jvm/java-8-openjdk-amd64"
ENV PATH="$PATH:$JAVA_HOME/jre/bin/java:/root/.dotnet/tools:/sonar-scanner-${SONAR_SCANNER_VERSION}-linux/bin:/opt/gradle/gradle-5.0/bin"

ENV ASPNETCORE_URLS="http://*:5000"
ENV ASPNETCORE_ENVIRONMENT="Development"
WORKDIR /app


FROM build as debug
WORKDIR /app
COPY ./ProjectQualityChecker/ProjectQualityChecker.csproj ./
RUN dotnet restore
COPY ./ProjectQualityChecker/ ./
RUN dotnet publish -c Release -o /app/publish --no-restore
WORKDIR /app/publish
ENTRYPOINT ["dotnet", "ProjectQualityChecker.dll"]

FROM build as test
WORKDIR /app
COPY . .
RUN dotnet restore ProjectQualityChecker.sln
RUN dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat="opencover" /p:CoverletOutput="./TestResults/" --logger:trx; exit 0;
ENTRYPOINT [ "cp", "-r", "/app/Tests/TestResults","/app/TestResults/*"]
#ENTRYPOINT ["tail", "-f", "/dev/null"]
