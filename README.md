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
Overall, I see this challenge as an opportunity to balance **technical excellence** with **visual polish**, while being mindful of time allocation. 

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

### **Initial Setup**

* Forked the base repository.
* Created `development` branch for new features.
* Wrote initial README and challenge breakdown/planning.
* Installed `Addressables`, `UniTask`, `LeanPool` and `OdinInspector`.
* Created initial scene files.

*(Future days will detail added features, fixes, and design choices.)*

