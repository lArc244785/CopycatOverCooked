using Unity.Netcode;

namespace CopycatOverCooked.NetWork
{
	public static class NetworkBehaviourExtensions
	{
		public const ulong NULL_NETWORK_OBJECT_ID = ulong.MaxValue;

		public static bool TryGet(this NetworkBehaviour networkBehaviour, ulong networkObjectID, out NetworkObject networkObject)
		{
			networkObject = null;
			if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectID, out networkObject))
				return true;

			return false;
		}

	}
}