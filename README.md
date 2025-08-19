# Match-3 Prototype Challenge — Gazeus Games Developer Assessment

This repository contains my work for a **one-week developer assessment** given by **Gazeus Games**. The challenge is based on a barebones Match-3 puzzle game, and my task was to **expand and improve it** with new features, mechanics, and polish.

The project will be documented and updated incrementally as I commit files, so anyone reviewing it can see the evolution of the work.

## 📜 Challenge Context

* **Type:** Technical test for a game developer role.
* **Studio:** Gazeus Games.
* **Duration:** 1 week.
* **Starting point:** A functioning Match-3 board that already detects and removes matches of 3+ pieces.
* **Main task:** Improve the game however I choose.

### What They Provided

* Working grid-based board. The board itself looks okay but everything else was stripped away; there are no other elements on the screen whatsoever.
* Pieces that fall from above to fill empty spaces.
* Match detection and piece removal for groups of 3+ of the same color.

### What They Asked For
They gave examples of potential improvements:

* **Scoring:** Implement a points system.
* **Advanced mechanics for matches above 3:**
    * Clear entire rows/columns.
    * Trigger area-of-effect explosions.
    * Remove all pieces of a single color.
* **Visual feedback:**
    * Animations for matches.
    * Explosions.
    * Hints for possible moves.
* **Progression systems:**
    * Objectives.
    * Limited lives or move counts.
    * Missions.
* **Gameplay variations:**
    * Match in different shapes (e.g., squares, L-shapes).
    * Match of 6 pieces.
    * Rotate 4 pieces instead of swapping 2.
    * Dead zones/blocked tiles in levels.

From my interpretation, the idea is to **iterate creatively**, not just implement one mechanic after the other.

### Rules
They gave a list of rules for this challenge:
* **Environment:** The game must be developed in Unity and C#.
* **Delivery:** It must be a **pull request**.
* **Coding:** Pertinent inline comments are allowed.

## 📊 What Is Being Evaluated

Your submission will be assessed based on the following factors:

1. **Logical Reasoning**
    - The ability to present a clear, consistent line of thought when solving problems.
    - Solutions should demonstrate a logical flow from problem understanding to implementation.

2. **Correctness of Proposed Solutions**
    - The solution must meet the requirements of the challenge.
    - All features should work as expected without breaking existing functionality.

3. **Adherence to Development Best Practices**
    - Code should follow established software engineering principles such as maintainability, scalability, and reusability.
    - Avoiding anti-patterns and unnecessary complexity is key.

4. **Unity & C# Conventions and Standards**
    - Code must comply with Unity development guidelines (e.g., naming conventions, component usage, scene organization).
    - C# style conventions should be respected, including naming, formatting, and structure.

5. **Code Organization**
    - Code should be structured in a way that is easy to navigate and maintain.
    - Clear separation of responsibilities, meaningful file/folder organization, and modular design are probably expected.

6. **Code Readability**
    - Code should be easy for others to read and understand.
    - Use descriptive names, consistent formatting, and appropriate comments/documentation.

7. **Performance**
    - Solutions should be efficient and avoid unnecessary resource usage.
    - Optimizations should be balanced with readability and maintainability.

Of course, this is **my** interpretation of what is being evaluated. Evaluation is never 100% objective. 

## Credits
* **Tiles:** imbusdev / Georg Eckert
* **SpaceBackground:** Digital Moons https://digitalmoons.itch.io/\
* **Font:** Grisly Beast by nhsfonts
* **Music:** Dvir Silverstone from Pixabay

## 📅 Development Log

This section will be updated as commits are made.

### **Day 1 — Gaining Ground**

#### **First Impressions Of The Challenge**
After reviewing the provided project and instructions, here are my first impressions:

1. **Unity Version & UI System**
    - The challenge is built on **Unity 2022** with the **legacy component-based UI system**. With Unity 6.1 stable and **UI Toolkit** available, this feels outdated for a task meant to test code quality and scalability.
    - UI Toolkit, despite its current CSS limitations, enables a more code-driven, maintainable, and dependency-injection-friendly approach. It fits SOLID principles better in C#, produces cleaner code, and integrates more cleanly with Git versioning.
    - While I understand this may reflect Gazeus Games’ current production environment, an interview challenge could benefit from using modern, scalable tech. Given the one-week time limit and the fact that the provided code already uses the old UI, replacing it is not my top priority.

