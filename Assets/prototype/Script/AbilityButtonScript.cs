using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonScript : MonoBehaviour
{
    //アビリティをゲットした時に使われるやつ
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image abilitySprite;

    //セットアップに必要な変数　アビリティ選択編
    [Header("AbilityLevelUp")]
    [TextArea(1,6),SerializeField] private List<string> abilityDescriptions = new List<string>(32);
    [SerializeField] private List<Sprite> abilitySprites = new List<Sprite>(32);

    //セットアップに必要な変数　アビリティレベルMAXになったあと
    [Header("TadanoLebelUp")]
    [SerializeField] private string thisButtonRole;
    [SerializeField] private string abilityMAX_Text;
    [SerializeField] private Sprite abilityMAX_Sprite;

    [Space(5)]

    [SerializeField] private PlayerScript PLScript;

    [System.NonSerialized] private int thone;
    [System.NonSerialized] private int curePoint;
    [System.NonSerialized] private int avoidPercent;

    [System.NonSerialized] private bool _afterAbility;


    public void SetUp_Ability(string AbilityName)
    {
        _afterAbility = false;
        thone = PLScript.thonePoint + 1;
        curePoint = PLScript.curePoint + 1;
        avoidPercent = PLScript.avoidPercent + 10;
        Debug.Log($"thone = {thone}, CurePoint = {curePoint}, AvoidPercent = {avoidPercent}");

        abilityName.text = AbilityName;

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
        abilityDescriptions[index] = Replace_Placeholders(abilityDescriptions[index]);
        description.text = abilityDescriptions[index];
        abilitySprite.sprite = abilitySprites[index];
    }

    public void PushedThis()
    {
        ADXSoundManager.Instance.PlaySound(E_Sounds.SE_UI);
        if (_afterAbility)
        {
            PLScript.Decide_AftAbility(thisButtonRole);
        }
        else
        {
            PLScript.DecideAbility(abilityName.text);
        }
    }

    public void SetUp_AfterAbility()
    {
        _afterAbility = true;
        description.text = abilityMAX_Text;
        abilitySprite.sprite = abilityMAX_Sprite;
    }

    //テキストの変数置き換え
    private string Replace_Placeholders(string input)
    {
        // 正規表現で{}内のプレースホルダーを検出
        return Regex.Replace(input, @"\{(\w+)\}",
            match =>
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