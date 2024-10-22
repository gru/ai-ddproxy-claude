. .\get-dadata-keys.ps1

$keys = Get-DaDataKeys

$url = "http://localhost:5000/suggestions/api/4_1/rs/suggest/address"

$headers = @{
    "Content-Type" = "application/json"
}

$body = @{
    query = "Москва, Чаянова"
    count = 5  # Количество возвращаемых результатов
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body

# Вывод результатов
foreach ($suggestion in $response.suggestions) {
    Write-Host "Адрес: $($suggestion.value)"
    Write-Host "Полный адрес: $($suggestion.unrestricted_value)"
    Write-Host "Координаты: Lat $($suggestion.data.geo_lat), Lon $($suggestion.data.geo_lon)"
    Write-Host "-----"
}

# Вывод полного ответа в JSON для дополнительной информации
$response | ConvertTo-Json -Depth 5