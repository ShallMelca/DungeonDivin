using CriWare;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    //GameManager辺りに入ってる統括スクリプト

    [Header("System")]
    public bool game;   //ゲームそのもののブーリアン　これがfalseだと全てが止まる
    [SerializeField] private int turn;
    [SerializeField] public int score;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private Enable Enable;

    [Header("GameObject")]
    [SerializeField] private LimSc[] Limits = new LimSc[7];    //パズル補給のためのオブジェクト
    [SerializeField] private GameObject[] puzzles;              //召喚するパズル物体そのものを入れる変数
    [SerializeField] private EnemSc enemyPuzzle;
    [SerializeField] private float puzSize_Min;
    [SerializeField] private float puzSize_Max;
    [SerializeField] private float puzDistance;

    [System.NonSerialized] public bool limmitAreaintered;
    [System.NonSerialized] public GameObject interLimmitObj;

    [System.NonSerialized] private List<GameObject> puzzleList = new List<GameObject>(32);  //選択中のパズルオブジェクトのリスト。
    private GameObject puzzleListObj;

    private Vector2 containerBottomLeftPos;

    private int nowPLAttack;                //プレイヤーの「現在の」攻撃力　基礎値はPlayerScript内のPlayerAttack
    private int manaPlusCount = 0;          //その時々のマナ回復量
    private int curePlusCount = 0;          //その時々のHP回復量

    //ここからUI関係
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI popText;     //モノをドラッグしたりしている時についてくるUI　現状見づらいので改善したい

    [SerializeField] private TextMeshProUGUI turnPoint;

    [SerializeField] private TextMeshProUGUI last_Score_UI;
    [SerializeField] private TextMeshProUGUI last_Turn_UI;
    [SerializeField] private TextMeshProUGUI last_Level_UI;
    [SerializeField] private Canvas failCanvas;
    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private LineRenderer puzzleLineCanvas;


    //音関係
    [Header("Sound")]
    [SerializeField] private CriWare.Assets.CriAtomCueReference cueReference_SE;
    [SerializeField] private CriWare.Assets.CriAtomCueReference cueRefernce_BGM;
    [System.NonSerialized] private bool bgm = false;
    [System.NonSerialized] private string bgm_Name = "PlayBGM";

    //展示用
    [Header("Trial")]
    public bool trial_ver = false;
    [SerializeField] private int turnlimit;
    [SerializeField] private Canvas thankPlay;
    [SerializeField] private TextMeshProUGUI last_Score_UI_Trial;
    [SerializeField] private TextMeshProUGUI last_Turn_UI_Trial;
    [SerializeField] private TextMeshProUGUI last_Level_UI_Trial;


    void Awake()
    {
        loadingCanvas.enabled = true;
        popText.enabled = false;

        MakePuz(15);

        failCanvas.enabled = false;
        containerBottomLeftPos = new Vector2(-11, -10);
        ADXSoundManager.Instance.GameValue(000, playerScript.hp_Percent);

        if (cueRefernce_BGM.AcbAsset.Status == CriAtomExAcbLoader.Status.Complete) return;

        if (cueRefernce_BGM.AcbAsset.Status != CriAtomExAcbLoader.Status.Loading)
        {
            cueRefernce_BGM.AcbAsset.LoadImmediate();
        }

        puzzleLineCanvas.positionCount = 0;
        puzzleLineCanvas.numCapVertices = 100;
        puzzleLineCanvas.numCornerVertices = 100;

        thankPlay.enabled = false;
    }

    void Update()
    {
        //BGM開始処理
        if (cueRefernce_BGM.AcbAsset.Status == CriAtomExAcbLoader.Status.Complete && game == false && bgm == false)
        {
            ADXSoundManager.Instance.PlaySound(bgm_Name, cueRefernce_BGM.AcbAsset.Handle, 000, null, false);
            game = true;
            bgm = true;
            loadingCanvas.enabled = false;
        }

        if (!game) return;

        popText.transform.position = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            FirstPuz();
        }
        if (Input.GetMouseButton(0) && puzzleList.Count > 0)
        {
            Dragging();
        }
        if (Input.GetMouseButtonUp(0))
        {
            DeletePuz();
            popText.enabled = false;
            manaPlusCount = 0;
            curePlusCount = 0;
            nowPLAttack = playerScript.playerAttackPT;      //プレイヤー攻撃力の初期化
        }

        //LimitArea内にA.Limが入った時
        if (limmitAreaintered == true)
        {
            float lpos = interLimmitObj.transform.position.x;
            ReMake(lpos);
            interLimmitObj.transform.position = new Vector2(lpos, 80);
            limmitAreaintered = false;
        }

        //死んだ時の処理
        if (playerScript.GetHP() <= 0)
        {
            //PLScript.hp_Percent = 1;
            game = false;
            failCanvas.enabled = true;
            last_Score_UI.text = "SCORE:" + score;
            last_Turn_UI.text = "TURN:" + turn;
            last_Level_UI.text = "LEVEL:" + playerScript.playerLevel;
            ADXSoundManager.Instance.StopSound(bgm_Name);
        }

        //展示用BoolがTrueの時の処理
        if (trial_ver == true && turn == turnlimit)
        {
            game = false;
            thankPlay.enabled = true;
            last_Score_UI_Trial.text = "SCORE:" + score;
            last_Turn_UI_Trial.text = "TURN:" + turn;
            last_Level_UI_Trial.text = "LEVEL:" + playerScript.playerLevel;
        }

        //ゲーム変数の書き換え
        ADXSoundManager.Instance.GameValue(000, playerScript.hp_Percent);

    }



    /// <summary>
    /// パズル選択。クリック押下時の処理
    /// </summary>
    void FirstPuz()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), containerBottomLeftPos, 2.3f);
        if (hit2D.collider == null) return;

        string tag = hit2D.collider.tag;

        if (tag == "Enemy" || tag == "Sword" || tag == "Mana" || tag == "Cure")
        {
            nowPLAttack = playerScript.playerAttackPT;
            playerScript.Revengeing(nowPLAttack);
            GameObject thisPuz = hit2D.collider.gameObject;
            puzzleList.Add(thisPuz);
            Color PuzColor = thisPuz.GetComponent<SpriteRenderer>().color;
            PuzColor.a = 0.5f;
            thisPuz.GetComponent<SpriteRenderer>().color = PuzColor;
            puzzleListObj = thisPuz;
            AddNewLinePos(thisPuz.transform);
        }
        if (tag == "Enemy")
        {
            popText.enabled = true;
            popText.color = Color.red;
            popText.text = nowPLAttack + " Dam";
            ADXSoundManager.Instance.PlaySound("Enem", cueReference_SE.AcbAsset.Handle, 002, null, false);
        }
        else if (tag == "Sword")
        {
            nowPLAttack += 1;
            popText.enabled = true;
            popText.color = Color.red;
            popText.text = nowPLAttack + " Dam";
            ADXSoundManager.Instance.PlaySound("sword", cueReference_SE.AcbAsset.Handle, 005, null, false);
        }
        else if (tag == "Mana")
        {
            manaPlusCount++;
            popText.enabled = true;
            popText.color = Color.cyan;
            popText.text = manaPlusCount + " +MP";
            ADXSoundManager.Instance.PlaySound("mana", cueReference_SE.AcbAsset.Handle, 003, null, false);
        }
        else if (tag == "Cure")
        {
            curePlusCount++;
            popText.enabled = true;
            popText.color = Color.green;
            popText.text = curePlusCount + " +HP";
            ADXSoundManager.Instance.PlaySound("cure", cueReference_SE.AcbAsset.Handle, 004, null, false);
        }

    }

    /// <summary>
    /// ドラッグ中の処理
    /// </summary>
    void Dragging()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), containerBottomLeftPos, 2.3f);

        if (hit2D.collider == null) return;

        string tag = hit2D.collider.tag;
        if (tag == puzzleListObj.tag)
        {
            GameObject nowPuzzle = hit2D.collider.gameObject;
            Vector2 puzDist = nowPuzzle.transform.position - puzzleListObj.transform.position;

            //listPuzの中にすでに入っているものをRayが指しているか、
            //Rayの先にあるモノと先に触ってたものの距離がお隣さんではない時
            if (puzzleList.Contains(nowPuzzle) || puzDist.magnitude > puzDistance) return;


            puzzleList.Add(nowPuzzle);       //選択中リストに今指しているパズルオブジェクトを追加
            Color PuzColor = nowPuzzle.GetComponent<SpriteRenderer>().color;
            PuzColor.a = 0.5f;
            nowPuzzle.GetComponent<SpriteRenderer>().color = PuzColor;
            //アルファ値を半分にして、「選択中」であることを示す

            puzzleListObj = nowPuzzle;
            if (tag == "Mana")
            {
                manaPlusCount++;
                popText.text = manaPlusCount + " +MP";
                ADXSoundManager.Instance.PlaySound("mana", cueReference_SE.AcbAsset.Handle, 003, null, false);
            }
            else if (tag == "Cure")
            {
                curePlusCount++;
                popText.text = curePlusCount + " +HP";
                ADXSoundManager.Instance.PlaySound("cure", cueReference_SE.AcbAsset.Handle, 004, null, false);
            }
            else if (tag == "Sword")
            {
                nowPLAttack++;
                popText.text = nowPLAttack + " Dam";
                ADXSoundManager.Instance.PlaySound("sword", cueReference_SE.AcbAsset.Handle, 005, null, false);
            }
            else if (tag == "Enemy")
            {
                popText.text = nowPLAttack + " Dam";
                ADXSoundManager.Instance.PlaySound("Enem", cueReference_SE.AcbAsset.Handle, 002, null, false);
            }
            AddNewLinePos(nowPuzzle.transform);
        }
        else if(tag == "Enemy" && puzzleListObj.tag == "Sword")
        {
            GameObject thisPuz = hit2D.collider.gameObject;
            Vector2 dis = thisPuz.transform.position - puzzleListObj.transform.position;

            if (!puzzleList.Contains(thisPuz) && dis.magnitude <= puzDistance)
            {
                puzzleList.Add(thisPuz);
                Color PuzColor = thisPuz.GetComponent<SpriteRenderer>().color;
                PuzColor.a = 0.5f;
                thisPuz.GetComponent<SpriteRenderer>().color = PuzColor;
                puzzleListObj = thisPuz;
                ADXSoundManager.Instance.PlaySound("Enem", cueReference_SE.AcbAsset.Handle, 002, null, false);

                AddNewLinePos(thisPuz.transform);
            }
        }
        else if (tag == "Sword" && puzzleListObj.tag == "Enemy")
        {
            GameObject thisPuz = hit2D.collider.gameObject;
            Vector2 dis = thisPuz.transform.position - puzzleListObj.transform.position;

            if (!puzzleList.Contains(thisPuz) && dis.magnitude <= puzDistance)
            {
                puzzleList.Add(thisPuz);
                Color PuzColor = thisPuz.GetComponent<SpriteRenderer>().color;
                PuzColor.a = 0.5f;
                thisPuz.GetComponent<SpriteRenderer>().color = PuzColor;
                puzzleListObj = thisPuz;
                nowPLAttack++;
                popText.text = nowPLAttack + " Dam";
                ADXSoundManager.Instance.PlaySound("sword", cueReference_SE.AcbAsset.Handle, 005, null, false);

                AddNewLinePos(thisPuz.transform);
            }
        }
    }

    //void ConnectPuzzle(RaycastHit2D hit2D, string tag)
    //{
    //    GameObject thisPuz = hit2D.collider.gameObject;
    //    Vector2 dis = thisPuz.transform.position - lastPuzzleObj.transform.position;

    //    if (!listPuzzle.Contains(thisPuz) && dis.magnitude <= puzDistance)
    //    {
    //        listPuzzle.Add(thisPuz);
    //        Color PuzColor = thisPuz.GetComponent<SpriteRenderer>().color;
    //        PuzColor.a = 0.5f;
    //        thisPuz.GetComponent<SpriteRenderer>().color = PuzColor;
    //        lastPuzzleObj = thisPuz;
    //        nowPLAttack++;
    //        popText.text = nowPLAttack + " Dam";
    //        ADXSoundManager.Instance.PlaySound("sword", cueReference_SE.AcbAsset.Handle, 005, null, false);

    //        AddNewLinePos(thisPuz.transform);
    //    }
    //}

    //選択したパズルたちを消す
    void DeletePuz()
    {
        if (puzzleList.Count >= 2)
        {
            if (puzzleList[1].tag == "Mana")
            {
                playerScript.MPAdd(puzzleList.Count);
            }
            else if (puzzleList[1].tag == "Cure")
            {
                playerScript.HPAdd(puzzleList.Count);
            }
            int EnemCount = 0;
            foreach (var item in puzzleList)
            {
                string tag = item.tag;
                if (tag == "Enemy")
                {
                    EnemSc EnemScript = item.GetComponent<EnemSc>();
                    EnemScript.enemyHp = EnemScript.enemyHp - nowPLAttack;
                    SpriteRenderer itemSprite = item.GetComponent<SpriteRenderer>();
                    Color PuzColor = itemSprite.color;
                    PuzColor.a = 1;
                    itemSprite.color = PuzColor;
                    EnemCount++;
                    continue;
                }
                else
                {
                    Enable.count--;
                    Destroy(item);
                }
            }
            turn++;
            turnPoint.text = turn.ToString();
            Invoke(nameof(CallHPDown), 0.2f);
        }
        else
        {
            foreach (var item in puzzleList)
            {
                Color PuzColor = item.GetComponent<SpriteRenderer>().color;
                PuzColor.a = 1;
                item.GetComponent<SpriteRenderer>().color = PuzColor;
            }
        }
        puzzleList.Clear();
        puzzleLineCanvas.positionCount = 0;
    }

    void ReMake(float x)
    {
        for(int i = 0; i< 6; i++)
        {
            int r_kind = UnityEngine.Random.Range(0, 4);
            float r_Scale = UnityEngine.Random.Range(puzSize_Min, puzSize_Max);
            float r_Pos = UnityEngine.Random.Range(0f, 1f);
            GameObject puz = null;
            if (r_kind == 3) //Enemyを引いた時
            {
                EnemSc Enem = Instantiate(enemyPuzzle); //Enemyを生成
                Enem.ChangeLev((playerScript.playerLevel / 2) + 1);   //レベル調整
                Enable.allEnemies.Add(Enem);
                puz = Enem.gameObject;
                Debug.Log($"puz:{puz.name}");
            }
            else
            {
                puz = Instantiate(puzzles[r_kind]);
                Debug.Log($"puz:{puz.name}");
            }
            puz.transform.position = new Vector2(x+r_Pos, 40 + i * 4);
            puz.transform.localScale = new Vector3(r_Scale, r_Scale, 1);
        }
    }

    //パズル生成(初期)
    void MakePuz(int n)
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < n; j++)
            {
                int r_kind = UnityEngine.Random.Range(0, 4);
                float r_Scale = UnityEngine.Random.Range(puzSize_Min, puzSize_Max);
                float r_Pos = UnityEngine.Random.Range(0f, 1f);
                int r = 0;
                GameObject puz = null;
                Debug.Log($"r_kind:{r_kind}");
                if (r_kind == 3) //Enemyを引いた時
                {
                    r = UnityEngine.Random.Range(0, 3 + 1); //2/3判定
                    if (r == 0) return;     //はずれ

                    EnemSc Enem = Instantiate(enemyPuzzle); //Enemyを晴れて生成
                    Enable.allEnemies.Add(Enem);
                    puz = Enem.gameObject;
                    Debug.Log($"puz:{puz.name}");

                }
                else
                {
                    puz = Instantiate(puzzles[r_kind]);
                    Debug.Log($"puz:{puz.name}");
                }
                puz.transform.position = new Vector2(-9 + i * 4 + r_Pos, -8 + j * 4);
                puz.transform.localScale = new Vector3(r_Scale, r_Scale, 1);
            }
        }
        for(int i =0; i < 7; i++)
        {
            Limits[i].transform.position = new Vector2(-9 + i * 4, 80);
        }
    }

    //PLScriptのHPDownをコルーチン(Invoke)するための関数
    void CallHPDown()
    {
        playerScript.HPDown();
    }

    //Lineを伸ばすための関数
    void AddNewLinePos(Transform tr)
    {
        puzzleLineCanvas.positionCount++;
        puzzleLineCanvas.SetPosition(puzzleLineCanvas.positionCount - 1, tr.position);
    }
}