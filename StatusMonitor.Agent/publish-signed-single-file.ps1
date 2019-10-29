$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$publishDir = "$scriptPath\publish\JJISStatusAgent"
New-Item -ItemType Directory -Path $publishDir -ErrorAction Ignore

Copy-Item -Force $scriptPath\install-windows-service.ps1 $publishDir\
Copy-Item -Force $scriptPath\remove-windows-service.ps1 $publishDir\

$publishFilePath = "$publishDir\JJISStatusAgent.exe"

#For the following to work you must have dotnet-warp. If not, run the following in powershell: dotnet tool install --global dotnet-warp
dotnet-warp.exe $scriptPath -o $publishFilePath

#For the following to work signtool must be in your path.  If not add it to you path environment variable.  Look for the exe under C:\Program Files (x86)\Windows Kits\10\
#Also the Oregon Youth Authority Code Signing certificate must be in your certifificate store.  The certificate can be found in the developer keypass database
signtool sign /n "Oregon Youth Authority" $publishFilePath