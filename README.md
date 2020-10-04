# anino_exam 

Unity version 2019.1.9f1

System setup

Main classes:
DataManager -> All data related, symbols, paylines, slot config. Gives access to data to other classes. User data is here for this test. 

RemoteConfigFetcher -> get the info from remote config and pases is to DataManager. Has local option using a bool.

SlotController -> controls the flow of the game, contains references to reelControlles, stores the curernt status vars.

ReelController -> Handles the reel movements, setup and calculations regarding that particular reel

AudioManager -> really simple audio manager

UiController -> references and functions related to UI. Also contains references to Paylines and Payout canvas scripst.

SymbolData -> contains symbol data, and setups Symbol gameobjects.

DATA:

All is based on Json. Remote config or local json strings can be used:

local examples:

string _local_AllSymbols =
        "{\"Symbols\":[{\"Id\":0,\"Name\":\"A\",\"Image\":\"symbols_0\",\"Payout\":[0,0,1,5,10]},{\"Id\":1,\"Name\":\"B\",\"Image\":\"symbols_1\",\"Payout\":[0,0,2,8,25]},{\"Id\":2,\"Name\":\"C\",\"Image\":\"symbols_2\",\"Payout\":[0,0,1,5,10]},{\"Id\":3,\"Name\":\"D\",\"Image\":\"symbols_3\",\"Payout\":[0,0,1,5,10]},{\"Id\":4,\"Name\":\"E\",\"Image\":\"symbols_4\",\"Payout\":[0,0,1,5,10]},{\"Id\":5,\"Name\":\"F\",\"Image\":\"symbols_5\",\"Payout\":[0,0,1,5,10]},{\"Id\":6,\"Name\":\"G\",\"Image\":\"symbols_6\",\"Payout\":[0,0,1,5,10]},{\"Id\":7,\"Name\":\"H\",\"Image\":\"symbols_7\",\"Payout\":[0,0,1,5,10]},{\"Id\":8,\"Name\":\"I\",\"Image\":\"symbols_8\",\"Payout\":[0,0,1,5,10]},{\"Id\":9,\"Name\":\"J\",\"Image\":\"symbols_9\",\"Payout\":[0,0,1,5,10]},{\"Id\":10,\"Name\":\"K\",\"Image\":\"symbols_10\",\"Payout\":[0,0,1,5,10]},{\"Id\":11,\"Name\":\"L\",\"Image\":\"symbols_11\",\"Payout\":[0,0,1,5,10]},{\"Id\":12,\"Name\":\"M\",\"Image\":\"symbols_12\",\"Payout\":[0,0,1,5,10]},{\"Id\":13,\"Name\":\"N\",\"Image\":\"symbols_13\",\"Payout\":[0,0,1,5,10]},{\"Id\":14,\"Name\":\"O\",\"Image\":\"symbols_14\",\"Payout\":[0,0,1,5,10]},{\"Id\":15,\"Name\":\"P\",\"Image\":\"symbols_15\",\"Payout\":[0,0,1,5,10]}]}";
        
        modify any value. Image has to be on the symbols sprite atlas.
        Payouts can be edited on Payout array

string _local_AllPaylines =
        "{\"Paylines\":[{\"Payline\":[1,1,1,1,1]},{\"Payline\":[0,0,0,0,0]},{\"Payline\":[2,2,2,2,2]},{\"Payline\":[0,1,2,1,0]},{\"Payline\":[2,1,0,1,2]},{\"Payline\":[0,0,1,2,2]},{\"Payline\":[2,2,1,0,0]},{\"Payline\":[1,0,1,2,1]},{\"Payline\":[1,2,1,0,1]},{\"Payline\":[1,0,0,1,0]},{\"Payline\":[1,2,2,1,2]},{\"Payline\":[0,1,0,0,1]},{\"Payline\":[2,1,2,2,1]},{\"Payline\":[0,2,0,2,0]},{\"Payline\":[2,0,2,0,2]},{\"Payline\":[1,0,2,0,1]},{\"Payline\":[1,2,0,2,1]},{\"Payline\":[0,1,1,1,0]},{\"Payline\":[2,1,1,1,2]},{\"Payline\":[0,2,2,2,0]}]}";
        
        Paylines can be modified or addeed on here
        
        

