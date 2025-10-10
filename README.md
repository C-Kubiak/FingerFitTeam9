# FingerFit











## \- Changelog -



#### 9/27/25

* Changed Settings menu button to toggle the menu off/on instead of reopening and repeating the fade in.
* Settings menu "reset to default" option now adjusts UI elements accordingly to match actual values.





#### 9/26/25



* Created new Git Repo with project contents
* Reworked TypeRushManager script to highlight correct text in green and incorrect text in red as user types. Additionally the user is no longer charged with mistakes for backspaces.

  * Reason: Easier for users to find where mistakes are made, and mistakes don't count as double anymore. This change is more in line with Typeracer and other typing games.







## \- Known Issues -
KNOWN ISSUES 10/8: Playing a game, then exiting to main menu, the button will not take you to your destination. The same is true for the settings panel. (potential cause: event listeners staying active after scene is unloaded) Accuracy in type rush is only updated after a phrase is completed. User design lacking, increasing font size can lead to text overfilling buttons.
