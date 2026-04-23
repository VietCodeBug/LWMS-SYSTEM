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

Write-Host "🛡️ STARTING AUTHORIZATION TEST" -ForegroundColor Cyan

# TC01: Unauthenticated
Write-Host "`nTC01: CALL API WITHOUT TOKEN"
$resp = Call-API POST "/api/parcels" $null @{ senderName="No Token" }
if ($resp.StatusCode -eq 401) { Write-Host "✅ Pass: Received 401 Unauthorized" -ForegroundColor Green }
else { Write-Host "❌ Fail: Expected 401, got $($resp.StatusCode)" -ForegroundColor Red }

# TC02: Forbidden (Shipper tries to Create Parcel)
Write-Host "`nTC02: SHIPPER TRIES TO CREATE PARCEL (Forbidden)"
$loginShipper = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Headers @{"Content-Type"="application/json"} -Body (@{ phone="0908887776"; password="123456" } | ConvertTo-Json)
$shipperToken = $loginShipper.token

$resp = Call-API POST "/api/parcels" $shipperToken @{ 
    senderName="Hacker Shipper"; 
    senderPhone="0999999999";
    receiverName="Victim";
    receiverPhone="0888888888";
    province="HN";
    weight=1;
    codAmount=0;
    originHubId=$originHubId;
    destHubId=$destHubId;
    serviceId=$serviceId
}
if ($resp.StatusCode -eq 403) { 
    Write-Host "✅ Pass: Received 403 Forbidden" -ForegroundColor Green 
    $content = $resp.Content | ConvertFrom-Json
    Write-Host "   Message: $($content.message)" -ForegroundColor Gray
}
else { Write-Host "❌ Fail: Expected 403, got $($resp.StatusCode)" -ForegroundColor Red }

# TC03: Success (Admin creates Parcel)
Write-Host "`nTC03: ADMIN CREATES PARCEL"
$loginAdmin = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Headers @{"Content-Type"="application/json"} -Body (@{ phone="0901234567"; password="123456" } | ConvertTo-Json)
$adminToken = $loginAdmin.token

$trackingCode = "AUTH-" + (Get-Random -Minimum 1000 -Maximum 9999)
$resp = Call-API POST "/api/parcels" $adminToken @{ 
    trackingCode=$trackingCode;
    senderName="Admin User"; 
    senderPhone="0901234567";
    receiverName="Receiver";
    receiverPhone="0900000000";
    province="HN";
    weight=1.5;
    codAmount=50000;
    originHubId=$originHubId;
    destHubId=$destHubId;
    serviceId=$serviceId
}
if ($resp.StatusCode -eq 200) { Write-Host "✅ Pass: Parcel Created by Admin ($trackingCode)" -ForegroundColor Green }
else { Write-Host "❌ Fail: Expected 200, got $($resp.StatusCode)" -ForegroundColor Red }

# TC04: Query Side Check
Write-Host "`nTC04: QUERY PARCEL HISTORY"
$resp = Call-API GET "/api/parcels/$trackingCode" $adminToken $null
if ($resp.StatusCode -eq 200) { 
    $data = $resp.Content | ConvertFrom-Json
    Write-Host "✅ Pass: Received Parcel Data" -ForegroundColor Green
    Write-Host "   Status: $($data.status)" -ForegroundColor Gray
    Write-Host "   History Count: $($data.trackingHistory.Count)" -ForegroundColor Gray
}
else { Write-Host "❌ Fail: Expected 200, got $($resp.StatusCode)" -ForegroundColor Red }

Write-Host "`n🏆 AUTHORIZATION TEST COMPLETED!" -ForegroundColor Cyan
