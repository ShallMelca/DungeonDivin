using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class AbilityButton
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image image;
    [System.NonSerialized] public AbilityEnum buttonRole;
}

public class AbilityButtonScript : MonoBehaviour
{
    [SerializeField] public event Action<AbilityEnum> buttonAction;
    [SerializeField] private List<AbilityButton> abilityButtons = new List<AbilityButton>();

    /// <summary>
    /// 各アビリティ選択ボタンに割り当てる関数。
    /// </summary>
    /// <param name="num">自分が何個目のボタンなのかを識別するための数値。インスペクターでの設定要注意</param>
    public void AbilityButtonPushed(int num)
    {
        if (num < 0 || num >= abilityButtons.Count) //
        {
            Debug.Log("[AbilityButtonScript] [AbilityButtonPushed] num is error number! minus or Over abilityButtons.Count");
            return;
        }

        for (int i = 0; i < abilityButtons.Count; i++)
        {
            if (num != i) continue;

            PushButtonRole(abilityButtons[i].buttonRole);
        }

    }

    public void Setup_AbilityChooseButton(AbilitiesData data)
    {
        foreach (AbilityButton button in abilityButtons)
        {
            int pickNum = UnityEngine.Random.Range(0, data.abilities.Count - 1);

            button.buttonRole = data.abilities[pickNum].name;

            button.title.text = data.abilities[pickNum].name_str;
            button.description.text = data.abilities[pickNum].description;
            button.image.sprite = data.abilities[pickNum].sprite;
        }
    }

    void PushButtonRole(AbilityEnum abilityEnum)
    {
        buttonAction.Invoke(abilityEnum);
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