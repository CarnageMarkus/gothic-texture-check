echo "Downloading NSIS 3.03..."

$url = "https://downloads.sourceforge.net/project/nsis/NSIS%203/3.03/nsis-3.03-setup.exe?r=https%3A%2F%2Fsourceforge.net%2Fprojects%2Fnsis%2Ffiles%2FNSIS%25203%2F3.03%2Fnsis-3.03-setup.exe%2Fdownload%3Fuse_mirror%3Ddatapacket%26r%3Dhttps%253A%252F%252Fsourceforge.net%252Fprojects%252Fnsis%252Ffiles%252FNSIS%2525203%252F3.03%252Fnsis-3.03-setup.exe%252Fdownload&ts=1537653908&use_mirror=netix"
$output = $env:TEMP + "\nsis-3.03-setup.exe"

(New-Object System.Net.WebClient).DownloadFile($url, $output)

echo "Installing NSIS 3.03..."

& $env:TEMP"/nsis-3.03-setup.exe" /S /D=$env:TEMP"\nsis"

echo "Building installer of GMBT"

& makensis gtchck.nsi

if ($LASTEXITCODE -like 0) {
	write-host "Done without errors." -foregroundcolor "green"
	$LastExitCode = 0
} else {
	write-host "Error!" -foregroundcolor "red"
	$LastExitCode = 1
}
