$baseUrl = "http://localhost:5555/api/v1"
$reportFile = "TestResult.txt"

function Log-Test($id, $objective, $success, $message) {
    $status = if ($success) { "PASS" } else { "FAIL" }
    Write-Host "[$id] $status : $objective - $message"
}

Write-Host "--- START TEST ---"

# 1. Login
$loginBody = @{ phone = "0999999999"; password = "123456" } | ConvertTo-Json
try {
    $auth = Invoke-RestMethod -Uri "$baseUrl/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
    $headers = @{ "Authorization" = "Bearer $($auth.token)" }
    Log-Test "AUTH" "Login" $true ""
} catch {
    Log-Test "AUTH" "Login" $false $_.Exception.Message
    exit
}

# 2. Create Parcel
$body = @{ 
    senderName = "Shop Demo"; senderPhone = "0888888888"
    receiverName = "Khách Hàng"; receiverPhone = "0901234567"
    receiverAddress = "123 Quận 1"; province = "HCM"
    weight = 500; codAmount = 150000
    serviceId = "aa1e9c52-7360-4927-aa72-5b91cf8e9661"
    originHubId = "001e9c52-7360-4927-aa72-5b91cf8e9662"
    destHubId = "001e9c52-7360-4927-aa72-5b91cf8e9661"
} | ConvertTo-Json
try {
    $res = Invoke-RestMethod -Uri "$baseUrl/parcels" -Method Post -Body $body -ContentType "application/json" -Headers $headers
    $tracking = $res.trackingCode
    Log-Test "TC-01" "Create" $true "Code: $tracking"
} catch {
    Log-Test "TC-01" "Create" $false $_.Exception.Message
    exit
}

# 3. State Machine Check (Direct Success)
try {
    $res = Invoke-WebRequest -Uri "$baseUrl/bags/delivery-success" -Method Post -Body (@{ trackingCode = $tracking } | ConvertTo-Json) -Headers $headers -ContentType "application/json" -SkipHttpErrorCheck
    if ($res.StatusCode -ne 200) {
        Log-Test "TC-07" "State Machine Security" $true "Blocked as expected ($($res.StatusCode))"
    } else {
        Log-Test "TC-07" "State Machine Security" $false "Should have been blocked"
    }
} catch {
    Log-Test "TC-07" "State Machine Security" $true "Exception caught (Good)"
}

# 4. Inbound
try {
     Invoke-RestMethod -Uri "$baseUrl/inbound/scan" -Method Post -Body (@{ trackingCode = $tracking; hubId = "001e9c52-7360-4927-aa72-5b91cf8e9661" } | ConvertTo-Json) -Headers $headers -ContentType "application/json"
     Log-Test "TC-03" "Inbound" $true ""
} catch {
     Log-Test "TC-03" "Inbound" $false $_.Exception.Message
}

Write-Host "--- END TEST ---"
