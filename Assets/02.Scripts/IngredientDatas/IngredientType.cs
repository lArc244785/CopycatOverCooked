using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CopycatOverCooked.Datas
{
	public enum IngredientType
	{
		None = -1,
		Onion = 1 << 0,
		Tomato = 1 << 1,
		Shrimp = 1 << 2,
		Bread =	1 << 3,
		Trimmed_Onion =	1 << 4,
		Trimmed_Tomato = 1 << 5,
		Trimmed_Shrimp = 1 << 6,
		Trimmed_Bread =	1 << 7,
		Grilled_Shrimp = 1 << 8,
		Grilled_Tomato = 1 << 9,
		Soup_Tomato = 1 << 10,
		
		//--조합 요리--
		Salad_OnionTomato = Trimmed_Onion | Trimmed_Tomato,
		Hamburger =			Trimmed_Onion | Trimmed_Tomato | Trimmed_Shrimp | Trimmed_Bread,
	}
}