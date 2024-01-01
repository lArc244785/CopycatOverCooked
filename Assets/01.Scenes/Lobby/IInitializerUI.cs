using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InitializerUI : MonoBehaviour
{
    [SerializeField] private GameObject UI;

    private void Awake()
    {
        if (UI.TryGetComponent(out Initializer initializer))
        {
            initializer.Init();
        }
    }
}