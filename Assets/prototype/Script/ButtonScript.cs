using CriWare;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CriWare.Assets.CriAtomAssetsLoader;

public class ButtonScript : MonoBehaviour
{
    //汎用的なボタンに使われているモノ

    [SerializeField] private Canvas PauseCanvas;
    [SerializeField] private Canvas ConfigCanvas;
    [SerializeField] private CriWare.Assets.CriAtomCueReference CueRefarence;
    [SerializeField] private Controller Ctrl;
    private string sceneName;


    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        PauseCanvas.enabled = false;
        ConfigCanvas.enabled = false;
    }

    //リトライボタン
    public void _Button_GameRetry()
    {
        UIClick();
        ADXSoundManager.Instance.ResumeSound("PlayBGM");
        SceneManager.LoadScene(sceneName);
    }

    //タイトルに戻るボタン
    public void _Button_BackTitle()
    {
        UIClick();
        ADXSoundManager.Instance.StopSound("PlayBGM");
        SceneManager.LoadScene("Title_Prototype");
    }

    //ポーズボタン
    public void _Pause_Pless(string name)
    {
        UIClick();
        switch (name)
        {
            case "on":
                ADXSoundManager.Instance.PauseSound("PlayBGM");
                PauseCanvas.enabled = true;
                Ctrl.Game = false;
                break;
            case "off":
                ADXSoundManager.Instance.ResumeSound("PlayBGM");
                PauseCanvas.enabled = false;
                Ctrl.Game = true;
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
