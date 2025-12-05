using CriWare;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagicManager : MonoBehaviour
{
    //敵をすべて消し去る魔法のスクリプト

    [SerializeField] private PlayerScript plScript;
    [SerializeField] private Enable enable;

    [SerializeField] public bool destroyBool;
    [SerializeField] private TextMeshProUGUI changeText;
    private Color magicColor;
    private Color disapperColor;


    void Awake()
    {
        magicColor = changeText.color;
        disapperColor = new Color(0, 0, 0, 0);
        changeText.color = disapperColor;
        destroyBool = false;
    }

    void Update()
    {
        if(plScript.GetMP >= 20)
        {
            changeText.color = magicColor;
        }
        else
        {
            changeText.color = disapperColor;
        }
    }

    public void Magic()
    {
        if (plScript.GetMP < 20) return;

        destroyBool = true;
        plScript.MPGensyo(20);
        foreach (var item in enable.enableEnemies)
        {
            item.death = true;
        }
        Invoke(nameof(FalseDestroyBool), 0.15f);
        ADXSoundManager.Instance.PlaySound(E_Sounds.SE_Magic);
    }

    void FalseDestroyBool()
    {
        destroyBool = false;
    }
}
