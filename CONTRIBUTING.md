# Contributing Guide

## Prerequisites

- [GitHub](https://github.com/) account
- [GitHub Desktop](https://desktop.github.com/)
- [Unity Hub](https://unity.com/download)
- Unity editor version **6000.0.024f1** — download from the [Unity Editor Archive](https://unity.com/releases/editor/archive) (select "All versions" to find it)

## Local Setup

1. Clone the [PharmacyGO repository](https://github.com/LucyCheng111/PharmacyGO) using GitHub Desktop
2. Open the project in Unity Hub and confirm the editor version is **6000.0.024f1**
3. In the Project window, navigate to `Assets/Scenes/Main Levels/` and open **Main Menu**
4. Press **Play** — if the game runs without errors, your setup is good

## Expected Contribution Workflow

### 1. Branch Naming

Keep branch names short, lowercase, and descriptive of what you're working on.

Examples:
- `feature/doctor-peek-ui`
- `fix/caption-text-display`
- `chore/cleanup-hub-scene`

### 2. Opening a Pull Request

1. Push your changes to your branch
2. Go to the [Pull Requests tab](https://github.com/LucyCheng111/PharmacyGO/pulls) and open a new PR
3. Assign relevant team members as reviewers (see [Team](#team) below)

### 3. What to Include in Your PR

- **Subject** — what you changed and why
- **How to test it** — exact steps a reviewer should follow to verify the outcome
- **Screenshots or recordings** if the change is visual

### 4. Definition of Done

- No compiler errors
- Tested as described in the PR and works as expected
- At least one team member has approved the PR

### 5. Code Review Expectations

- Leave comments on logic that isn't immediately obvious
- Flag anything that touches shared systems (Hub scene, DialogBox, BattleCanvas) — these affect multiple contributors
- Approve only if you actually tested it

## Reporting Bugs / Requesting Changes

[Open an Issue](https://github.com/LucyCheng111/PharmacyGO/issues) in the repository and include:

1. A clear description of the bug or requested change
2. Which file(s) are involved
3. The condition or steps that trigger it
4. Expected vs. actual behavior

## Version 2 Development Team

| Name | Area | GitHub | Email |
|---|---|---|---|
| Annmarie Geiger | Database Management | [@kyofyufufufufufufufu](https://github.com/kyofyufufufufufufufu) | geigerta@oregonstate.edu |
| Lucy Cheng | AI Development | [@LucyCheng111](https://github.com/LucyCheng111) | chengjuh@oregonstate.edu |
| Nick Shininger | Game Content Development, Minigames | [@shiningn-osu](https://github.com/shiningn-osu) | shiningn@oregonstate.edu |
| Jakob Poore | Level Design | [@poorej](https://github.com/poorej) | poorej@oregonstate.edu |
| Max Baker | Game Mechanics Development | [@Crimson-Ender](https://github.com/Crimson-Ender) | bakerm7@oregonstate.edu |

## Version 1 Development Team
 
| Name | Role |
|---|---|
| Jinpeng Chen | Programmer |
| Alec Duval | Database Manager |
| Quinn Glenn | Menu Designer |
| Xiaoyu Luo | Scene Developer |
| Teagan Simoneau | Project Manager |
| Hoimau Tan | Index Designer |
| Erik Tornquist | Database Programmer |
| Samuel Westerham | Music / Level Designer |

## Handoff & Support

This project was developed as a capstone at Oregon State University. For installation help, technical issues, feature requests, or general questions, submit a request via the [PharmacyGO Support & Feedback Form](https://forms.gle/NajcPYnEa8jS3CWN6). Submissions are monitored by the project manager and will be routed to the appropriate team or future development group.

For bugs in the codebase, please open a [GitHub Issue](https://github.com/LucyCheng111/PharmacyGO/issues) instead.

### Related Repositories

| Repository | Description |
|---|---|
| [PharmacyGO Unity Project](https://github.com/LucyCheng111/PharmacyGO) | Main Unity game |
| [Database Management App](https://github.com/kyofyufufufufufufufu/test_database1) | WinForms C# app for managing game database content |