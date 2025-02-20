using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandScript", menuName = "Scriptable Objects/HandScript")]
public class HandScript : ScriptableObject
{
    // list of card prefabs that make up a hand
    [field: SerializeField] public List<CardScript> cards;
}
