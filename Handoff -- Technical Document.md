# Project Overview

* **Purpose**  
  Help PharmD students to get practice via an exploration game where players answer quiz questions (“battles”) to progress 

* **Core Loop**  
  * **FreeRoam**: 2D map-based world exploration  
  * **Random Encounter**: Wild questions appear while walking or in dangerous zones  
  * **Battle**: Quiz with text or image options.  
  * **Reward:** Coins, Score Points, Boss Fight to unlock the next Level  
  * **Index UI:** Separate panel listing medication reference sheets

# Project Structure

![][image1]

# Scripts Structure

![][image2]

# Core System

* ## GameControl

  * Singleton managing overall game states: **FreeRoam, Battle, Dialogue.**  
  * Hooks into Unity’s *“SceneManager.sceneLoaded”* to rebind player/camera on scene load.  
  * StartBattle  
    * Switches cameras and UI panels  
    * Delegates to the BattleSystem to set up questions  
  * EndBattle  
    

* ## LevelManager

  * Connect all maps together  
  * Maintains “unlockedLevel” via “PlayerPrefs”  
  * Maps logical level numbers to Build Settings indices  
  * **LoadLevel** triggers asynchronous scene load

# Player Movement & Encounter

* **PlayerControl**  
  * Grid-based movement via coroutine, reading keyboard or virtual joystick input  
  * Detects collisions against solidObjects and interactable layers  
  * **CheckForEncounters()**: 50% chance in grass; boss zones flagged as “dangerous” with a lower opportunity.  
  * On interact key, raycasts for Interactable and calls its Interact()

* **FollowCam.cs**   
  * Follows the player with a fixed offset, recovers if the player reference is lost.

.

# Battle & Data

* **BattleSystem.cs**   
  * Setup Battle  
    * Clones and shuffles answers via ***ShuffleAnswers***()  
    * Displays question text (***QuestionSection***), optional image (***QuestionUnit***)  
    * Type intro dialog (“A wild question appeared\!”) with ***DialogBox***.  
        
  * PlayerAnswer  
    * HandleUpdate() invokes HandleAnswer() to read input

    

  * End  
    * Wait and then award coins and scores.  
    * Scores are awarded according to the difficulty and streak modifier  
    * On boss completion, it unlocks the next level and shows the ***levelCompletePanel***  
* **Remote Database**  
  * Database class loads a JSON file via GitHub API. Data was transformed into Questions


* MapArea Question Logic

# Level Creation
The way we have level creation set up is depended on from one person to another. However, we have used specific steps when we have created most of our levels to make them consistent.

* **Step 1: Make a copy of the level "_Scene Copies" and rename that copy to whatever you want it to be named.**
* We have a quick template that has the grid set up and functioning for level creation.
* It has some objects missing, like the light source. These can be added by generally looking at other levels and just copying and pasting them.

* **Step 2: Make a rough outline of what the level should look like.**
* This is normally done on the "background (0)" layer in the "Grid" object.

* **Step 3: Fill out the outline and any major details.**
* This can be anything from the small islands in Level 4 to as big as the cave tunnels in Level 2.

* **Step 4: Put in all the small details.**
* This would be trees, decorations, houses, etc.

* **Explanation Of Each Layer**
* background (-1): A background layer that is below the standard background. Used for placing backgrounds behind transparent background items.
* background (0): A background layer that is called the standard background. Where most everything that's in the background is.
* Background Cosmetics: A background layer that is above the standard background. Used mostly for anything that the player should walk behind.
* Foreground Cosmetics: A foreground layer that is normally the standard foreground. Used normally for anything that should be shown in front of the player as they walk, like trees and house tops.
* SolidObjects: A Solid Object layer that does just that. Has a collider so that any tiles in it will prevent the player from walking through them.
* Background Solids: A background layer that is two above the standard background. Has a collider, so is normally used for anything that should be behind the player, but the player should collide with.
* LongGrass: A foreground layer that is at the same layer as the standard foreground. This is where any tile will be treated as Long Grass, causing the player to get questions more freqently inside of it. 


* **Things to look out for**
* A major reason why there are many layers in the Grid is simply to allow the level creator the freedom of having multiple tiles on top of each other. Make sure that the level you're placing tiles on is what you want.
* Certan tiles need to have there Collider Type changed in Unity ether for the player to not clip though it, or for it to actualy have a collider. This is done by going into Assets > Art > Tiles and picking on the tile you want to change. Then changing its Collider Type to "Grid" if you want to stop players from clipping though it, or "Sprite" if you just need it to have a collider.

# Adding Art
Adding the art from the art pack we use can be finiky. We use 16 x 16 sprites in our game.

* **Step 1: Add your art into Assets > Art > gfx**

* **Step 2: Open the sprite editor and hit the slice**
* Make sure that Type is "Grid By Cell Size", Pixel Size as 16 X and 16 Y.
* Then you can hit slice.
* Make sure to hit Apply in the top center-right area.

* **Step 3: Change the base image settings**
*Change the Pixels Per Unit to 16.
* Change the Filter Mode to "Point (no filter)"
* If the art that's getting added is too big, change the Max Size from 2048 to some type of power of 2, like 4096.
* Make sure to hit the Apply button on the right side below the box thing.

