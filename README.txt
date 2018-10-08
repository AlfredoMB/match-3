Alfredo Barcellos - Match 3 Game


The project was implemented with a MVC structure where the board and its pieces were detached from their view implementations.

PieceViews control the visual output and input for BoardPieces. BoardPieces are the model for the pieces.

Board is the one that holds the information about all the pieces and can be checked for Matches.

Most of the communication between layers are done via EventHandlers to ensure decoupling where it makes sense. 

I also started out with some tests but decided to focus on the game features to take the most out of the remaining time.

The build is on the build\ folder.


Below is a list of features that I would like to have implemented but wasn't able to do it for now:

Backlog:
- object pool for memory management
- reshuffle logic for 'out of possible matches' scenario
- improve UI for universal resolutions
- addressables for remote asset loading
- stage seed loading from scriptable objects to enable more levels
- match vfx and sfx
- multiplier vfx and sfx
- timer vfx and sfx
- game start countdown
- improved game over screen
- level selection
- title
- complete test coverage
- piece conversions into special pieces
- new behaviours for special pieces