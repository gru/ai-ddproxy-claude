#!/bin/bash

# Prompt for variables
read -p "Enter the migration name: " migrationName
projectPath="Src/AI.DaDataProxy.Entities/AI.DaDataProxy.Entities.csproj"
startupProjectPath="Src/AI.DaDataProxy.Migrations/AI.DaDataProxy.Migrations.csproj"
context="AI.DaDataProxy.Entities.DaDataProxyDbContext"

# Create the migration
command="dotnet ef migrations add $migrationName --project \"$projectPath\" --startup-project \"$startupProjectPath\" --context \"$context\""

# Output the command (optional, for verification)
echo "Executing command: $command"

# Execute the command
eval $command

# Check if the command was successful
if [ $? -eq 0 ]; then
    echo "Migration '$migrationName' created successfully."
else
    echo "Failed to create migration '$migrationName'. Please check the error messages above."
fi