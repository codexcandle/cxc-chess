using System;

namespace cbc.cbcchess
{	
	public enum GameState
	{
		SETUP,
		STARTING,
		LOAD_PLAYER,
		TURN_READY,
		SELECT_START,
		SELECT_DESTINATION,
		GAME_OVER,


		Invalid,
		RequestMoveOptions,
		ShowMoveOptions,
		SelectDestination,
		Move,
		MoveComplete
	}
}