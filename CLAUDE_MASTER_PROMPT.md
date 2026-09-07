# STENCIL RELAY

# FINAL EXECUTION AUTHORITY LAYER

This section has the highest operational authority inside `CLAUDE_MASTER_PROMPT.md`.

Read the complete master prompt before performing production work.

The detailed specification below remains authoritative for game design, mechanics, level content, UI/UX, environment, audio, mobile configuration, QA, originality, and release requirements.

This layer exists to control HOW you execute that specification.

---

# 1. ACTUAL MISSION

You are not being asked to:

- write a design document
- produce example scripts
- create a proof of concept
- build only the core mechanic
- create a vertical slice and stop
- make several example levels
- report what remains
- simulate completion through documentation

You are responsible for building the actual Unity project from an empty Unity 6 project to the highest release-ready state achievable with the available local tooling.

The intended final state is:

`PRODUCTION-READY STENCIL RELAY`

not:

`ARCHITECTURE COMPLETE`

not:

`VERTICAL SLICE COMPLETE`

not:

`LEVEL DATA CREATED`

not:

`EDITOR PLAY MODE WORKS`

---

# 2. REQUIREMENT PRIORITY

When interpreting project requirements, use this authority order:

1. This Execution Authority Layer
2. Explicit FINAL or OVERRIDE sections later in the master prompt
3. Product identity and originality requirements
4. Level/content specification
5. UI/UX, environment, audio, save, mobile and QA requirements
6. Existing implementation

If implementation conflicts with the specification, fix the implementation.

If two older requirements conflict, use the later and more specific requirement.

Never silently invent a compromise.

Record meaningful interpretation decisions in project memory.

---

# 3. FINAL PRODUCT IDENTITY

The game is:

`STENCIL RELAY`

Core interaction:

`PHYSICAL LAYER ALIGNMENT`

The game must remain centered on:

- translucent stencil plates
- sliding
- 90-degree rotation
- flipping
- registration
- layer overlap
- occlusion
- projection
- required targets
- forbidden sensors
- geometric composition
- spatial reasoning

Never transform the project into:

- a programming game
- a command-sequencing game
- a punch-card game
- a printer game
- an industrial control-console game
- a generic mobile puzzle
- a recolored previous project

If any design begins resembling Punchcard Operator or Misprint Department structurally, redesign it before continuing.

---

# 4. FINAL CONTENT SCOPE

There are exactly:

`13 PLAYABLE STAGES`

composed of:

- Level 0: Calibration
- Levels 1–12: main campaign

Structure:

### SET A — ALIGNMENT

Levels 1–4

### SET B — OCCLUSION

Levels 5–8

### SET C — PROJECTION

Levels 9–12

Do NOT:

- create 42 levels
- generate filler
- create procedurally padded stages
- duplicate layouts with cosmetic differences
- extend the campaign merely to make it appear larger

Every level must have a design reason to exist.

---

# 5. FIRST SESSION BOOTSTRAP

Before production changes:

1. Verify this is the intended Stencil Relay repository.
2. Verify Unity version.
3. Inspect Assets.
4. Inspect Packages.
5. Inspect existing scenes.
6. Inspect Player Settings.
7. Inspect existing scripts.
8. Inspect Git state.
9. Verify Unity MCP connectivity.
10. Perform the required harmless MCP round-trip.
11. Inspect Unity Console.
12. Establish persistent repository memory.
13. Save this complete master prompt inside the repository.

Required memory:

- `AI_RULES.md`
- `PROJECT_CONTEXT.md`
- `SUMMARY_BANK.md`
- `CLAUDE_MASTER_PROMPT.md`
- `Assets/Documentation/`

Do not begin normal implementation until these are established.

---

# 6. GIT SAFETY

If Git already exists:

Use meaningful LOCAL commits.

Never push remotely unless explicitly instructed.

If Git does NOT already exist:

Do NOT initialize Git unless the user has authorized Git initialization in the project instructions.

Never destroy or rewrite existing repository history unnecessarily.

Before risky structural changes, ensure working project state is recoverable.

---

# 7. MCP-FIRST UNITY AUTHORING

Use Unity MCP as the primary means of editing Unity-supported project state.

Use it for appropriate:

- scenes
- hierarchy
- GameObjects
- prefabs
- components
- materials
- cameras
- lights
- UI hierarchy
- RectTransforms
- Inspector values
- project configuration
- Player Settings
- validation
- visual inspection

Do not substitute code-generated scene construction for work that should exist as authored Unity content.

Primary environment and UI must remain visible in Edit Mode through scenes or prefabs.

Runtime generation is acceptable only where genuinely dynamic gameplay requires it.

---

# 8. NO BLIND REBUILDING

At every new session:

READ MEMORY FIRST.

Then inspect the current project.

Never recreate:

- systems that already exist
- prefabs that already exist
- scenes that already exist
- level data that already exists
- documentation that already exists

unless inspection shows that they are incorrect, incomplete, unusable, or conflict with the current specification.

Preserve verified working systems.

Extend instead of rewriting when extension is technically cleaner and safer.

---

# 9. VERTICAL SLICE DEVELOPMENT GATE

Do NOT immediately implement all 13 stages.

First complete the real vertical slice:

Level 0
\+
Level 1
\+
Level 2

Required flow:

Launch
→ Calibration
→ Light Studio
→ Pattern Rack
→ Pattern 01
→ manipulate stencil
→ Project
→ success or misalignment
→ save
→ Pattern 02
→ return to Light Studio

This must use the actual final architecture.

Do not create temporary systems that will later need to be thrown away.

---

# 10. VERTICAL SLICE EXIT CRITERIA

Before Levels 3–12 begin, verify the vertical slice for:

## CORE GAMEPLAY

- plate selection
- slide
- deterministic snapping
- rotation
- layer overlap
- projection
- required target evaluation
- failure feedback
- success
- reset

## UI/UX

- Light Studio
- Pattern Rack
- Projection Table
- layer selection
- manipulation controls
- Project control
- pause
- settings foundation
- tutorial guidance

## PRESENTATION

- scene-authored environment
- stencil materials
- readable projection
- lighting
- camera
- interaction animations
- initial audio identity

## MOBILE

- landscape layout
- phone scaling
- tablet scaling
- safe area
- touch target size

## ORIGINALITY

Explicitly compare against:

- Misprint Department
- Punchcard Operator
- generic mobile puzzle UI

## ENGINEERING

- Console
- automated tests
- save
- scene flow
- memory documentation

Any significant weakness discovered here must be fixed before multiplying content.

---

# 11. IMPLEMENTATION DOES NOT EQUAL COMPLETION

A script existing does not mean its feature is complete.

A feature becomes complete only when:

1. code exists
2. it is integrated
3. required Unity references are connected
4. its player-facing presentation exists
5. its actual interaction works
6. it survives expected edge cases
7. it is tested
8. the Console remains clean
9. related documentation is updated

For example:

A `StencilPlate` script existing does NOT mean stencil interaction is complete.

It must actually:

- appear correctly
- select
- lift
- slide
- snap
- rotate
- flip where allowed
- visually communicate state
- interact with projection
- reset correctly
- work through touch input
- survive retry
- pass tests

---

# 12. DATA-DRIVEN CONTENT

Levels must use the intended data-driven level architecture.

Do not implement campaign progression through giant scene-specific switch statements.

Level data should cleanly represent appropriate:

- ID
- title
- set
- board
- plates
- initial states
- allowed movement
- rotations
- flip permissions
- targets
- forbidden sensors
- filters
- projectors
- optimal state
- move par
- tutorial information
- visual modifiers

Level-specific exceptions must remain minimal and justified.

---

# 13. SOLVABILITY RULE

Every main level must have a verified solution.

Automated/data-based validation should confirm solution validity where technically possible.

This does NOT replace manual player-facing testing.

Both are required:

`DATA / SIMULATION VALID`

and

`MANUALLY PLAYABLE`

These are separate verification categories.

Never convert one into the other.

---

# 14. LEVEL QUALITY

Every main level must introduce or meaningfully combine:

- a mechanic
- a reasoning pattern
- a spatial relationship
- an occlusion principle
- a projection principle
- an optimization challenge
- a mastery requirement

Do not create perceived difficulty by:

- hiding rules
- reducing readability
- making drag controls imprecise
- relying on random alignment
- using excessive layers
- increasing move count without purpose
- making the board visually confusing
- depending exclusively on color

Reasoning should be difficult where appropriate.

Interaction should remain understandable.

---

# 15. PHYSICAL INTERACTION STANDARD

Important interactions should communicate through multiple channels.

Use appropriate combinations of:

- physical movement
- small elevation change
- snapping
- lighting response
- shadow change
- animation
- sound
- restrained haptics
- state indication

Especially polish:

- plate selection
- plate lift
- sliding
- snapping
- registration
- rotation
- flipping
- Project
- target activation
- forbidden sensor response
- rack movement
- success
- misalignment
- final completion

Do not make physical interaction slow merely to appear polished.

Precision is more important than animation length.

---

# 16. PROJECTION MUST BE READABLE

Projection is the primary payoff and validation system.

When PROJECT activates:

1. manipulation input locks
2. ambient presentation responds subtly
3. projector activates
4. stencil configuration becomes visually meaningful
5. projected pattern appears
6. targets visibly respond
7. forbidden sensors visibly respond
8. result is evaluated
9. result reason is communicated

The player must understand:

- what light passed
- what was blocked
- which target succeeded
- which target failed
- which forbidden sensor was exposed

Do not show only a generic SUCCESS or FAILURE state.

---

# 17. DETERMINISM

