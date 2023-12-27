using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CopycatOverCooked.GamePlay
{
	

	public class StageManager : MonoBehaviour
	{
		public enum Step
		{
			Idle,
			WaitUntilAllPlayersAreReady,
			BeforeStartStage,
			StartStage,
			AfterStartStage,
			DuringStartStage,
		}
		public static StageManager instance;
		public Step current;

		private void Awake()
		{
			instance = this;
		}


	}
}