* **Step 4: Make a new pallet (skip if adding to a new pallet)**
* Go to the Tile Pallet and click on the left side dropdown.
* Then go to the bottom and create a new pallet. Name it whatever you want it to be.
* The folder you want to save these to is Assets > Art > Tiles

* **Step 5: Drag your art into the pallet**
* The folder you want to save these to is Assets > Art > Tiles
* Warning, it can take a few minutes if your artwork has numerous tiles. This is normal.


# Map Transition

* **SpawnPoint.cs**   
  * Teleports player to other scenes if its spawnPointID matches saved key.

* **Portal.cs**   
  * On trigger: saves SpawnPointID, calls LoadLevel, and then teleports the player to the targeted scene with the spawnpoint location.  
  * Particle effect enabled if this portal’s level \<= UnlockedLevel

# Map Transition Set Up

Maps are connected by portals and spawn points.  

* **Step 1**: Drag the **Portal, SpawnPoint** object from the prefab to the scene hierarchy. Rename them accordingly. e.g. (SpawnPoint\_From\_Map1\_to\_Map2), (Portal\_Map1\_To\_Map2) 

* **Step 2: Configuring Map 1**  
  * Drag the **portal** to the position you want on the map. This is the portal to **Map2**  
  * Resize it if needed.  
  * Drag the **SpawnPoint** object to the position you want on the map. Often near the portal so that it gives a visual sense two maps are connected. This is the SpawnPoint **Map2** that will go to.  
  * The **spawnID** of the **SpawnPoint** in **Map 1** should match the ID of **Portal in Map 2**

* **Step 3: Configuring Map 2**   
  * Repeat Step 2, but the portal this time goes to **Map 1,** and **SpawnPoint** is where **Map 1** will go.

* **Step 4: Go to MainMenu Scene**  
  * Click LevelManager.  
  * Add an element there with the specified level and the build index. For example, you can put Map 1 (Index 5\) with level number 1, then Map 1 would be level 1\.  
  * Note:  
    Hub is the default level (level 0).

# HUD & Award System

* **HUDController.cs**   
  * Toggles UI panels like coins, score, and timer when entering or exiting battles.

* **CoinManager.cs & CoinDisplay.cs**   
  * Singleton persisting “CoinCount” in PlayerPrefs  
  * AddCoin(), RemoveCoin(), GetCoinCount().

* **ScoreManager.cs & ScoreDisplay.cs**   
  * Singleton persisting “ScoreCount”  
  * AddScore() incorporates question difficulty, correct streak, and timer multiplier.

* **TimerManager.cs & TimerDisplay.cs**   
  * Tracks the level elapsed time.   
  * GetMultiplier(): time-based scoring multiplier tiers.

# Extension Points & Future to do

* Refers to [https://docs.google.com/document/d/15g5myaUmi3ZtB-9sSFVDsmlTkK20WzN-eSPucrvHBLA/edit?tab=t.0\#heading=h.9ibmsy1p3kpx](https://docs.google.com/document/d/15g5myaUmi3ZtB-9sSFVDsmlTkK20WzN-eSPucrvHBLA/edit?tab=t.0#heading=h.9ibmsy1p3kpx) 

[image1]: <https://private-user-images.githubusercontent.com/97140904/449366559-e7aa0af3-42ef-4da5-a145-3fff324c9ff3.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NDg2MzI4OTQsIm5iZiI6MTc0ODYzMjU5NCwicGF0aCI6Ii85NzE0MDkwNC80NDkzNjY1NTktZTdhYTBhZjMtNDJlZi00ZGE1LWExNDUtM2ZmZjMyNGM5ZmYzLnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNTA1MzAlMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjUwNTMwVDE5MTYzNFomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPWFiNjlhNDc1MTc2YTBlMDgwZjNiYzMwOWY3MzU4ODM0ODI0NjI0NGJhNzYzODQyZGZlOWQzYzEzMzM0NjljNGEmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.IpN9Ts5KXcgabdav7zrHCOu8F5IGgLhSUlJmuOxUFbU>

[image2]: https://private-user-images.githubusercontent.com/97140904/449366559-e7aa0af3-42ef-4da5-a145-3fff324c9ff3.png?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3NDg2MzI4OTQsIm5iZiI6MTc0ODYzMjU5NCwicGF0aCI6Ii85NzE0MDkwNC80NDkzNjY1NTktZTdhYTBhZjMtNDJlZi00ZGE1LWExNDUtM2ZmZjMyNGM5ZmYzLnBuZz9YLUFtei1BbGdvcml0aG09QVdTNC1ITUFDLVNIQTI1NiZYLUFtei1DcmVkZW50aWFsPUFLSUFWQ09EWUxTQTUzUFFLNFpBJTJGMjAyNTA1MzAlMkZ1cy1lYXN0LTElMkZzMyUyRmF3czRfcmVxdWVzdCZYLUFtei1EYXRlPTIwMjUwNTMwVDE5MTYzNFomWC1BbXotRXhwaXJlcz0zMDAmWC1BbXotU2lnbmF0dXJlPWFiNjlhNDc1MTc2YTBlMDgwZjNiYzMwOWY3MzU4ODM0ODI0NjI0NGJhNzYzODQyZGZlOWQzYzEzMzM0NjljNGEmWC1BbXotU2lnbmVkSGVhZGVycz1ob3N0In0.IpN9Ts5KXcgabdav7zrHCOu8F5IGgLhSUlJmuOxUFbU