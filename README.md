# SaladChef
TentWorks Test



Usage:

Player1 – Movement: W,A,S,D. 	Action: F1
Player2 – Movement: Arrow Keys. 	Action: R-Ctrl

Salad Chef Test Notes:
In this application, I wanted to show several methods of inter-object communication including “SendMessage()”, Singleton direct calls, and the basic “m_object.publicValue = 42” direct assignment.  I also wanted to show game flow, UI, audio, and animation and how they could be implemented using those techniques. I also wanted to show how to handle frequent allocations and deallocations of fixed size objects—this to combat memory leaks over time.
The main application’s state machine proceeds as such:
Init		Merely connects some pointers and resets the player(s)
Start  		Player names, One/Two player mode select, Start
Game 	The Game
Hi-Scores  	List of high scores. This will return to Idle above on “Retry” or “Quit” the app completely.
No “Pause” functionality

Here are some things to look for in the code
1) All reusable objects manage their own allocations. Namely in Order.cs, Ingredient.cs, Main.cs. “Instantiate()” called once. All objects that allocate,  are responsible for freeing that memory. The do not pass allocated objects along in the hopes another object will manage it.
2) Message passing using “SendMessageUpwards()” from peripherals for game events “OnPlayerScored()” is a good example that comes from game logic. I also then for UI, animation, and collision. This is to show an OO approach. Also
3) Singletons are used as well. These are not OO but they are clean. Look at “Ingredient.Grab()” or “Waiter.SubmitPlate()”. The whole Audio system, as seen by the game, is a singleton.
4) There is also some valid but not recommended uses of public member variables such as “”m_playerVitals.m_timer = waitTime;”. I use these internally for only the class, “Player”, that needs them.
5) Anything requiring a frequent search is contained in a “Dictionary<>”. Any objects that require frequent iteration are in “List<>”
6) I used extremely verbose member and method names. This is for self-documenting the code. Rare but their might be some “int i” or “int iter” decls in there. Just a few.
7) Both the player and main application use state machines.

I tested it but there are still a few bugs I fixed at the 11th hour. The kind that only occur after playing all the edge cases.
I think with a week or two of love, that you could actually submit this to the app store. It is “fun” for a few minutes but gets boring quickly. The “love” part could add other challenges including ingredient shortages and the players battling for them. Something…would be cool.


Problem:
========


// **********************************************
// Th following is a recap of the commits from the beginning of the project
// **********************************************
11/14/2020 - Initial framework. Restaurant,characters, animations, props, Input system and game wrapper
11/15/2020 3:18:23 AM +00:00	Base Game Object Player
11/16/2020 11:48:17 PM +00:00	Added some collision to tables.Fix a Rigidbody
11/16/2020 7:10:06 PM +00:00	Pickup/Put down ingredients
11/16/2020 12:27:27 AM +00:00	Oops.With last commit
11/16/2020 12:26:59 AM +00:00	More framework. Basic player movement
11/17/2020 5:51:59 AM + 00:00    Player logic.Pick up, put down veggies,
11/17/2020 1:02:40 AM +00:00	Quickly added pickup animation/event
11/18/2020 10:23:12 PM + 00:00   Added trash can to dump unwanted ingredients
11/18/2020 8:04:49 PM + 00:00    1) Player carrys multiple ingredients
11/18/2020 7:16:11 AM + 00:00    Multiple customers/orders WIP
11/18/2020 4:16:32 AM + 00:00    WIP--Orders being submitted, plated, customers satisty condition. Some visual artifacts
11/19/2020 8:15:41 AM + 00:00    New layout for the spec.
11/19/2020 3:39:36 AM + 00:00    Added player 2 control config. Arrow keys, R_Ctrl
11/19/2020 12:33:11 AM + 00:00   Prep for two player. Start screen
11/19/2020 8:15:41 AM + 00:00    New layout for the spec.
11/19/2020 3:39:36 AM + 00:00    Added player 2 control config. Arrow keys, R_Ctrl
11/19/2020 12:33:11 AM + 00:00   Prep for two player. Start screen
11/20/2020 12:46:33 AM + 00:00   For last change
11/20/2020 12:46:10 AM + 00:00   Made chopping tables player specific PLAYER_ID
11/21/2020 6:15:56 AM + 00:00    With last submt
11/21/2020 6:15:23 AM + 00:00    With last submit
11/21/2020 6:14:26 AM + 00:00    Hooking up UI
11/21/2020 4:49:49 PM +00:00	Fixed a simple thing in the scorekeeper
11/22/2020 2:52:29 AM + 00:00    Add Player timeout and go to hi score screen
11/22/2020 1:23:25 AM + 00:00    Adding High Scores: Part 1
11/21/2020 10:47:10 PM + 00:00   Time bonus added for finishing orders early. Punishment for missing orders11/21/2020 9:35:39 PM + 00:00    More UI.Player names
11/21/2020 8:36:38 PM +00:00	Added simple audio system
 11/21/2020 6:35:00 PM +00:00	Added dissatisfied customers and point penalties
11/23/2020 2:59:56 AM + 00:00    Fixed high scores. Load/Save
11/23/2020 1:52:31 AM + 00:00    Fixed a restart Bug
11/23/2020 1:13:51 AM + 00:00    Restart
 11/23/2020 12:40:50 AM + 00:00   Fixed placement of ingreidents in player satchel and chopping table
11/23/2020 12:04:48 AM + 00:00   Fixed a silly memory bug









