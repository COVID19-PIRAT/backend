## Note: This is a multi-stage build to reduce final image size and provide a clean separation between building and serving the app.

## build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder

WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet build --configuration Release --no-restore
RUN dotnet publish -c Release --no-restore -o Pirat

## runtime container
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runner
WORKDIR /app

COPY --from=builder /app/Pirat ./
ENTRYPOINT [ "./Pirat" ]

