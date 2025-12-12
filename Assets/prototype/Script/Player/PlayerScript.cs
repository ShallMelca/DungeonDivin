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

    [SerializeField] private int playerAttackPT;          //Playerのその時の基礎攻撃力
    [SerializeField] public int playerLevel;              //現在のレベル
    [SerializeField] public Animator playerAnimator;

    [Space(10)]

    [Header("HP")]
    [SerializeField] private int maxHP = 50;        //inspecterから：HPの「最大値」
    [SerializeField] public float hp_Percent => (float)nowHP/maxHP;
    [System.NonSerialized] private int nowHP;        //HPの現在値
    [SerializeField] private Slider hpBar;          //HPバー
    [SerializeField] TextMeshProUGUI hpNow_UI;         //HPバーの数値
    [SerializeField] TextMeshProUGUI hpMax_UI;         //HPバーの数値

    [Header("MP")]
    [SerializeField] private int maxMP = 40;              //inspecterから：MPの「最大値」
    [System.NonSerialized] private int nowMP;        //MPの現在値
    [SerializeField] private Slider mpBar;           //MPバー
    [SerializeField] private TextMeshProUGUI mpNow_UI;  //MPバーの現在値
    [SerializeField] private TextMeshProUGUI mpMax_UI;  //MPバーの最大値

    [Header("EXP")]
    [SerializeField] private int toNextLevelEXP;             //次のレベルまでの経験値量
    [System.NonSerialized] private int toLevUp_UpRange;     //レベルあがった時に上のがどのくらい増えるか
    [System.NonSerialized] private int exp;                  //経験値
    [SerializeField] private Slider expBar;                  //経験値バー
    [SerializeField] private TextMeshProUGUI expNow_UI;     //経験値バーの数値/現在値
    [SerializeField] private TextMeshProUGUI expMax_UI;     //経験値バーの数値/次のレベルまで

    [Space(10)]

    [SerializeField] private Controller ctrl;   //Controllerスクリプトを参照
    [SerializeField] private Enable enab;       //EnableAreaからスクリプトを参照

    [Header("Ability")]
    [System.NonSerialized] private bool finishAbiLevUp;
    [SerializeField] private Canvas abilityChoiceCanvas;        //アビリティを選ぶ時のCanvas
    [SerializeField] private List<AbilityButtonScript> abilityButtons = new List<AbilityButtonScript>(2);
    //アビリティを選ぶボタンのスクリプト
    [SerializeField] private List<AbilityIcon> abilityIcon = new List<AbilityIcon>(16);

    [SerializeField] private int maxAbilityNum;
    [SerializeField] private int maxAbilityLevel;
    [SerializeField] private List<string> abilityDictionary = new List<string>(16);    //特殊能力用辞書
    [SerializeField] private List<string> nowAbilities = new List<string>(3);         //今の特殊能力。最大数=3
    [SerializeField] private List<int> abilitieLevels = new List<int>(3);          //今の特殊能力のレベル


    [SerializeField] private List<string> choicingAbtDic = new List<string>(16);//選択中の辞書
    [System.NonSerialized] private int choicedAbtNumber;    //ランダムで選ばれたアビリティのindex
    [System.NonSerialized] private string choicedAbtName;   //ランダムで選ばれたアビリティの名前
    [System.NonSerialized] private List<string> choicingAbtNames = new List<string>(4);   //ランダムで選ばれてるアビリティの名前たち

    //各アビリティ関連の変数
    [System.NonSerialized] public int thonePoint;
    [System.NonSerialized] public int curePoint;
    [System.NonSerialized] public int revengePoint;
    [System.NonSerialized] public int avoidPercent;
    [System.NonSerialized] public int vampPoint;

    //メモ Thone CureBody AdaptMana Revenger Avoiding Vampire

    private void Awake()
    {
        hpBar.maxValue = maxHP;
        nowHP = maxHP;
        hpBar.value = nowHP;
        hpMax_UI.text = maxHP.ToString();
        hpNow_UI.text = nowHP.ToString();

        mpBar.maxValue = maxMP;
        mpBar.value = 0;
        mpMax_UI.text = maxMP.ToString();
        mpNow_UI.text = nowMP.ToString();

        expBar.maxValue = toNextLevelEXP;
        expBar.value = 0;
        expMax_UI.text = toNextLevelEXP.ToString();
        expNow_UI.text = exp.ToString();

        abilityChoiceCanvas.enabled = false;

        finishAbiLevUp = false;

        choicingAbtNames = new List<string>();
        choicingAbtDic = new List<string>(abilityDictionary);
        choicingAbtDic.Reverse();
    }


    private void Update()
    {
        if (!ctrl.game) return;

        //MPが20を超えて回復した時 プレイヤー関係
        if (nowMP >= maxMP)
        {
            nowMP = maxMP;
            ChangeMP();
        }

        //上限を超えて回復しないよう調整　プレイヤー関連処理
        if (nowHP >= maxHP)
        {
            nowHP = maxHP;
            ChangeHP();
        }

        // レベルアップ処理
        if (exp >= toNextLevelEXP)
        {
            LevelUP();
        }

        //合わせてダメージの奴も調整
        playerAnimator.SetFloat("DamagePercent", hp_Percent);

    }

    public int GetPLAttackPT
    {
        get
        {
            return playerAttackPT;
        }
    }

    /// <summary>
    /// 現在のHPを外部に渡す
    /// </summary>
    /// <returns></returns>
    public int GetHP
    {
        get
        {
            return nowHP;
        }
    }

    /// <summary>
    /// HP増加メソッド
    /// </summary>
    /// <param name="AddNum"></param>
    public void HPAdd(int AddNum)
    {
        nowHP += AddNum;
        playerAnimator.SetFloat("DamagePercent", hp_Percent);
        ChangeHP();
    }

    public int GetMP
    {
        get
        {
            return nowMP;
        }
    }

    /// <summary>
    /// MP増加メソッド
    /// </summary>
    /// <param name="AddNum"></param>
    public void MPAdd(int AddNum)
    {
        nowMP += AddNum;
        ChangeMP();
    }


    public void EXPAdd(int AddNum)
    {
        exp += AddNum;
        ChangeEXP();
    }

    public void HPgensyo(int num)
    {
        nowHP -= num;
        playerAnimator.SetFloat("DamagePercent", hp_Percent);
        ChangeHP();
    }

    /// <summary>
    /// MP減少
    /// </summary>
    /// <param name="num"></param>
    public void MPGensyo(int num)
    {
        nowMP -= num;
        ChangeMP();
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    public void HPDown()
    {
        float r = UnityEngine.Random.Range(0f, 100f);
        if (r <= avoidPercent) return;

        if (enab.enableEnemies.Count <= 0) return;

        foreach (EnemSc item in enab.enableEnemies)
        {
            HPgensyo(item.enemyAttack);
            if (nowAbilities.Contains("Thone"))
            {
                item.enemyHp -= thonePoint;
            }
            playerAnimator.SetTrigger("Damage");
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


        //Invoke(nameof(CureBody_Cureing), 0.1f);
    }

    private void GetDamage()
    {

    }

    /// <summary>
    /// UIの変更
    /// </summary>
    private void ChangeHP()
    {
        hpBar.value = nowHP;
        hpNow_UI.text = nowHP.ToString();
    }

    private void ChangeMP()
    {
        mpBar.value = nowMP;
        mpNow_UI.text = nowMP.ToString();
    }

    private void ChangeEXP()
    {
        expBar.value = exp;
        expNow_UI.text = exp.ToString();
    }

    private void LevelUP()
    {
        exp = 0;
        ChangeEXP();
        toLevUp_UpRange++;
        toNextLevelEXP += toLevUp_UpRange * 2;
        expBar.maxValue = toNextLevelEXP;
        expMax_UI.text = toNextLevelEXP.ToString();

        maxHP += 5;
        nowHP = maxHP;
        hpBar.maxValue = maxHP;
        hpMax_UI.text = maxHP.ToString();
        ChangeHP();
        playerLevel++;
        if (finishAbiLevUp)
        {
            //LevUp_AftAbility();
        }
        else
        {
            //StartChoiceAbility();
        }
    }
}
