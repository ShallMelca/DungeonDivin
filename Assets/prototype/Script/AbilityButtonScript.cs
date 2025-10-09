using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonScript : MonoBehaviour
{
    //アビリティをゲットした時に使われるやつ
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private Image ThisSprite;

    //セットアップに必要な変数　アビリティ選択編
    [Header("AbilityLevelUp")]
    [TextArea(1,6),SerializeField] private List<string> AbilityDescriptions = new List<string>(32);
    [SerializeField] private List<Sprite> AbilitySprites = new List<Sprite>(32);

    //セットアップに必要な変数　アビリティレベルMAXになったあと
    [Header("TadanoLebelUp")]
    [SerializeField] private string ThisButtonRole;
    [SerializeField] private string AftAbilityMAX_Text;
    [SerializeField] private Sprite AftAbilityMAX_Sprite;

    [Space(5)]

    [SerializeField] private PlayerScript PLScript;
    [SerializeField] private CriWare.Assets.CriAtomCueReference CueRefarence;

    [System.NonSerialized] public int thone;
    [System.NonSerialized] public int CurePoint;
    [System.NonSerialized] public int AvoidPercent;

    [System.NonSerialized] private bool AfterAbility;


    public void SetUp_Ability(string AbilityName)
    {
        AfterAbility = false;
        thone = PLScript.ThonePoint + 1;
        CurePoint = PLScript.CurePoint + 1;
        AvoidPercent = PLScript.AvoidPercent + 10;
        Debug.Log($"thone = {thone}, CurePoint = {CurePoint}, AvoidPercent = {AvoidPercent}");

        Title.text = AbilityName;

        int index;
        switch (AbilityName)
        {
            case "Thone":
                index = 0;
                ChangeText_Ability(index);
                break;
            case "CureBody":
                index = 1;
                ChangeText_Ability(index);
                break;
            case "AdaptMana":
                index = 2;
                ChangeText_Ability(index);
                break;
            case "Revenger":
                index = 3;
                ChangeText_Ability(index);
                break;
            case "Avoiding":
                index = 4;
                ChangeText_Ability(index);
                break;
            case "Vampire":
                index = 5;
                ChangeText_Ability(index);
                break;
        }
    }

    private void ChangeText_Ability(int index)
    {
        AbilityDescriptions[index] = Replace_Placeholders(AbilityDescriptions[index]);
        Description.text = AbilityDescriptions[index];
        ThisSprite.sprite = AbilitySprites[index];
    }

    public void PushedThis()
    {
        ADXSoundManager.Instance.PlaySound("UI", CueRefarence.AcbAsset.Handle, 000, null, false);
        if (!AfterAbility)
        {
            PLScript.DecideAbility(Title.text);
        }
        else
        {
            PLScript.Decide_AftAbility(ThisButtonRole);
        }
    }

    public void SetUp_AfterAbility()
    {
        AfterAbility = true;
        Description.text = AftAbilityMAX_Text;
        ThisSprite.sprite = AftAbilityMAX_Sprite;
    }

    //テキストの変数置き換え
    private string Replace_Placeholders(string input)
    {
        // 正規表現で{}内のプレースホルダーを検出
        return Regex.Replace(input, @"\{(\w+)\}", match =>
        {
            // {}内の文字列に対応するクラスのフィールドを探す
            string fieldName = match.Groups[1].Value;
            var field = GetType().GetField(fieldName);

            if (field != null)
            {
                // フィールドが存在する場合、その値で置き換える
                return field.GetValue(this)?.ToString();
            }
            return match.Value; // フィールドが見つからなければそのまま
        });
    }
}