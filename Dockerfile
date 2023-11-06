FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app

# Copy everything
COPY . .

# Set the working directory to the API project folder
WORKDIR /app/webapi

# Install Entity Framework Core tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Restore, build, and publish
RUN dotnet restore "webapi.csproj"
RUN dotnet build "webapi.csproj" -c Release --no-restore
RUN dotnet publish "webapi.csproj" -c Release --no-restore -o out

# Expose the port the app runs on
EXPOSE 7068

RUN chmod +x api-entrypoint.sh

# Run the app
ENTRYPOINT ["./api-entrypoint.sh"]