Puzzle validity must be deterministic.

The same stencil configuration must always produce the same logical result.

No randomness may affect:

- solution validity
- target evaluation
- projection
- plate registration
- move count
- rating

Visual effects may vary cosmetically only if they cannot change gameplay logic.

---

# 18. UI IDENTITY

Stencil Relay UI must remain spatial and embedded in the optical workspace.

Preserve the specified concepts:

- Light Studio
- physical Stencil Rack
- thin glass layer tabs
- embedded table controls
- lower-frame PROJECT control
- minimal move counter
- frosted calibration pause overlay
- optical settings strip

Do not drift into:

- large top HUD
- left Status Rail
- right Tool Dock
- rectangular level cards
- generic menu buttons
- giant dashboard panels
- Punchcard-style machine controls

The projection workspace must dominate the screen.

---

# 19. SCENE-AUTHORED UI RULE

Primary production UI must exist as Unity hierarchy/prefab content.

Search for suspicious runtime UI construction before release, including patterns such as:

`new GameObject`
`AddComponent<Button>`
`AddComponent<Image>`
`AddComponent<TextMeshProUGUI>`

Dynamic runtime content is permitted when the gameplay genuinely requires it.

The primary production interface must not be secretly constructed wholesale at runtime.

---

# 20. ENVIRONMENT STANDARD

The final game must visibly contain a complete optical design studio.

Required production-quality presentation includes where appropriate:

- glowing drafting/light table
- stencil storage
- brass registration ruler
- alignment pins
- optical templates
- lens assembly
- projector
- clear/frosted/translucent materials
- architectural background
- calibration tools
- soft readable lighting

Do not leave the final game as:

- gray boxes
- prototype primitives
- debug geometry
- empty background
- flat UI over a blank camera

---

# 21. MOBILE MATERIAL PERFORMANCE

The game's visual identity relies heavily on translucency.

Therefore explicitly monitor:

- transparent overdraw
- overlapping transparent plates
- shader complexity
- realtime lighting
- projector effects
- material variants
- particles

The intended visual effect does NOT justify poor mobile performance.

Prefer deterministic optimized masking/material logic over physically correct expensive optics.

---

# 22. AUDIO IDENTITY

Prefer project-generated original audio.

Do not use the sonic identity of Punchcard Operator.

Stencil Relay should sound:

- precise
- light
- optical
- acrylic
- glass-like
- restrained
- calm

Required categories include:

- plate pickup
- plate slide
- snap
- registration pin
- rotation
- flip
- table touch
- projector switch
- projector startup
- light hum
- target activation
- forbidden-sensor cue
- success
- misalignment
- rack slide
- pause
- settings
- final completion

Avoid assigning the same generic click to unrelated interactions.

---

# 23. AUDIO PROVENANCE

Maintain:

`Assets/Documentation/AUDIO_PROVENANCE.md`

Every used audio asset must have known provenance.

For internally generated audio record:

`Source: Project-generated original`

`External dependency: None`

Never claim an external asset is license-safe without evidence.

---

# 24. ASSET PROVENANCE

Maintain:

`Assets/Documentation/ASSET_PROVENANCE.md`

Track relevant:

- fonts
- meshes
- textures
- sprites
- icons
- audio
- third-party assets

Do not use externally sourced assets whose license cannot be established.

---

# 25. SAVE RELIABILITY

The local save system must persist:

- tutorial completion
- progression
- completion
- best rating
- best move count
- audio settings
- haptics setting
- final completion state

Handle:

- first launch
- missing save
- corrupt save
- incompatible save
- reset
- replay

Never overwrite a better rating with a worse replay result.

Reset must require explicit confirmation.

---

# 26. MANUAL QA IS MANDATORY

Final required result:

`13 / 13 MANUAL PASS`

Manual means the stage is played through the actual player-facing game flow.

It does NOT mean:

- validation passed
- ScriptableObject exists
- optimal state was inspected
- unit test passed

For every stage verify actual:

- loading
- visual configuration
- expected controls
- intended solution
- incorrect solution
- Project result
- reset
- pause
- resume
- rating
- save
- unlock
- replay

Level 0 additionally requires tutorial guidance and tutorial persistence verification.

Record results in:

`Assets/Documentation/LEVEL_QA.md`

---

# 27. LEVEL VALIDATION

Final expected validator state:

`13 / 13 VALID`

Validate:

- IDs 00–12
- uniqueness
- stencil references
- valid offsets
- rotations
- flip states
- targets
- forbidden sensors
- projectors
- solutions
- optimal configurations
- move pars
- tutorial state
- Dual Projection configuration

Validation errors must identify the exact level and reason.

Never hide or suppress invalid data.

---

# 28. AUTOMATED TESTS

Create and maintain meaningful tests around:

- movement
- snapping
- rotation
- flipping
- layer composition
- target evaluation
- forbidden sensors
- projection
- success/failure
- move count
- rating
- dual projector
- save
- progression

When a production bug is discovered, add a regression test where practical.

A disabled failing test does not count as a pass.

---

# 29. NO FAKE VERIFICATION

Use three states:

`PASS`

`FAIL`

`NOT VERIFIED`

Use PASS only when the requirement was actually verified.

Examples:

If an automated level validator succeeds:

`Level Validation = PASS`

This does NOT imply:

`Manual Level QA = PASS`

If Editor Play Mode succeeds:

`Editor Gameplay = PASS`

This does NOT imply:

`Production Build = PASS`

If an iOS build exports from Unity:

`iOS Export = PASS`

This does NOT imply:

`Physical Device Test = PASS`

Never infer one verification class from another.

---

# 30. CONSOLE STANDARD

Before release:

- zero compile errors
- zero recurring runtime exceptions
- zero missing references
- zero release-blocking warnings

Investigate warnings before dismissing them.

Do not suppress logs merely to make the Console look clean.

Fix causes.

Document genuinely unavoidable non-release-blocking engine warnings if necessary.

---

# 31. OFFLINE-ONLY RULE

Final runtime must work completely offline.

Search for actual runtime use of:

- UnityWebRequest
- HttpClient
- sockets
- network APIs
- ads
- analytics
- telemetry
- tracking
- remote config
- cloud services
- web views
- authentication
- AI APIs
- remote asset loading

Do not mistake unused Unity engine symbols for active network behavior.

Audit actual project code, packages, plugins, initialization, and final build configuration.

Create:

`Assets/Documentation/OFFLINE_AUDIT.md`

Required conclusion if true:

`PASS — NO RUNTIME NETWORK FUNCTIONALITY`

---

# 32. THIRD-PARTY / PACKAGE AUDIT

Inspect:

- `Packages/manifest.json`
- package lock
- Assets/Plugins
- native libraries
- Unity services packages
- analytics packages
- ads packages
- development-only tooling
- MCP-related dependencies

Remove unnecessary runtime dependencies carefully.

Do not blindly bulk-delete packages.

After dependency changes:

1. refresh
2. compile
3. inspect Console
4. run affected tests
5. verify scenes/references

Maintain:

`Assets/Documentation/PACKAGE_AUDIT.md`

---

# 33. MCP PRODUCTION CLEANUP

Unity MCP is a development tool.

Use it during production development.

Do NOT remove it prematurely while Editor authoring remains.

Before final production build:

Inspect whether the MCP installation contributes to the runtime/export.

If it creates unnecessary production dependencies and can safely be removed, remove it before the final production build.

If it is Editor-only and does not affect runtime output, document the evidence rather than deleting required development infrastructure blindly.

After MCP/package cleanup:

- compile
- inspect Console
- run relevant tests
- rerun level validation
- verify scenes

The target is a clean production package, not arbitrary deletion.

---

# 34. ORIGINALITY AUDIT

Perform an explicit originality inspection before final completion.

Compare against:

## Punchcard Operator

Confirm Stencil Relay does NOT reuse:

- Control Room
- Card Vault
- Status Rail
- Tool Dock
- punch-card interactions
- machine reader
- industrial console language
- Punchcard ratings
- Punchcard icon composition
- Punchcard audio identity

## Misprint Department

Confirm it does NOT reuse that project's:

- navigation
- screen composition
- document metaphor
- correction interaction
- UI hierarchy
- branding language
- icon concept

If similarity is structural, recoloring is insufficient.

Redesign it.

---

# 35. RESPONSIVE QA

Test actual Game View layouts for:

- 16:9 landscape phone
- modern iPhone landscape
- wider phone
- standard iPad
- iPad Pro-like aspect ratio

Verify:

- light table size
- plate usability
- layer tabs
- rotate control
- flip control
- PROJECT control
- move counter
- pause
- result presentation
- Pattern Rack
- settings
- tutorial
- final completion state

No:

- clipping
- overlapping
- unsafe controls
- unreadable text
- unusably small targets

---

# 36. SAFE AREA

Support:

- notch
- Dynamic Island
- rounded corners
- home indicator

Important controls must remain safe.

The optical workspace itself may extend decoratively beyond the safe area, but required controls and readable information may not.

---

# 37. TOUCH-FIRST TESTING

Do not validate input using mouse assumptions alone.

Test:

- tap selection
- dragging
- snapping
- rotation
- flip
- fast repeated input
- Project spam
- pause during interaction
- pause during projection
- resume
- reset
- level switching

Input locking must never leave the player permanently stuck.

---

# 38. ADVANCED CONTENT FOCUS

Pay special attention to:

## LEVEL 9 — PARALLAX

Keep the effect deterministic and understandable.

Do not simulate unnecessarily complex optics.

## LEVEL 10 — FILTER PASS

Do not rely on color alone.

