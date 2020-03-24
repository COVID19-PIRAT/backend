# Backend service

# Requirements

.Net Core Sdk 3.1

See https://dotnet.microsoft.com/download/dotnet-core/3.1 to get the SDK

# Developing

Highly recommanded: Visual Studio 2019 with ReSharper ( hopefully you are a student for the licence ;) )

# Running the project

Set the following environment variables:

* PIRAT_GOOGLE_API_KEY
* PIRAT_CONNECTION
* PIRAT_HOST
* PIRAT_SENDER_MAIL_ADDRESS
* PIRAT_SENDER_MAIL_USERNAME
* PIRAT_SENDER_MAIL_PASSWORD


Use dotnet publish on the sln File

Example: dotnet publish Pirat.sln (this builds the executable Debug version)
Example: dotnet publish -c Release Pirat.sln (this builds the executable Release version)

If you want to specify the runtime platform use -r. To have a self-contained executable use --self-contained true

Examples :

dotnet publish -c Release -r linux-x64 --self-contained true Pirat.sln

dotnet publish -c Release -r osx-x64 --self-contained true Pirat.sln

dotnet publish -c Release -r win-x64 --self-contained true Pirat.sln

Running the executuable, the backend will listen on localhost:5000 for http and on localhost:5001 for https






