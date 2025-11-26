using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIcon : MonoBehaviour
{
    [SerializeField] private Image thisImg;
    [SerializeField] private List<Sprite> abilityimg = new List<Sprite>(16);
    int index;

    private void Awake()
    {
        thisImg.enabled = false;
    }

    public void DecideImg(string AbilityName)
    {
        switch (AbilityName)
        {
            case "Thone":
                ChangeImg(0);
                break;
            case "CureBody":
                ChangeImg(1);
                break;
            case "AdaptMana":
                ChangeImg(2);
                break;
            case "Revenger":
                ChangeImg(3);
                break;
            case "Avoiding":
                ChangeImg(4);
                break;
            case "Vampire":
                ChangeImg(5);
                break;
        }
    }

    void ChangeImg(int i)
    {
        index = i;
        thisImg.sprite = abilityimg[index];
        thisImg.enabled = true;
    }
}
