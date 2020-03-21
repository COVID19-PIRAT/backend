# Backend service

# Running the project

Use dotnet publish on the sln File

Example: dotnet publish Pirat.sln (this builds the executable Debug version)
Example: dotnet publish -c Release Pirat.sln (this builds the executable Release version)

If you want to specify the runtime platform use -r. To have a self-contained executable use --self-contained true

Examples :

dotnet publish -c Release -r linux-x64 --self-contained true Pirat.sln

dotnet publish -c Release -r osx-x64 --self-contained true Pirat.sln

dotnet publish -c Release -r win-x64 --self-contained true Pirat.sln

Running the executuable, the backend will listen on localhost:5000 for http and on localhost:5001 for https






