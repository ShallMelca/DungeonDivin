using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    [Header("Ability")]

    [SerializeField] private AbilitiesData abilityData;
    [SerializeField] private AbilitiesData abilityData_AfterLevelMax;

    [SerializeField] private Controller ctrl;   //Controllerスクリプトを参照
    [SerializeField] private Enable enab;       //EnableAreaからスクリプトを参照
    [SerializeField] private PlayerScript mainPlayerSctipt;
    [SerializeField] private AbilityButtonScript abilityButtonScript;

    private bool _IsAllAbilitiesLevelMax = false;
    private bool _IsAbilitiesCountMax = false;

    void Awake()
    {
        abilityButtonScript.buttonAction += AbilityLevelUp;
    }

    public void AbilityLevelUp()
    {
        ctrl.game = false;
        if(_IsAllAbilitiesLevelMax)
        {
            AfterAbilityLevelMax();
        }
        else
        {
            if(_IsAbilitiesCountMax)
            {
                LevelUpAbilities();
            }
            else
            {
                ChooseOrLevelUpAbilities();
            }
        }
    }

    void AfterAbilityLevelMax()
    {
        abilityButtonScript.Setup_AbilityChooseButton(abilityData_AfterLevelMax);
    }

    void LevelUpAbilities()
    {

    }

    void ChooseOrLevelUpAbilities()
    {

    }

    /// <summary>
    /// アビリティ選択ボタンが押されたときに実行される、各アビリティごとに変わるレベルアップ項目
    /// </summary>
    /// <param name="abilityEnum"></param>
    void AbilityLevelUp(AbilityEnum abilityEnum)
    {
        switch (abilityEnum)
        {
            case AbilityEnum.Thone:
                break;

            case AbilityEnum.CureBody:
                break;

            case AbilityEnum.AdaptMana:
                break;

            case AbilityEnum.Revenger:
                break;

            case AbilityEnum.Avoiding:
                break;

            case AbilityEnum.Vampire:
                break;

            case AbilityEnum.AddAttack:
                break;

            case AbilityEnum.AddHP:
                break;
        }
    }

    /*
    /// <summary>
    /// アビリティ辞書から一個取り出す
    /// </summary>
    private void ChoiceAbility()
    {
        int PickNum = UnityEngine.Random.Range(0, choicingAbtDic.Count - 1);
        choicedAbtNumber = PickNum;
        choicedAbtName = choicingAbtDic[PickNum];
        choicingAbtNames.Add(choicedAbtName);
        choicingAbtDic.Remove(choicedAbtName);
    }

    /// <summary>
    /// アビリティ選択を始める
    /// </summary>
    private void StartChoiceAbility()
    {
        ctrl.game = false;
        ChoiceAbility();
        abilityButtons[0].SetUp_Ability(choicedAbtName);
        ChoiceAbility();
        abilityButtons[1].SetUp_Ability(choicedAbtName);
        abilityChoiceCanvas.enabled = true;
    }

    /// <summary>
    /// 追加アビリティを決定する
    /// </summary>
    /// <param name="AbilityName"></param>
    public void DecideAbility(string AbilityName)
    {
        if (nowAbilities.Count != 3)    //アビリティ、何を選ぶか？を選んでいる時
        {
            //手持ちに反映
            //新規アビリティか、もうあるもののレベルアップか
            int index;
            if (nowAbilities.Contains(AbilityName) == false)    //今あるアビリティの中に取ったアビリティがなかったら
            {
                Debug.Log("無かったよ");
                //選択中の諸々の整理
                choicingAbtNames.Remove(AbilityName);    //選ばれた方を削除
                choicingAbtDic.Add(choicingAbtNames[0]);    //選ばれなかった方を戻す
                choicingAbtNames.Clear();                   //空っぽにする

                if (nowAbilities.Count >= maxAbilityNum) return;

                nowAbilities.Add(AbilityName);
                abilitieLevels.Add(1);
                AbilityLevUP(AbilityName);
                abilityIcon[nowAbilities.Count - 1].DecideImg(AbilityName);

                if (nowAbilities.Count != maxAbilityNum) return;

                choicingAbtDic.Clear();
                for (int i = 0; i < maxAbilityNum; i++)
                {
                    choicingAbtDic.Add(nowAbilities[i]);
                }
            }
            else
            {
                index = nowAbilities.IndexOf(AbilityName);
                if (abilitieLevels[index] >= maxAbilityLevel) return;

                abilitieLevels[index]++;
                AbilityLevUP(AbilityName);
                if (abilitieLevels[index] == maxAbilityLevel)
                {
                    abilityDictionary.Remove(AbilityName);
                }
                Debug.Log($"{AbilityName}.Level = {abilitieLevels[index]}");

                choicingAbtDic.Add(choicingAbtNames[0]);
                choicingAbtDic.Add(choicingAbtNames[1]);
                choicingAbtNames.Clear();

            }
        }
        else        //アビリティえらびおわったあとの処理
        {
            int index = nowAbilities.IndexOf(AbilityName);
            if (abilitieLevels[index] < maxAbilityLevel)
            {
                choicingAbtDic.Add(choicingAbtNames[0]);
                choicingAbtDic.Add(choicingAbtNames[1]);
                choicingAbtNames.Clear();

                abilitieLevels[index]++;
                AbilityLevUP(AbilityName);
                if (abilitieLevels[index] == maxAbilityLevel)
                {
                    abilityDictionary.Remove(AbilityName);
                    choicingAbtDic.Remove(AbilityName);
                }
                Debug.Log($"{AbilityName}.Level = {abilitieLevels[index]}");

                if (abilitieLevels[0] == maxAbilityLevel
                    && abilitieLevels[1] == maxAbilityLevel
                    && abilitieLevels[2] == maxAbilityLevel)
                {
                    finishAbiLevUp = true;
                    choicingAbtDic.Clear();
                }
            }

            int foreach_Index = 0;
            foreach (int item in abilitieLevels)
            {
                if (item == maxAbilityLevel) continue;

                if (!choicingAbtDic.Contains(nowAbilities[foreach_Index]))
                {
                    choicingAbtDic.Add(nowAbilities[foreach_Index]);
                }

                foreach_Index++;
            }
            if (choicingAbtDic.Count == 1)
            {
                choicingAbtDic.Add(choicingAbtDic[0]);
            }
        }

        choicedAbtName = null;
        abilityChoiceCanvas.enabled = false;
        ctrl.game = true;
    }

    void AbilityLevUP(string AbilityName)
    {
        switch (AbilityName)
        {
            case "Thone":
                thonePoint++;
                break;
            case "CureBody":
                curePoint++;
                break;
            case "AdaptMana":
                maxMP += 20;
                mpMax_UI.text = maxMP.ToString();
                mpBar.maxValue = maxMP;
                break;
            case "Avoiding":
                avoidPercent += 10;
                break;
            case "Vampire":
                vampPoint++;
                break;
            default:
                abilityDictionary.Remove(choicedAbtName);
                break;
        }
    }

    //CureBody
    void CureBody_Cureing()
    {
        if (nowAbilities.Contains("CureBody"))
        {
            nowHP += curePoint;
            ChangeHP();
        }
    }

    //Vampire
    public void Vampireing()
    {
        if (nowAbilities.Contains("Vampire"))
        {
            nowHP += vampPoint;
            ChangeHP();
        }
    }

    /// <summary>
    /// Revenger
    /// </summary>
    /// <param name="PLAtt"></param>
    public void Revengeing(int PLAtt)
    {
        if (!nowAbilities.Contains("Revenger")) return;

        if (hp_Percent >= 0.3)
        {
            revengePoint = 3;
        }
        else if (hp_Percent >= 0.5)
        {
            revengePoint = 2;
        }
        else if (hp_Percent >= 0.7)
        {
            revengePoint = 1;
        }
        else
        {
            revengePoint = 0;
        }
        PLAtt += revengePoint;

    }


    private void LevUp_AftAbility()
    {
        ctrl.game = false;
        abilityButtons[0].SetUp_AfterAbility();
        abilityButtons[1].SetUp_AfterAbility();
        abilityChoiceCanvas.enabled = true;
    }

    public void Decide_AftAbility(string Erabareta_Hou)
    {
        switch (Erabareta_Hou)
        {
            case "Add_HP":
                maxHP += 5;8
                nowHP = maxHP;
                hpBar.maxValue = maxHP;
                hpMax_UI.text = maxHP.ToString();
                break;
            case "Add_AttackPow":
                playerAttackPT++;
                break;
        }

        abilityChoiceCanvas.enabled = false;
        ctrl.game = true;
    }
    */
}