2. **Vagueness of the Instructions**
    - The brief was intentionally open (“improve the game however you want”), but this openness introduces **bias risks** in evaluation.
    - Without clear scoring criteria or priorities, developers may focus on different areas — some on code architecture, others on visual polish. This can result in uneven comparisons and skewed perceptions of skill.
    - For fair assessment, the challenge could define:
        - Exact scoring breakdown (e.g., 60% code quality, 20% features, 20% visuals)
        - Performance targets (e.g., 60 FPS on mobile, memory usage thresholds)
        - Minimum baseline features to implement
    - They gave me a project that already uses outdated tech (old UI system) and didn’t clarify whether I should refactor, work around it, or ignore it. This means two candidates could be judged differently for making opposite choices. All are valid strategies, but without clear guidance, the choice itself becomes a hidden evaluation criterion.
    - Most of the suggested improvements — explosions, animations, hints, special combos — are player-facing features, not architectural improvements. This subtly nudges candidates toward making the game prettier and more “fun-looking” instead of better-engineered.

3. **Evaluation Criteria Provided**
    - These are valid in theory but lack **specific, measurable definitions**. For example, “Performance” is unclear without knowing the target platform or metrics.
    - Saying “do whatever you want” sounds empowering but, in practice, forces me to **guess** what Gazeus value most.
    - The best coding assessments usually mix clear must-haves with a little room for creativity.
    - It’s likely performant enough already on desktop, so any optimization could be wasted effort unless they specify a mobile target.

4. **No Testing or Documentation Requirement**
    - Not asking for basic technical documentation is a missed opportunity.
    - They’re assessing my output, but not my ability to make code maintainable for a team. That’s odd if the role involves long-term game updates.

In practice, this may result in the best-looking and least-buggy game being favored, even if another developer has superior architecture or scalability in their code.
Selection might gravitate toward a submission that looks like a Gazeus-style game, consciously or not.

#### **First Impressions Of The Code**
The project follows a lightweight MVC-ish separation:

* **Model/Logic**: `GameService` holds board state and core rules.
* **View**: `BoardView` and `TileSpotView` render and animate tiles (Unity UI + DOTween).
* **Controller**: `GameController` orchestrates input → rules → animation cascades.
* **Data/DTOs**: `BoardSequence`, `MovedTileInfo`, `AddedTileInfo`, `Tile` act as simple data carriers between logic and view.
* **Assets/Repository**: `TilePrefabRepository` (`ScriptableObject`) exposes prefabs per tile type.

This yields a clear entry point and a clean gameplay loop for a prototype. However, state and responsibilities leak across layers, and several choices make testing, extensibility, and performance harder than needed once features grow (in my opinion).

#### **Probable Next Steps**
* **Get Initial Project Setup Out of The Way**
  * Add helpful plugins and tools.
  * Implement scene injection system to facilitate context management.
* **Tackle High Impact, Low Risk Changes**
  * Fix `FindMatches` init loop boundary (use newBoard[y].Count).
  * Add `OnDestroy` unsubscriptions in TileSpotView and BoardView.
  * Introduce enum `TileType` and change repository to map from enum → prefab.
  * Extract an `IRandom` and seed in `StartGame` for determinism.
  * Introduce an `IAnimationPlayer` (or `BoardAnimator`) that takes `BoardSequence` and returns a `UniTask`/callback; remove recursion in `AnimateBoard`.
  * Add a simple object pool for tiles; replace `Destroy` with `ReturnToPool`.
* **Implement Scoring System**
  * Scoring with multipliers per cascade depth and match length.
* **Unlock Advanced Mechanics**
  * **Rules Engine Layer**: Encapsulate detection into run-length analyses that return groups with metadata: direction, length, shape (row, column, L, T, square). This enables special-piece creation and context-aware scoring.
  * **Event Stream**: Emit `BoardEvent` records (e.g., MatchFound, TilesCleared, TilesDropped, TilesSpawned, SpecialCreated). Views subscribe and animate; scoring subscribes and accumulates.
  * **Config-Driven Tuning**: `BoardConfig` (`ScriptableObject`) for board size, tile set, spawn weights, gravity, cascade limits, target FPS/platform.
  * **Testing**: Pure C# tests for `FindMatches`, gravity, spawn rules, and deterministic seeds.

