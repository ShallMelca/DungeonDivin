using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimSc : MonoBehaviour
{
    //これがLimitArea(パズルプレイ部分のそこそこ上にあるTrigger当たり判定)に触れると
    //新しいパズルパネルが生成されるための「Contoroller」スクリプトのBoolがTrueになったり
    //「ここがギリギリになったよ！」っていうのを送ったりするスクリプト。

    [SerializeField] private Controller Ctrl;
    private string LimArea_Name = "LimitArea";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == LimArea_Name)
        {
            Ctrl.LimAreaintered = true;
            Ctrl.InterLimObj = this.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == LimArea_Name)
        {
            Ctrl.LimAreaintered = true;
            Ctrl.InterLimObj = this.gameObject;
        }
    }
}
