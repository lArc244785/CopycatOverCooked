using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.NetWork
{
	public class PreparingNetworkObjectManager : MonoBehaviour
	{
		public static PreparingNetworkObjectManager instance;
		public List<NetworkObject> networkObjects;

#if UNITY_EDITOR
		public static PreparingNetworkObjectManager RegisterAll()
		{
			PreparingNetworkObjectManager manager = GameObject.FindObjectOfType<PreparingNetworkObjectManager>();
			if (manager == null)
				manager = new GameObject(nameof(PreparingNetworkObjectManager)).AddComponent<PreparingNetworkObjectManager>();
			else
				manager.networkObjects.Clear();

			manager.networkObjects = GameObject.FindObjectsOfType<NetworkObject>().ToList();
			return manager;
		}
#endif

		private void Awake()
		{
			instance = this;
		}
	}
}
