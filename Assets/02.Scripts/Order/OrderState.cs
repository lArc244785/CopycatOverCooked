using System;
using Unity.VisualScripting;

namespace CopycatOverCooked.Orders
{
    public struct OrderState : IEquatable<OrderState>
    {
        public int ingredientType;
        public float timeMark;

        public OrderState(int ingredientType, float timeMark)
        {
            this.ingredientType = ingredientType;
            this.timeMark = timeMark;
        }

        public bool Equals(OrderState other)
        {
            return (this.ingredientType == other.ingredientType) &&
                   (this.timeMark == other.timeMark);
        }
    }
}