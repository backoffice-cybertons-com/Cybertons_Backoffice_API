$p = "D:\Hetak_Acc\SoftBookGit_React_UI_API\Backend_Workspace\SoftBook.Infrastructure"
$f = "$p\Postgres_Complete_Schema.sql"
Write-Host "Appending Chunk 2..."
Add-Content -Path $f -Value "`r`n"
Get-Content "$p\temp_chunk2.sql" | Add-Content $f
Write-Host "Appending Chunk 3..."
Add-Content -Path $f -Value "`r`n"
Get-Content "$p\temp_chunk3.sql" | Add-Content $f
Write-Host "Appending Functions..."
Add-Content -Path $f -Value "`r`n-----------------------------------------------------------`r`n-- 11. STORED PROCEDURES & FUNCTIONS`r`n-----------------------------------------------------------`r`n"
if (Test-Path "$p\temp_chunk4.sql") {
    Get-Content "$p\temp_chunk4.sql" | Add-Content $f
} else {
    Write-Host "Chunk 4 missing!"
}
Write-Host "Cleaning up..."
Remove-Item "$p\temp_chunk2.sql", "$p\temp_chunk3.sql", "$p\temp_chunk4.sql" -ErrorAction SilentlyContinue
Write-Host "Done."
