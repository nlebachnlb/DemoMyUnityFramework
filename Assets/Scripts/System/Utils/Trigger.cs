using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerTypes
{
    Entrance, Exit
}

public class Trigger : MonoBehaviour
{
    public TriggerTypes type;
}
