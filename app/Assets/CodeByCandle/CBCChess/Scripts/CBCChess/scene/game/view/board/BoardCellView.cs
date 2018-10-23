using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace cbc.cbcchess
{
	public class BoardCellView:View
	{
		#region vars (public)
		// children
		public GameObject nameLabel;
		public GameObject cube;

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
					switch(value)
					{
						case STATE.Init:
							UpdateColor(baseColor);
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

					_state = value;
				}
			}
		}

		// indices
		public int viewIndex	 				{get; set;}
		public int ownerIndex
		{
			get
			{
				return _ownerIndex;
			}
			set
			{
				// TODO - replace below hack
				int totalPieceCount = GameConstants.ROW_CELL_COUNT * GameConstants.ROW_CELL_COUNT;

				if((value < 0) || (value > (totalPieceCount - 1)))
				{
					value = -1;
				}

				_ownerIndex = value;
			}
		}

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

		#region vars (private)
		private STATE _state;
		private int _ownerIndex;
		private bool _selectableTarget;
		private Color baseColor;
		#endregion

		#region constants (internal)
		internal Color COLOR_HIGHLIGHTED 				= Color.cyan;
		internal Color COLOR_START 						= Color.blue;
		internal Color COLOR_SELECTABLE_DESTINATION 	= Color.blue;
		internal Color COLOR_DESTINATION				= Color.green;
		#endregion

		#region functions (public)
		public void Init(int viewIndex, int ownerIndex, Color baseColor)
		{
			this.viewIndex = viewIndex;
			this.ownerIndex = ownerIndex;
			this.baseColor = baseColor;

			UpdateColor(baseColor);
		}
		#endregion

		#region functions (private)
		private void UpdateColor(Color clr)
		{
			cube.GetComponent<Renderer>().material.color = clr;
		}
		#endregion
	}
}