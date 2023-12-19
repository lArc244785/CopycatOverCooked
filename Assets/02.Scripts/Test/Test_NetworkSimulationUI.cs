using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked
{
	public class Test_NetworkSimulationUI : MonoBehaviour
	{
		private void OnGUI()
		{
			if (GUI.Button(new Rect(20, 40, 80, 20), "Host"))
			{
				NetworkManager.Singleton.StartHost();
			}

			if (GUI.Button(new Rect(20, 70, 80, 20), "Server"))
			{
				NetworkManager.Singleton.StartServer();
			}

			if (GUI.Button(new Rect(20, 100, 80, 20), "Client"))
			{
				NetworkManager.Singleton.StartClient();
			}
		}
	}
}
