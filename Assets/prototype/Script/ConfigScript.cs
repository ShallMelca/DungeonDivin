using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigScript : MonoBehaviour
{
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SESlider;
    [SerializeField] private Canvas ConfigCanvas;
    [SerializeField] private CriWare.Assets.CriAtomCueReference CueRefarence;

    private void Awake()
    {
        BGMSlider.value = ADXSoundManager.Instance.GetCategoryVolume("BGM");
        SESlider.value = ADXSoundManager.Instance.GetCategoryVolume("SE");
    }

    public void BGMVolChange()
    {
        ADXSoundManager.Instance.SetCategoryVolume("BGM", BGMSlider.value);
        Debug.Log($"BGM Volume = {ADXSoundManager.Instance.GetCategoryVolume("BGM")}");
    }

    public void SEVolChange()
    {
        ADXSoundManager.Instance.SetCategoryVolume("SE", SESlider.value);
        Debug.Log($"SE Volume = {ADXSoundManager.Instance.GetCategoryVolume("SE")}");
    }

    public void CloseConfig()
    {
        ADXSoundManager.Instance.PlaySound("UI", CueRefarence.AcbAsset.Handle, 000, null, false);
        ConfigCanvas.enabled = false;
    }
}
