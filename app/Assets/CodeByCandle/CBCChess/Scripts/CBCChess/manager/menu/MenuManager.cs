using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;

namespace cbc.cbcchess
{
	public class MenuManager:MonoBehaviour, IMenuManager
	{
		private static bool _initialized = false;
		// private static List<PieceDataVO> _units = new List<PieceDataVO>();
		
		// player
		private static int activePlayerIndex;
		
		// functions (public) --------------------------
		public MenuManager()
		{
		}

		/*
		public static List<PieceDataVO> GetUnits()
		{            
			if(!_initialized)
			{
				Initialize();
			}
			
			return _units;
		}
		*/
		public static void Initialize()
		{
			if(_initialized)
			{
				return;
			}
			
			_initialized = true;
		}
	}
}