int _local_MaxLines = 20;

    
string _local_ReelsIDs = "{\"ReelsSymbolData\":[{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15]}]}";

contains the reels used in the reel, by id
here a setup with more chance of prize:

    string _local_ReelsIDs = "{\"ReelsSymbolData\":[{\"SymbolDataIDs\":[0,1,2,3,4,5]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6]},{\"SymbolDataIDs\":[0,1,2,3,4,5]},{\"SymbolDataIDs\":[0,1,2,3,4,5,6]},{\"SymbolDataIDs\":[0,1,2,3,4,5]}]}";


MVC:

M: data classes like user, symbol data and some structures of the dataManager

C: most of the controller classes. DataManager also has some responsabilities on this. Could be splitted from there, but kept it for simplicity/fast implementation

V: Ui controller, symbols and reel_controller

Overall I think that the structure is ok. Is hard for me sometimes to keep a clear MVC structure, as I use to adapt the pararigm to my use case, not having strong separation on few classes/structures.

IMPROVEMENTS

Take care of different screen ratios.

Remove game state logic from SlotController to a GameManager.

Event handler class. So classes don't need to know about each other. Just trigger events/listen to events on a list of events there. Helps decoupling classes and cuts dependencies. Didn't do due to time constrains.

DataManager does too much, decouple model and controller functionalities.

Finish the code to accept different size reels. Code is directed and prepared for that on a degree.

Reels should stop with better visuals. Add reels spinning sound.

AudioManager is super simple. Allow loops, use audio mixers. Actually write a proper AudioManager.

UI controller is "dirty" and implemented fast just to make it work. Use of events would be better to update it than the current system.

UI -> separate static and not static images/texts. Improve performance as not need to make all dirty and render again.

Better audio system. Is simple and just made for the purpose of the test. Doesn't have system to stop audio, have loops, audio mixers. Definately to improve in a better proyect.

All visuals. Is clearly a test exercise on current state

Store and save user data, retrieve it from backend

Move result generation logic out from client. Don't send the results until spin starts!

Better win feedback and flow. Define big, great, ultra wins with animations.

Particles and visuals.

TIME USED


I am aware that i had the luck of doing the work on several days. This allows for couple of things that are not possible on a 24h timeframe.
Less preassure. Even that I tried to do it as fast as possible, not going always for the best solution but for the one I considered worth time/quality wise.
Allows for reflection, thinking outside the time strictly using for programming. Sometimes I found myself thinkink about the problem even not in front of the pc. This time is hard to quantify, but still important and IMO has a lot of value. Sorry if the estimations are not super detailed. I tried to do it has sincere and clearly as possible, but some might be hard to meassure. I could easily add 1.5h of just thinking aboout the project outside of workstation.


Initial setup                                                                           40m
Reels spinning                                                                          30m
Send result to reels 30 mins

trying to make the reels show result stopping on site                                   1h
30 mins reels stop on result positon animated
paylines creation                                                                       20m

2h

process paylines, create logic                                                          30m

generate random results on local, fix small bugs, review result logic                   1h

start settting UI and UI manager                                                        45m

setting firebase                                                                        30m


heady code refactor, from Scriptable onjects to data classes to work with firebase
change loading system, online fetcher, create json files, parse them...                 3h

basic ui                                                                                45m

bug fixing                                                                              1h

audio manager                                                                           30m

paylines canvas // realized later that is payout info :(                                45m
payout canvas                                                                           45m
Refactor/cleaning                                                                       45m
 
Documentation and setup for sending the test                                            30m

    TOTAL:                                                                              12h 15m