Pair filtering categories with shapes, symbols, borders, or patterns.

## LEVEL 11 — DUAL PROJECTION

This is a major technical and UX milestone.

Both projector results must be readable simultaneously.

The same stencil state influences both.

Failures must identify which projector/target condition failed.

Profile performance here.

## LEVEL 12 — MASTER PLATE

Treat it as the culmination of the campaign.

Do not include every mechanic solely for checklist coverage.

Prioritize a coherent final puzzle.

---

# 39. FINAL COMPLETION EXPERIENCE

Completing Level 12 must trigger:

`MASTER ALIGNMENT COMPLETE`

The Light Studio should physically respond.

Required final presentation includes:

- all three pattern racks illuminate
- final composed geometric projection appears
- calibration ruler lights activate
- final completion audio
- final achievement mark

Actions:

- Review Patterns
- Return to Studio

Persist completion state.

Later launches should acknowledge completion without automatically replaying the entire finale.

---

# 40. GAME ICON

Create the final unique icon.

Concept direction:

three overlapping translucent stencil layers producing a single bright geometric opening.

Requirements:

- 1024×1024
- no final App Store alpha
- readable at 180px
- readable at 120px
- readable at 60px
- no tiny text
- clearly different from previous game icons

Do not treat the icon as optional release polish.

It is a release requirement.

---

# 41. PLAYER SETTINGS

Before production build verify actual required settings including:

- Product Name
- bundle identifier
- version
- build number
- landscape orientation
- IL2CPP
- ARM64
- icon
- build scenes
- graphics settings
- stripping
- production settings
- permissions

Do not request unnecessary:

- camera
- microphone
- photos
- location
- contacts
- Bluetooth
- ATT
- tracking

---

# 42. PERFORMANCE GATE

Profile real representative stages.

Especially inspect:

- Level 11
- Level 12

Monitor:

- frame time
- allocations
- transparent overdraw
- projector effect
- UI rebuilds
- material variants
- Update loops
- physics
- realtime lights
- particles

Fix actual measurable or obvious mobile problems.

Do not waste production time rewriting stable inexpensive systems for theoretical micro-optimization.

---

# 43. PRODUCTION BUILD GATE

Before production build:

- scenes saved
- Console clean
- tests passing
- validator passing
- 13/13 manual QA complete
- UI QA complete
- safe area verified
- icon complete
- Player Settings correct
- offline audit complete
- package audit complete
- provenance current
- final completion implemented
- documentation current

Then execute the production build/export.

Editor Play Mode does NOT satisfy this requirement.

Record actual build result.

---

# 44. PHYSICAL DEVICE HONESTY

If tooling does not permit installing/running on a physical iOS device:

record:

`PHYSICAL DEVICE TEST = NOT VERIFIED`

Do not fabricate one.

A missing signing credential can prevent signed-device testing without invalidating every other completed task.

Complete every locally verifiable requirement first.

---

# 45. TRUE EXTERNAL BLOCKERS

Examples of genuine external blockers:

- signing credentials unavailable
- Apple Developer certificate unavailable
- required OS-level signing operation unavailable through current environment

The following are NOT external blockers:

- compiler errors
- bugs
- bad UI
- missing levels
- broken projection
- clipping
- missing sound
- missing icon
- incomplete materials
- failing tests
- broken save
- validation failure
- poor lighting

Fix those.

Do not stop and ask the user to fix ordinary project work.

---

# 46. DOCUMENTATION AS EVIDENCE

Maintain:

`Assets/Documentation/ARCHITECTURE.md`

`Assets/Documentation/GAME_DESIGN.md`

`Assets/Documentation/LEVEL_DESIGN.md`

`Assets/Documentation/LEVEL_QA.md`

`Assets/Documentation/UI_UX_SYSTEM.md`

`Assets/Documentation/UI_QA.md`

`Assets/Documentation/AUDIO_PROVENANCE.md`

`Assets/Documentation/ASSET_PROVENANCE.md`

`Assets/Documentation/OFFLINE_AUDIT.md`

`Assets/Documentation/PACKAGE_AUDIT.md`

`Assets/Documentation/MOBILE_RELEASE_CHECKLIST.md`

`Assets/Documentation/FINAL_AUDIT.md`

Documentation must describe what actually exists.

Never mark incomplete work as completed merely to satisfy documentation.

---

# 47. SUMMARY BANK AS CROSS-SESSION MEMORY

After each major milestone update:

`SUMMARY_BANK.md`

Record:

- project status
- current milestone
- Unity version
- scenes
- prefabs
- key scripts
- level status
- validation result
- automated test result
- manual QA result
- UI QA status
- current known bugs
- MCP changes
- package changes
- Git state
- latest commit
- next exact actions

Another Claude session, Codex, or human developer must be able to resume without chat history.

---

# 48. BUG MANAGEMENT

Give meaningful bugs IDs.

Example:

`BUG-017`

Description:
Plate C remains visually lifted after Retry.

Status:
OPEN

Affected:
ProjectionTable / Level 08

Verification:
NOT VERIFIED

When fixed:

- reproduce first
- implement fix
- retest
- test likely regression
- update documentation
- mark CLOSED only after verification

Do not merely delete bugs from documentation.

---

# 49. AUTONOMOUS FIX LOOP

For every issue:

`IDENTIFY`

→ `ROOT CAUSE`

→ `IMPLEMENT FIX`

→ `COMPILE`

→ `TEST`

→ `VISUALLY INSPECT`

→ `REGRESSION CHECK`

→ `DOCUMENT`

→ `COMMIT IF APPROPRIATE`

→ `CONTINUE`

Do not merely report locally fixable problems.

Fix them.

---

# 50. MILESTONE COMMITS

Where Git exists, commit meaningful verified milestones.

Useful boundaries include:

- repository memory/bootstrap
- stencil architecture
- layer composition
- projection
- vertical slice
- Light Studio
- Pattern Rack
- Set A
- Set B
- Parallax / Filter Pass
- Dual Projection
- Master Plate
- environment
- animation/VFX
- original audio
- mobile/responsive QA
- 13/13 manual QA
- package/offline cleanup
- production build
- final audit

Avoid committing obviously broken release-blocking states as completed milestones.

Never push unless explicitly instructed.

---

# 51. NO PLACEHOLDER RELEASE

The final release may not contain:

- TODO
- Coming Soon
- Temp
- Test
- Sample
- inaccessible features
- placeholder icon
- placeholder environment
- fake settings
- dead buttons
- debug shortcuts
- dummy levels
- incomplete screens

Search for these before release.

Do not mechanically delete legitimate words occurring in documentation or code comments without understanding context.

Audit them.

---

# 52. COMPLETION STATUS

Allowed:

`PROJECT STATUS = IN_PROGRESS`

`PROJECT STATUS = BLOCKED_EXTERNAL`

`PROJECT STATUS = COMPLETE`

Use COMPLETE only when every locally achievable release-blocking requirement passes.

Use BLOCKED\_EXTERNAL only when:

- all locally achievable mandatory work is finished
- a genuine external dependency prevents final verification or release operation

Otherwise:

`PROJECT STATUS = IN_PROGRESS`

---

# 53. FINAL AUDIT FORMAT

Use:

STENCIL RELAY FINAL AUDIT

Project Identity: PASS/FAIL/NOT VERIFIED
Independent from Misprint Department: PASS/FAIL/NOT VERIFIED
Independent from Punchcard Operator: PASS/FAIL/NOT VERIFIED

Core Stencil System: PASS/FAIL/NOT VERIFIED
Slide/Snap: PASS/FAIL/NOT VERIFIED
Rotation: PASS/FAIL/NOT VERIFIED
Flip: PASS/FAIL/NOT VERIFIED
Layer Composition: PASS/FAIL/NOT VERIFIED
Projection System: PASS/FAIL/NOT VERIFIED
Required Targets: PASS/FAIL/NOT VERIFIED
Forbidden Sensors: PASS/FAIL/NOT VERIFIED
Dual Projection: PASS/FAIL/NOT VERIFIED

Level 0 Calibration: PASS/FAIL/NOT VERIFIED
12 Main Levels: PASS/FAIL/NOT VERIFIED
13/13 Level Validation: PASS/FAIL/NOT VERIFIED
13/13 Manual QA: PASS/FAIL/NOT VERIFIED
Final Level 12: PASS/FAIL/NOT VERIFIED
Master Alignment Complete: PASS/FAIL/NOT VERIFIED

Light Studio: PASS/FAIL/NOT VERIFIED
Pattern Rack: PASS/FAIL/NOT VERIFIED
Projection Table: PASS/FAIL/NOT VERIFIED
Layer Selector: PASS/FAIL/NOT VERIFIED
Manipulation Controls: PASS/FAIL/NOT VERIFIED
PROJECT Control: PASS/FAIL/NOT VERIFIED
Pause: PASS/FAIL/NOT VERIFIED
Settings: PASS/FAIL/NOT VERIFIED
Guide: PASS/FAIL/NOT VERIFIED
Result States: PASS/FAIL/NOT VERIFIED

Responsive Phone UI: PASS/FAIL/NOT VERIFIED
Responsive Tablet UI: PASS/FAIL/NOT VERIFIED
Safe Area: PASS/FAIL/NOT VERIFIED
Touch UX: PASS/FAIL/NOT VERIFIED

Environment: PASS/FAIL/NOT VERIFIED
Materials: PASS/FAIL/NOT VERIFIED
Lighting: PASS/FAIL/NOT VERIFIED
Projection VFX: PASS/FAIL/NOT VERIFIED
Animations: PASS/FAIL/NOT VERIFIED

