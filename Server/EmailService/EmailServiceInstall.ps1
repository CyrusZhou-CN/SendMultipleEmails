$serviceName = "EmailService"
$exePath = "C:\SendMultipleEmails\EmailService.exe"

# ��װ����
New-Service -Name $serviceName -Binary $exePath -Description "Email Service" -StartupType Automatic

# ��������
Start-Service -Name $serviceName
