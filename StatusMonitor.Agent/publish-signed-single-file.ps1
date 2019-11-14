$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$publishDir = "$scriptPath\publish\"
New-Item -ItemType Directory -Path $publishDir -ErrorAction Ignore

# If the exe name changes, then Installer.cs Install() must be updated
$publishFilePath = "$publishDir\JJISStatusMonitorAgent.exe"

#For the following to work you must have dotnet-warp. If not, run the following in powershell: dotnet tool install --global dotnet-warp
dotnet-warp.exe $scriptPath -o $publishFilePath

#For the following to work signtool must be in your path.  If not add it to you path environment variable.  Look for the exe under C:\Program Files (x86)\Windows Kits\10\
#Also the Oregon Youth Authority Code Signing certificate must be in your certifificate store.  The certificate can be found in the developer keypass database
signtool sign /n "Oregon Youth Authority" $publishFilePath

copy-Item $scriptPath\Readme.txt $publishDir
copy-Item $scriptPath\monitorSettings.json $publishDir

#Create zip archive
$archivePath = "$publishDir\JJIS.StatusMonitor.Agent.zip"
$tempFile = New-TemporaryFile
remove-item -force $tempFile
remove-Item -force $archivePath
add-type -assembly "system.io.compression.filesystem"
[io.compression.zipfile]::CreateFromDirectory($publishDir, $tempFile.FullName) 
move-item -force $tempFile $archivePath