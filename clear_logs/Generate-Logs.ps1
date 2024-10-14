$logDirectory = ".\logs"

if (-Not (Test-Path $logDirectory)) {
    New-Item -ItemType Directory -Path $logDirectory
}

$logFiles = @(
    @{Name = "log1.txt"; Lines = 2000},
    @{Name = "log2.txt"; Lines = 2000},
    @{Name = "log3.txt"; Lines = 2000},
    @{Name = "log4.txt"; Lines = 2000},
    @{Name = "log5.txt"; Lines = 2000},
    @{Name = "log6.txt"; Lines = 2000},
    @{Name = "log7.txt"; Lines = 2000},
    @{Name = "log8.txt"; Lines = 2000},
    @{Name = "log9.txt"; Lines = 2000},
    @{Name = "log10.txt"; Lines = 2000},
    @{Name = "empty_log.txt"; Lines = 0}
)

foreach ($logFile in $logFiles) {
    $filePath = Join-Path $logDirectory $logFile.Name
    
    if ($logFile.Lines -gt 0) {
        1..$logFile.Lines | ForEach-Object { "This is log line $_" } | Set-Content $filePath
    } else {
        New-Item -ItemType File -Path $filePath
    }
}

Write-Host "Log files created in $logDirectory"
