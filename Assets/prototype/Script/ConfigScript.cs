using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigScript : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Canvas configCanvas;
    [SerializeField] private CriWare.Assets.CriAtomCueReference CueRefarence;

    private void Awake()
    {
        bgmSlider.value = ADXSoundManager.Instance.GetCategoryVolume("BGM");
        seSlider.value = ADXSoundManager.Instance.GetCategoryVolume("SE");
    }

    public void BGMVolChange()
    {
        ADXSoundManager.Instance.SetCategoryVolume("BGM", bgmSlider.value);
        Debug.Log($"BGM Volume = {ADXSoundManager.Instance.GetCategoryVolume("BGM")}");
    }

    public void SEVolChange()
    {
        ADXSoundManager.Instance.SetCategoryVolume("SE", seSlider.value);
        Debug.Log($"SE Volume = {ADXSoundManager.Instance.GetCategoryVolume("SE")}");
    }

    public void CloseConfig()
    {
        ADXSoundManager.Instance.PlaySound("UI", CueRefarence.AcbAsset.Handle, 000, null, false);
        configCanvas.enabled = false;
    }
}
