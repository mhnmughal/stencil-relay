param([string]$ProjectRoot = (Split-Path $PSScriptRoot -Parent))
$ErrorActionPreference = 'Stop'
$violations = [System.Collections.Generic.List[string]]::new()
$engineWrappers = [System.Collections.Generic.List[string]]::new()
$inertEngineWrappers = @('UnityEngine.UnityWebRequestModule.dll', 'UnityEngine.UnityAnalyticsCommonModule.dll', 'UnityEngine.UnityAnalyticsModule.dll')
$packagePattern = 'unitywebrequest|analytics|ads$|advertis|multiplayer|netcode|transport|services\.|cloud|remoteconfig|collab|anklebreaker|ide\.rider|ide\.visualstudio'
foreach ($file in @('Packages/manifest.json', 'Packages/packages-lock.json')) {
    $config = Get-Content -LiteralPath (Join-Path $ProjectRoot $file) -Raw | ConvertFrom-Json
    foreach ($name in $config.dependencies.PSObject.Properties.Name) {
        if ($name -match $packagePattern) { $violations.Add("Network/development package in ${file}: $name") }
    }
}
$apiPattern = 'UnityWebRequest|HttpClient|System\.Net|WebClient|TcpClient|UdpClient|\bSocket\b|Application\.OpenURL|DownloadHandler|RemoteConfig|Firebase|Advertisement|UnityServices\.Initialize'
foreach ($folder in @('Assets/Scripts/Game', 'Assets/Scripts/Runtime')) {
    Get-ChildItem -LiteralPath (Join-Path $ProjectRoot $folder) -Filter '*.cs' -Recurse | ForEach-Object {
        if ((Get-Content -LiteralPath $_.FullName -Raw) -match $apiPattern) { $violations.Add("Runtime network API in $($_.FullName)") }
    }
}
$connect = Get-Content -LiteralPath (Join-Path $ProjectRoot 'ProjectSettings/UnityConnectSettings.asset') -Raw
if ($connect -match '(m_Enabled|m_InitializeOnStartup|m_EngineDiagnosticsEnabled|m_EnableCloudDiagnosticsReporting|m_CaptureEditorExceptions): 1') {
    $violations.Add('Unity service/diagnostic startup is enabled')
}
$settings = Get-Content -LiteralPath (Join-Path $ProjectRoot 'ProjectSettings/ProjectSettings.asset') -Raw
if ($settings -match 'forceInternetPermission: 1') { $violations.Add('Android forces Internet permission') }
foreach ($build in @('Builds/Offline/Win/StencilRelay_Data/Managed', 'Builds/Offline/iOS/Data/Managed')) {
    $path = Join-Path $ProjectRoot $build
    if (Test-Path -LiteralPath $path) {
        Get-ChildItem -LiteralPath $path -Filter '*.dll' | ForEach-Object {
            if ($_.Name -match 'UnityWebRequest|Analytics|UnityMCP|AnkleBreaker|Unity\.Services|UnityEngine\.Networking') {
                if ($_.Name -in $inertEngineWrappers) { $engineWrappers.Add("${build}/$($_.Name)") }
                else { $violations.Add("Network-related assembly in ${build}: $($_.Name)") }
            }
        }
        foreach ($dll in Get-ChildItem -LiteralPath $path -Filter '*.dll') {
            if ($dll.Name -eq 'UnityEngine.dll' -or $dll.Name -in $inertEngineWrappers) { continue }
            $stream = [System.IO.File]::OpenRead($dll.FullName)
            try {
                $reader = [System.Reflection.PortableExecutable.PEReader]::new($stream)
                $metadata = [System.Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($reader)
                foreach ($reference in $metadata.AssemblyReferences) {
                    $name = $metadata.GetString($metadata.GetAssemblyReference($reference).Name)
                    if ($name -match 'UnityWebRequest|UnityAnalytics|UnityMCP|AnkleBreaker') {
                        $violations.Add("Runtime assembly $($dll.Name) references $name")
                    }
                }
                $reader.Dispose()
            } finally { $stream.Dispose() }
        }
    }
}
$result = [ordered]@{
    date = (Get-Date -Format o)
    passed = ($violations.Count -eq 0)
    scope = 'Manifest, resolved lockfile, runtime C#, service startup, forced Internet permission, existing output Managed DLLs'
    violations = @($violations)
    retainedEngineFacades = @($engineWrappers)
    facadeNote = 'Unity may retain built-in type-forwarding wrappers without their packages. Only UnityEngine.dll and the wrappers may reference these; project/runtime library references fail this audit. Do not manually delete engine DLLs.'
}
$result | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $ProjectRoot 'AuditEvidence/offline-static.json') -Encoding utf8
if ($violations.Count) { throw ($violations -join "`n") }
Write-Output 'PASS: offline configuration and runtime API audit'
