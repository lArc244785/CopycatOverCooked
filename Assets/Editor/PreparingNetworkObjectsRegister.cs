using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CopycatOverCooked.NetWork;

public class PreparingNetworkObjectsRegister : Editor
{
    [MenuItem("Tools", menuItem = "Hierarchy registers/Register NetworkObjects")]
    private static void RegisterAllNetworkObjectsInHierarchy()
    {
        PreparingNetworkObjectManager.RegisterAll();
    }
}
