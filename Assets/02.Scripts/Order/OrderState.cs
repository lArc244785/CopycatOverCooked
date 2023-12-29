using System;
using Unity.Netcode;
using Unity.VisualScripting;

namespace CopycatOverCooked.Orders
{
    public struct OrderState : INetworkSerializeByMemcpy, IEquatable<OrderState>
    {
        public uint ingredientType; 
        public float timeMark; // 주문한 시간

        public OrderState(uint ingredientType, float timeMark)
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