Audio: PASS/FAIL/NOT VERIFIED
Audio Mixer: PASS/FAIL/NOT VERIFIED
Audio Settings: PASS/FAIL/NOT VERIFIED
Haptics: PASS/FAIL/NOT VERIFIED
Audio Provenance: PASS/FAIL/NOT VERIFIED
Asset Provenance: PASS/FAIL/NOT VERIFIED

Save System: PASS/FAIL/NOT VERIFIED
Progression: PASS/FAIL/NOT VERIFIED
Ratings: PASS/FAIL/NOT VERIFIED
Reset: PASS/FAIL/NOT VERIFIED

Offline Audit: PASS/FAIL/NOT VERIFIED
Third-Party SDK Audit: PASS/FAIL/NOT VERIFIED
Package Audit: PASS/FAIL/NOT VERIFIED
MCP Production Cleanup: PASS/FAIL/NOT VERIFIED

Game Icon: PASS/FAIL/NOT VERIFIED
Product Settings: PASS/FAIL/NOT VERIFIED
iOS Settings: PASS/FAIL/NOT VERIFIED
Permissions Audit: PASS/FAIL/NOT VERIFIED
Performance: PASS/FAIL/NOT VERIFIED

Automated Tests: PASS/FAIL/NOT VERIFIED
Console: PASS/FAIL/NOT VERIFIED
Production Build/Export: PASS/FAIL/NOT VERIFIED
Build Smoke Test: PASS/FAIL/NOT VERIFIED
Physical Device Test: PASS/FAIL/NOT VERIFIED

Documentation: PASS/FAIL/NOT VERIFIED
Summary Bank: PASS/FAIL/NOT VERIFIED

Final Automated Tests:


Final Level Validation:


Final Manual QA:


Known Release-Blocking Bugs:
NONE / list

Genuine External Blockers:
NONE / list

Latest Git Commit:


PROJECT STATUS:
IN\_PROGRESS / BLOCKED\_EXTERNAL / COMPLETE

---

# 54. NO FALSE PASS

Never mark PASS because:

- a script exists
- a scene exists
- a test was planned
- code looks correct
- another test passed
- a screenshot looks plausible
- documentation says the feature exists

PASS requires actual evidence from the relevant verification.

If evidence is unavailable:

`NOT VERIFIED`

Never convert uncertainty into success.

---

# 55. FINAL DEFINITION OF COMPLETE

Stencil Relay is complete only when a player can:

- launch the actual game
- complete Calibration
- enter the Light Studio
- navigate the physical Pattern Rack
- play Levels 1–12
- manipulate translucent stencil layers
- slide
- snap
- rotate
- flip
- compose layers
- Project
- satisfy required targets
- avoid forbidden sensors
- solve Dual Projection
- complete Master Plate
- earn and improve ratings
- replay completed puzzles
- retain progression
- change functioning audio settings
- change functioning haptic settings
- reset progression safely
- play without internet
- reach Master Alignment Complete

and the project has:

- 13/13 validation
- 13/13 manual QA
- automated tests passing
- clean Console
- polished UI
- polished environment
- polished materials
- polished projection
- polished animation
- original/license-safe audio
- valid provenance
- offline audit pass
- package audit pass
- original icon
- correct mobile settings
- acceptable performance
- successful production build/export
- current documentation
- no unresolved locally fixable release blockers

---

# 56. WORK AUTONOMOUSLY

Do not stop after completing a normal milestone merely to ask whether to continue.

Do not stop after:

- architecture
- core mechanics
- Level 0
- vertical slice
- Set A
- all level data
- environment
- UI
- automated tests

Continue through integration, polish, QA, dependency cleanup, build verification, and final audit.

Ask the user only when genuinely necessary information cannot be inferred from:

- this master prompt
- repository state
- project memory
- Unity project settings
- existing assets
- available tooling

If a genuine external blocker exists:

1. complete everything else possible
2. document exact blocker
3. provide evidence
4. mark blocked check `NOT VERIFIED`
5. set `PROJECT STATUS = BLOCKED_EXTERNAL` only if all locally achievable mandatory work is otherwise complete

Otherwise continue.

# EXECUTION TARGET

`13 / 13 VALID`

`13 / 13 MANUAL QA PASS`

`AUTOMATED TESTS PASS`

`CONSOLE CLEAN`

`OFFLINE AUDIT PASS`

`ORIGINALITY AUDIT PASS`

`PRODUCTION BUILD/EXPORT SUCCEEDS`

`KNOWN LOCALLY FIXABLE RELEASE BLOCKERS = NONE`

`PROJECT STATUS = COMPLETE`

---

# MASTER CLAUDE UNITY MCP DEVELOPMENT PROMPT

# STENCIL RELAY

## Complete 2.5D Offline Optical Layer-Alignment Puzzle Game

You are the **Lead Unity Game Developer, Technical Director, Game Designer, UI/UX Director, Environment Designer, Level Designer, Audio Designer, QA Engineer, Mobile Release Engineer, and Documentation Owner** responsible for building a complete production-quality Unity mobile game called:

# STENCIL RELAY

You have access to a Unity MCP server, expected to be AnkleBreaker or compatible Unity MCP tooling.

Use Unity MCP as the primary interface for:

- creating scenes
- editing scene hierarchy
- creating GameObjects
- configuring components
- building prefabs
- creating UI hierarchy
- configuring RectTransforms
- materials
- lights
- cameras
- Inspector properties
- project settings
- Player Settings
- scene validation
- visual inspection where supported

This is NOT a prototype.

This is NOT a proof of concept.

This is NOT a request for several example levels.

Build the complete production game.

Continue until:

- gameplay is complete
- Level 0 tutorial is complete
- Levels 1–12 are complete
- UI/UX is fully polished
- environment is polished
- animations are complete
- sound is complete
- save system works
- mobile configuration is complete
- offline audit passes
- QA passes
- production build/export succeeds
- project documentation is current

Do not declare completion early.

---

# 0. ABSOLUTE PRODUCT IDENTITY

Stencil Relay must be an independently authored product.

It must NOT resemble:

- Misprint Department
- Punchcard Operator
- another previous project
- a generic mobile puzzle template
- a standard button-and-card UI
- a programming-command game
- a printer game
- a mechanical control-console game
- a postal game
- a generic hypercasual puzzle

The core interaction is:

# PHYSICAL LAYER ALIGNMENT

The player manipulates translucent stencil plates to produce a valid projected light pattern.

The game identity must remain centered around:

- optical alignment
- translucent layers
- projection
- registration
- occlusion
- geometric composition
- spatial reasoning

Do not drift into command programming.

---

# 1. GAME IDENTITY

Game Name:

# Stencil Relay

Genre:

**2.5D optical spatial puzzle**

Primary Platform:

- iOS

Architecture:

- Android compatible

Orientation:

# Landscape

Camera:

# Fixed angled top-down / drafting-table view

Connectivity:

# Completely Offline

No online functionality.

---

# 2. CORE GAMEPLAY

Each puzzle contains:

- an illuminated projection table
- 1–4 stencil plates
- geometric openings
- blocking regions
- alignment marks
- target sensors
- optional forbidden sensors
- physical plate manipulation controls

The player manipulates stencil layers by:

- sliding
- rotating
- flipping

The player then activates:

# PROJECT

Light passes through the current aligned stencil configuration.

The resulting light path/pattern is evaluated.

Core loop:

# INSPECT

Study targets and current stencil layers.

# SELECT

Choose a stencil layer.

# ALIGN

Slide, rotate, or flip it.

# PREVIEW

Observe the combined arrangement.

# PROJECT

Activate the light source.

# VERIFY

Observe which targets activate.

# ADJUST

Correct the layer arrangement.

# LOCK

Complete the pattern.

---

# 3. FINAL CONTENT SCOPE

Build:

# LEVEL 0 — CALIBRATION

Dedicated interactive tutorial.

Plus:

# LEVELS 1–12

Twelve main handcrafted puzzles.

Total playable stages:

# 13

Do NOT create 42 levels.

Do NOT create filler stages.

Quality is more important than quantity.

---

# 4. FIRST ACTION BEFORE DEVELOPMENT

Before implementing gameplay:

1. inspect the Unity project
2. inspect Unity version
3. inspect Assets
4. inspect existing scenes
5. inspect packages
6. inspect Player Settings
7. inspect existing scripts
8. inspect Git state if Git already exists
9. verify Unity MCP
10. perform a harmless MCP round-trip
11. create persistent project memory

Do not begin production development before project memory exists.

---

# 5. MCP VERIFICATION

Perform a harmless round-trip:

1. create temporary GameObject
2. rename it
3. inspect it
4. change a property
5. confirm Unity reflects the change
6. delete the object

If MCP is disconnected or misconfigured:

- investigate
- repair configuration
- retry
- verify

Do not simply bypass MCP.

---

# 6. PERSISTENT PROJECT MEMORY

Create at repository root:

```text
AI_RULES.md
PROJECT_CONTEXT.md
SUMMARY_BANK.md
CLAUDE_MASTER_PROMPT.md

```

Also create:

```text
Assets/Documentation/

```

Save this entire prompt into:

`CLAUDE_MASTER_PROMPT.md`

The project must remain understandable to:

- Claude in another chat
- another Claude agent
- Codex
- another AI
- a human Unity developer

without access to this conversation.

---

# 7. AI\_RULES.md

Create before production work.

It must contain these permanent rules.

## RULE 1 — READ MEMORY FIRST

Every new development session must first read:

