FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RideGoo.TelegramBot/RideGoo.TelegramBot.csproj RideGoo.TelegramBot/
COPY RideGoo.BLL/RideGoo.BLL.csproj RideGoo.BLL/
COPY RideGoo.DAL/RideGoo.DAL.csproj RideGoo.DAL/
COPY RideGoo.Domain/RideGoo.Domain.csproj RideGoo.Domain/
COPY RideGoo.Shared/RideGoo.Shared.csproj RideGoo.Shared/

RUN dotnet restore RideGoo.TelegramBot/RideGoo.TelegramBot.csproj

COPY RideGoo.TelegramBot/ RideGoo.TelegramBot/
COPY RideGoo.BLL/ RideGoo.BLL/
COPY RideGoo.DAL/ RideGoo.DAL/
COPY RideGoo.Domain/ RideGoo.Domain/
COPY RideGoo.Shared/ RideGoo.Shared/

RUN rm -rf RideGoo.TelegramBot/obj RideGoo.TelegramBot/bin \
    RideGoo.BLL/obj RideGoo.BLL/bin \
    RideGoo.DAL/obj RideGoo.DAL/bin \
    RideGoo.Domain/obj RideGoo.Domain/bin \
    RideGoo.Shared/obj RideGoo.Shared/bin

WORKDIR /src/RideGoo.TelegramBot
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RideGoo.TelegramBot.dll"]