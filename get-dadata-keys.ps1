function Get-DaDataKeys {
    $configPath = Join-Path $PSScriptRoot "Src/AI.DaDataProxy.Host/appsettings.Development.json"
    
    if (Test-Path $configPath) {
        $config = Get-Content $configPath | ConvertFrom-Json
        $apiKey = $config.DaData.ApiKey
        $secret = $config.DaData.Secret
        
        if ($apiKey -and $secret) {
            return @{
                ApiKey = $apiKey
                Secret = $secret
            }
        }
        else {
            Write-Host "Ошибка: Не удалось найти ApiKey или Secret в файле конфигурации."
            exit
        }
    }
    else {
        Write-Host "Ошибка: Файл конфигурации не найден."
        exit
    }
}