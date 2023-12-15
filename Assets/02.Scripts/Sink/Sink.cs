using CopycatOverCooked;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Sink : NetworkBehaviour
{
    [SerializeField] private GameObject _platePrefab;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Plate _testplate;
    [SerializeField] private GameObject _plateSpawnPoint;

    private float _washingTime = 3f;
    public NetworkVariable<bool> _hasPlates;
    private NetworkObject _plate;

    private void Awake()
    {
        _hasPlates = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        _progressBar.gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    //[ClientRpc]
    public void SliderActive()
    {
        _progressBar.gameObject.SetActive(true);
    }

    //[ClientRpc]
    public void SliderInactive() 
    {
        _progressBar.gameObject.SetActive(false);
        _progressBar.value = 0;
    }

    [ServerRpc]
    public void SpawnPlateServerRpc()
    {
        GameObject cleanPlate = Instantiate(_platePrefab, _plateSpawnPoint.transform.position, Quaternion.identity);
        cleanPlate.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc]
    public void WashingPlateServerRpc()
    {
        if(!_hasPlates.Value)
        {
            return;
        }

        _testplate.GetComponent<NetworkObject>().Despawn();

        SliderActive();
        CorotineClientRpc();
    }

    [ClientRpc]
    public void CorotineClientRpc()
    {
        StartCoroutine(WashPlateCoroutine());
    }

    IEnumerator WashPlateCoroutine()
    {
        while(_progressBar.value < _washingTime)
        {
            _progressBar.value += Time.deltaTime;

            yield return new WaitForSeconds(0.001f);
        }

        SpawnPlateServerRpc();

        _hasPlates.Value = false;
        SliderInactive();
    }

    public void AddPlate(/* NetworkObject _dirtyPlate */)
    {
        if(_hasPlates.Value == true)
        {
            return;
        }

        /*_plate = _dirtyPlate;*/

        _hasPlates.Value = true;
    }
}