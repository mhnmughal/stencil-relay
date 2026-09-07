# OFFLINE_AUDIT.md — Stencil Relay

> CURRENT 2026-09-07: UnityWebRequest and MCP are both removed from manifest and
> resolved lockfile. Rider/Visual Studio integrations are also removed. UnityConnect,
> engine diagnostics and analytics/ads startup are disabled; Android does not force
> Internet permission. `Tools/Verify-Offline.ps1` is the repeatable audit; evidence is
> `AuditEvidence/offline-static.json`. Current detailed report: POLISH_OFFLINE_AUDIT.md.
> The retained-WebRequest/MCP findings below are historical and superseded.

Finalised 2026-09-06 (M10).

## Method
1. grep all runtime scripts (`Assets/Scripts/`) for: `UnityWebRequest`, `HttpClient`,
   `System.Net`, `Socket`, `WebClient`, `TcpClient`, `UdpClient`, `WebView`,
   `Firebase`, `Advertisement`, `Analytics`, `RemoteConfig`, `RemoteSettings`,
   `DownloadHandler`, `WWW`, `.Ping(`, `Application.OpenURL`.
2. Inspect `Packages/manifest.json` for network / ads / analytics / attribution SDKs.
3. Inspect Bootstrap / service init for any online service startup.
4. Inspect build config (Player Settings, UnityConnectSettings).

## Findings

### 1. Runtime code
`grep` over `Assets/Scripts/` for every pattern above → **no matches**. The game
loads levels from `Resources`, saves to `Application.persistentDataPath` (local file),
plays local audio, renders locally. No socket, no HTTP, no download, no `OpenURL`.

### 2. Packages
No Firebase / AdMob / Unity Ads / Unity Analytics / AppsFlyer / GameAnalytics /
Facebook / attribution package. `com.unity.modules.unityanalytics` **removed**
(PACKAGE_AUDIT.md). `com.unity.modules.unitywebrequest` remains as a transitive
dependency of the IDE/URP packages but **no project code calls it**; build stripping
excludes unused code.

### 3. Services / init
`GameServices.Awake` → `SaveSystem` (local file), `AudioController` (local clips),
`Application.targetFrameRate`. No login, no cloud, no remote config, no telemetry.
`ProjectSettings/UnityConnectSettings.asset` — Unity Analytics not enabled.

### 4. Build config
No network permissions requested (MOBILE_RELEASE_CHECKLIST.md). iOS: no ATS
exceptions needed (no networking).

## Conclusion

`PASS — NO RUNTIME NETWORK FUNCTIONALITY`

The MCP editor plugin (`com.anklebreaker.unity-mcp`) runs a **local editor-only** HTTP
server on port 7890 — a development tool, not part of the player build (PACKAGE_AUDIT.md
§MCP production cleanup). It does not affect the offline conclusion for the shipped game.
