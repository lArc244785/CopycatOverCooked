using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.NetWork.Untesils;
using UnityEngine.UI;
using Unity.VisualScripting;
using CopycatOverCooked;
using Unity.Netcode;

public class UtensillTestUI : MonoBehaviour
{
    [SerializeField]
    private NetUtensillBase _utensil;
    [SerializeField] private Plate _plate;
    [SerializeField] private Sink _sink;

    [SerializeField] private Button _btnAddOnion;
    [SerializeField] private Button _btnAddTomato;
    [SerializeField] private Button _btnAddShrimp;
    [SerializeField] private Button _btnAddBread;
    [SerializeField] private Button _btnSpillToPlate;
    [SerializeField] private Button _btnUntensillSpillToTrash;
    [SerializeField] private Button _btnPlateSpillToTrash;
    [SerializeField] private Button _btnAddPlate;
    [SerializeField] private Button _btnWashPlate;

    void Start()
    {
        _btnAddOnion.onClick.AddListener(() => _utensil.AddResourceServerRpc((int)CopycatOverCooked.Datas.IngredientType.Onion));
        _btnAddTomato.onClick.AddListener(() => _utensil.AddResourceServerRpc((int)CopycatOverCooked.Datas.IngredientType.Tomato));
        _btnAddShrimp.onClick.AddListener(() => _utensil.AddResourceServerRpc((int)CopycatOverCooked.Datas.IngredientType.Shrimp));
        _btnAddBread.onClick.AddListener(() => _utensil.AddResourceServerRpc((int)CopycatOverCooked.Datas.IngredientType.Bread));
        _btnSpillToPlate.onClick.AddListener(() => _utensil.SpillToPlateServerRpc());
        _btnUntensillSpillToTrash.onClick.AddListener(()=> _utensil.SpillToTrashServerRpc());
        _btnPlateSpillToTrash.onClick.AddListener(() => _plate.EmptyServerRpc());
        _btnAddPlate.onClick.AddListener(() => _sink.AddPlate());
        _btnWashPlate.onClick.AddListener(() => _sink.WashingPlateServerRpc());
 	}
}