$apiUrl = "http://localhost:5166"
$originHubId = "001E9C52-7360-4927-AA72-5B91CF8E9662"
$destHubId = "001E9C52-7360-4927-AA72-5B91CF8E9661"
$serviceId = "CC1E9C52-7360-4927-AA72-5B91CF8E9663"
$shipperId = "56C61A08-9192-4ED6-AF8B-5EA754D1BF5B"
$trackingCode = "PROD-" + (Get-Random -Minimum 100000 -Maximum 999999)

function Call-API($method, $path, $token, $body) {
    $headers = @{"Content-Type"="application/json"}
    if ($token) { $headers["Authorization"] = "Bearer $token" }
    
    try {
        $resp = Invoke-RestMethod -Uri "$apiUrl$path" -Method $method -Headers $headers -Body ($body | ConvertTo-Json)
        return $resp
    } catch {
        $msg = $_.Exception.Message
        if ($_.ErrorDetails) { $msg += " | " + $_.ErrorDetails.Message }
        throw $msg
    }
}

Write-Host "🚀 STARTING PRODUCTION FULL FLOW TEST" -ForegroundColor Cyan

# 1. LOGIN
Write-Host "`n🥇 STEP 1: LOGIN"
$login = Call-API POST "/api/auth/login" $null @{ phone="0901234567"; password="123456" }
$token = $login.token
Write-Host "✅ Login Success. Token Received." -ForegroundColor Green

# 2. CREATE PARCEL
Write-Host "`n🥈 STEP 2: CREATE PARCEL"
$parcel = Call-API POST "/api/parcels" $token @{
    senderName = "Nguyen Van A"
    senderPhone = "0988888888"
    receiverName = "Tran Van B"
    receiverPhone = "0999999999"
    province = "Ha Noi"
    weight = 2.5
    codAmount = 150000
    trackingCode = $trackingCode
    originHubId = $originHubId
    destHubId = $destHubId
    serviceId = $serviceId
}
Write-Host "✅ Created Parcel: $trackingCode" -ForegroundColor Green

# 3. SCAN INBOUND
Write-Host "`n🥉 STEP 3: SCAN INBOUND (Origin Hub)"
Call-API POST "/api/parcels/scan-inbound" $token @{ trackingCode=$trackingCode; hubId=$originHubId }
Write-Host "✅ Inbound Success." -ForegroundColor Green

# 4. SORT
Write-Host "`n🧠 STEP 4: SORT"
Call-API POST "/api/parcels/sort" $token @{ trackingCode=$trackingCode }
Write-Host "✅ Sort Success." -ForegroundColor Green

# 5. BAG FLOW
Write-Host "`n📦 STEP 5: BAG FLOW"
# 5.1 Create Bag
$bag = Call-API POST "/api/bags/create" $token @{ fromHubId=$originHubId; toHubId=$destHubId }
$bagId = $bag.id
Write-Host "✅ Bag Created: $bagId" -ForegroundColor Green

# 5.2 Add to Bag
Call-API POST "/api/bags/add-parcel" $token @{ bagId=$bagId; trackingCode=$trackingCode }
Write-Host "✅ Added to Bag." -ForegroundColor Green

# 5.3 Seal Bag
Call-API POST "/api/bags/seal" $token @{ bagId=$bagId }
Write-Host "✅ Bag Sealed." -ForegroundColor Green

# 6. RECEIVE BAG
Write-Host "`n🚚 STEP 6: RECEIVE BAG (Dest Hub)"
Call-API POST "/api/bags/receive" $token @{ bagId=$bagId }
Write-Host "✅ Bag Received at Dest Hub." -ForegroundColor Green

# 7. ASSIGN SHIPPER
Write-Host "`n🥇 STEP 7: ASSIGN SHIPPER"
Call-API POST "/assign-shipper" $token @{ trackingCode=$trackingCode; shipperId=$shipperId }
Write-Host "✅ Assigned to Shipper." -ForegroundColor Green

# 8. DELIVERY FAILED 1
Write-Host "`n💥 STEP 9: DELIVERY FAILED (Attempt 1)"
Call-API POST "/delivery-failed" $token @{ trackingCode=$trackingCode; reason="Khach di vang" }
Write-Host "✅ Recorded Failed 1." -ForegroundColor Green

# 9. RE-DELIVERY (RE-ASSIGN)
Write-Host "`n🔁 STEP 10: RE-DELIVERY (Re-Assign)"
Call-API POST "/re-assign-shipper" $token @{ trackingCode=$trackingCode; shipperId=$shipperId }
Write-Host "✅ Re-Assigned Success." -ForegroundColor Green

# 10. DELIVERY SUCCESS
Write-Host "`n🥇 STEP 11: DELIVERY SUCCESS"
Call-API POST "/delivery-success" $token @{ trackingCode=$trackingCode; codAmount=150000 }
Write-Host "✅ Delivery Success." -ForegroundColor Green

# 11. COD FLOW
Write-Host "`n💰 STEP 12: COD FLOW"
Call-API POST "/submit-cod" $token @{ trackingCode=$trackingCode; amount=150000 }
Write-Host "✅ COD Submitted." -ForegroundColor Green

Call-API POST "/settle-cod" $token @{ trackingCode=$trackingCode }
Write-Host "✅ COD Settled." -ForegroundColor Green

Write-Host "`n🏆 FULL FLOW TEST PASSED!" -ForegroundColor Cyan
