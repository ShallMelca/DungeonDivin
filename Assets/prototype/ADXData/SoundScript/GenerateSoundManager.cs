using UnityEngine;

public class GenerateSoundManager : MonoBehaviour
{
    // GenerateSoundManager が1つだけになるように Instance とする
    public static GenerateSoundManager Instance { get; private set; }

    // Initialize を PlayMode で呼び出す
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (Instance == null)
        {
            // SoundManager が Resources フォルダの CRIWARE フォルダ内に用意されている前提で、名前で検索してロード
            var prefab = Resources.Load<GameObject>("CRIWARE/SoundManager");
            if (prefab == null)
            {
                // 検索に失敗した場合( Assets/Resources/CRIWARE/ に SoundManager.prefab が存在しない場合) はエラー
                Debug.LogError("[GenerateSoundManager] SoundManager not found in Editor Default Resources folder.");
                return;
            }

            // SoundManagerを生成
            var soundManager = Instantiate(prefab);
            soundManager.AddComponent<GenerateSoundManager>();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }
}

