using CopycatOverCooked;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Sink : MonoBehaviour
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

    [ServerRpc]
    public void WashingPlateServerRPC()
    {
        if(!_hasPlates.Value)
        {
            return;
        }

        Debug.Log(_testplate.GetComponent<NetworkObject>());

        _testplate.GetComponent<NetworkObject>().Despawn();

        _progressBar.gameObject.SetActive(true);
        StartCoroutine(WashPlateCoroutine());
    }

    IEnumerator WashPlateCoroutine()
    {
        while(_progressBar.value < _washingTime)
        {
            _progressBar.value += Time.deltaTime;

            yield return new WaitForSeconds(0.001f);
        }

        GameObject cleanPlate = Instantiate(_platePrefab, _plateSpawnPoint.transform.position, Quaternion.identity);
        cleanPlate.GetComponent<NetworkObject>().Spawn(true);

        _hasPlates.Value = false;
        _progressBar.gameObject.SetActive(false);
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