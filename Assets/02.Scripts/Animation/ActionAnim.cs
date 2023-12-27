using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Idle,
    Grip,
    Walk,
    GripAndWalk,
    Cutting,
    Wash
}

public class ActionAnim : StateMachineBehaviour
{
    [SerializeField] private State _action;

}
