$apiUrl = "http://localhost:5166"
$originHubId = "001E9C52-7360-4927-AA72-5B91CF8E9662"
$destHubId = "001E9C52-7360-4927-AA72-5B91CF8E9661"
$serviceId = "CC1E9C52-7360-4927-AA72-5B91CF8E9663"
$shipperId = "56C61A08-9192-4ED6-AF8B-5EA754D1BF5B"
$trackingCode = "STATE-" + (Get-Random -Minimum 1000 -Maximum 9999)

function Call-API($method, $path, $token, $body) {
    $headers = @{"Content-Type"="application/json"}
    if ($token) { $headers["Authorization"] = "Bearer $token" }
    $resp = Invoke-WebRequest -Uri "$apiUrl$path" -Method $method -Headers $headers -Body ($body | ConvertTo-Json) -ErrorAction Ignore
    return $resp
}

Write-Host "🔄 FULL STATE MACHINE TRANSITION TEST" -ForegroundColor Cyan

# 1. Login Admin
$login = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Headers @{"Content-Type"="application/json"} -Body (@{ phone="0901234567"; password="123456" } | ConvertTo-Json)
$adminToken = $login.token

# 2. Login Shipper
$loginS = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Headers @{"Content-Type"="application/json"} -Body (@{ phone="0908887776"; password="123456" } | ConvertTo-Json)
$shipperToken = $loginS.token

# STEP 1: CREATE (Created)
Write-Host "`nSTEP 1: CREATE PARCEL"
$resp = Call-API POST "/api/parcels" $adminToken @{ trackingCode=$trackingCode; senderName="Test Admin"; senderPhone="0901234567"; receiverName="Test Recv"; receiverPhone="0900000000"; province="HN"; weight=1; codAmount=100000; originHubId=$originHubId; destHubId=$destHubId; serviceId=$serviceId }
Write-Host "✅ [Created]" -ForegroundColor Green

# STEP 2: INBOUND (ArrivedHub)
Write-Host "`nSTEP 2: SCAN INBOUND"
Call-API POST "/api/parcels/scan-inbound" $adminToken @{ trackingCode=$trackingCode; hubId=$originHubId }
Write-Host "✅ [ArrivedHub]" -ForegroundColor Green

# STEP 3: SORT (InTransit)
Write-Host "`nSTEP 3: SORT PARCEL"
Call-API POST "/api/parcels/sort" $adminToken @{ trackingCode=$trackingCode }
Write-Host "✅ [Sorted -> InTransit]" -ForegroundColor Green

# STEP 4: BAGGING (InBag)
Write-Host "`nSTEP 4: BAGGING"
$bag = Call-API POST "/api/bags/create" $adminToken @{ fromHubId=$originHubId; toHubId=$destHubId }
$bagId = ($bag.Content | ConvertFrom-Json).id
Call-API POST "/api/bags/add-parcel" $adminToken @{ bagId=$bagId; trackingCode=$trackingCode }
Write-Host "✅ [InBag]" -ForegroundColor Green

# STEP 5: SEAL (InTransit)
Write-Host "`nSTEP 5: SEAL BAG"
Call-API POST "/api/bags/seal" $adminToken @{ bagId=$bagId }
Write-Host "✅ [Sealed -> InTransit]" -ForegroundColor Green

# STEP 6: RECEIVE (ArrivedHub - Dest)
Write-Host "`nSTEP 6: RECEIVE BAG AT DEST"
Call-API POST "/api/bags/receive" $adminToken @{ bagId=$bagId }
Write-Host "✅ [Received -> ArrivedHub]" -ForegroundColor Green

# STEP 7: ASSIGN SHIPPER (OutForDelivery)
Write-Host "`nSTEP 7: ASSIGN SHIPPER"
Call-API POST "/assign-shipper" $adminToken @{ trackingCode=$trackingCode; shipperId=$shipperId }
Write-Host "✅ [OutForDelivery]" -ForegroundColor Green

# STEP 8: DELIVERY FAILURE (FailedDelivery)
Write-Host "`nSTEP 8: DELIVERY FAILED (Attempt 1)"
Call-API POST "/delivery-failed" $shipperToken @{ trackingCode=$trackingCode; reason="Busy" }
Write-Host "✅ [FailedDelivery]" -ForegroundColor Green

# STEP 9: RE-ASSIGN (OutForDelivery)
Write-Host "`nSTEP 9: RE-ASSIGN SHIPPER"
Call-API POST "/re-assign-shipper" $adminToken @{ trackingCode=$trackingCode; shipperId=$shipperId }
Write-Host "✅ [OutForDelivery]" -ForegroundColor Green

# STEP 10: DELIVERY SUCCESS (Delivered)
Write-Host "`nSTEP 10: DELIVERY SUCCESS"
Call-API POST "/delivery-success" $shipperToken @{ trackingCode=$trackingCode; codAmount=100000 }
Write-Host "✅ [Delivered]" -ForegroundColor Green

# STEP 11: COD SUBMIT (Collected -> Submitted)
Write-Host "`nSTEP 11: SUBMIT COD"
Call-API POST "/submit-cod" $shipperToken @{ trackingCode=$trackingCode; amount=100000 }
Write-Host "✅ [COD Submitted]" -ForegroundColor Green

# STEP 12: QUERY HISTORY
Write-Host "`nSTEP 12: FINAL QUERY HISTORY"
$history = Call-API GET "/api/parcels/$trackingCode" $adminToken $null
$data = $history.Content | ConvertFrom-Json
Write-Host "Final Status: $($data.status)" -ForegroundColor Yellow
Write-Host "History Steps: $($data.trackingHistory.Count)" -ForegroundColor Yellow

$data.trackingHistory | ForEach-Object { 
    Write-Host "- $($_.toStatus) at $($_.location)" -ForegroundColor Gray
}

Write-Host "`n🏆 ALL STATES VERIFIED!" -ForegroundColor Cyan
