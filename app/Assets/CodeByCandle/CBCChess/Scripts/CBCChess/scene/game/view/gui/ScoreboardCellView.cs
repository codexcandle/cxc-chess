using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;

namespace cbc.cbcchess
{
	public class ScoreboardCellView:View
	{
		// TODO - replace below w/ spritesheet
		public Sprite[] sprites_light;
		public Sprite[] sprites_dark;

		#region vars (public)
		public Text label;
		#endregion
		
		#region vars (private)
		private int _count;
		#endregion
		
		#region functions (public)
		public int count
		{
			get
			{
				return _count;
			}
			set
			{
				label.text = value.ToString();
				
				label.enabled = (value > 1);

				_count = value;
			}
		}

		public void Init(int groupIndex, int typeIndex)
		{
			UpdateImage(groupIndex, typeIndex);

			this.count = 1;
		}
		#endregion
		
		#region functions (private)
		private void UpdateImage(int groupIndex, int typeIndex)
		{
			Sprite[] group = (groupIndex == 0) ? sprites_light : sprites_dark;
			Image image = GetComponent<Image>();
			image.sprite = group[typeIndex];
		}
		#endregion
	}
}