$outputPath = "C:\gitadr"

if (Test-Path $outputPath) {
    Write-Host "Removendo arquivos de $outputPath..."
    Remove-Item $outputPath -Recurse -Force
    Write-Host "Arquivos removidos."
} else {
    Write-Host "$outputPath não existe, nada para remover."
}

$oldPath = [Environment]::GetEnvironmentVariable("Path", "User")
if ($oldPath -like "*$outputPath*") {
    Write-Host "Removendo $outputPath do PATH..."
    $newPath = (( $oldPath -split ";" ) | Where-Object { $_ -ne $outputPath } | ForEach-Object { $_.Trim() }) -join ";"
    [Environment]::SetEnvironmentVariable("Path", $newPath, "User")
    Write-Host "PATH atualizado! gitadr removido."
} else {
    Write-Host "$outputPath não está no PATH, nada para remover."
}

