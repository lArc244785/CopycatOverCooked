using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public void gotoHell(GameObject garbage)
    {
        garbage.GetComponent<NetworkObject>().Despawn();
    }
}
