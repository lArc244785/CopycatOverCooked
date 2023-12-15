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

    [ClientRpc]
    public void SliderActiveClientRpc()         //슬라이더 On
    {
        _progressBar.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void SliderInactiveClientRpc()       //슬라이더 Off
    {
        _progressBar.gameObject.SetActive(false);
        _progressBar.value = 0;
    }

    [ClientRpc]
    public void SpawnPlateClientRpc()
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

        SliderActiveClientRpc();
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

            yield return null;
        }

        SpawnPlateClientRpc();

        _hasPlates.Value = false;
        SliderInactiveClientRpc();
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