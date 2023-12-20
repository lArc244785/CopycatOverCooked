using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using CopycatOverCooked.Datas;
using System.Collections.Generic;

namespace CopycatOverCooked.Orders
{
    public class OrderUI : NetworkBehaviour
    {
        public StageData this[IngredientType type] => _stageData[type];
        private Dictionary<IngredientType, StageData> _stageData;
    }
}
