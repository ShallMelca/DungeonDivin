using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityEnum
{
    Thone,
    CureBody,
    AdaptMana,
    Revenger,
    Avoiding,
    Vampire,
    AddAttack,
    AddHP
}

[CreateAssetMenu(fileName = "AbilityData", menuName = "CustomData/Ability Data")]
public class AbilitiesData : ScriptableObject
{
    public List<AbilityData> abilities;
}

[System.Serializable]
public class AbilityData
{
    public AbilityEnum name;
    public string name_str;
    [TextArea(2,3)]public string description;
    public Sprite sprite;
}
