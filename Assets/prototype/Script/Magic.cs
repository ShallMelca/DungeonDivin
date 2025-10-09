using CriWare;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Magic : MonoBehaviour
{
    //敵をすべて消し去る魔法のスクリプト

    [SerializeField] private PlayerScript PL;
    [SerializeField] private Enable Enab;

    public bool DestroyBool;
    [SerializeField] private TextMeshProUGUI ChangeText;
    Color MagicColor;
    Color Disapper;

    [SerializeField] CriWare.Assets.CriAtomCueReference CueRefarence;

    void Awake()
    {
        MagicColor = ChangeText.color;
        Disapper = new Color(0, 0, 0, 0);
        ChangeText.color = Disapper;
        DestroyBool = false;
    }

    void Update()
    {
        if(PL.NowMP >= 20)
        {
            ChangeText.color = MagicColor;
        }
        else
        {
            ChangeText.color = Disapper;
        }
    }

    public void magic()
    {
        if (PL.NowMP >= 20)
        {
            DestroyBool = true;
            PL.MPGensyo(20);
            foreach (var item in Enab.EnableEnemies)
            {
                item.Death = true;
            }
            Invoke(nameof(ddes), 0.15f);
            ADXSoundManager.Instance.PlaySound("Magic", CueRefarence.AcbAsset.Handle, 007, null, false);
        }
    }
    void ddes()
    {
        DestroyBool = false;
    }
}
