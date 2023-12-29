using CopycatOverCooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.GamePlay;
using Unity.Netcode;

public class TrashCan : NetworkBehaviour , IInteractable 
{
	public InteractableType type => InteractableType.TrashCan;

	public void BeginInteraction(Interactor interactor)
	{

	}

	public void EndInteraction(Interactor interactor)
	{
	}

}
