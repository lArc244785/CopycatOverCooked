using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TEST : MonoBehaviour
{
    [SerializeField] private TMP_Text _temp;

    public void print(string a)
    {
        Debug.Log(_temp);
        _temp.text = a;
    }

    public void hide()
    {
        this.gameObject.SetActive(false);
    }
}
