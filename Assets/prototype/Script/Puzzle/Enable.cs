using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable : MonoBehaviour
{
    //これはパズルをプレイするエリアについてるスクリプト

    [SerializeField] private Controller ctrl;
    [SerializeField] private PlayerScript plScript;
    [SerializeField] public int encount;                 //操作可能範囲内にいる敵の数
    [SerializeField] public int count;                   //操作可能範囲内にいるパズルオブジェクト全体の数
    [SerializeField] public List<EnemSc> enableEnemies = new List<EnemSc>(32);  //操作可能範囲内にいる敵のEnemScクラス
    [SerializeField] public List<EnemSc> allEnemies = new List<EnemSc>(64);


    void Update()
    {
        if(encount <= 0)
        {
            encount = 0;    //エネミー数が負にならないように調整
        }

        List<EnemSc> enem = new List<EnemSc>(enableEnemies);    //Removeするための準備
        foreach (EnemSc Enem in enem)
        {
            if (!Enem.death) return;

            encount--;
            count--;
            enableEnemies.Remove(Enem);
            allEnemies.Remove(Enem);
            //plScript.Vampireing();

            ctrl.score += 100;
            plScript.EXPAdd(Enem.enemyLevel);
        }
    }

    /// <summary>
    /// 操作可能範囲内にものが入っている時
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!ctrl.game) return;

        if (other.GetComponent<MonoSc>())
        {
            bool init = other.gameObject.GetComponent<MonoSc>().init;
            if (other.gameObject.tag == "Cure"
                || other.gameObject.tag == "Sword"
                || other.gameObject.tag == "Mana")
            {
                if (init) return;
                count++;    //敵以外の数を数える
                other.gameObject.GetComponent<MonoSc>().init = true;
            }
        }
        else if (other.GetComponent<EnemSc>())
        {
            bool init = other.gameObject.GetComponent<EnemSc>().inEnable;
            if (other.gameObject.tag != "Enemy" || init == true) return;

            count++;    //全体の数にも加える
            encount++;  //敵の数を数える
            EnemSc EnemScript = other.gameObject.GetComponent<EnemSc>();
            EnemScript.inEnable = true;
            enableEnemies.Add(EnemScript);
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
