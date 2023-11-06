#!/bin/bash

# Path to the migration folder
migrationFolder="./Migrations"

# Check if the migration folder exists and is empty
if [ ! -d "$migrationFolder" ] || [ -z "$(ls -A "$migrationFolder")" ]; then
    echo "No migrations found. Creating initial migration..."
    dotnet ef migrations add Initial
fi

# apply migrations
dotnet ef database update

# run the server
dotnet out/webapi.dll