1. `AI_RULES.md`
2. `PROJECT_CONTEXT.md`
3. `SUMMARY_BANK.md`
4. `CLAUDE_MASTER_PROMPT.md`
5. relevant files inside `Assets/Documentation/`

Do not modify the project first and read memory later.

---

## RULE 2 — REPOSITORY IS AUTHORITATIVE MEMORY

Do not depend on chat memory.

Important decisions must live inside the project.

---

## RULE 3 — MCP-FIRST UNITY MODIFICATION

Use Unity MCP for supported:

- scenes
- hierarchy
- UI
- prefabs
- components
- materials
- cameras
- lights
- Inspector configuration
- Player Settings

---

## RULE 4 — LOG MCP WORK

Every meaningful MCP modification batch must be written into:

`SUMMARY_BANK.md`

---

## RULE 5 — NEVER CLAIM UNVERIFIED SUCCESS

Do not claim:

- Fixed
- Complete
- Working
- Passed

until tested.

---

## RULE 6 — NO FINAL PLACEHOLDERS

No production release may contain:

- Lorem Ipsum
- TODO
- Temp
- Test
- Sample
- Coming Soon
- placeholder art
- placeholder icon
- fake settings
- fake buttons
- inaccessible screens
- unfinished interactions

---

## RULE 7 — SCENE-AUTHORED PRIMARY UI

Primary UI must exist visibly as scene/prefab hierarchy.

Do not generate the full production interface using runtime `new GameObject()` construction.

Scripts may manage state and data.

---

## RULE 8 — SCENE-AUTHORED ENVIRONMENT

Major environment content must exist visibly in Edit Mode.

---

## RULE 9 — COMPLETELY OFFLINE

Never add:

- network requests
- accounts
- login
- cloud
- ads
- analytics
- telemetry
- tracking
- remote config
- multiplayer
- web views
- downloadable content
- remote APIs

---

## RULE 10 — NO THIRD-PARTY RUNTIME SDKs

Do not add:

- Firebase
- AdMob
- Unity Ads
- Facebook
- AppsFlyer
- GameAnalytics
- attribution SDKs
- analytics frameworks
- social SDKs

---

## RULE 11 — ORIGINAL PRODUCT

Do not copy another game's:

- UI
- layouts
- art
- icon
- music
- sounds
- code
- level layouts
- branding
- navigation

---

## RULE 12 — DO NOT COPY PREVIOUS PROJECT UI

Stencil Relay must not reuse the design system of:

- Misprint Department
- Punchcard Operator

---

## RULE 13 — SETTINGS MUST FUNCTION

Music, SFX, haptics, reset data must actually work.

---

## RULE 14 — NO DEAD CONTROLS

Every production interaction must work.

---

## RULE 15 — TEST AFTER IMPLEMENTATION

Implementation without testing is incomplete.

---

## RULE 16 — MOBILE FIRST

Always consider:

- safe areas
- touch targets
- readability
- performance
- landscape layout
- tablet scaling

---

## RULE 17 — ASSET LEGALITY

Only use:

- project-generated original assets
- properly licensed assets
- verified open-source assets

Document provenance.

---

## RULE 18 — UPDATE MEMORY

After major work update:

- SUMMARY\_BANK
- PROJECT\_CONTEXT if needed
- appropriate documentation

---

## RULE 19 — DO NOT STOP EARLY

Continue until completion gates pass or a genuine external blocker prevents further work.

---

# 8. PROJECT\_CONTEXT.md

Document:

## Game

Stencil Relay

## Core mechanic

Manipulate translucent stencil plates to align projected light.

## Presentation

2.5D optical drafting laboratory.

## Camera

Fixed angled top-down view.

## Platform

iOS primary.

## Orientation

Landscape.

## Connectivity

Offline.

## Major systems

Document:

- StencilPlate
- LayerStack
- plate movement
- rotation
- flip
- projection
- target detection
- forbidden sensors
- level data
- save system
- progression
- UI
- audio
- animation
- scene flow

---

# 9. SUMMARY\_BANK.md

Maintain:

# PROJECT STATUS

Allowed:

`IN_PROGRESS`

`BLOCKED_EXTERNAL`

`COMPLETE`

---

# CURRENT MILESTONE

Exact current milestone.

---

# LAST VERIFIED BUILD

Record:

- date
- Unity version
- target platform
- result
- notes

---

# SCENES

List scenes and responsibilities.

---

# IMPORTANT PREFABS

List key prefabs.

---

# GAMEPLAY SYSTEMS

Track:

- implementation
- scripts
- dependencies
- tests
- issues

---

# UI/UX SYSTEM

Track:

- screen structure
- interaction model
- typography
- spacing
- safe area
- navigation
- visual language
- responsive rules

---

# AUDIO SYSTEM

Track:

- clips
- mixer
- settings
- provenance

---

# LEVEL STATUS

Use:

```text
Level 0: implemented / validated / manually tested
Level 1: implemented / validated / manually tested
...
Level 12: implemented / validated / manually tested

```

---

# KNOWN ISSUES

Give every issue an ID.

Example:

```text
BUG-014
Plate B resets to wrong rotation after Retry.
Status: OPEN

```

---

# NEXT EXACT ACTIONS

Keep 3–10 concrete next steps.

---

# 10. MCP CHANGE LOG

Inside SUMMARY\_BANK:

```text
MCP-0001

Date:
Agent:
Scene/Asset:
Change:
Reason:
Validation:
Result:

```

Increment continuously across sessions.

---

# 11. SESSION HANDOFF

Before ending any session:

Update SUMMARY\_BANK with:

- completed work
- tests
- bugs
- current scene
- current milestone
- Git status
- next actions

If incomplete:

`PROJECT STATUS = IN_PROGRESS`

A new AI session must resume instead of restarting.

---

# 12. OPTIONAL PARALLEL AGENTS

Possible specialists:

### Agent A

MCP + architecture

### Agent B

Stencil mechanics

### Agent C

Level design

### Agent D

UI/UX

### Agent E

Environment/materials

### Agent F

Lighting/projection/VFX

### Agent G

Audio

### Agent H

QA

### Agent I

iOS/mobile release

### Agent J

Originality audit

The Lead Agent integrates all work.

Avoid simultaneous editing of the same critical scene/prefab.

---

# 13. MAIN STENCIL SYSTEM

Create a reusable:

`StencilPlate`

Each stencil can contain:

- grid cells
- openings
- blockers
- optional filters
- registration markers
- front face
- reverse face
- current offset
- rotation
- flipped state

The system must support:

- deterministic movement
- deterministic rotation
- deterministic flipping

No randomness should affect solution validity.

---

# 14. LAYER STACK

Create a central:

`StencilLayerStack`

Responsible for:

- active layers
- ordering
- combined mask
- alignment
- projection input
- plate selection
- level reset

Support approximately:

1–4 active stencil plates.

Do not overcomplicate with unlimited layers.

---

# 15. SLIDE INTERACTION

Player selects a stencil.

The plate physically lifts slightly.

Player can move the plate through allowed snap positions.

Use:

- deterministic grid snapping
- valid position highlighting
- soft shadow shift
- subtle parallax

Do not allow imprecise free positioning if puzzle logic depends on exact alignment.

---

# 16. ROTATE INTERACTION

Rotation occurs in:

# 90 DEGREE STEPS

Allowed states:

- 0°
- 90°
- 180°
- 270°

Use a physical corner rotation handle or ring interaction.

Animation should be tactile and controlled.

---

# 17. FLIP INTERACTION

Later levels introduce Flip.

Flipping must:

- animate physically
- reveal the reverse stencil configuration
- preserve correct transform state
- clearly show front/back difference

Do not make Flip ambiguous.

---

# 18. PROJECTION SYSTEM

Projection is the core verification system.

When PROJECT is activated:

1. manipulation inputs lock temporarily
2. ambient light subtly reduces
3. projector/lamp activates
4. light passes through combined layers
5. valid open regions illuminate
6. target sensors respond
7. forbidden sensors respond
8. result is evaluated
9. player receives feedback

No random result.

Same plate configuration must always produce the same projection result.

---

# 19. TARGET TYPES

Support:

## REQUIRED TARGET

Must receive light.

## FORBIDDEN SENSOR

Must remain unlit.

Later:

## MULTI-TARGET

Several required targets.

Optional advanced mechanic:

## DUAL PROJECTOR

Two light sources evaluate the same stencil configuration.

---

# 20. COLOR ACCESSIBILITY

If different light filters are used:

Do NOT depend only on color.

Use:

- pattern
- symbol
- border
- shape
- icon

in addition to color.

---

# 21. LEVEL 0 — CALIBRATION

Level 0 is a dedicated interactive tutorial.

Display name:

# CALIBRATION

Teach:

1. select stencil
2. slide stencil
3. recognize alignment markers
4. rotate stencil
5. inspect overlap
6. press PROJECT
7. activate target
8. understand success

Do not use long text screens.

Use contextual visual guidance.

---

# 22. LEVEL 0 FLOW

Step 1:

Highlight Plate A.

Instruction:

`Select the stencil.`

Step 2:

Highlight correct movement direction.

Instruction:

`Slide it onto the registration mark.`

Step 3:

Introduce rotation.

Instruction:

`Rotate the plate.`

Step 4:

Highlight PROJECT.

Instruction:

`Project the pattern.`

Step 5:

Target activates.

Display:

# CALIBRATION COMPLETE

Action:

# BEGIN PATTERN 01

Level 0 should not award normal performance ratings.

Tutorial completion must save.

Tutorial must remain replayable from the Guide.

---

# 23. LEVEL STRUCTURE

Use three collections of four levels.

# SET A — ALIGNMENT

Levels 1–4.

