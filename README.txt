Full source code available on Github.

This was my first time working with machine learning on this scale and my first time ever working with reinforcement learning. The source code is extremely disorganized due to the trial and error I needed to learn how to use ML-Agents, tensorboard, etc. 
To organize things in the Github repo, I've zipped the source code into the file "465-Project" and uploaded the three builds of the game used in the user study into the files "ManualControl" "AutomatedControl" and "AdaptiveControl"
For convenience, the "Scripts" directory in the root of the repo has the same scripts that are in the Assets folder in the zip of the source code. 

The game requires the player to move the red block on the sliding block puzzle to the far right of the puzzle wall. This will open a door in the center of the room revealing a yellow keycard. Insert this into the slot in the UI panel to reveal the code needed for the keypad
in the room on the opposite side of the sliding block puzzle. Successful completion of the game requires the player entering the code, once the correct code is entered it will print to the console. The code can be changed in the KeypadManagr.cs script by changing the correctCode string. 

The manual control build allows grabbing of the UI panel with the controllers as well as voice control - say the word "move" and the panel will teleport to your field of view. The other builds do not allow interaction with the panel. 

Link to Short Video: https://youtu.be/h-cnmhoGUJc
Link to Presentation Video:
Link to Programming Video: https://youtu.be/JgJ4MBAWZ30
Link to Overleaf Document: https://www.overleaf.com/read/wgddqjvvzsqh#ef4502
Link to GitHub Repo: https://github.com/csu-hci-projects/CS465_AllpassM

LLM Disclosure:
ChatGPT was used when writing the report for help with general feedback on drafts, section ordering/flow, finding synonyms/phrases when needed, final spell checks and word counts, as well as help with formatting in latex. 
