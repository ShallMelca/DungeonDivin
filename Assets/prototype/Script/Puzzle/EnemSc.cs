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
    [SerializeField] public int enemyLevel;
    [System.NonSerialized] public int enemyHp;          //敵の体力
    [System.NonSerialized] public int enemyAttack;      //敵の攻撃力
    [System.NonSerialized] public bool inEnable;        //EnableAreaに入ってるかどうか
    [System.NonSerialized] public bool death;           //死んだかどうか

    [SerializeField] private TextMeshProUGUI hp_UI;
    [SerializeField] private Canvas enemUI;
    [SerializeField] private TextMeshProUGUI attack_UI;
    [Space(10)]
    [SerializeField] private List<Vector3> level_HPAttRecast = new List<Vector3>(32);
        //Vector3(x = HP, y = Attack, z = Recast)

    private void Awake()
    {
        inEnable = false;
        LevChange(enemyLevel - 1);
        //EnemyRecast = MaxEnemyRecast;
        //Debug.Log($"EnemLev = {EnemyLevel}, HP = {EnemyHp}, Recast = {EnemyRecast}");
    }

    void Update()
    {
        hp_UI.text = enemyHp.ToString();
        //Pos.y = this.transform.position.y;
        //LaneMove();
        if(enemyHp <= 0)
        {
            Disable();
            ADXSoundManager.Instance.PlaySound(E_Sounds.SE_EnemyDeath);
        }
        if(death == true && inEnable == true)
        {
            Disable();
        }
        enemUI.transform.rotation = Quaternion.Euler(0,0,0);
    }

    /// <summary>
    /// 倒された時に呼び出すメソッド
    /// </summary>
    void Disable()
    {
        death = true;
        Destroy(this.gameObject);
    }

    public void ChangeLev(int lev)
    {
        if (inEnable) return;

        enemyLevel = lev;
        if (level_HPAttRecast.Count > enemyLevel)
        {
            LevChange(enemyLevel - 1);
        }
        else
        {
            LevChange(level_HPAttRecast.Count - 1);
        }

    }

    void LevChange(int LevValueIndex)
    {
        Debug.Log($"EnemyLevel:{LevValueIndex}");
        enemyHp = (int)Mathf.Round(level_HPAttRecast[LevValueIndex].x);
        enemyAttack = (int)Mathf.Round(level_HPAttRecast[LevValueIndex].y);
        attack_UI.text = enemyAttack.ToString();
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
