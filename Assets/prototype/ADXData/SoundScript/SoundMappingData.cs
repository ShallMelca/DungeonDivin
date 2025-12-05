using CriWare.Assets;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound/Master Mapping Data")]
public class SoundMappingData : ScriptableObject
{
    // 全てのマッピング情報をリストで保持.
    public List<CueMappingEntry> Entries;
}

// CueMappingEntry.cs (インスペクターで設定する単一のマッピング).
[System.Serializable]
public class CueMappingEntry
{
    public E_Sounds Key;

    public CriAtomCueReference AcbReference;

    public string PlayerKey;
}