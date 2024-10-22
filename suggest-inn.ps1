. .\get-dadata-keys.ps1

$keys = Get-DaDataKeys

$url = "https://suggestions.dadata.ru/suggestions/api/4_1/rs/findById/party"

$headers = @{
    "Authorization" = "Token $($keys.ApiKey)"
    "Content-Type" = "application/json"
}

$body = @{
    query = "7707083893"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body

$response | ConvertTo-Json -Depth 5