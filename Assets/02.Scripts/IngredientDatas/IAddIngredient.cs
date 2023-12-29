using CopycatOverCooked.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopycatOverCooked
{
	public interface IAddIngredient
	{
		bool CanAdd(IngredientType type);

		void AddIngredientServerRpc(ulong netObjectID);
	}
}
