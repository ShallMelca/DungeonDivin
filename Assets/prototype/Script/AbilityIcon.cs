using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    [SerializeField] private Image ThisImg;
    [SerializeField] private List<Sprite> Abilityimg = new List<Sprite>(16);
    int index;

    private void Awake()
    {
        ThisImg.enabled = false;
    }

    public void DecideImg(string AbilityName)
    {
        switch (AbilityName)
        {
            case "Thone":
                index = 0;
                ChangeImg(index);
                ThisImg.enabled = true;
                break;
            case "CureBody":
                index = 1;
                ChangeImg(index);
                ThisImg.enabled = true;
                break;
            case "AdaptMana":
                index = 2;
                ChangeImg(index);
                ThisImg.enabled = true;
                break;
            case "Revenger":
                index = 3;
                ChangeImg(index);
                ThisImg.enabled = true;
                break;
            case "Avoiding":
                index = 4;
                ChangeImg(index);
                ThisImg.enabled = true;
                break;
            case "Vampire":
                index = 5;
                ChangeImg(index);
                ThisImg.enabled = true;
                break;
        }
    }

    void ChangeImg(int index)
    {
        ThisImg.sprite = Abilityimg[index];
    }
}
