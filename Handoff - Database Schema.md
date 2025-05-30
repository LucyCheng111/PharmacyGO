Database Link: https://github.com/AlecDuval/gitDatabaseTest/tree/main

# Schema

QuestionList:
- Array of questions
	- Implemented as List<Question>

Question:
- Question name
	- String
- Question image
	- String
	- As a Github link
- Array of options
	- Implemented as List<Option>
- Answer index
	- Integer
	- Always set to 0
- Difficulty
	- Ranged from 1 to 5
- Locations (flag int)
	- A 13-bit integer
	- The right 8 bits are the location flags, with each bit correlating to a location where the question can be found.
	- The left 5 bits are the module flags, with each bit correlating to a module where the question can be found.
	- Locations: Bladder, Brain, Eyes, GI Tract, Heart, Lungs, Smooth Muscle, Other
	- Modules: Module 0, Module 1, Module 2, Module 3, Module 4

Option:
- Option text
	- String
- Option image
	- String
	- As a Github link
- Should use the image instead
	- Boolean


# GitHub Format
- Root Folder
	- Image folder
		- Image1.png
		- Image2.png
		- Image3.png
		- etc
	- jsonTest.json

	The jsonTest.json file is the database file. All questions can be found here in a JSON format. The image folder contains images that are referenced by the questions.
