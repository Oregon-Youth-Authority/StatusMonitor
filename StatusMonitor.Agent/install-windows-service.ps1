#For powershell 2 compatibility
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition

sc.exe create JJISStatusAgent binpath="$scriptPath\JJISStatusAgent.exe --service" start=auto obj="NT Authority\NetworkService" displayname="JJIS Status Agent" 
sc.exe description JJISStatusAgent "Periodically verifies JJIS connectivity and reports the status back to the JJIS Team."