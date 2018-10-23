using System;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

using cbc.cbcutils;

namespace cbc.cbcchess
{
	public class ReclaimMediator:CBCMediator
	{
		[Inject]
		public ReclaimView view { get; set; }
		
		[Inject]
		public IGameModel gameModel { get; set; }
		
		// signals
		[Inject]
		public ShowRescuePanelSignal showRescuePanelSignal {get; set;}

		// functions (public) ----------------------------
		public override void OnRegister()
		{
			base.OnRegister();
			
			enableListeners(true);

			// hide to start
			view.Enable(false);

			// TODO - get list from model...
			List<string> prefabIDs = new List<string>();
			prefabIDs.Add(PrefabNames.PAWN_LIGHT);
			prefabIDs.Add(PrefabNames.KNIGHT_LIGHT);
			prefabIDs.Add(PrefabNames.BISHOP_LIGHT);
			prefabIDs.Add(PrefabNames.ROOK_LIGHT);
			prefabIDs.Add(PrefabNames.QUEEN_LIGHT);
			// ...
			view.Init(prefabIDs);
		}
		
		public override void OnRemove()
		{
			enableListeners(false);
		}
		
		// functions (private) ---------------------------
		private void enableListeners(bool enable)
		{
			if(enable)
			{
				// ... app
				showRescuePanelSignal.AddListener(onShowRescuePanelSignal);
				// ... view
				// view.selectionMapRequested.AddListener(requestSelectionMap);
			}
			else
			{
				// ... app
				showRescuePanelSignal.RemoveListener(onShowRescuePanelSignal);
				// ... view
				// view.selectionMapRequested.RemoveListener(requestSelectionMap);
			}
		}

		private void onShowRescuePanelSignal()
		{
			// TODO - uncomment below; hidden for testing..
			// view.Enable(true);
		}
	}
}