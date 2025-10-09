using CriWare;
using CriWare.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    [SerializeField] private Canvas HowtoCanvas;
    [SerializeField] private Canvas ConfigCanvas;
    [SerializeField] private List<string> sceneName = new List<string>(4);
    [SerializeField] CriWare.Assets.CriAtomCueReference CueRefarence;

    private void Awake()
    {
        //HowtoCanvas.enabled = false;
        ConfigCanvas.enabled = false;
    }

    public void _Button_GameStart()
    {
        UIClick();
        ADXSoundManager.Instance.Dispose();
        SceneManager.LoadScene(sceneName[0]);
    }
    public void _Button_Quit()
    {
        ADXSoundManager.Instance.Dispose();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
        Application.Quit();//ゲームプレイ終了
#endif
    }

    public void _Howto_Pless(string name)
    {
        UIClick();
        switch (name)
        {
            case "on":
                HowtoCanvas.enabled = true;
                break;
            case "off":
                HowtoCanvas.enabled = false;
                break;
        }
    }

    public void OpenConfig()
    {
        UIClick();
        ConfigCanvas.enabled = true;
    }

    // 音
    private void UIClick()
    {
        ADXSoundManager.Instance.PlaySound("UI", CueRefarence.AcbAsset.Handle, 000, null, false);
    }
}
