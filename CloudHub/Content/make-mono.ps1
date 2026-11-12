# Converts Content\cloudhub.css from navy/blue to black/grey/white.
# Run it from the folder that contains cloudhub.css (your Content folder).
# A backup is saved as cloudhub.css.bak before anything is changed.

$path = Join-Path (Get-Location) 'cloudhub.css'
if (-not (Test-Path $path)) {
    Write-Host 'cloudhub.css not found. Open PowerShell in your Content folder and try again.'
    exit 1
}

Copy-Item $path ($path + '.bak') -Force
$css = [System.IO.File]::ReadAllText($path)

# Order matters: the specific rgba values come before the general ones.
$pairs = @(
    # --- rgba colours ---
    @('rgba(59, 130, 246, 0.45)', 'rgba(255, 255, 255, 0.10)'),
    @('rgba(37, 99, 235, 0.35)', 'rgba(255, 255, 255, 0.06)'),
    @('rgba(59, 130, 246,', 'rgba(255, 255, 255,'),
    @('rgba(96, 165, 250, 0.55)', 'rgba(255, 255, 255, 0.18)'),
    @('rgba(147, 183, 255,', 'rgba(255, 255, 255,'),
    @('rgba(37, 99, 235,', 'rgba(17, 19, 21,'),
    @('rgba(29, 78, 216,', 'rgba(17, 19, 21,'),
    @('rgba(11, 31, 58,', 'rgba(17, 19, 21,'),
    @('rgba(8, 18, 36,', 'rgba(17, 19, 21,'),

    # --- avatar / hub gradient ---
    @('linear-gradient(135deg, #3b82f6, #1d4ed8)', 'linear-gradient(135deg, #4a5057, #1c1f23)'),

    # --- section 6 variables ---
    @('--reg-navy: #0b1f3a;', '--reg-navy: #16181b;'),
    @('--reg-navy-2: #12305c;', '--reg-navy-2: #2a2e33;'),
    @('--reg-blue: #2563eb;', '--reg-blue: #1c1f23;'),
    @('--reg-blue-dark: #1d4ed8;', '--reg-blue-dark: #000000;'),
    @('--reg-blue-soft: #e8f0ff;', '--reg-blue-soft: #eceef1;'),
    @('--reg-ink: #0f1f38;', '--reg-ink: #15181b;'),
    @('--reg-muted: #475569;', '--reg-muted: #555b62;'),
    @('--reg-line: #dfe5ee;', '--reg-line: #dcdfe3;'),

    # --- section 7 variables ---
    @('--ch-navy: #0b1f3a;', '--ch-navy: #23272b;'),
    @('--ch-navy-2: #12305c;', '--ch-navy-2: #343a40;'),
    @('--ch-blue: #2563eb;', '--ch-blue: #1c1f23;'),
    @('--ch-blue-dark: #1d4ed8;', '--ch-blue-dark: #000000;'),
    @('--ch-blue-soft: #e8f0ff;', '--ch-blue-soft: #eceef1;'),
    @('--ch-ink: #0f1f38;', '--ch-ink: #15181b;'),
    @('--ch-muted: #5b6b82;', '--ch-muted: #60666d;'),
    @('--ch-line: #e2e8f1;', '--ch-line: #e4e7ea;'),
    @('--ch-bg: #f3f6fb;', '--ch-bg: #f5f6f8;'),

    # --- hard-coded hex colours ---
    @('#0a1a31', '#1b1e22'),
    @('#eef3fa', '#f4f5f7'),
    @('#e3ebf6', '#ececec'),
    @('#d3dbe6', '#d3d7dc'),
    @('#5f6d84', '#6a7078'),
    @('#6b778c', '#6f757c'),
    @('#e4e9f0', '#e6e8eb'),
    @('#4c9be8', '#8a9099'),
    @('#8a97ab', '#8a9099'),
    @('#f4f7fb', '#f4f5f7'),
    @('#b6c2d4', '#b8bdc3'),
    @('#f6f8fb', '#f5f6f7'),
    @('#e3e9f2', '#e6e8eb'),
    @('#475569', '#555b62'),
    @('#566277', '#5f656c'),
    @('#334155', '#3a3f45'),
    @('#f9fbfe', '#f7f7f8'),
    @('#cdd6e2', '#cdd1d6'),
    @('#43536b', '#4a5057'),
    @('#0f2a52', '#1f2327'),
    @('#cfe0ff', '#e5e7ea'),
    @('#b9c8e0', '#b4bac1'),
    @('#dbe6fa', '#d5d9de'),
    @('#60a5fa', '#d5d9de'),
    @('#c6d3e8', '#d0d4d9'),
    @('#8fa3c4', '#9aa0a8'),
    @('#6f84a6', '#8a9099'),
    @('#9fb2d1', '#b0b5bc'),
    @('#93b7ff', '#c4c9cf'),
    @('#b8c6dc', '#c1c5ca'),
    @('#8795ab', '#8a9099'),
    @('#7b8aa0', '#7d838a'),
    @('#a9c2f5', '#a9acb1'),
    @('#b7cdf8', '#bfc4ca'),
    @('#e9eef6', '#eceef1'),
    @('#6f7f96', '#6f757c'),
    @('#f7f9fc', '#f7f7f8'),
    @('#1e4fa8', '#4a5057'),
    @('#6c9bff', '#8a9099')
)

foreach ($p in $pairs) {
    $css = $css.Replace($p[0], $p[1])
}

# The active sidebar item was "blue"; now it must be lighter than the sidebar.
$extra = @'


/* ===== Monochrome adjustments ===== */
.ch-nav-link.active,
.ch-nav-link.active:hover {
    background: #3a4046;
    color: #ffffff;
    box-shadow: none;
}
'@
$css = $css + $extra

[System.IO.File]::WriteAllText($path, $css, (New-Object System.Text.UTF8Encoding($false)))

Write-Host 'Done. Backup saved as cloudhub.css.bak'
Write-Host ''

# Report anything blue that is still left
$pattern = '#(0b1f3a|12305c|2563eb|1d4ed8|e8f0ff|0f1f38|3b82f6|60a5fa|93b7ff)|rgba\((37, 99|59, 130|11, 31|29, 78|96, 165|147, 183)'
$left = Select-String -Path $path -Pattern $pattern
if ($left) {
    Write-Host 'Blue values still in the file (fix these by hand):'
    $left | ForEach-Object { '  line {0}: {1}' -f $_.LineNumber, $_.Line.Trim() }
} else {
    Write-Host 'No known blue values left.'
}
