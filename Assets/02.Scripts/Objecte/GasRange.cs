using CopycatOverCooked.Untesil;
using UnityEngine;

namespace CopycatOverCooked.Object
{
	internal class GasRange : Table
	{
		private PickUtensil _putUtensil;

		private void Start()
		{
			onChangeputNetworkObject += UpdatePutUtensil;
		}

		private void UpdatePutUtensil(ulong objectID)
		{
			if (objectID == EMPTY_PUT_OBJECT)
				_putUtensil = null;
			else if (TryGetPutObject(out var putObject))
				_putUtensil = putObject.GetComponent<PickUtensil>();
		}


		private void Update()
		{
			if (IsServer == false)
				return;

			if (_putUtensil == null)
				return;

			_putUtensil.UpdateProgressServerRpc(Time.deltaTime);

		}

	}
}
