![.NET Core](https://github.com/COVID19-PIRAT/backend/workflows/.NET%20Core/badge.svg)

# Backend service

# Requirements

.Net Core Sdk 3.1

See https://dotnet.microsoft.com/download/dotnet-core/3.1 to get the SDK

# Developing

Highly recommanded: Visual Studio (2019) 

Recommanded: ReSharper for Visual Studio

Use Feature Branches for the development and open a pull request.

Code quality: To improve the quality we use the analyzer tools FxCop and Puma. If you make a pull request try to fix all warnings. To enable both anaylzers in order to check your code locally you might have to change settings in your IDE. For Visual Studio check that full solution analysis is allowed: Optionen/Options -> Text-Editor/Text Editor -> C# -> Erweitert/Advanced -> Bereich für Hintegrundanalyse/Background analysis scope -> Gesamte Projektmappe/Entire solution must be ticked.

# Executing Tests

Currently we have three Test projects: Pirat.Tests, Pirat.DatabaseTests, Pirat.IntegrationTests

If you want to execute the tests locally make sure you have a fresh build of Pirat and the Test projects. You can build them in your IDE or via console using the command "dotnet build" in the main directory.

Executing Pirat.Tests: This project contains Unit Tests. Run them in your IDE or via console using the command "dotnet test Pirat.Tests/".

Executing Pirat.DatabaseTests: This project contains Tests that need access to a database. Before starting the including tests you need first to start the postgres container. Do this by using the command "(sudo) docker-compose up" in your console. Now a local postgres container runs on port 5432 and you can run the tests in your IDE or via the console with the command "dotnet test Pirat.DatabaseTests/".

Executing Pirat.IntegrationTests: Documentation will come - no Integration Tests to far. 


# Running the project

Set the following environment variables:

* PIRAT_GOOGLE_API_KEY
* PIRAT_CONNECTION
* PIRAT_HOST
* PIRAT_SENDER_MAIL_ADDRESS
* PIRAT_SENDER_MAIL_USERNAME
* PIRAT_SENDER_MAIL_PASSWORD
* PIRAT_ADMIN_KEY
* PIRAT_CONFIG_DIR (optional, default: current directory)


Use dotnet publish on the sln File

Example: dotnet publish Pirat.sln (this builds the executable Debug version)
Example: dotnet publish -c Release Pirat.sln (this builds the executable Release version)

If you want to specify the runtime platform use -r. To have a self-contained executable use --self-contained true

Examples :

dotnet publish -c Release -r linux-x64 --self-contained true Pirat.sln

dotnet publish -c Release -r osx-x64 --self-contained true Pirat.sln

dotnet publish -c Release -r win-x64 --self-contained true Pirat.sln

Running the executuable, the backend will listen on localhost:5000 for http and on localhost:5001 for https






