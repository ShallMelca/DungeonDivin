using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ability
{
    Thone,
    CureBody,
    AdaptMana,
    Revenger,
    Avoiding,
    Vampire
}

[CreateAssetMenu(fileName = "AbilityData", menuName = "CustomData/Ability Data")]
public class AbilityData : ScriptableObject
{
    Ability name;
    string name_str;
    string description;
    Sprite sprite;
}
