using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CopycatOverCooked.Datas
{
	public enum IngredientType
	{
		None = 0,
		Trash = -9999,
		Onion = 1 << 0,
		Tomato = 1 << 1,
		Shrimp = 1 << 2,
		Bread =	1 << 3,
		Trimmed_Onion =	1 << 4,
		Trimmed_Tomato = 1 << 5,
		Trimmed_Shrimp = 1 << 6,
		Trimmed_Bread =	1 << 7,
		Grilled_Shrimp = 1 << 8,
		Soup_Tomato = 1 << 9,

		

		//--조합 요리--
		Salad_OnionTomato = Trimmed_Onion | Trimmed_Tomato,
		TrimmedMix_Onion_Shrimp = Trimmed_Onion | Trimmed_Shrimp,
		
		TrimmedMix_Bread_Onrion = Trimmed_Bread | Trimmed_Onion,
		TrimmedMix_Bread_Tomato = Trimmed_Bread | Trimmed_Tomato,
		TrimmedMix_Bread_Shrimp = Trimmed_Bread | Trimmed_Shrimp,

		TrimmedMix_Onion_Tomato_Shrimp = Trimmed_Onion | Trimmed_Tomato | Trimmed_Shrimp,

		Hamburger =			Trimmed_Onion | Trimmed_Tomato | Trimmed_Shrimp | Trimmed_Bread,
	}
}