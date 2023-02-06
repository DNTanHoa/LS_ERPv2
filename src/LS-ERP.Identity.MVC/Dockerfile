#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/LS-ERP.Identity.MVC/LS-ERP.Identity.MVC.csproj", "src/LS-ERP.Identity.MVC/"]
RUN dotnet restore "src/LS-ERP.Identity.MVC/LS-ERP.Identity.MVC.csproj"
COPY . .
WORKDIR "/src/src/LS-ERP.Identity.MVC"
RUN dotnet build "LS-ERP.Identity.MVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LS-ERP.Identity.MVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LS-ERP.Identity.MVC.dll"]