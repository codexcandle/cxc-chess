using System;

namespace cbc.cbcchess
{
	public class AppModel:IAppModel
	{
		public string data {get;set;}

		static public int currentLevel = 0;

		static public GameType gameType {get; set;}

		public AppModel(){}
	}
}