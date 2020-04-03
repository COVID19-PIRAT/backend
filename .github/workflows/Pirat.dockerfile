FROM microsoft/dotnet:3.1-sdk as build
WORKDIR /app
COPY ../../Pirat /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
RUN ["dotnet", "publish -c Release --self-conained true"]
ENTRYPOINT [./Pirat]
#RUN chmod +x ./entrypoint.sh
#CMD /bin/bash ./entrypoint.sh