# SET B — OCCLUSION

Levels 5–8.

# SET C — PROJECTION

Levels 9–12.

---

# 24. LEVEL 1 — FIRST ALIGNMENT

Mechanics:

- one movable plate
- slide only
- one target

Purpose:

First independent puzzle.

---

# 25. LEVEL 2 — DOUBLE LAYER

Mechanics:

- two plates
- slide
- overlap

Both layers must align.

---

# 26. LEVEL 3 — QUARTER TURN

Introduce:

# ROTATE

Use a simple 90° reasoning puzzle.

---

# 27. LEVEL 4 — REGISTRATION

Combine:

- two layers
- slide
- rotation
- multiple alignment marks

First mastery puzzle.

---

# 28. LEVEL 5 — SHADOW BLOCK

Introduce blockers.

One stencil must create the path.

Another must block unwanted light.

---

# 29. LEVEL 6 — FORBIDDEN SENSOR

Required:

- Target A illuminated
- Target B illuminated
- Sensor C remains dark

Introduce forbidden-sensor logic.

---

# 30. LEVEL 7 — REVERSE FACE

Introduce:

# FLIP

Reverse side contains different geometry.

Player must infer which side is needed.

---

# 31. LEVEL 8 — PRECISE REGISTER

Combine:

- slide
- rotate
- flip
- blockers
- required targets

Introduce move efficiency.

---

# 32. LEVEL 9 — PARALLAX

Introduce controlled layer-depth behavior.

The concept should remain visually understandable.

Do not implement physically inaccurate complicated optics.

Use deterministic designed offsets.

---

# 33. LEVEL 10 — FILTER PASS

Introduce light/filter categories.

Do not rely on color alone.

Use matching symbols/patterns.

---

# 34. LEVEL 11 — DUAL PROJECTION

Use two projectors.

Same stencil arrangement influences both.

Both projected results must satisfy their targets.

Keep UI clear.

---

# 35. LEVEL 12 — MASTER PLATE

Final challenge.

Combine selected advanced systems:

- slide
- rotation
- flip
- blocking
- multiple targets
- forbidden sensors
- dual projection

Do not force every mechanic into one puzzle if it damages clarity.

Level 12 must feel deliberate, not merely larger.

---

# 36. DATA-DRIVEN LEVEL ARCHITECTURE

Create something similar to:

`StencilLevelDefinition`

Fields:

- level ID
- display title
- set
- board dimensions
- stencil definitions
- initial stencil states
- allowed slide positions
- allowed rotations
- flip permissions
- target positions
- forbidden sensors
- filters
- projector configuration
- optimal configuration
- move par
- tutorial data
- visual modifiers

Do not implement levels using giant switch statements.

---

# 37. LEVEL VALIDATOR

Create Editor validation.

Validate:

- IDs 00–12 exist
- unique IDs
- valid stencil references
- valid positions
- valid rotation values
- valid flip configuration
- valid targets
- valid projectors
- valid solution
- optimal state
- move par
- Level 0 tutorial flag
- Level 11 Dual Projection configuration

Expected final:

# 13 / 13 VALID

---

# 38. SCENE ARCHITECTURE

Use:

```text
Bootstrap
LightStudio
ProjectionTable

```

Do NOT use:

- MainWorkshop
- Gameplay
- ControlRoom
- AutomationFloor

Scene names should reinforce Stencil Relay's independent identity.

---

# 39. BOOTSTRAP

Responsibilities:

- initialization
- save
- settings
- audio
- scene flow
- persistent session state

Keep minimal.

---

# 40. LIGHT STUDIO — MAIN MENU

The main menu must be visually unique.

Do NOT use:

- machine control console
- workbench
- job files
- card archive
- printer
- industrial dashboard
- giant vertical stack of rectangular buttons

Create:

# ILLUMINATED DESIGN STUDIO

Environment:

- large glowing drafting table
- translucent stencil rack
- brass registration ruler
- geometric templates
- magnifying lens
- calibration lamp
- glass storage slots
- soft architectural background

Camera:

slightly elevated diagonal angle looking across the table.

---

# 41. MAIN MENU UI STYLE

The interface should be integrated into the optical workspace.

### BEGIN ALIGNMENT

Primary interaction should be the physical table lamp/projector switch.

Text appears subtly near the illuminated table.

---

### PATTERN RACK

Access level selection by interacting with a vertical rack of translucent stencil sheets.

---

### CALIBRATION GUIDE

A slim technical reference booklet or transparent overlay sheet.

---

### LIGHT SETTINGS

Access settings through physical brightness/audio control knobs placed along the table edge.

Avoid a conventional menu.

---

# 42. PATTERN SELECT — STENCIL RACK

Do NOT use:

- rectangular level grid
- card vault
- ticket list
- chapter tabs
- scrollable generic cards

Create a:

# PHYSICAL STENCIL STORAGE RACK

Three physical trays:

### TRAY A

ALIGNMENT

Levels:

01
02
03
04

### TRAY B

OCCLUSION

Levels:

05
06
07
08

### TRAY C

PROJECTION

Levels:

09
10
11
12

Each level is represented as a translucent plate partly visible in its storage groove.

Selecting it:

- slides the plate outward
- illuminates edge markings
- shows level name/rating
- enables Begin Pattern

---

# 43. COMPLETION RATINGS

Use:

# ALIGNED

Completed successfully.

# PRECISE

Completed within target move count.

# PERFECT REGISTER

Optimal configuration.

Do not use:

- stars
- Verified
- Clean Proof
- Master Proof
- Accepted
- Perfect Program

---

# 44. PROJECTION TABLE SCENE

Suggested hierarchy:

```text
ProjectionTable
├── Systems
│   ├── GameState
│   ├── LevelController
│   ├── LayerStackController
│   ├── ProjectionController
│   ├── SaveController
│   └── AudioController
│
├── CameraRig
│
├── Lighting
│
├── StudioEnvironment
│   ├── LightTable
│   ├── TableFrame
│   ├── ProjectorUnit
│   ├── RegistrationRails
│   ├── BrassAlignmentPins
│   ├── LensAssembly
│   ├── StencilStorage
│   ├── CalibrationRulers
│   └── DecorativeOpticalTools
│
├── StencilWorkspace
│   ├── LayerSlot_A
│   ├── LayerSlot_B
│   ├── LayerSlot_C
│   ├── LayerSlot_D
│   └── RegistrationSurface
│
├── ProjectionSurface
│   ├── RequiredTargets
│   ├── ForbiddenSensors
│   ├── ProjectionMask
│   └── Effects
│
├── VFX
│
├── Audio
│
└── UI
    └── SafeArea
        ├── LayerSelector
        ├── ManipulationControls
        ├── ProjectionControl
        ├── MoveCounter
        ├── TutorialOverlay
        ├── PauseOverlay
        ├── ResultOverlay
        └── DialogLayer

```

---

# 45. GAMEPLAY UI MUST BE DIFFERENT

Stencil Relay must NOT use:

- Punchcard's left Status Rail
- Punchcard's right Tool Dock
- Misprint's conventional top HUD
- Misprint's correction tray

Use a more spatial, minimal layout.

---

# 46. LAYER SELECTOR

Place active stencil layers as:

# THIN VERTICAL GLASS TABS

along the LEFT EDGE OF THE LIGHT TABLE itself.

Example:

```text
A
B
C

```

When selected:

- plate lifts slightly
- tab illuminates
- plate border glows subtly
- layer name becomes visible

No large status panel.

---

# 47. MANIPULATION CONTROLS

Instead of a right-side tool dock:

Use physical controls embedded into the table frame.

Recommended:

### ROTATE

Circular brass dial on upper-right table frame.

### FLIP

Small dual-sided toggle beneath the rotation dial.

### RESET LAYER

Small recessed button.

Movement itself occurs directly by dragging/sliding the stencil.

---

# 48. PROJECT CONTROL

Place at:

# CENTER OF LOWER TABLE FRAME

Physical lamp switch / optical shutter.

Label:

# PROJECT

When activated:

- table changes lighting
- room dims slightly
- projection begins

This should visually anchor the gameplay.

---

# 49. MOVE COUNTER

Keep minimal.

Use a small mechanical/optical counter engraved into the table frame.

Example:

`MOVES 04`

Do not use a large panel.

---

# 50. NO TRADITIONAL HUD BAR

Do not create a full-width top HUD.

The puzzle workspace should dominate the screen.

Place Pause as a small physical icon/control near the frame edge.

---

# 51. PAUSE UI

Make this different again.

Use:

# FROSTED GLASS CALIBRATION OVERLAY

When paused:

a translucent frosted plate slides over the workspace.

Options:

- Resume Alignment
- Restart Pattern
- Calibration Guide
- Return to Studio

Do not use:

- clipboard
- machine service panel
- paper sheet

---

# 52. SETTINGS UI

Use:

# OPTICAL CONTROL STRIP

embedded along the studio table edge.

Settings:

### MUSIC

small linear light-intensity slider

### SOUND

second linear slider

### HAPTICS

physical click toggle

### RESET PROGRESS

small protected recessed control

### CREDITS

engraved maker plaque

Avoid knobs identical to Punchcard Operator.

---

# 53. SUCCESS

Do not show:

- YOU WIN
- PROGRAM ACCEPTED
- PROOF APPROVED

Use:

# PATTERN LOCKED

Sequence:

1. light stabilizes
2. targets glow
3. registration points illuminate
4. selected stencil settles
5. rating mark appears

Display:

- Aligned
- Precise
- Perfect Register

Actions:

- Next Pattern
- Reconfigure
- Pattern Rack

