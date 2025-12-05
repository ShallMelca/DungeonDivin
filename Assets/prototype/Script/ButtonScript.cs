using CriWare;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CriWare.Assets.CriAtomAssetsLoader;

public class ButtonScript : MonoBehaviour
{
    //汎用的なボタンに使われているモノ

    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Canvas configCanvas;
    [SerializeField] private Controller ctrl;
    private string sceneName;


    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        pauseCanvas.enabled = false;
        configCanvas.enabled = false;
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
                pauseCanvas.enabled = true;
                ctrl.game = false;
                break;
            case "off":
                ADXSoundManager.Instance.ResumeSound("PlayBGM");
                pauseCanvas.enabled = false;
                ctrl.game = true;
                break;
        }
    }

    public void OpenConfig()
    {
        UIClick();
        configCanvas.enabled = true;
    }


    // 音
    private void UIClick()
    {
        ADXSoundManager.Instance.PlaySound(E_Sounds.SE_UI);
    }
}
