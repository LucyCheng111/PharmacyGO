# Pharmacy-GO
![Image](https://github.com/user-attachments/assets/56d7289f-a588-450d-bb35-16983da2590f)

Link to the game:
https://phgo.itch.io/pharmacy-go


Pharmacy GO! is a Pokémon-style adventure game that transforms the PHAR736 curriculum into an interactive quest.

### Why it Matters
Traditional study tools (flashcards, quizzes) often fail to engage students. Pharmacy GO! fixes this with:
Gamified Learning: Answer randomized questions to explore levels, defeat bosses, and earn coins.
Adaptive Difficulty: Questions adjust based on performance.
Cross-Platform: Playable on web browsers (desktop/mobile) with no downloads.

Some of the core features of our game include: 
* Multi-Level Map
  * Themed levels (e.g., Lung, Heart) with randomized questions
* Boss Battles
  * Test mastery with high-stakes end-level questions
* Coin Rewards
  * Earn coins for correct answer to unlock new levels
* High Scores
  * Better performance each run rewards more points
### The Team
Our team of 8 OSU students, advised by Dr. Benjamin Philmus, that brought Pharmacy GO! to life:
* Jinpeng Chen - Programmer
* Alec Duval - Database Manager 
* Quinn Glenn - Menu Designer
* Xiaoyu Luo - Scene Developer 
* Teagan Simoneau - Project Manager
* Hoimau Tan - Index Designer
* Erik Tornquist - Database Programmer
* Samuel Westerham - Music/Level Designer

Though we each took on specific roles and titles, we all worked together in many aspects of the project to turn it into a reality. It took a lot of time, communication, and collaboration to make it possible.



## Gamifying Studying

### The idea of “Pharmacy GO!”
The core idea of our project is to create a game that can help students, specifically those from Oregon State University (OSU) PHAR736, study for their course. This is done by challenging the players to answer questions provided by the professor of the course inside the game. The game is modeled after Pokémon, the major difference being that instead of battling and collecting creatures, you “battle” questions and collect coins for right answers. These questions can be found either by walking around the levels, or diving into tall grass to increase the odds of finding them. Once you get enough coin, you can battle the level’s pharmacist. The pharmacist will have multiple questions back to back, needing the player to get all of them correct to complete the level. Once the level’s pharmacist is beaten, you then can move on to the next level. Each major level has questions specific to the each module inside PHAR736.

The levels ares built with the idea of study first, exploration second. From the spawn point of each map, there is an easy path to the center with grass, allowing quick access to many questions. Exploration of the maps is one of the major ways the game tries to keep the player entertained and interested in the game, thus allowing the player to study more.

![Image](https://github.com/user-attachments/assets/be1bf46e-c5b2-4f72-8777-60d67131efd6)
Hub level, which leads to all the other levels in the game
<br/><br/>

![Image](https://github.com/user-attachments/assets/3ea0b3f4-9a39-4cc6-8a28-62f92f5fce9c)
Level 3, a maze for players to explore
<br/><br/>

![Image](https://github.com/user-attachments/assets/83c539d0-f209-4724-a0fd-7aad5bbc410e)
Muscle level, focusing only on muscle questions
<br/><br/>


Why we chose “Pharmacy GO!” as a study tool?
A major example of “Pharmacy GO!” as a study tool is the simple fact that it's a game. This can allow students who struggle with studying, especially if they find it boring or tedious, to have some fun with studying. The game’s focus on prioritizing studying is reinforced by having the core loop of the game be centered around answering questions. This forces students to learn as they play to progress in the game. The more that they learn, the farther they can progress, encountering even more questions as they beat levels and accumulate points.

## Built with Unity
### Core Engine
The whole game is built with the latest Unity6 technology. The sprites and tiles in the game utilize art obtained from a commercial game license, built with multiple layers created to let users have an immersive game environment. Component-based GameObjects drive every in-game entity, such as player, NPC, portals. Scripts using C# are used for different game events such as player movement, camera control, normal and boss battles, map transitions, etc.
Many Unity tools are used to keep a structured hierarchy as the player progresses through levels. Singleton controllers like GameController, LevelManager with DontDestroyOnLoad maintain the global state. Cinemachines & TextMeshPro are used for smooth camera follow and dynamic text rendering for questions, dialogs, and HUD. 
### Data Storage
We also utilize an external database that connects directly to the game. Enemies and questions are pulled from the database when players encounter battles. Database information is stored as JSON, which is placed on GitHub. The game then pulls that data through use of GitHub’s REST API, and saves that data into a custom object class implementation. 

## Database Interface
The database is stored on Github, and can be interacted with using a Database Interface that’s created using a WinForms application on .NET 4.7.2. It loads the JSON file from Github and creates a data structure that allows us to modify the database through code, which is then updated and sent back to the database.
![Image](https://github.com/user-attachments/assets/f2234f3c-9045-4ca0-8e8d-f5c6a0044783)<br/><br/>
![Image](https://github.com/user-attachments/assets/015aa489-f45a-4c5a-8f1c-505d5212392f)<br/><br/>
![Image](https://github.com/user-attachments/assets/6839158e-6381-44d4-82b7-25341090cd71)<br/><br/>

## Database Format
class QuestionList:
* List<Question> questions

class Question:
* string question
* string imageLink
* List<Option> options
* int answerIndex
* int difficulty
* int locations

class Option:
* string text
* string imageLink
* bool useImage

## Play Anywhere
Pharmacy Go is a browser based game, no installation is required to play the game. It is fully playable on PCs, laptops and mobile devices. Questions are loaded from a database on GitHub, so an internet connection is required to play the game. Your progression is based on answering questions correctly and challenging bosses. Coins, score, and progress are saved automatically between sessions. 

The game is deployed on itch.io and can be reached here: (https://phgo.itch.io/pharmacy-go)

![Image](https://github.com/user-attachments/assets/20503137-bb98-451a-b86d-76c56ccf85c4)<br/><br/>
![Image](https://github.com/user-attachments/assets/b687641b-8cdf-4ceb-b741-591d848fa19f)<br/><br/>
![Image](https://github.com/user-attachments/assets/49b6bd9d-7c79-4cab-a3f4-0da9b774d726)<br/><br/>
![Image](https://github.com/user-attachments/assets/61f5a0fe-9efd-4455-b979-c38f526fec7d)<br/><br/>

## Contact Us
If you have any feedback or bugs to report about the game, please follow the link here: https://oregonstate.qualtrics.com/jfe/form/SV_3IfO3l2H7FbOOuW

To contact the team for professional inquiries, please reach out to pharmacygoteam@gmail.com
