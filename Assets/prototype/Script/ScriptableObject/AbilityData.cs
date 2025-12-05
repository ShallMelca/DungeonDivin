using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ability
{
    Thone,
    CureBody,
    AdaptMana,
    Revenger,
    Avoiding,
    Vampire
}

[CreateAssetMenu(fileName = "AbilityData", menuName = "CustomData/Ability Data")]
public class AbilityData : ScriptableObject
{
    Ability name;
    string name_str;
    string description;
    Sprite sprite;
}
/*
身体にとげが生える。
敵に{thone}ダメージの反撃

回復能力が高まる。
毎ターンHP{CurePoint}回復

魔力により適合した。
MPの上限+20

逆境ででこそ輝く。
HPの割合に応じて攻撃力増加

狭い空間でも回避できる。
{AvoidPercent}%で攻撃を回避

敵の血で傷が治る。
敵を倒すとHP回復

基礎攻撃力+1


*/
