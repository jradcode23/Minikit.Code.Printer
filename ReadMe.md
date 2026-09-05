# SETUP GUIDE AND INSTRUCTIONS FOR LEGO Pirates Minikit Code Printer
- --
## Background
- In Lego Games, each individual minikit has an associated code. 
    - An example of a minikit code is m_pup3
- In order to determine which AP location was collected, we need to map the minikit codes to where they are collected in game.
- This mod will print out the minikit code and associated map code/name
## Required software
- An unmodified copy of the Steam release of Lego Pirates of the Carribean
    - This will likely work with the GOG version, though I don't own it to test and the setup will be slightly different.
- Reloaded II Mod Loader [Link](https://reloaded-project.github.io/Reloaded-II/QuickStart)
  - Requires .NET 9.0 or higher
- The Lego Pirates of the Carribean Minikit Code Printer Mod [Link](https://github.com/jradcode23/Minikit.Code.Printer)
- --
## Optional Software (but recommended)
- DXWnd [Link](https://sourceforge.net/projects/dxwnd/)
- --
## First Time Setup 
- Reloaded II does not allow non-ASCII characters in the file path, so we will have to modify the game path. Here are the steps to modify the game path
    - Open Steam
    - In the top left corner click "Steam", then click "Exit"
    - This process will NOT work if you click the red X in the top right corner
    - Navigate to the "steamapps" folder. File path is typically, "C:\Program Files (x86)\Steam\steamapps"
    - Open the file called "appmanifest_311770.acf" with a text editor
    - Under "Installdir" remove the ® from the game name.
    - Save and close the file.
    - Then open the "common" folder back in the file directory and remove the ® from the game's folder.
    - Reopen steam and try to launch the game.
        - If steam tries to redownload the game or tells you it could not find the game, that means you didn't close steam properly and you'll need to repeat the process.
- Install Reloaded II Mod Loader
- Add Lego Pirates of the Carribean to Reloaded II either through setup or by clicking the + on the left.
  - Must use the exe found in the Steam folder that we just adjusted.
- If you are using the Steam version, we will need to use the ASI loader (optional for GOG version) 
  - Under Lego Pirates of the Carribean, click Edit Application
  - Under "Advanced Tools & Options", make sure that "Don't Inject Loader" is enabled
    - Indicated by the red +
  - Click "Deploy ASI Loader" and then "Okay"
- Navigate to the "Mods" folder in wherever you installed Reloaded II
- Move the LPotC.Minikit.Codes folder into the Reloaded II Mods folder
- --
## Instructions
- Launch the game with the Mod enabled (indicated by the red +)
- Whenever you change maps or collect a minikit, the Reloaded II terminal will print the updated Map ID or the Minikit code
- Once the reloaded terminal prints the Map ID/Name or minikit code, please write the information in the associated [spreadsheet](https://docs.google.com/spreadsheets/d/1c_CvgT9XiXmq8D-UcWYsPBxjijTzDuxHXJ0TIIy0rvs/edit?usp=sharing)
- IMPORTANT: Capitalization and spelling matters. There have been instances in other lego games where there is an M_pup minikit and a m_pup minikit in the same level
- IMPORTANT: If the minikit involves multiple items, i.e., break 5 objects, there may be multiple minikit codes associated with it. This is usually determined by the last item (i.e., last object broken). So to test, we will need to adjust the order of unlocking the minikit, specifically the last object.
- IMPORTANT: If there are duplicate minikit codes in the same level, let me know. Across multiple different levels is fine, but if one level has two identical minikit codes, we need to know to research.