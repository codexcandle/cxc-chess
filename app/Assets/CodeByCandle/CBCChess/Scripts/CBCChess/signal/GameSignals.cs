using System;
using System.Collections.Generic;

using strange.extensions.signal.impl;

namespace cbc.cbcchess
{	
	public class LoadGameSignal:Signal<GameType>{}

	public class SetupGameSignal:Signal{}

	public class RequestStartNewGameSignal:Signal{}
	public class StartGameSignal:Signal{}
	public class EndGameSignal:Signal{}

	public class ShowRescuePanelSignal:Signal{}

	public class CameraPlayerZoomCompleteSignal:Signal{}
	public class CameraMoveCompleteSignal:Signal{}
	public class CameraFocusPlayerSignal:Signal<int>{}

	public class GameStateChangeSignal:Signal<GameState>{}
	public class CompleteMoveSignal:Signal{}
	public class StubGameSignal:Signal{}
	public class StubMoveSignal:Signal<RequestMoveVO>{}
	public class RequestSelectionMapSignal:Signal<RequestSelectionMapVO>{}
	public class UpdateSelectionMapSignal:Signal<UpdateSelectionMapVO>{}
	public class RequestMoveMapSignal:Signal<RequestMoveMapVO>{}
	public class DisplayMoveMapSignal:Signal<DisplayMoveMapVO>{}
	public class SuggestMoveSignal:Signal<MoveVO>{}
	public class ValidateMoveSignal:Signal<RequestMoveVO>{}
	public class MovePieceSignal:Signal<List<MoveVO>>{}
	public class DestroyPieceSignal:Signal<int>{}
	public class RecordMoveSignal:Signal<MoveVO>{}

	public class PieceDeselectedSignal:Signal{}
}