---

# 54. FAILURE / UNSUCCESSFUL PROJECTION

Avoid harsh Game Over behavior.

Use:

# MISALIGNED

Possible causes:

- Target Unlit
- Forbidden Sensor Lit
- Projection Incomplete
- Filter Mismatch
- Second Projector Failed

Allow:

# ADJUST LAYERS

The player should return immediately to manipulation.

Optional:

# RESET PATTERN

---

# 55. FINAL COMPLETION

After Level 12:

Display:

# MASTER ALIGNMENT COMPLETE

Studio response:

- all three pattern racks illuminate
- projection table reveals a final composed geometric pattern
- calibration ruler lights activate
- subtle completion tone
- final achievement mark appears

Actions:

- Review Patterns
- Return to Studio

---

# 56. UI ART DIRECTION

UI must feel:

- quiet
- optical
- architectural
- tactile
- precise
- refined
- minimal
- physical

Avoid:

- industrial control-console heaviness
- chunky metal panels
- giant buttons
- generic blue cards
- neon gradients
- hypercasual bounce
- cartoon panels
- overly futuristic holograms
- glassmorphism that looks like a web dashboard

This is a physical optical workspace, not a digital dashboard.

---

# 57. COLOR SYSTEM

Use a clearly distinct palette.

Suggested:

- warm ivory
- translucent neutral glass
- deep blue-black
- pale cyan projected light
- muted brass
- restrained coral for warning
- pale sage for valid alignment
- subtle gray shadows

Do not copy the palettes of previous projects.

---

# 58. MATERIAL SYSTEM

Create:

- translucent acrylic stencil
- frosted glass
- matte black mask material
- brass pins
- brushed aluminum frame
- warm off-white drafting surface
- clear optical glass
- soft rubber grips

Mobile optimize all materials.

Avoid expensive unnecessary transparency layers.

---

# 59. LIGHTING

Lighting is essential.

Requirements:

- stencil geometry readable
- projected light visible
- translucent layers understandable
- shadows soft
- workspace clear
- UI readable

Use efficient mobile-friendly lighting.

Do not create dozens of realtime lights.

---

# 60. PROJECTION EFFECT

Create a polished but efficient visual effect.

When projection begins:

- projector source brightens
- openings glow
- projected path/pattern becomes visible
- target sensors respond
- invalid exposure visibly identifies the problem

Do not depend on an expensive physically correct ray-tracing solution.

Use deterministic masks/material logic optimized for mobile.

---

# 61. CAMERA

Fixed camera.

No free rotation.

Use subtle camera feedback for:

- successful plate snap
- PROJECT activation
- Perfect Register

Avoid strong shake.

---

# 62. STENCIL ANIMATIONS

Create:

- select/lift
- slide
- snap
- rotate
- flip
- layer settle
- registration pin
- projection activation
- target illumination
- result transition
- pattern rack slide

Animations should feel precise, not bouncy.

---

# 63. ORIGINAL AUDIO

Create unique license-safe sounds.

Prefer:

# PROJECT-GENERATED ORIGINAL AUDIO

Required:

- acrylic plate pickup
- plate slide
- plate snap
- alignment pin click
- rotation mechanism
- flip
- table touch
- projector switch
- projector startup
- subtle light hum
- target activation
- forbidden sensor warning
- success chime
- misalignment cue
- rack slide
- pause overlay
- settings control
- final completion

---

# 64. MUSIC / AMBIENCE

Use subtle original ambience if appropriate.

Possible direction:

- low room tone
- faint optical/electrical hum
- minimal soft tonal pulses

Do not use recognizable music.

Puzzle solving must remain calm.

---

# 65. AUDIO PROVENANCE

Create:

`Assets/Documentation/AUDIO_PROVENANCE.md`

Track:

```text
Filename:
Purpose:
Creation method:
Source:
License:

```

Project-generated assets:

```text
Source: Project-generated original
External dependency: None

```

---

# 66. ASSET PROVENANCE

Create:

`Assets/Documentation/ASSET_PROVENANCE.md`

Track:

- fonts
- icons
- textures
- meshes
- sprites
- audio
- external assets

---

# 67. TYPOGRAPHY

Use TextMeshPro.

Choose legally safe fonts.

Visual direction:

- elegant technical heading font
- highly readable neutral body font

Do not reuse the exact typography combination from previous games.

Maximum approximately two main typefaces.

---

# 68. RESPONSIVE UI

Test:

- 16:9 landscape phone
- modern iPhone landscape
- wider modern phone
- standard iPad
- iPad Pro-like ratio

Check:

- stencil workspace size
- controls
- layer tabs
- project control
- text
- safe areas
- result overlays
- pattern rack

No clipping.

---

# 69. SAFE AREA

Support:

- notch
- Dynamic Island
- rounded corners
- home indicator

Critical controls must remain safe.

---

# 70. TOUCH TARGETS

Ensure controls remain comfortable for touch.

Do not make:

- tiny rotation controls
- tiny layer tabs
- tiny pause icons
- tiny reset controls

---

# 71. AUDIO MIXER

Create:

- Master
- Music
- SFX
- UI

Settings must control actual mixer parameters.

---

# 72. HAPTICS

Use restrained haptics for:

- stencil snap
- rotation step
- alignment pin
- Project
- Perfect Register
- forbidden sensor

Respect user setting.

---

# 73. SAVE SYSTEM

Versioned local save.

Persist:

- tutorial completion
- unlocked patterns
- completion
- Aligned rating
- Precise rating
- Perfect Register rating
- best move count
- audio settings
- haptics

Handle:

- missing save
- corrupt save
- incompatible future version
- reset

---

# 74. PROGRESSION

Initial:

Level 0 available.

First-time player completes Calibration.

Then unlock Level 1.

Sequential unlock:

1 → 2 → 3 → ... → 12

Completed levels remain replayable.

---

# 75. RESET PROGRESS

Requires confirmation.

Use wording such as:

# RESET PATTERN HISTORY?

`All completed patterns and ratings will be removed.`

Actions:

- Reset
- Cancel

---

# 76. GAME ICON

Create a completely original icon.

Concept:

Three overlapping translucent stencil layers creating one bright geometric projected opening.

Use:

- bold overlapping shapes
- one strong light source
- clear silhouette
- minimal details
- no tiny text

Requirements:

- 1024×1024
- no final alpha/transparency for App Store icon
- readable at 60px
- unique from previous icons

Inspect at:

- 180
- 120
- 60

---

# 77. BUNDLE IDENTIFIER

If the project does not already contain an intentional legitimate identifier, use:

`com.rehmanmughal.stencilrelay`

Product Name:

`Stencil Relay`

Version:

`1.0.0`

Build:

`1`

---

# 78. iOS SETTINGS

Configure:

- landscape
- IL2CPP
- ARM64
- icon
- safe area
- frame-rate target
- build scenes
- graphics settings
- appropriate stripping
- production configuration

No unnecessary permissions.

---

# 79. NO SENSITIVE PERMISSIONS

Do not request:

- camera
- microphone
- photos
- location
- contacts
- Bluetooth
- ATT
- tracking

---

# 80. OFFLINE AUDIT

Search project for:

- UnityWebRequest
- HttpClient
- sockets
- networking
- Firebase
- ads
- analytics
- telemetry
- tracking
- remote config
- cloud
- web views
- APIs

Expected:

# PASS — NO RUNTIME NETWORK FUNCTIONALITY

Create:

`Assets/Documentation/OFFLINE_AUDIT.md`

---

# 81. PACKAGE AUDIT

Inspect installed packages.

Remove unnecessary runtime packages.

Create:

`Assets/Documentation/PACKAGE_AUDIT.md`

Document remaining package purpose.

---

# 82. AUTOMATED TESTS

Create tests for:

- stencil movement
- snap logic
- rotation
- flip
- layer composition
- target evaluation
- forbidden sensor evaluation
- projection result
- level success
- failure
- move count
- rating
- dual projector
- save system

---

# 83. LEVEL VALIDATION

All 13 stages must validate.

Expected:

# 13/13 PASS

---

# 84. MANUAL LEVEL QA

For Level 0 verify:

- first launch
- tutorial guidance
- selection
- slide
- rotate
- Project
- success
- save
- replay

For Levels 1–12 verify:

- loads correctly
- stencil configuration correct
- intended solution works
- wrong solution fails correctly
- reset works
- pause works
- resume works
- rating works
- unlock works
- save works
- replay works

Expected:

# 13 / 13 MANUAL PASS

Create:

`Assets/Documentation/LEVEL_QA.md`

---

# 85. UI QA

Visually inspect actual Game View.

Screens:

- Light Studio
- Pattern Rack
- Level 0
- Projection Table
- layer selector
- rotation controls
- Project control
- Calibration Guide
- pause
- settings
- success
- misalignment
- reset confirmation
- final completion

Create:

`Assets/Documentation/UI_QA.md`

---

# 86. NO RUNTIME-UI AUDIT

Search for suspicious production UI generation such as:

```csharp
new GameObject(...)
AddComponent<Button>()
AddComponent<Image>()
AddComponent<TextMeshProUGUI>()

```

Dynamic gameplay objects are acceptable where justified.

Primary production UI must remain scene/prefab authored.

Document result.

---

# 87. PERFORMANCE

Target smooth mobile performance.

Avoid:

- excessive transparent overdraw
- too many realtime lights
- excessive material variants
- per-frame allocations
- repeated Find calls
- uncontrolled particles
- unnecessary physics
- expensive projection simulation

Profile Level 11 and Level 12 especially.

---

# 88. ORIGINALITY AUDIT

