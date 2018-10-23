using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace cbc.cbcchess
{
	public class UpdateSelectionMapVO
	{
		public List<int> data;
		
		public UpdateSelectionMapVO(List<int> data)
		{
			this.data = data;
		}
	}
}