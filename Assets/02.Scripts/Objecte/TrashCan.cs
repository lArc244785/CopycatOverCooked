using CopycatOverCooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
	public void TrashUse(ISpill spill)
	{
		spill.Spill();
	}
}
