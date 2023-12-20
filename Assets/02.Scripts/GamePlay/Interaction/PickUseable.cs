using CopycatOverCooked.GamePlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace CopycatOverCooked.Interaction
{
	public abstract class PickUseable : Pickable, IUsable
	{
		public abstract void Use(NetworkObject user);
	}
}
