using CopycatOverCooked.Datas;
using System.Collections.Generic;
using UnityEngine;

namespace CopycatOverCooked.Orders
{
    [CreateAssetMenu(fileName = "new StageData", menuName = "ScriptableObjects/StageData")]
    public class StageData : ScriptableObject
    {
        public List<IngredientType> menu;
        public float orderPeriod;
    } //È÷È÷ ¹ß½Î
}