FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RideGoo.Api/RideGoo.Api.csproj RideGoo.Api/
COPY RideGoo.BLL/RideGoo.BLL.csproj RideGoo.BLL/
COPY RideGoo.DAL/RideGoo.DAL.csproj RideGoo.DAL/
COPY RideGoo.Domain/RideGoo.Domain.csproj RideGoo.Domain/
COPY RideGoo.Shared/RideGoo.Shared.csproj RideGoo.Shared/

RUN dotnet restore RideGoo.Api/RideGoo.Api.csproj

COPY RideGoo.Api/ RideGoo.Api/
COPY RideGoo.BLL/ RideGoo.BLL/
COPY RideGoo.DAL/ RideGoo.DAL/
COPY RideGoo.Domain/ RideGoo.Domain/
COPY RideGoo.Shared/ RideGoo.Shared/

# Windows'dan (Visual Studio) qolgan eski obj/bin papkalarini har safar tozalaymiz
RUN rm -rf RideGoo.Api/obj RideGoo.Api/bin \
    RideGoo.BLL/obj RideGoo.BLL/bin \
    RideGoo.DAL/obj RideGoo.DAL/bin \
    RideGoo.Domain/obj RideGoo.Domain/bin \
    RideGoo.Shared/obj RideGoo.Shared/bin

WORKDIR /src/RideGoo.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "RideGoo.Api.dll"]