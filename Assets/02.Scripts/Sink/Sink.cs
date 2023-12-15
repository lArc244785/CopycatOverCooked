using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Sink : MonoBehaviour
{
    [SerializeField] private GameObject _platePrefab;

    private float _washingTime = 3f;
    public NetworkVariable<bool> _hasPlates = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkObject _plate;

    [ServerRpc]
    public void WashingPlateServerRPC()
    {
        _plate.Despawn();

        /*
         
        Progress Bar
        
        */

        GameObject cleanPlate = Instantiate(_platePrefab, Vector3.up,Quaternion.identity);
        cleanPlate.GetComponent<NetworkObject>().Spawn(true);

        _hasPlates.Value = false;
    }

    public void AddPlate(NetworkObject _dirtyPlate)
    {
        if(_hasPlates.Value == true)
        {
            return;
        }

        _plate = _dirtyPlate;
        _hasPlates.Value = true;
    }
}