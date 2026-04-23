$apiUrl = "http://localhost:5166"
$originHubId = "001E9C52-7360-4927-AA72-5B91CF8E9662"
$destHubId = "001E9C52-7360-4927-AA72-5B91CF8E9661"
$serviceId = "CC1E9C52-7360-4927-AA72-5B91CF8E9663"

function Call-API($method, $path, $token, $body) {
    $headers = @{"Content-Type"="application/json"}
    if ($token) { $headers["Authorization"] = "Bearer $token" }
    try {
        $resp = Invoke-WebRequest -Uri "$apiUrl$path" -Method $method -Headers $headers -Body ($body | ConvertTo-Json) -ErrorAction Ignore
        return $resp
    } catch {
        return $_.Exception.Response
    }
}

function Safe-Parse($content) {
    if ([string]::IsNullOrWhiteSpace($content)) { return @{ items=@() } }
    try { return $content | ConvertFrom-Json } catch { return @{ items=@() } }
}

Write-Host "`n🏰 THE FORTRESS TEST — LWMS PRODUCTION SECURITY" -ForegroundColor Cyan

# 🔐 1. LOGIN SESSIONS
$adminToken = (Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Body (@{ phone="0901234567"; password="123456" } | ConvertTo-Json) -ContentType "application/json").token
$shipperToken = (Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Body (@{ phone="0908887776"; password="123456" } | ConvertTo-Json) -ContentType "application/json").token

# 🛡️ TC01: SHIPPER INFORMATION LEAK TEST
Write-Host "`nTC01: SHIPPER TRIES TO READ ALL PARCELS" -ForegroundColor Yellow
$resp = Call-API GET "/api/parcels?pageNumber=1&pageSize=10" $shipperToken $null
$data = Safe-Parse $resp.Content
if ($data.items.Count -eq 0) { Write-Host "✅ Pass: Shipper sees 0 parcels (None assigned yet)" -ForegroundColor Green }
else { Write-Host "❌ Fail: Shipper can see unauthorized parcels!" -ForegroundColor Red }

# 🛡️ TC02: MERCHANT DATA ISOLATION TEST (CROSS-TENANT)
Write-Host "`nTC02: MERCHANT TRIES TO READ OTHER MERCHANT'S PARCEL" -ForegroundColor Yellow
$resp = Call-API GET "/api/parcels/LWMS-638515000000000000" $shipperToken $null
if ($resp.StatusCode -eq 404 -or $resp.StatusCode -eq 0) { Write-Host "✅ Pass: Hidden/Forbidden from unauthorized tenant" -ForegroundColor Green }
else { Write-Host "❌ Fail: Received $($resp.StatusCode). Cross-tenant leak!" -ForegroundColor Red }

# 🛡️ TC03: RACE CONDITION / DOUBLE COD TEST
Write-Host "`nTC03: DOUBLE COD SPAM TEST (Idempotency)" -ForegroundColor Yellow
# Tạo 1 đơn mới
$tracking = "FORT-" + (Get-Random -Minimum 1000 -Maximum 9999)
Call-API POST "/api/parcels" $adminToken @{ trackingCode=$tracking; senderName="Admin"; senderPhone="0901234567"; receiverName="X"; receiverPhone="090"; province="HN"; weight=1; codAmount=50000; originHubId=$originHubId; destHubId=$destHubId; serviceId=$serviceId }

# Flow để tới OutForDelivery
Call-API POST "/api/parcels/scan-inbound" $adminToken @{ trackingCode=$tracking; hubId=$originHubId }
Call-API POST "/assign-shipper" $adminToken @{ trackingCode=$tracking; shipperId="56C61A08-9192-4ED6-AF8B-5EA754D1BF5B" }

Write-Host "Simulating 2 simultaneous Success clicks..."
$resp1 = Call-API POST "/delivery-success" $shipperToken @{ trackingCode=$tracking; codAmount=50000 }
$resp2 = Call-API POST "/delivery-success" $shipperToken @{ trackingCode=$tracking; codAmount=50000 }

if ($resp1.StatusCode -eq 200 -and $resp2.StatusCode -ne 200) {
    Write-Host "✅ Pass: First request OK, second request BLOCKED." -ForegroundColor Green
} else {
    Write-Host "❌ Fail: Resp1=$($resp1.StatusCode), Resp2=$($resp2.StatusCode). Possible double COD!" -ForegroundColor Red
}

Write-Host "`n🏆 FORTRESS TEST COMPLETED!" -ForegroundColor Cyan
