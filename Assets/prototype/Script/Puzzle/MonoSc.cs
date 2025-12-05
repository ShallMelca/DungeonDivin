using CriWare;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static CriWare.Assets.CriAtomAssetsLoader;

public class MonoSc : MonoBehaviour
{
    //これは剣、ポーション、回復アイテムについているスクリプト

    //Vector2 Pos;      //LaneMoveに使用
    //private bool OutofLane;
    public bool init;


    private void Awake()
    {
        //OutofLane = false;
        init = false;
    }

    //void Update()
    //{
    //    //LaneMove();
    //}

    /*
    void LaneMove()         //変な位置にX座標がずれないように固定するためのメソッド。過去に使用していた
    {
        if (this.transform.position.x > 13)
        {
            Pos.x = 15;
            Zureterude = true;
        }
        else if (this.transform.position.x > 9 && this.transform.position.x <= 13)
        {
            Pos.x = 11;
            Zureterude = true;
        }
        else if (this.transform.position.x > 5 && this.transform.position.x <= 9)
        {
            Pos.x = 7;
            Zureterude = true;
        }
        else if (this.transform.position.x > 1 && this.transform.position.x <= 5)
        {
            Pos.x = 3;
            Zureterude = true;
        }
        else if (this.transform.position.x > -3 && this.transform.position.x <= 1)
        {
            Pos.x = -1;
            Zureterude = true;
        }
        else if (this.transform.position.x > -7 && this.transform.position.x <= -3)
        {
            Pos.x = -5;
            Zureterude = true;
        }
        else if (this.transform.position.x <= -7)
        {
            Pos.x = -9;
            Zureterude = true;
        }

        if (Zureterude == true)
        {
            Pos.y = this.transform.position.y;
            this.transform.position = Pos;
            Zureterude = false;
        }
    }
    */
}
