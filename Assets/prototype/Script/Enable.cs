using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable : MonoBehaviour
{
    //これはパズルをプレイするエリアについてるスクリプト

    [SerializeField] private Controller Ctrl;
    [SerializeField] private PlayerScript PlScript;
    [SerializeField] public int Encount;                 //操作可能範囲内にいる敵の数
    [SerializeField] public int count;                   //操作可能範囲内にいるパズルオブジェクト全体の数
    [SerializeField] public List<EnemSc> EnableEnemies = new List<EnemSc>(32);  //操作可能範囲内にいる敵のEnemScクラス
    [SerializeField] public List<EnemSc> AllEnemies = new List<EnemSc>(64);


    void Update()
    {
        if(Encount <= 0)
        {
            Encount = 0;    //エネミー数が負にならないように調整
        }

        List<EnemSc> enem = new List<EnemSc>(EnableEnemies);    //Removeするための詠唱
        foreach (EnemSc Enem in enem)
        {
            if (Enem.Death)
            {
                Encount--;
                count--;
                EnableEnemies.Remove(Enem);
                AllEnemies.Remove(Enem);
                PlScript.Vampireing();

                Ctrl.score += 100;
                PlScript.EXPAdd(Enem.EnemyLevel);
            }
        }
    }

    //操作可能範囲内にものが入っている時
    private void OnTriggerStay2D(Collider2D other)
    {
        if(Ctrl.Game == true )
        {
            if (other.GetComponent<MonoSc>())
            {
                bool init = other.gameObject.GetComponent<MonoSc>().init;
                if (other.gameObject.tag == "Cure"
                    || other.gameObject.tag == "Sword"
                    || other.gameObject.tag == "Mana")
                {
                    if (init == false)
                    {
                        count++;    //敵以外の数を数える
                        other.gameObject.GetComponent<MonoSc>().init = true;
                    }
                }
                else return;
            }
            else if (other.GetComponent<EnemSc>())
            {
                bool init = other.gameObject.GetComponent<EnemSc>().inEnable;
                if (other.gameObject.tag == "Enemy" && init == false)
                {
                    count++;    //全体の数にも加える
                    Encount++;  //敵の数を数える
                    EnemSc EnemScript = other.gameObject.GetComponent<EnemSc>();
                    EnemScript.inEnable = true;
                    EnableEnemies.Add(EnemScript);
                }
                else return;
            }
        }
    }

    /*
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<MonoSc>().init = true;
        }

    }
    */
}
