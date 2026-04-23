$apiUrl = "http://localhost:5166"
$tracking = "LWMS-638515000000000000"

# 1. Login
$shipper = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Body (@{ phone="0908887776"; password="123456" } | ConvertTo-Json) -ContentType "application/json"
$token = $shipper.token

Write-Host "🚀 SPAMMING DOUBLE SUCCESS..." -ForegroundColor Cyan

$headers = @{ "Authorization" = "Bearer $token"; "Content-Type" = "application/json" }
$body = @{ trackingCode = $tracking; codAmount = 50000 } | ConvertTo-Json

# Gọi 2 request gần như đồng thời
$task1 = [System.Threading.Tasks.Task]::Run({
    try { return Invoke-WebRequest -Uri "$apiUrl/delivery-success" -Method POST -Headers $headers -Body $body -ErrorAction Ignore } catch { return $_.Exception.Response }
})
$task2 = [System.Threading.Tasks.Task]::Run({
    try { return Invoke-WebRequest -Uri "$apiUrl/delivery-success" -Method POST -Headers $headers -Body $body -ErrorAction Ignore } catch { return $_.Exception.Response }
})

[System.Threading.Tasks.Task]::WaitAll($task1, $task2)

Write-Host "Result 1: $($task1.Result.StatusCode)"
Write-Host "Result 2: $($task2.Result.StatusCode)"

if ($task1.Result.StatusCode -eq 200 -and $task2.Result.StatusCode -ne 200) {
    Write-Host "🏆 ATTACK BLOCKED SUCCESSFULLY! Idempotency Guaranteed." -ForegroundColor Green
} else {
    Write-Host "⚠️ Warning: Both requests returned $($task1.Result.StatusCode). Check if data is consistent." -ForegroundColor Yellow
}
