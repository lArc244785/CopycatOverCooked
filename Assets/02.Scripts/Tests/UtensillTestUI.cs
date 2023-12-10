using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.NetWork.Untesils;
using UnityEngine.UI;

public class UtensillTestUI : MonoBehaviour
{
    [SerializeField]
    private NetUtensillBase _utensil;
    [SerializeField] private TestPlate _plate;

    [SerializeField] private Button _btnAddOnion;
    [SerializeField] private Button _btnAddTomato;
    [SerializeField] private Button _btnAddShrimp;
    [SerializeField] private Button _btnAddBread;
    [SerializeField] private Button _btnUpdateProgress;
    [SerializeField] private Button _btnSpilPlate;
    [SerializeField] private Button _btnSpil;

    void Start()
    {
        //_btnAddOnion.onClick.AddListener(() => _utensil.AddResourceServerRpc(CopycatOverCooked.Datas.IngredientType.Onion));
        //_btnAddTomato.onClick.AddListener(() => _utensil.AddResourceServerRpc(CopycatOverCooked.Datas.IngredientType.Tomato));
        //_btnAddShrimp.onClick.AddListener(() => _utensil.AddResourceServerRpc(CopycatOverCooked.Datas.IngredientType.Shrimp));
        //_btnAddBread.onClick.AddListener(() => _utensil.AddResourceServerRpc(CopycatOverCooked.Datas.IngredientType.Bread));
        _btnUpdateProgress.onClick.AddListener(() => _utensil.UpdateProgress());
        _btnSpilPlate.onClick.AddListener(() => _plate.TestSoil());
        _btnSpil.onClick.AddListener(()=> _utensil.SpillToTrashServerRpc());
    }

}
