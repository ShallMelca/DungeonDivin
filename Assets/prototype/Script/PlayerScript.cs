using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    //プレイヤーに関する、HPやMPなどの数値を管理する

    [SerializeField] public int PL_Attack;          //Playerのその時の基礎攻撃力
    [SerializeField] public int Level;              //現在のレベル
    [SerializeField] public Animator PLAnim;

    [Space(10)]

    [Header("HP")]
    [SerializeField] private int MaxHP = 50;        //inspecterから：HPの「最大値」
    [SerializeField] public float HP_Percent;
    [System.NonSerialized] public int NowHP;        //HPの現在値
    [SerializeField] private Slider HpBar;          //HPバー
    [SerializeField] TextMeshProUGUI HpNow_UI;         //HPバーの数値
    [SerializeField] TextMeshProUGUI HpMax_UI;         //HPバーの数値

    [Header("MP")]
    [SerializeField] public int MaxMP;              //inspecterから：MPの「最大値」
    [System.NonSerialized] public int NowMP;        //MPの現在値
    [SerializeField] public Slider MpBar;           //MPバー
    [SerializeField] public TextMeshProUGUI MpNow_UI;  //MPバーの現在値
    [SerializeField] public TextMeshProUGUI MpMax_UI;  //MPバーの最大値

    [Header("EXP")]
    [SerializeField] public int ToNextLevelEXP;             //次のレベルまでの経験値量
    [System.NonSerialized] private int ToLevUp_UpRange;     //レベルあがった時に上のがどのくらい増えるか
    [System.NonSerialized] public int EXP;                  //経験値
    [SerializeField] public Slider ExpBar;                  //経験値バー
    [SerializeField] public TextMeshProUGUI ExpNow_UI;     //経験値バーの数値/現在値
    [SerializeField] public TextMeshProUGUI ExpMax_UI;     //経験値バーの数値/次のレベルまで

    [Space(10)]

    [SerializeField] private Controller Ctrl;   //Controllerスクリプトを参照
    [SerializeField] private Enable Enab;       //EnableAreaからスクリプトを参照

    [Header("Ability")]
    [System.NonSerialized] private bool FinishAbiLevUp;
    [SerializeField] private Canvas AbilityChoiceCanvas;        //アビリティを選ぶ時のCanvas
    [SerializeField] private List<AbilityButtonScript> AbilityButtons = new List<AbilityButtonScript>(2);
    //アビリティを選ぶボタンのスクリプト
    [SerializeField] private List<AbilityIcon> AbilityIcon = new List<AbilityIcon>(16);

    [SerializeField] private int MaxAbilityNum;
    [SerializeField] private int MaxAbilityLevel;
    [SerializeField] private List<string> AbilityDictionary = new List<string>(16);    //特殊能力用辞書
    [SerializeField] private List<string> NowAbilities = new List<string>(3);         //今の特殊能力。最大数=3
    [SerializeField] private List<int> AbilitieLevels = new List<int>(3);          //今の特殊能力のレベル


    [SerializeField] private List<string> ChoicingAbtDic = new List<string>(16);//選択中の辞書
    [System.NonSerialized] private int ChoicedAbtNumber;    //ランダムで選ばれたアビリティのindex
    [System.NonSerialized] private string ChoicedAbtName;   //ランダムで選ばれたアビリティの名前
    [System.NonSerialized] private List<string> ChoicingAbtNames = new List<string>(4);   //ランダムで選ばれてるアビリティの名前たち

    //各アビリティ関連の変数
    [System.NonSerialized] public int ThonePoint;
    [System.NonSerialized] public int CurePoint;
    [System.NonSerialized] public int RevengePoint;
    [System.NonSerialized] public int AvoidPercent;
    [System.NonSerialized] public int VampPoint;

    //メモ Thone CureBody AdaptMana Revenger Avoiding Vampire

    private void Awake()
    {
        HpBar.maxValue = MaxHP;
        NowHP = MaxHP;
        HpBar.value = NowHP;
        HpMax_UI.text = MaxHP.ToString();
        HpNow_UI.text = NowHP.ToString();

        MpBar.maxValue = MaxMP;
        MpBar.value = 0;
        MpMax_UI.text = MaxMP.ToString();
        MpNow_UI.text = NowMP.ToString();

        ExpBar.maxValue = ToNextLevelEXP;
        ExpBar.value = 0;
        ExpMax_UI.text = ToNextLevelEXP.ToString();
        ExpNow_UI.text = EXP.ToString();

        HP_Percent = Mathf.Abs((NowHP / MaxHP) - 1);
        AbilityChoiceCanvas.enabled = false;

        PLAnim.SetFloat("DamagePercent", HP_Percent);

        FinishAbiLevUp = false;

        ChoicingAbtNames = new List<string>();
        ChoicingAbtDic = new List<string>(AbilityDictionary);
        ChoicingAbtDic.Reverse();
    }

    private void Update()
    {
        if (Ctrl.Game)
        {
            //MPが20を超えて回復した時 プレイヤー関係
            if (NowMP >= MaxMP)
            {
                NowMP = MaxMP;
                ChangeMP();
            }

            //上限を超えて回復しないよう調整　プレイヤー関連処理
            if (NowHP >= MaxHP)
            {
                NowHP = MaxHP;
                ChangeHP();
            }

            if (EXP >= ToNextLevelEXP)
            {
                LevelUP();
            }

            //プレイヤーのHPを0～1に変換(残りHPの割合を算出)
            //ゲーム変数を変える為
            HP_Percent = Mathf.Abs(((float)NowHP / (float)MaxHP) - 1);

            //合わせてダメージの奴も調整
            PLAnim.SetFloat("DamagePercent", HP_Percent);
        }
    }

    //HP増加メソッド
    public void HPAdd(int AddNum)
    {
        NowHP += AddNum;
        ChangeHP();
    }

    //MP増加メソッド
    public void MPAdd(int AddNum)
    {
        NowMP += AddNum;
        ChangeMP();
    }


    public void EXPAdd(int AddNum)
    {
        EXP += AddNum;
        ChangeEXP();
    }

    public void HPgensyo(int num)
    {
        NowHP -= num;
        ChangeHP();
    }

    //MP減少
    public void MPGensyo(int num)
    {
        NowMP -= num;
        ChangeMP();
    }

    //ダメージ処理
    public void HPDown()
    {
        float r = UnityEngine.Random.Range(0f, 100f);
        if (!(r <= AvoidPercent))
        {
            if (Enab.EnableEnemies.Count > 0)
            {
                foreach (EnemSc item in Enab.EnableEnemies)
                {
                    HPgensyo(item.EnemyAttack);
                    if (NowAbilities.Contains("Thone"))
                    {
                        item.EnemyHp -= ThonePoint;
                    }
                    PLAnim.SetTrigger("Damage");
                    /*
                    item.EnemyRecast--;
                    if (item.EnemyRecast <= 0)
                    {
                        HPgensyo(item.EnemyAttack);
                        if (NowAbilities.Contains("Thone"))
                        {
                            item.EnemyHp -= ThonePoint;
                        }
                        item.EnemyRecast = item.MaxEnemyRecast;
                        PLAnim.SetTrigger("Damage");
                    }
                    */
                }
            }
        }
        Invoke(nameof(CureBody_Cureing), 0.1f);
    }

    private void GetDamage()
    {

    }

    //UIの変更
    private void ChangeHP()
    {
        HpBar.value = NowHP;
        HpNow_UI.text = NowHP.ToString();
    }

    private void ChangeMP()
    {
        MpBar.value = NowMP;
        MpNow_UI.text = NowMP.ToString();
    }

    private void ChangeEXP()
    {
        ExpBar.value = EXP;
        ExpNow_UI.text = EXP.ToString();
    }

    private void LevelUP()
    {
        EXP = 0;
        ChangeEXP();
        ToLevUp_UpRange++;
        ToNextLevelEXP += ToLevUp_UpRange * 2;
        ExpBar.maxValue = ToNextLevelEXP;
        ExpMax_UI.text = ToNextLevelEXP.ToString();

        MaxHP += 5;
        NowHP = MaxHP;
        HpBar.maxValue = MaxHP;
        HpMax_UI.text = MaxHP.ToString();
        ChangeHP();
        Level++;
        if (FinishAbiLevUp)
        {
            LevUp_AftAbility();
        }
        else
        {
            StartChoiceAbility();
        }
    }

    //アビリティ辞書から一個取り出す
    private void ChoiceAbility()
    {
        int PickNum = UnityEngine.Random.Range(0, ChoicingAbtDic.Count-1);
        ChoicedAbtNumber = PickNum;
        ChoicedAbtName = ChoicingAbtDic[PickNum];
        ChoicingAbtNames.Add(ChoicedAbtName);
        ChoicingAbtDic.Remove(ChoicedAbtName);
    }

    //アビリティ選択を始める
    private void StartChoiceAbility()
    {
        Ctrl.Game = false;
        ChoiceAbility();
        AbilityButtons[0].SetUp_Ability(ChoicedAbtName);
        ChoiceAbility();
        AbilityButtons[1].SetUp_Ability(ChoicedAbtName);
        AbilityChoiceCanvas.enabled = true;
    }

    //追加アビリティを決定する
    public void DecideAbility(string AbilityName)
    {
        if (NowAbilities.Count != 3)    //アビリティ、何を選ぶか？を選んでいる時
        {
            //手持ちに反映
            //新規アビリティか、もうあるもののレベルアップか
            int index;
            if (NowAbilities.Contains(AbilityName) == false)    //今あるアビリティの中に取ったアビリティがなかったら
            {
                Debug.Log("無かったよ");
                //選択中の諸々の整理
                ChoicingAbtNames.Remove(AbilityName);    //選ばれた方を削除
                ChoicingAbtDic.Add(ChoicingAbtNames[0]);    //選ばれなかった方を戻す
                ChoicingAbtNames.Clear();                   //空っぽにする

                if (NowAbilities.Count < MaxAbilityNum)
                {
                    NowAbilities.Add(AbilityName);
                    AbilitieLevels.Add(1);
                    AbilityLevUP(AbilityName);
                    AbilityIcon[NowAbilities.Count - 1].DecideImg(AbilityName);
                    if (NowAbilities.Count == MaxAbilityNum)
                    {
                        ChoicingAbtDic.Clear();
                        for (int i = 0; i < MaxAbilityNum; i++)
                        {
                            ChoicingAbtDic.Add(NowAbilities[i]);
                        }
                    }
                }
            }
            else
            {
                index = NowAbilities.IndexOf(AbilityName);
                if (AbilitieLevels[index] < MaxAbilityLevel)
                {
                    AbilitieLevels[index]++;
                    AbilityLevUP(AbilityName);
                    if (AbilitieLevels[index] == MaxAbilityLevel)
                    {
                        AbilityDictionary.Remove(AbilityName);
                    }
                    Debug.Log($"{AbilityName}.Level = {AbilitieLevels[index]}");

                    ChoicingAbtDic.Add(ChoicingAbtNames[0]);
                    ChoicingAbtDic.Add(ChoicingAbtNames[1]);
                    ChoicingAbtNames.Clear();
                }
            }
        }
        else        //アビリティえらびおわったあとの処理
        {
            int index = NowAbilities.IndexOf(AbilityName);
            if (AbilitieLevels[index] < MaxAbilityLevel)
            {
                ChoicingAbtDic.Add(ChoicingAbtNames[0]);
                ChoicingAbtDic.Add(ChoicingAbtNames[1]);
                ChoicingAbtNames.Clear();

                AbilitieLevels[index]++;
                AbilityLevUP(AbilityName);
                if (AbilitieLevels[index] == MaxAbilityLevel)
                {
                    AbilityDictionary.Remove(AbilityName);
                    ChoicingAbtDic.Remove(AbilityName);
                }
                Debug.Log($"{AbilityName}.Level = {AbilitieLevels[index]}");

                if (AbilitieLevels[0] == MaxAbilityLevel)
                {
                    if (AbilitieLevels[1] == MaxAbilityLevel)
                    {
                        if (AbilitieLevels[2] == MaxAbilityLevel)
                        {
                            FinishAbiLevUp = true;
                            ChoicingAbtDic.Clear();
                        }
                    }
                }
            }

            int foreach_Index = 0;
            foreach (int item in AbilitieLevels)
            {
                if (item != MaxAbilityLevel)
                {
                    if (!ChoicingAbtDic.Contains(NowAbilities[foreach_Index]))
                    {
                        ChoicingAbtDic.Add(NowAbilities[foreach_Index]);
                    }
                }
                foreach_Index++;
            }
            if (ChoicingAbtDic.Count == 1)
            {
                ChoicingAbtDic.Add(ChoicingAbtDic[0]);
            }
        }

        ChoicedAbtName = null;
        AbilityChoiceCanvas.enabled = false;
        Ctrl.Game = true;
    }

    void AbilityLevUP(string AbilityName)
    {
        switch (AbilityName)
        {
            case "Thone":
                ThonePoint++;
                break;
            case "CureBody":
                CurePoint++;
                break;
            case "AdaptMana":
                MaxMP += 20;
                MpMax_UI.text = MaxMP.ToString();
                MpBar.maxValue = MaxMP;
                break;
            case "Avoiding":
                AvoidPercent += 10;
                break;
            case "Vampire":
                VampPoint++;
                break;
            default:
                AbilityDictionary.Remove(ChoicedAbtName);
                break;
        }
    }

    //CureBody
    void CureBody_Cureing()
    {
        if (NowAbilities.Contains("CureBody"))
        {
            NowHP += CurePoint;
            ChangeHP();
        }
    }

    //Vampire
    public void Vampireing()
    {
        if (NowAbilities.Contains("Vampire"))
        {
                NowHP+=VampPoint;
                ChangeHP();
        }
    }

    //Revenger
    public void Revengeing(int PLAtt)
    {
        if (NowAbilities.Contains("Revenger"))
        {
            if(HP_Percent >= 0.3)
            {
                RevengePoint = 3;
            }
            else if (HP_Percent >= 0.5)
            {
                RevengePoint = 2;
            }
            else if (HP_Percent >= 0.7)
            {
                RevengePoint = 1;
            }
            else
            {
                RevengePoint = 0;
            }
            PLAtt += RevengePoint;
        }
    }


    private void LevUp_AftAbility()
    {
        Ctrl.Game = false;
        AbilityButtons[0].SetUp_AfterAbility();
        AbilityButtons[1].SetUp_AfterAbility();
        AbilityChoiceCanvas.enabled = true;
    }

    public void Decide_AftAbility(string Erabareta_Hou)
    {
        switch (Erabareta_Hou)
        {
            case "Add_HP":
                MaxHP += 5;
                NowHP = MaxHP;
                HpBar.maxValue = MaxHP;
                HpMax_UI.text = MaxHP.ToString();
                break;
            case "Add_AttackPow":
                PL_Attack++;
                break;
        }

        AbilityChoiceCanvas.enabled = false;
        Ctrl.Game = true;
    }
}
