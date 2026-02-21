
$inputFile = "D:\Hetak_Acc\SoftBookGit_React_UI_API\Backend_Workspace\SoftBook.Infrastructure\exported_data.sql"
$outputFile = "D:\Hetak_Acc\SoftBookGit_React_UI_API\Backend_Workspace\SoftBook.Infrastructure\postgresql_full_converted.sql"

# Read UTF-16LE
$raw = [System.IO.File]::ReadAllText($inputFile, [System.Text.Encoding]::Unicode)

# Flatten and clean
$raw = $raw -replace '(?m)^GO\s*$', ''
$raw = $raw -replace '(?m)^SET\s+IDENTITY_INSERT\s+.*?$', ''
$flattened = $raw -replace '(?m)\r?\n(?!\s*INSERT)', ' '

function To-SnakeCase($name) {
    if (!$name) { return $null }
    $name = $name -replace '\[|\]', ''
    $name = $name -replace '^dbo\.', ''
    # Use script block for safety
    $s = [regex]::Replace($name, '([a-z0-9])([A-Z])', { param($m) $m.Groups[1].Value + "_" + $m.Groups[2].Value })
    $s = [regex]::Replace($s, '([A-Z])([A-Z][a-z])', { param($m) $m.Groups[1].Value + "_" + $m.Groups[2].Value })
    return $s.ToLower()
}

$lines = $flattened -split "`r?`n"
$output = New-Object System.Collections.Generic.List[string]

Write-Host "Executing foolproof conversion..."

$reWithCols = [regex]"^INSERT\s+(?:\[?dbo\]?\.)?\[?(\w+)\]?\s*\((.+)\)\s*VALUES\s*\((.*)\)"
$reNoCols = [regex]"^INSERT\s+(?:\[?dbo\]?\.)?\[?(\w+)\]?\s*VALUES\s*\((.*)\)"
$reNPrefix = [regex]"N'((?:''|[^'])*)'"

foreach ($line in $lines) {
    $line = $line.Trim()
    if (!$line.StartsWith("INSERT")) { continue }

    if ($reWithCols.IsMatch($line)) {
        $m = $reWithCols.Match($line)
        $tableName = To-SnakeCase $m.Groups[1].Value
        $colsText = $m.Groups[2].Value
        $valsText = $m.Groups[3].Value
        
        $cols = $colsText -split "," | ForEach-Object { To-SnakeCase ($_.Trim()) }
        $hasCompanyId = ($cols -contains "company_id")
        
        # 🔥 FOOLPROOF STRING REPLACEMENT using script block
        $vals = $reNPrefix.Replace($valsText, { param($m) "'" + $m.Groups[1].Value + "'" })
        
        $vals = $vals -replace "CAST\(\s*N?''\s*AS\s+\w+\)", "NULL"
        $vals = $vals -replace "DateTime", "TIMESTAMP"
        $vals = $vals -replace " AS Date", " AS DATE"
        
        $colList = $cols -join ", "
        $valList = $vals.Trim()
        
        if (!$hasCompanyId) {
            $colList += ", company_id"
            $valList += ", 1"
        }
        
        $output.Add("INSERT INTO $tableName ($colList) VALUES ($valList);")
    }
}

$output | Out-File -FilePath $outputFile -Encoding Utf8
Write-Host "Success! Converted $($output.Count) records to $outputFile"
