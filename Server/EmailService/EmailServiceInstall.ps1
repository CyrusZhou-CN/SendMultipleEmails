$serviceName = "EmailService"
$exePath = "C:\SendMultipleEmails\EmailService.exe"

# 安装服务
New-Service -Name $serviceName -Binary $exePath -Description "Email Service" -StartupType Automatic

# 启动服务
Start-Service -Name $serviceName
