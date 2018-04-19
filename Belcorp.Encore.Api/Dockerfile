FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 8022

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY Belcorp.Encore.Api/Belcorp.Encore.Api.csproj Belcorp.Encore.Api/
COPY Belcorp.Encore.Entities/Belcorp.Encore.Entities.csproj Belcorp.Encore.Entities/
COPY Belcorp.Encore.Data/Belcorp.Encore.Data.csproj Belcorp.Encore.Data/
COPY Belcorp.Encore.Application/Belcorp.Encore.Application.csproj Belcorp.Encore.Application/
COPY Microsoft.EntityFrameworkCore.UnitOfWork/Microsoft.EntityFrameworkCore.UnitOfWork.csproj Microsoft.EntityFrameworkCore.UnitOfWork/
COPY Belcorp.Encore.Repositories/Belcorp.Encore.Repositories.csproj Belcorp.Encore.Repositories/
RUN dotnet restore
COPY . .
WORKDIR /src/Belcorp.Encore.Api
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Belcorp.Encore.Api.dll"]
