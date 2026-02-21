
$inputFile = "D:\Hetak_Acc\SoftBookGit_React_UI_API\Backend_Workspace\SoftBook.Infrastructure\exported_data.sql"
$outputFile = "D:\Hetak_Acc\SoftBookGit_React_UI_API\Backend_Workspace\SoftBook.Infrastructure\import_postgres_data.sql"

# Read everything
$rawContent = [System.IO.File]::ReadAllText($inputFile, [System.Text.Encoding]::Unicode)

# 1. FLATTEN: Join lines that are split (common in SSMS exports)
# We look for a newline that is NOT followed by a new INSERT or other SQL command
$flattened = $rawContent -replace '(?m)\r?\n(?!\s*(INSERT|SET|GO|--))', ' '

# Helper function for snake_case
function To-SnakeCase($name) {
    if ($null -eq $name) { return $null }
    $name = $name -replace '\[|\]', ''
    if ($name -match "^Tab") { $name = "tab_" + $name.Substring(3) }
    $name = $name -creplace '([a-z0-9])([A-Z])', '$1_$2'
    $name = $name -creplace '([A-Z])([A-Z][a-z])', '$1_$2'
    return $name.ToLower()
}

$lines = $flattened -split "`r?`n"
$finalLines = New-Object System.Collections.Generic.List[string]

foreach ($line in $lines) {
    $line = $line.Trim()
    if ($null -eq $line -or $line.Length -lt 10) { continue }

    # Robust Regex: 
    # 1. Capture Table Name
    # 2. Capture Column List block (between first '(' and first ')' before VALUES)
    # 3. Capture Values block (between first '(' after VALUES and last ')' on line)
    
    if ($line -match "^INSERT\s+(?:\[?dbo\]?\.)?\[?(\w+)\]?\s*\((.+)\)\s*VALUES\s*\((.*)\)") {
        $tableRaw = $Matches[1]
        $colsRaw = $Matches[2]
        $valsRaw = $Matches[3]
        
        $tableName = To-SnakeCase $tableRaw
        
        # Process columns
        $cols = $colsRaw -split "," | ForEach-Object { To-SnakeCase ($_.Trim()) }
        $colList = ($cols -join ", ") + ", company_id"
        
        # Process values
        # Remove N prefix: N'...' -> '...'
        $vals = $valsRaw -replace "N'((?:''|[^'])*)'", "'$1'"
        # Fix DateTime
        $vals = $vals -replace "DateTime", "TIMESTAMP"
        
        # 🔥 The Fix for Syntax Error:
        # $valsRaw was everything between '(' and the LAST ')' because regex is greedy.
        # This preserves all nested parentheses for CAST, Decimal, etc.
        $valList = $vals.Trim() + ", 1"
        
        $finalLines.Add("INSERT INTO $tableName ($colList) VALUES ($valList);")
    }
}

$finalLines | Out-File -FilePath $outputFile -Encoding Utf8
Write-Host "Success! Generated $($finalLines.Count) robust import statements."
