using System;
using System.Collections.Generic;

namespace cbc.cbcchess
{	
	public interface IGameModel
	{	
		bool initialized { get; }

		GameType gameType { get; }
		void SetGameType(GameType value);	

		GameState state { get; }



		void BeginGame();

		void PlayTurn();

		void EndGame(int victorIndex);

		int player { get; }	
		void LoadPlayer(int playerIndex);

		bool active { get; }
		void SetActive(bool value);	

		int victor { get; }
		void SetVictor(int value);	

		void Setup();

		void ForfeitGame();

		List<PieceDataVO> GetPieceData();

		List<int> RequestMoveStartCandidates(int playerIndex);

		List<int> RequestMoveEndCandidates(RequestMoveMapVO vo);

		MoveVO RequestMove(RequestMoveVO request);

		bool RecordMove(MoveVO move);

		int GetTypeIndex(int pieceIndex);
	}
}