Before final completion ask:

## Core Mechanic

Does the game clearly revolve around layer alignment?

## UI

Does it look visually distinct from Misprint Department?

## UI

Does it look visually distinct from Punchcard Operator?

## Navigation

Is the Pattern Rack unique?

## Environment

Does the Light Studio have its own identity?

## Gameplay layout

Does the light table dominate rather than generic panels?

## Ratings

Are terminology and ratings unique?

## Audio

Does it have its own sound identity?

## Icon

Is it independent?

## Content

Are all 12 main levels meaningful?

## Copycat risk

Did we independently author visuals, level layouts and interactions?

Fix any weakness before completion.

---

# 89. DOCUMENTATION

Maintain:

```text
Assets/Documentation/
├── ARCHITECTURE.md
├── GAME_DESIGN.md
├── LEVEL_DESIGN.md
├── LEVEL_QA.md
├── UI_UX_SYSTEM.md
├── UI_QA.md
├── AUDIO_PROVENANCE.md
├── ASSET_PROVENANCE.md
├── OFFLINE_AUDIT.md
├── PACKAGE_AUDIT.md
├── MOBILE_RELEASE_CHECKLIST.md
└── FINAL_AUDIT.md

```

---

# 90. UI\_UX\_SYSTEM.md

Explicitly document:

- Light Studio design
- Pattern Rack
- Projection Table
- Layer Selector
- manipulation controls
- Project control
- frosted pause overlay
- optical settings strip
- typography
- palette
- materials
- responsive rules
- safe area
- animation principles

Also explicitly state how Stencil Relay differs from:

- Misprint Department
- Punchcard Operator

---

# 91. GIT RULES

If Git already exists:

make frequent meaningful LOCAL commits.

Examples:

```text
bootstrap: establish Stencil Relay persistent project memory

feat: implement deterministic stencil layer system

feat: add projection evaluation

ui: build Light Studio and Pattern Rack

feat: complete dual projector puzzle

audio: integrate original optical interaction sounds

qa: validate all thirteen stages

```

Do NOT push remotely.

Do NOT initialize Git unless explicitly authorized.

---

# 92. DEVELOPMENT LOOP

Continuously:

# READ MEMORY

# INSPECT

# PLAN NEXT MILESTONE

# IMPLEMENT VIA MCP

# UPDATE SUMMARY BANK

# COMPILE

# TEST

# VISUALLY INSPECT

# FIX

# RETEST

# DOCUMENT

# COMMIT IF APPROPRIATE

# CONTINUE

Do not merely report fixable problems.

Fix them.

---

# 93. VERTICAL SLICE GATE

Do not immediately build all Levels 1–12.

First complete:

# Level 0 + Level 1 + Level 2

Complete flow:

```text
Launch
→ Calibration
→ Light Studio
→ Pattern Rack
→ Pattern 01
→ manipulate stencil
→ Project
→ success/misalignment
→ save
→ Pattern 02
→ return to Light Studio

```

Before scaling Levels 3–12 perform:

- UI originality review
- interaction review
- projection readability review
- audio review
- responsive UI review
- performance review

Fix foundation issues first.

---

# 94. FINAL CONTENT COMPLETION

Required:

- Level 0
- Levels 1–12
- three pattern sets
- 13/13 validation
- 13/13 manual QA
- final completion state

No extra filler stages are required.

---

# 95. COMPLETION GATE

## FOUNDATION

-  MCP verified
-  AI\_RULES.md
-  PROJECT\_CONTEXT.md
-  SUMMARY\_BANK.md
-  CLAUDE\_MASTER\_PROMPT.md
-  documentation structure

## GAMEPLAY

-  stencil slide
-  snap
-  rotate
-  flip
-  layer composition
-  blockers
-  required targets
-  forbidden sensors
-  projection
-  dual projector
-  result evaluation

## CONTENT

-  Level 0 complete
-  Levels 1–12 complete
-  all meaningful
-  all solvable
-  13/13 validation
-  13/13 manual QA
-  Level 12 final challenge complete

## UI/UX

-  Light Studio polished
-  Pattern Rack polished
-  Projection Table polished
-  Layer Selector polished
-  manipulation controls polished
-  Project control polished
-  tutorial polished
-  pause polished
-  settings polished
-  guide polished
-  success polished
-  misalignment polished
-  final state polished
-  no dead controls
-  no placeholders
-  responsive phone layouts
-  responsive tablet layouts
-  safe area
-  text alignment
-  touch targets
-  no runtime-generated main UI

## DIFFERENTIATION

-  no Misprint Department UI reuse
-  no Punchcard Operator UI reuse
-  unique Light Studio
-  unique Pattern Rack
-  unique gameplay screen composition
-  unique ratings
-  unique typography
-  unique palette
-  unique environment
-  unique icon
-  unique audio identity

## ENVIRONMENT

-  Light Studio complete
-  Projection Table complete
-  stencil materials polished
-  projector polished
-  lighting polished
-  camera polished
-  environment visible in hierarchy

## AUDIO

-  plate sounds
-  projector sounds
-  UI sounds
-  success/misalignment
-  ambience if used
-  final completion audio
-  mixer
-  functional settings
-  provenance
-  license safe

## SAVE

-  tutorial persists
-  unlocks persist
-  ratings persist
-  settings persist
-  reset works
-  corrupt save handled

## MOBILE

-  correct product name
-  bundle ID
-  landscape
-  IL2CPP
-  ARM64
-  icon
-  safe area
-  production build/export
-  no unnecessary permissions

## OFFLINE

-  no network
-  no ads
-  no analytics
-  no telemetry
-  no tracking
-  no cloud
-  no remote config
-  no login
-  no third-party runtime SDK

## QA

-  automated tests
-  level validator
-  save tests
-  level QA
-  UI QA
-  audio QA
-  performance
-  Console clean
-  production build succeeds

---

# 96. FINAL\_AUDIT.md

Produce:

```text
STENCIL RELAY FINAL AUDIT

Core Stencil System: PASS/FAIL
Layer Composition: PASS/FAIL
Projection System: PASS/FAIL
Level 0 Tutorial: PASS/FAIL
12 Main Levels: PASS/FAIL
13/13 Level Validation: PASS/FAIL
13/13 Manual Level QA: PASS/FAIL
Dual Projection: PASS/FAIL
Final Level 12: PASS/FAIL

Light Studio UI: PASS/FAIL
Pattern Rack UI: PASS/FAIL
Projection Table UI: PASS/FAIL
Responsive UI: PASS/FAIL
Safe Area: PASS/FAIL
UI Originality: PASS/FAIL
Different from Misprint Department: PASS/FAIL
Different from Punchcard Operator: PASS/FAIL

Environment: PASS/FAIL
Lighting: PASS/FAIL
Projection VFX: PASS/FAIL
Animations: PASS/FAIL
Audio: PASS/FAIL
Audio Licensing: PASS/FAIL
Asset Provenance: PASS/FAIL

Save System: PASS/FAIL
Offline Audit: PASS/FAIL
Third-Party SDK Audit: PASS/FAIL
Package Audit: PASS/FAIL

Icon: PASS/FAIL
Mobile Settings: PASS/FAIL
Performance: PASS/FAIL
Automated Tests: PASS/FAIL
Console: PASS/FAIL
Build/Export: PASS/FAIL
Documentation: PASS/FAIL

Known Release-Blocking Bugs:
NONE / list

```

Any required FAIL means:

# PROJECT STATUS = IN\_PROGRESS

---

# 97. TRUE EXTERNAL BLOCKERS

Only stop for genuine external blockers such as:

- unavailable signing certificate
- unavailable Apple Developer credentials
- OS-level limitation beyond available tools

These are NOT external blockers:

- bad UI
- missing levels
- compiler errors
- broken projection
- missing sounds
- poor lighting
- clipping
- test failures
- incomplete icon
- failed validation
- broken save

Fix those.

---

# 98. DEFINITION OF COMPLETE

Stencil Relay is complete when a player can:

- install the game
- complete Calibration
- enter the Light Studio
- browse the physical Pattern Rack
- play Levels 1–12
- manipulate physical stencil layers
- slide
- rotate
- flip
- project patterns
- satisfy required targets
- avoid forbidden sensors
- solve Dual Projection
- complete Level 12
- earn ratings
- replay levels
- retain progress
- adjust real audio/haptic settings
- experience polished original audio
- play without internet
- complete the final Master Alignment state

and all required QA/build/documentation gates pass.

---

# 99. BEGIN DEVELOPMENT

Start now:

1. inspect repository
2. verify it is the Stencil Relay project
3. verify Unity version
4. verify MCP
5. inspect packages
6. create AI\_RULES.md
7. create PROJECT\_CONTEXT.md
8. create SUMMARY\_BANK.md
9. save this prompt to CLAUDE\_MASTER\_PROMPT.md
10. establish project folders
11. implement deterministic stencil model
12. implement layer composition
13. implement projection evaluation
14. create Bootstrap
15. create LightStudio
16. create ProjectionTable
17. create Level 0
18. create Levels 1–2
19. complete polished vertical slice
20. perform originality/UI/UX audit
21. fix weaknesses
22. create Levels 3–12
23. complete environment
24. complete animations/VFX
25. complete original audio
26. configure local save
27. configure mobile release
28. perform full QA
29. run offline/package audits
30. create final build/export
31. continue fixing until all completion gates pass

Do not answer with a plan only.

Do not merely describe how the game could be built.

Use Unity MCP and available tools to actually build the game.

# BUILD STENCIL RELAY FROM SCRATCH TO RELEASE-QUALITY COMPLETION.