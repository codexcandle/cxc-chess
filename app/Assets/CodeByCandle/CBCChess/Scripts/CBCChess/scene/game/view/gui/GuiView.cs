using UnityEngine;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace cbc.cbcchess
{
	public class GuiView:View
	{
		#region VARS (public)
		public Image dimmerMatting;
		public GameObject networkPanel;
		public GameObject gameEndPanel;
		public GameObject scoreboard;
		public GameObject[] scoreboardCells_p1;
		public GameObject[] scoreboardCells_p2;
		public GameObject debugPanel;
		public Text statusLabel;
		public Text gameResultLabel;
		public Signal playComputerClick = new Signal();
		public Signal playNetworkClick = new Signal();
		public Signal newGameClick = new Signal();
		public Signal quitClick = new Signal();
		#endregion

		#region funcs (public)
		public void Init()
		{
			InitScoreboard(false);

			dimmerMatting.CrossFadeAlpha(0f, 2.0F, false);
		}

		// ... show
		public void ShowNetworkPanel(bool show = true)
		{
			networkPanel.SetActive(show);
		}

		public void ShowGameEndPanel(bool show, string result = "")
		{
			gameEndPanel.SetActive(show);
			
			if(result != null)
				gameResultLabel.text = result;
		}

		public void ShowScoreboard(bool show = true)
		{
			scoreboard.SetActive(show);
		}

		// ... scoreboard
		public void UpdateScoreboard(int teamIndex, int pieceIndex, bool enable)
		{
			GameObject[] set = (teamIndex == 0) ? scoreboardCells_p1 : scoreboardCells_p2;

			int count = set.Length;
			if((pieceIndex >= 0) && (pieceIndex < count))
			{
				GameObject go = set[pieceIndex];

				ScoreboardCellView cell = go.GetComponent<ScoreboardCellView>();
				if(cell != null)
				{
					if(enable)
					{
						cell.count++;
					}
					else
					{
						cell.count = 0;
					}
				}

				go.SetActive(enable);
			}
		}

		public void ResetScoreboard()
		{
			UpdateScoreboard(0, 0, false);
			UpdateScoreboard(0, 1, false);
			UpdateScoreboard(0, 2, false);
			UpdateScoreboard(0, 3, false);
			UpdateScoreboard(0, 4, false);
			
			UpdateScoreboard(1, 0, false);
			UpdateScoreboard(1, 1, false);
			UpdateScoreboard(1, 2, false);
			UpdateScoreboard(1, 3, false);
			UpdateScoreboard(1, 4, false);
		}

		// ... click
		public void ClickNewGame()
		{
			Debug.Log("clickNewGame @ GuiMediator.");

			newGameClick.Dispatch();
		}

		public void ClickPlayComputer()
		{
			playComputerClick.Dispatch();
		}

		public void ClickPlayNetwork()
		{
			playNetworkClick.Dispatch();
		}

		public void ClickQuit()
		{
			quitClick.Dispatch();
		}

		// ... status
		public void UpdateStatus(string txt)
		{
			statusLabel.text = txt;
		}

		public void AlignStatusText(TextAnchor align)
		{
			statusLabel.alignment = align;
		}
		#endregion

		#region METHODS (private)
		private void InitScoreboard(bool enableChildren)
		{
			/////////////
			int teamIndex = 0;
			// ...
			GameObject[] set = (teamIndex == 0) ? scoreboardCells_p2 : scoreboardCells_p1;

			int count = set.Length;
			for(int i = 0; i < count; i++)
			{
				GameObject go = set[i];
				ScoreboardCellView cell = go.GetComponentInChildren<ScoreboardCellView>();
				if(cell != null)
				{
					cell.Init(teamIndex, i);

					go.SetActive(enableChildren);
				}
			}

			/////////////
			teamIndex = 1;
			// ...
			set = (teamIndex == 0) ? scoreboardCells_p2 : scoreboardCells_p1;

			count = set.Length;
			for(int j = 0; j < count; j++)
			{
				GameObject go = set[j];
				ScoreboardCellView cell = go.GetComponentInChildren<ScoreboardCellView>();
				if(cell != null)
				{
					cell.Init(teamIndex, j);

					go.SetActive(enableChildren);
				}
			}
		}
		#endregion
	}
}