#### **Implementation**
* Forked the base repository.
* Created `development` branch for new features.
* Wrote initial README and challenge breakdown/planning.
* Installed `Addressables`, `UniTask`, `LeanPool` and `OdinInspector`.
* Created initial scene files.
* Implemented initial scene injection system.
  * **Scenes**: created Initialization, Main Menu, and Gameplay scenes; Initialization handles bootstrapping and scene transitions.
  * **GameInitializationContainer**: implemented container to initialize Addressables, load `GameConfig`, and manage scene loading/unloading for Main Menu and Gameplay.
  * **LoadSceneService**: added a service for async scene loading/unloading via Addressables, supporting loading screens, enums for scene keys, and error handling.
  * **MainMenuController**: implemented controller connecting `MainMenuView` events to `PlayRequested` and `ExitRequested` events.
  * **MainMenuView**: created serialized buttons with events (`PlayButtonClicked`, `ExitButtonClicked`).
  * **Configuration**: added `GameConfig` ScriptableObject with `SceneLoadInfo` for Main Menu and Gameplay; enums used instead of strings for scene keys.
  * **Development Notes**: followed SOLID principles (lacking interfaces), dependency injection, async/await with UniTask, and decoupled controllers, views, and scene loading.
  * **Known Limitations**: gameplay initialization is placeholder; loading screens have fixed delay; assumes single instance of view/controller per scene.

### **Day 2 (Aug 14th) — Codebase Preparation**
* **Prefab-Based UI for DI**: Switched MainMenu and LoadingScreen to addressable prefabs to remove scene refs and ease dependency injection.
* **Addressables Provider**: Added AddressablesAssetProvider (LoadAssetAsync/InstantiateAsync/Release) used across controllers.
* **Initialization Flow**: Implemented GameInitializationContainer to init Addressables, create services, load GameConfig, and bootstrap UI.
* **Loading Screen Controller**: Added LoadingScreenScreenController with async CanvasGroup fade (Show/Hide) and addressable instantiation.
* **Main Menu Controller**: Added MainMenuController that instantiates MainMenuView via enum key and raises Play/Exit events.
* **Gameplay Controller Refactor**: Introduced pure GameplayController (no MonoBehaviour) that spawns BoardView prefab and wires tile events.
* **Controller Lifecycle Service**: Added ControllerLoadService to standardize controller Initialize/Dispose across the app.
* **Gameplay Service Extraction**: Moved match-3 logic into GameplayService implementing IGameplayService (StartGame/IsValidMovement/SwapTile).
* **Config Access**: Introduced AddressablesAssetKeys.GameConfigKey and loading of IGameConfig from Addressables during startup.
* **Enum Keys**: Created GameplayViewKey, MainMenuViewKey, LoadingScreenViewKey, SceneKey for type-safe addressable lookups.
* **View Utility**: Added LoadingScreenView with cached CanvasGroup for fades.
* **Cleanup & Safety**: Ensured event unsubscription and Addressables.Release in Dispose paths for all instantiated views.
* **Scoring Stubs**: Added placeholder interfaces (IScoreCalculator/IScoreRule/IScoreService) with TODOs; no scoring implementation yet.
* **TileInfo System**:  Renamed `Tile` to `TileInfo` and replaced `int type` with enum `TileKey` for type-safe tile identification.
* **Board Prefab Injection**:  GameplayController now injects BoardCellView prefab and TileInfo prefabs via GameConfig, removing hard-coded prefab references.
* **TileKey Enum**:  Added enum `TileKey` with all tile types (Blue, Green, Orange, Pink, Purple, Red, Yellow) for consistency across the system.
* **IBoardCellView Interface**:  Introduced interface for `BoardCellView` to expose events, Transform access, and tile animation methods.
* **BoardCellView Implementation**:  Updated BoardCellView to implement `IBoardCellView`.
* **BoardView Refactor**:  Updated `BoardView` to use LeanPool for tile and board cell instantiation; now supports injected tile prefabs and prefab-based board cells.
* **CreateBoard Logic**:  BoardView.CreateBoard now assigns TileInfo types to prefabs, sets positions, and wires click events using injected prefabs.
* **Tile Creation & Animation**:  BoardView.CreateTile uses injected tile prefabs with LeanPool and animates appearance using DOTween sequences.
* **Tile Movement & Swap**:  BoardView.MoveTiles and SwapTiles updated to animate tiles between BoardCellViews using AnimatedSetTile, keeping _tiles state consistent.
* **LeanPool Integration**:  Replaced direct Instantiate/Destroy calls with LeanPool.Spawn/Despawn for efficient tile and board cell reuse.
* **TODOs Maintained**:  TileInfo still has placeholder properties for BackgroundColor and Icon; scoring system still not implemented.

