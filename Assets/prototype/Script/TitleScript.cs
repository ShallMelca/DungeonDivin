using CriWare;
using CriWare.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    [SerializeField] private Canvas howtoCanvas;
    [SerializeField] private Canvas configCanvas;
    [SerializeField] private List<string> sceneName = new List<string>(4);

    private void Awake()
    {
        //HowtoCanvas.enabled = false;
        configCanvas.enabled = false;
    }

    public void Button_GameStart()
    {
        UIClick();
        ADXSoundManager.Instance.Dispose();
        SceneManager.LoadScene(sceneName[0]);
    }
    public void Button_Quit()
    {
        ADXSoundManager.Instance.Dispose();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
        Application.Quit();//ゲームプレイ終了
#endif
    }

    public void Howto_Pless(string name)
    {
        UIClick();
        switch (name)
        {
            case "on":
                howtoCanvas.enabled = true;
                break;
            case "off":
                howtoCanvas.enabled = false;
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
