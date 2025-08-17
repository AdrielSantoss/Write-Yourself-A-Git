$projectRelativePath = "..\Git\Git.csproj"
$outputPath = "C:\gitadr"

Write-Host "Instalando GITADR..."
dotnet publish $projectRelativePath -c Release -r win-x64 --self-contained false -o $outputPath

$exeName = "gitadr.exe"
$publishedExe = Get-ChildItem $outputPath -Filter *.exe | Select-Object -First 1
if ($publishedExe -and $publishedExe.Name -ne $exeName) {
    Write-Host "Renomeando $($publishedExe.Name) para $exeName..."
    Rename-Item $publishedExe.FullName -NewName $exeName -Force
}

$oldPath = [Environment]::GetEnvironmentVariable("Path", "User")
if ($oldPath -notlike "*$outputPath*") {
    Write-Host "Adicionando $outputPath ao PATH do usuário..."
    $newPath = "$oldPath;$outputPath"
    [Environment]::SetEnvironmentVariable("Path", $newPath, "User")
    Write-Host "PATH atualizado! Abra um novo PowerShell para usar o gitadr."
} else {
    Write-Host "$outputPath já está no PATH do usuário."
}