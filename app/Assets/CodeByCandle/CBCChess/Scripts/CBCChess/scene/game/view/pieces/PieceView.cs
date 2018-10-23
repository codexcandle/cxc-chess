using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace cbc.cbcchess
{
	public class PieceView:View
	{
		#region CONSTANTS (private)
		private Color32 COLOR_HIGHLIGHTED = Color.cyan;
		private Color32 COLOR_START = Color.blue;
		private Color32 COLOR_SELECTABLE_DESTINATION = Color.blue;
		private Color32 COLOR_DESTINATION = Color.green;
		#endregion

		#region vars (public)
		// state
		public enum STATE
		{
			Init,
			Highlighted,
			SelectedAsStart,
			SelectableAsDestination,
			SelectedAsDestination
		}

		public STATE state
		{
			get
			{
				return _state;
			}
			set
			{
				if(value != _state)
				{
					SetState(value);

					_state = value;
				}
			}
		}
		
		// indices
		// ... index of player who owns this piece (0, 1,...)
		public int playerIndex					{ get; set; }

		public int viewIndex	 				{ get; set; }

		public int locationIndex				{ get; set; }

		public bool selectableTarget
		{
			get
			{
				return _selectableTarget;
			}
			set
			{
				_selectableTarget = value;
			}
		}
		#endregion

		#region VARS (private)
		private STATE _state;
		private bool _selectableTarget;
		private Color defaultColor = new Color(0.643F, 0.565F, 0.431F, 0.44F);
		private int _locationIndex;
		#endregion

		#region METHODS (public)
		public void Init(int viewIndex, 
		                 int locationIndex, 
		                 Color defaultColor)
		{
			this.viewIndex = viewIndex;
			this.locationIndex = locationIndex;
			this.defaultColor = defaultColor;

			ResetColor();
		}

		public void Destroy()
		{
			gameObject.SetActive(false);
		}
		#endregion

		#region METHODS (private)
		private void SetState(STATE value)
		{
			switch(value)
			{
				case STATE.Init:
					ResetColor();
					break;
				case STATE.Highlighted:
					UpdateColor(COLOR_HIGHLIGHTED);
					break;
				case STATE.SelectedAsStart:
					UpdateColor(COLOR_START);
					break;
				case STATE.SelectedAsDestination:
					UpdateColor(COLOR_SELECTABLE_DESTINATION);
					break;
				case STATE.SelectableAsDestination:
					UpdateColor(COLOR_DESTINATION);
					break;
			}
		}

		private void UpdateColor(Color clr)
		{
			PaintObject(clr);
		}

		private void ResetColor()
		{
			PaintObject(defaultColor);
		}

		private void PaintObject(Color clr)
		{
			Renderer rend = gameObject.GetComponent<Renderer>();
			if(rend != null)
			{
				rend.material.color = clr;
			}
			else
			{
				// HACK - assume this is a "knight" piece, which is 2-levels for the model, so color both!
				Component[] rendererComponents = GetComponentsInChildren(typeof(Renderer));
				if(rendererComponents != null)
				{
					// ... level 1
					rend = rendererComponents[0] as Renderer;
					rend.material.color = clr;

					// ... level 2
					rend = rendererComponents[1] as Renderer;
					rend.material.color = clr;
				}
			}
		}
		#endregion
	}
}