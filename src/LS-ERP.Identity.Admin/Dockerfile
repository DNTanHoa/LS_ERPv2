#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/LS-ERP.Identity.Admin/LS-ERP.Identity.Admin.csproj", "src/LS-ERP.Identity.Admin/"]
RUN dotnet restore "src/LS-ERP.Identity.Admin/LS-ERP.Identity.Admin.csproj"
COPY . .
WORKDIR "/src/src/LS-ERP.Identity.Admin"
RUN dotnet build "LS-ERP.Identity.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LS-ERP.Identity.Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LS-ERP.Identity.Admin.dll"]