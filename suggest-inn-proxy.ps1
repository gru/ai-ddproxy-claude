. .\get-dadata-keys.ps1

$keys = Get-DaDataKeys

$url = "http://localhost:5000/suggestions/api/4_1/rs/findById/party"

$headers = @{
    "Content-Type" = "application/json"
}

$body = @{
    query = "7707083893"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body

$response | ConvertTo-Json -Depth 5