# FingerFit

How to Commit & Push Changes
----------------------------

1. Open a terminal or VS Code terminal in the project folder
   - Make sure you are inside the project’s root directory (where the .git folder is located).

2. Check your current branch
   $ git status

   - If you are not on the main branch (e.g., on dev or a feature branch), switch or create one as needed:
     $ git checkout main        # switch to main
     $ git pull origin main     # get the latest updates

3. Stage your changes
   $ git add .

4. Commit with a clear message
   $ git commit -m "Describe what you changed (e.g., added new UI scene)"

5. Push to the repository
   $ git push origin main

   Important: Always run
   $ git pull origin main
   before pushing, to make sure your local copy is up to date and avoid merge conflicts.

Notes
-----

- Missing folders like Library, Logs, and Obj are **normal** for Unity projects and are excluded via .gitignore.
- Unity should regenerate these folders automatically when opening the project in Unity.



## \- Changelog -



#### 9/27/25

* Changed Settings menu button to toggle the menu off/on instead of reopening and repeating the fade in.
* Settings menu "reset to default" option now adjusts UI elements accordingly to match actual values.





#### 9/26/25



* Created new Git Repo with project contents
* Reworked TypeRushManager script to highlight correct text in green and incorrect text in red as user types. Additionally the user is no longer charged with mistakes for backspaces.

  * Reason: Easier for users to find where mistakes are made, and mistakes don't count as double anymore. This change is more in line with Typeracer and other typing games.
 

#### 10/12/25

* Added UI asset package (GUIPack-Clean&Minimalist), updated main menu to be cleaner
* Added background to main menu (asset package: Simple Nature Package)
* Tried fixing settings not persisting, issue still remains
* Resolved scenes failing to load upon entering and exiting

#### 10/13/25

* Incorporated new asset package into Key Catch, Type Rush, and Main Menu
* Created a settings scene to use rather than the panel on the main menu (not completed)
* Testing a background in Type Rush/Main Menu





## \- Known Issues -

* SettingsManager.cs does not work across scenes, and fails to work after exiting and rentering a scene (moving away from a settings panel, trying a whole scene for settings)
* User Design Lacking in mini games
* Lacking data storage
* Sequence Spark shows <\ALPHA> in phrase box after making mistake
* Type Rush doesn't have a GAME OVER popup when reaching mistake count
