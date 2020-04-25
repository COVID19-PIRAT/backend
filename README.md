![.NET Core](https://github.com/COVID19-PIRAT/backend/workflows/.NET%20Core/badge.svg)

# PIRAT Backend

This repository contains the backend service for https://pirat-tool.com.


## Setup

#### Requirements

.Net Core Sdk 3.1

See https://dotnet.microsoft.com/download/dotnet-core/3.1 to get the SDK

#### Compile

Use dotnet publish on the sln File

```
Example: dotnet publish Pirat.sln (this builds the executable Debug version)
Example: dotnet publish -c Release Pirat.sln (this builds the executable Release version)
```

#### Environment Variables

The following environment variables are available, most of them are required.

**General**

* ASPNETCORE_URLS - Which address and port the server should listen to.
* ASPNETCORE_ENVIRONMENT - Set it to `development` to enable swagger.
* PIRAT_HOST - Host address of the frontend application: During development, it should be `http://localhost:4200`.
* PIRAT_ADMIN_KEY - An arbitrary value: It is used as the password for a small admin area.
* PIRAT_CONFIG_DIR (optional) - The path to the directory with the configuration files which are in `Pirat/Configuration`.
* PIRAT_CONNECTION - A NpgsqlConnection connection string.
* PIRAT_SWAGGER_PREFIX_PATH - The base href for swagger: It can be set to `/` in most cases.


**Email**

* PIRAT_SENDER_MAIL_ADDRESS
* PIRAT_SENDER_MAIL_USERNAME
* PIRAT_SENDER_MAIL_PASSWORD
* PIRAT_SENDER_MAIL_SMTP_HOST
* PIRAT_INTERNAL_RECEIVER_MAIL - An internal email address: A copy of every outgoing email will be sent to this address.

**API Keys**

* PIRAT_GOOGLE_API_KEY
* PIRAT_GOOGLE_RECAPTCHA_SECRET


Example (for Bash):

```
ASPNETCORE_URLS=http://127.0.0.1:5000
ASPNETCORE_ENVIRONMENT=development
PIRAT_ADMIN_KEY=i_am_admin
PIRAT_HOST=https://pirat-tool.com
PIRAT_CONNECTION="Server=localhost;Port=5432;Database=pirat;User ID=pirat;Password=secret_db_password"

PIRAT_SENDER_MAIL_ADDRESS=mail@pirat-tool.com
PIRAT_SENDER_MAIL_USERNAME=mail@pirat-tool.com
PIRAT_SENDER_MAIL_PASSWORD=secret_password
PIRAT_SENDER_MAIL_SMTP_HOST=smpt.pirat-tool.com
PIRAT_INTERNAL_RECEIVER_MAIL=mail@pirat-tool.com

PIRAT_GOOGLE_API_KEY=KKKKEEEEYYYYY
PIRAT_GOOGLE_RECAPTCHA_SECRET=SSSSEEECCCRRREEEEEETTTTTTT
```


#### Run

If you want to specify the runtime platform use -r. To have a self-contained executable use --self-contained true

Examples :

```
dotnet publish -c Release -r linux-x64 --self-contained true Pirat.sln
dotnet publish -c Release -r osx-x64 --self-contained true Pirat.sln
dotnet publish -c Release -r win-x64 --self-contained true Pirat.sln
```

The backend will listen on localhost:5000.


## Contributing

Use Feature Branches for the development and open a pull request.

Code quality: To improve the quality we use the analyzer tools FxCop and Puma. If you make a pull request try to fix all warnings. To enable both anaylzers in order to check your code locally you might have to change settings in your IDE. For Visual Studio check that full solution analysis is allowed: Optionen/Options -> Text-Editor/Text Editor -> C# -> Erweitert/Advanced -> Bereich für Hintegrundanalyse/Background analysis scope -> Gesamte Projektmappe/Entire solution must be ticked.

#### Executing Tests

Currently we have three Test projects: Pirat.Tests, Pirat.DatabaseTests, Pirat.IntegrationTests

If you want to execute the tests locally make sure you have a fresh build of Pirat and the Test projects. You can build them in your IDE or via console using the command "dotnet build" in the main directory.

Executing Pirat.Tests: This project contains Unit Tests. Run them in your IDE or via console using the command "dotnet test Pirat.Tests/".

Executing Pirat.DatabaseTests: This project contains Tests that need access to a database. Before starting the including tests you need first to start the postgres container. Do this by using the command "(sudo) docker-compose up" in your console. Now a local postgres container runs on port 5432 and you can run the tests in your IDE or via the console with the command "dotnet test Pirat.DatabaseTests/".

Executing Pirat.IntegrationTests: Documentation will come - no Integration Tests to far. 
