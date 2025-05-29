# Known Issues and Future Improvements

## Known Issues
* An issue in the Unity editor where selecting “new game” continues from previous save until the game is stopped and started again.
 * In the browser client, selecting “new game” starts a new game immediately.
* An issue wherein player position is not saved when stopping and starting the game.
* An issue wherein current level is not saved when stopping and starting the game.
  * This leads to an exploit where a player can earn points and coins in a level, refresh the game, and start the level timer over while still retaining all their previous score and coins.
* An exploit wherein the player can start a level, return to the hub, visit a previous level, and earn points in the level they’re already beaten.
* A bug wherein players can challenge a boss even after they’ve beaten that boss.
  * Being able to repeat the boss challenge may be a desired feature, but doing so currently consumes coins.
* An issue wherein challenging a previous boss instead of the current boss can unlock the next level.
* An issue wherein movement using the joystick, usually seen in mobile versions, allows the player to move off the tile alignment.
  * This may not necessarily be a problem, but collisions with boundaries and interactables may display unexpected behavior when unaligned from the grid.
* An issue where music is missing from certain levels.
* An issue where the tiles on the GI map do not display correctly.
* An issue where the exclamation mark above the player does not display correctly on some maps.
* An issue where some of the “Spacebar” interact prompts do not display correctly on some maps.

## Future Improvements
### Major Improvements
* Adding level 5, “review” or “test” level
* Adding map for questions where the organ is marked “other”
* Map or minimap feature
* “Run” feature
* Ability for user to easily re-use images stored in database for other questions
* Rewards or incentives for map exploration
* Bosses asking difficult questions
  *This can either be a list of questions specifically for the boss of each level, or simply pulling from the hardest difficulty of questions in the area
* Improved instructions and tutorial signage


### Minor Improvements
* Consistent ability to progress or “skip” in dialogue and in battle
* Adding logic or pathing to each level
  * Organ levels are currently placed nearly randomly around the island, and main levels lack a clear path to each one
* Scoring improvements
* Unique music for each level
* Drop shadow or window for timer/score/UI for visual clarity
* More animated tiles
* NPC movement
* Leaderboard, or high scores
* Multiple saves
  * A player may wish to start a new game to try for a higher score, but would lose the ability to select any level for studying purposes. There may be other solutions to this issue
* Battle scene animations and varied backgrounds
* Consistency in boss placement within levels
  * Currently, some are easy to find, some are very far or even hidden among other NPCs

### Workflow Improvements
The following are suggestions that may not affect the final product, but may make production of the game easier.
* Add the ability to load into a level without needing to start at main menu
* Create a test database / test index
  * The in-development version of the game in Unity still relies on the live database and index from Dr. Philmus, meaning we can only test whatever questions and information he provides. For example, at current time, no questions for later modules exist to be tested
* Improve grid alignment
  * All objects are aligned manually to grid, and alignment on some objects doesn’t seem to match with other objects
