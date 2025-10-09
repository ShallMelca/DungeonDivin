using CriWare;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static CriWare.Assets.CriAtomAssetsLoader;

public class EnemSc : MonoBehaviour
{
    //これは敵についているスクリプト

    //[System.NonSerialized] private Vector2 Pos;
    [SerializeField] public int EnemyLevel;
    [System.NonSerialized] public int EnemyHp;          //敵の体力
    [System.NonSerialized] public int EnemyAttack;      //敵の攻撃力
    [System.NonSerialized] public bool inEnable;        //EnableAreaに入ってるかどうか
    [System.NonSerialized] public bool Death;           //死んだかどうか

    [SerializeField] private TextMeshProUGUI HP_UI;
    [SerializeField] private Canvas EnemUI;
    [SerializeField] private TextMeshProUGUI Attack_UI;
    [Space(10)]
    [SerializeField] private List<Vector3> Level_HPAttRecast = new List<Vector3>(32);
        //Vector3(x = HP, y = Attack, z = Recast)

    //音
    [SerializeField] private CriWare.Assets.CriAtomCueReference CueRefarence;

    private void Awake()
    {
        inEnable = false;
        LevChange(EnemyLevel - 1);
        //EnemyRecast = MaxEnemyRecast;
        //Debug.Log($"EnemLev = {EnemyLevel}, HP = {EnemyHp}, Recast = {EnemyRecast}");
    }

    void Update()
    {
        HP_UI.text = EnemyHp.ToString();
        //Pos.y = this.transform.position.y;
        //LaneMove();
        if(EnemyHp <= 0)
        {
            Disbl();
            ADXSoundManager.Instance.PlaySound("death", CueRefarence.AcbAsset.Handle, 006, null, false);
        }
        if(Death == true && inEnable == true)
        {
            Disbl();
        }
        EnemUI.transform.rotation = Quaternion.Euler(0,0,0);
    }

    void Disbl()     //このパズルオブジェクトが敵で、倒された時に呼び出すメソッド
    {
        Death = true;
        Destroy(this.gameObject);
    }

    public void ChangeLev(int lev)
    {
        if (!inEnable)
        {
            EnemyLevel = lev;
            if (Level_HPAttRecast.Count > EnemyLevel)
            {
                LevChange(EnemyLevel - 1);
            }
            else
            {
                LevChange(Level_HPAttRecast.Count - 1);
            }
        }
    }

    void LevChange(int LevValueIndex)
    {
        EnemyHp = (int)Mathf.Round(Level_HPAttRecast[LevValueIndex].x);
        EnemyAttack = (int)Mathf.Round(Level_HPAttRecast[LevValueIndex].y);
        Attack_UI.text = EnemyAttack.ToString();
    }

    /*
    void LaneMove()         //変な位置にX座標がずれないように固定するためのメソッド
    {
        if (this.transform.position.x > 13)
        {
            Pos.x = 15;
        }
        else if (this.transform.position.x > 9 && this.transform.position.x <= 13)
        {
            Pos.x = 11;
        }
        else if (this.transform.position.x > 5 && this.transform.position.x <= 9)
        {
            Pos.x = 7;
        }
        else if (this.transform.position.x > 1 && this.transform.position.x <= 5)
        {
            Pos.x = 3;
        }
        else if (this.transform.position.x > -3 && this.transform.position.x <= 1)
        {
            Pos.x = -1;
        }
        else if (this.transform.position.x > -7 && this.transform.position.x <= -3)
        {
            Pos.x = -5;
        }
        else if (this.transform.position.x <= -7)
        {
            Pos.x = -9;
        }

        this.transform.position = Pos;
    }
    */
}