### **Day 3 to 4 (Aug 15th - 16th) — Scoring System & Game Loop**
* **Tile Prefab Injection**: Switched to runtime async prefab loading (`AvailableTileKeys[]`) to avoid holding direct tile prefab references since system startup.
* **Tile Selection Feedback**: Added some visual QoL improvements to tile selection and game board. This will make testing mechanics later much easier.
* **Aesthetics:** Quickly came up with a style for the game. 
* **AudioController**: Added `AudioController` that loads audio clips and audio sources via `IAssetLoadService`, exposes `PlayMusic`/`PlaySfx`, and can register/unregister gameplay event handlers.
* **GameOverScreenController**: Added `GameOverScreenController` that instantiates `IGameOverScreenView`, sets end-game data, and exposes `ReplayRequested` and `MainMenuRequested` events.
* **GameOverScreenView**: Added view implementation for game-over UI with final score/time text and replay/back buttons wired to events.
* **GameEndResults & GameRuleConfig**: Added `GameEndResults` struct and `GameRuleConfig` struct to carry end-of-game data and rule configuration (score threshold, timer).
* **GameEndRuleFactory**: Added factory to create end-rule implementations from `GameRuleConfig`.
* **TimerRule (async rule)**: Implemented `TimerRule` as an async game-end rule that invokes a callback when time elapses.
* **ScoreService (updated)**: Added a project `ScoreService` with `ScoreUpdated` event, `SetScore(int)` and `CalculateSequenceScore(BoardSequence,int)` that updates current score and returns `BoardSequenceScoreInfo`.
* **Audio keys and loading**: AudioController loads clips based on `AudioKey` and uses configs stored in `AudioControllerConfig`.
* **Event wiring for gameplay**: AudioController provides `RegisterGameplayEvents`/`UnregisterGameplayController` to hook into gameplay lifecycle and tile/score events.
* **Closed main loop wiring**: Implemented flow to go Main Menu → Gameplay → Game Over → Replay or Main Menu via the added controllers and events.
* **Notes on asset unloading**: Game loop is closed but asset unloading/cleanup when returning to the main menu still needs finalization to make the experience fully customizable (ensure all controllers/views/audio sources are released).

### **Day 5 (Aug 17th) — Advanced Mechanics**
* **GameplayService → BoardEngine**: Interface renamed from IGameplayService to IBoardService.
* **GameplayService modularization**: Replaced monolithic gameplay code with `BoardService` delegating to small services (tile generation, board creation, move validation, tile swap).
* **DefaultBoardCreationService**: New board creation service that fills board avoiding initial matches and uses `ITileGenerationService`.
* **DefaultMatchFindService**: New match-finding service that returns the boolean match matrix used by swap logic.
* **DefaultMoveValidationService**: New service to validate moves and check for any available moves.
* **DefaultTileGenerationService**: New tile generator that assigns Id and random TileKey from available keys.
* **DefaultTileSwapService**: Refactored swap/cascade logic into tile swap service; it handles swapping, matching, dropping and refilling using injected services.
* **Safe defaults & DI-friendly constructors**: All services have default implementations so `BoardService` remains easy to construct while still fully pluggable for tests and future features.
* **Board State Creation**: Removed `ref` parameters from board services, replacing them with `IBoardState` and `IBoardSwapResult`.
* **Match Interfaces**: `IMatchFindService`'s output (`List<List<bool>>`) was simple but limiting; it loses grouping, match type, tile keys, and special tile data.
* **Modular Board Service Architecture**: Split gameplay board logic into composable services (tile generation, board creation, move validation, match finding, tile swapping).
* **Modular Match Finder Architecture**: The match finder system now supports a list of `ITileMatchRule`'s.

### **Day 6 (Aug 18th) — Advanced Tile Generation and Match Effects**
* **Weighted Tile Generation Upgrade**: Tiles are now generated by weight to support rarer tiles.
  * **Known Issue**: I noticed sometimes some tiles get stuck, much probably a bug introduced while refactoring the match finder service.
  * **Known Issue**: Also, these last changes made board creation much more complicated. For each rule, the board has to check if any tile would generate a match. This complexity increases with the amount of concurrent matching rules.
* **Board Effects**: The game now can trigger board effects (line clearing, square explosion, same-color wipe) that can be triggered by `ITileMatchRule` or `TileKey`.
  * **Known Issue**: `BoardEffectConfig` is not very customizable and kind of all over the place.
  * **Known Issue**: All effects simply assume the first matched position is the center of the effect. That can be weird to someone playing or to someone testing the game, but I went with this for simplicity as I'm close to the deadline (tomorrow);
* **Tile Explosion**: Added a generic tile explosion animation as a placeholder. The original idea was to make each tile explode in different ways. 
* **Tile Swap Service Refactor**: Split `DefaultTileSwapService` into more digestible functions to spot visual bug more easily.
* **Tile Generation**: Somewhere along the way I made the classic class/struct mistake with `TileInfo`, which duplicated tiles in the gane. They still happen from time to time though, so I guess I must have made a similar mistake somewhere else.

*(Future days will detail added features, fixes, and design choices.)*

##  Conclusion
#### **Questions Likely To Be Asked By Evaluators**
* **Why refactor the preexisting codebase?**
* **Why so many big commits?** 


