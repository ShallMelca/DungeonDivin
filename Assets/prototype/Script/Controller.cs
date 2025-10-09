using CriWare;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    //GameManager辺りに入ってる統括スクリプト

    [Header("System")]
    public bool Game;   //ゲームそのもののブーリアン　これがfalseだと全てが止まる、はず。
    [SerializeField] private int turn;
    [SerializeField] public int score;
    [SerializeField] private PlayerScript PLScript;
    [SerializeField] private Enable Enab;

    [Header("GameObject")]
    [SerializeField] private LimSc[] Lim = new LimSc[7];    //パズル補給のためのオブジェクト
    [SerializeField] private GameObject[] Puz;              //召喚するパズル物体そのものを入れる変数
    [SerializeField] private EnemSc Enempuz;
    [SerializeField] private float PuzSize_Min;
    [SerializeField] private float PuzSize_Max;
    [SerializeField] private float PuzDistance;

    [System.NonSerialized] public bool LimAreaintered;
    [System.NonSerialized] public GameObject InterLimObj;

    [System.NonSerialized] private List<GameObject> ListPuz = new List<GameObject>(32);  //選択中のパズルオブジェクトのリスト。
    private GameObject LastPuz;

    private Vector2 nut;

    private int NowPLAttack;                //プレイヤーの「現在の」攻撃力　基礎値はPlayerScript内のPlayerAttack
    private int ManaPlusCount = 0;          //その時々のマナ回復量
    private int CurePlusCount = 0;          //その時々のHP回復量

    //ここからUI関係
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI PopText;     //モノをドラッグしたりしている時についてくるUI　現状見づらいので改善したい

    [SerializeField] private TextMeshProUGUI TurnPoint;

    [SerializeField] private TextMeshProUGUI Last_Score;
    [SerializeField] private TextMeshProUGUI Last_Turn;
    [SerializeField] private TextMeshProUGUI Last_Level;
    [SerializeField] private Canvas fail;
    [SerializeField] private Canvas Loading;
    [SerializeField] private LineRenderer PuzzleLine;


    //音関係
    [Header("Sound")]
    [SerializeField] private CriWare.Assets.CriAtomCueReference CueRef_SE;
    [SerializeField] private CriWare.Assets.CriAtomCueReference CueRef_BGM;
    [System.NonSerialized] private bool Bgm = false;
    [System.NonSerialized] private string BGM_Name = "PlayBGM";

    //展示用
    [Header("Trial")]
    public bool Trial_ver = false;
    [SerializeField] private int turnlimit;
    [SerializeField] private Canvas ThankPlay;
    [SerializeField] private TextMeshProUGUI Last_Score_Trial;
    [SerializeField] private TextMeshProUGUI Last_Turn_Trial;
    [SerializeField] private TextMeshProUGUI Last_Level_Trial;


    void Awake()
    {
        Loading.enabled = true;
        PopText.enabled = false;

        MakePuz(15);

        fail.enabled = false;
        nut = new Vector2(-11, -10);
        ADXSoundManager.Instance.GameValue(000, PLScript.HP_Percent);

        if (CueRef_BGM.AcbAsset.Status != CriAtomExAcbLoader.Status.Complete)
        {
            if (CueRef_BGM.AcbAsset.Status != CriAtomExAcbLoader.Status.Loading)
            {
                CueRef_BGM.AcbAsset.LoadImmediate();
            }
        }
        PuzzleLine.positionCount = 0;
        PuzzleLine.numCapVertices = 100;
        PuzzleLine.numCornerVertices = 100;

        ThankPlay.enabled = false;
    }

    void Update()
    {
        //BGM開始処理
        if (CueRef_BGM.AcbAsset.Status == CriAtomExAcbLoader.Status.Complete && Game == false && Bgm == false)
        {
            ADXSoundManager.Instance.PlaySound(BGM_Name, CueRef_BGM.AcbAsset.Handle, 000, null, false);
            Game = true;
            Bgm = true;
            Loading.enabled = false;
        }
        if (Game == true)
        {
            PopText.transform.position = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                FirstPuz();
            }
            if (Input.GetMouseButton(0) && ListPuz.Count > 0)
            {
                Dragging();
            }
            if (Input.GetMouseButtonUp(0))
            {
                DeletePuz();
                PopText.enabled = false;
                ManaPlusCount = 0;
                CurePlusCount = 0;
                NowPLAttack = PLScript.PL_Attack;      //プレイヤー攻撃力の初期化
            }

            //LimitArea内にA.Limが入った時
            if (LimAreaintered == true)
            {
                float lpos = InterLimObj.transform.position.x;
                ReMake(lpos);
                InterLimObj.transform.position = new Vector2(lpos, 80);
                LimAreaintered = false;
            }

            //死んだぞ！
            if (PLScript.NowHP <= 0)
            {
                PLScript.HP_Percent = 1;
                Game = false;
                fail.enabled = true;
                Last_Score.text = "SCORE:" + score;
                Last_Turn.text = "TURN:" + turn;
                Last_Level.text = "LEVEL:" + PLScript.Level;
                ADXSoundManager.Instance.StopSound(BGM_Name);
            }

            //展示用BoolがTrueの時の処理
            if (Trial_ver == true && turn == turnlimit)
            {
                Game = false;
                ThankPlay.enabled = true;
                Last_Score_Trial.text = "SCORE:" + score;
                Last_Turn_Trial.text = "TURN:" + turn;
                Last_Level_Trial.text = "LEVEL:" + PLScript.Level;
            }

            //ゲーム変数の書き換え
            ADXSoundManager.Instance.GameValue(000, PLScript.HP_Percent);
        }
    }



    //パズル選択。クリック押下時の処理
    void FirstPuz()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), nut, 2.3f);
        if(hit2D.collider != null)
        {
            string tag = hit2D.collider.tag;

            if (tag == "Enemy" || tag == "Sword" || tag == "Mana" || tag == "Cure")
            {
                NowPLAttack = PLScript.PL_Attack;
                PLScript.Revengeing(NowPLAttack);
                GameObject thisPuz = hit2D.collider.gameObject;
                ListPuz.Add(thisPuz);
                Color PuzColor = thisPuz.GetComponent<SpriteRenderer>().color;
                PuzColor.a = 0.5f;
                thisPuz.GetComponent<SpriteRenderer>().color = PuzColor;
                LastPuz = thisPuz;
                AddNewLinePos(thisPuz.transform);
            }
            if (tag == "Enemy")
            {
                PopText.enabled = true;
                PopText.color = Color.red;
                PopText.text = NowPLAttack + " Dam";
                ADXSoundManager.Instance.PlaySound("Enem", CueRef_SE.AcbAsset.Handle, 002, null, false);
            }
            else if (tag == "Sword")
            {
                NowPLAttack += 1;
                PopText.enabled = true;
                PopText.color = Color.red;
                PopText.text = NowPLAttack + " Dam";
                ADXSoundManager.Instance.PlaySound("sword", CueRef_SE.AcbAsset.Handle, 005, null, false);
            }
            else if (tag == "Mana")
            {
                ManaPlusCount++;
                PopText.enabled = true;
                PopText.color = Color.cyan;
                PopText.text = ManaPlusCount + " +MP";
                ADXSoundManager.Instance.PlaySound("mana", CueRef_SE.AcbAsset.Handle, 003, null, false);
            }
            else if (tag == "Cure")
            {
                CurePlusCount++;
                PopText.enabled = true;
                PopText.color = Color.green;
                PopText.text = CurePlusCount + " +HP";
                ADXSoundManager.Instance.PlaySound("cure", CueRef_SE.AcbAsset.Handle, 004, null, false);
            }
        }
    }

    //ぐい～って選択する奴
    void Dragging()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), nut, 2.3f);

        if (hit2D.collider != null)
        {
            string tag = hit2D.collider.tag;
            if (tag == LastPuz.tag)
            {
                GameObject thisPuz = hit2D.collider.gameObject;
                Vector2 dis = thisPuz.transform.position - LastPuz.transform.position;

                if(!ListPuz.Contains(thisPuz) && dis.magnitude <= PuzDistance)
                    //listPuzの中にすでに入っているもの「ではない」ものをRayが指していて、
                    //Rayの先にあるモノと先に触ってたものの距離がお隣さんの時
                {
                    ListPuz.Add(thisPuz);       //選択中リストに今指しているパズルオブジェクトを追加
                    Color PuzColor = thisPuz.GetComponent<SpriteRenderer>().color;
                    PuzColor.a = 0.5f;
                    thisPuz.GetComponent<SpriteRenderer>().color = PuzColor;
                    //アルファ値を半分にして、「選択中」であることを示す

                    LastPuz = thisPuz;
                    if (tag == "Mana")
                    {
                        ManaPlusCount++;
                        PopText.text = ManaPlusCount + " +MP";
                        ADXSoundManager.Instance.PlaySound("mana", CueRef_SE.AcbAsset.Handle, 003, null, false);
                    }
                    else if (tag == "Cure")
                    {
                        CurePlusCount++;
                        PopText.text = CurePlusCount + " +HP";
                        ADXSoundManager.Instance.PlaySound("cure", CueRef_SE.AcbAsset.Handle, 004, null, false);
                    }
                    else if (tag == "Sword")
                    {
                        NowPLAttack++;
                        PopText.text = NowPLAttack + " Dam";
                        ADXSoundManager.Instance.PlaySound("sword", CueRef_SE.AcbAsset.Handle, 005, null, false);
                    }
                    else if (tag == "Enemy")
                    {
                        PopText.text = NowPLAttack + " Dam";
                        ADXSoundManager.Instance.PlaySound("Enem", CueRef_SE.AcbAsset.Handle, 002, null, false);
                    }
                    AddNewLinePos(thisPuz.transform);
                }

            }
            else if(tag == "Enemy" && LastPuz.tag == "Sword")
            {
                GameObject thisPuz = hit2D.collider.gameObject;
                Vector2 dis = thisPuz.transform.position - LastPuz.transform.position;

                if (!ListPuz.Contains(thisPuz) && dis.magnitude <= PuzDistance)
                {
                    ListPuz.Add(thisPuz);
                    Color PuzColor = thisPuz.GetComponent<SpriteRenderer>().color;
                    PuzColor.a = 0.5f;
                    thisPuz.GetComponent<SpriteRenderer>().color = PuzColor;
                    LastPuz = thisPuz;
                    ADXSoundManager.Instance.PlaySound("Enem", CueRef_SE.AcbAsset.Handle, 002, null, false);

                    AddNewLinePos(thisPuz.transform);
                }
            }
            else if (tag == "Sword" && LastPuz.tag == "Enemy")
            {
                GameObject thisPuz = hit2D.collider.gameObject;
                Vector2 dis = thisPuz.transform.position - LastPuz.transform.position;

                if (!ListPuz.Contains(thisPuz) && dis.magnitude <= PuzDistance)
                {
                    ListPuz.Add(thisPuz);
                    Color PuzColor = thisPuz.GetComponent<SpriteRenderer>().color;
                    PuzColor.a = 0.5f;
                    thisPuz.GetComponent<SpriteRenderer>().color = PuzColor;
                    LastPuz = thisPuz;
                    NowPLAttack++;
                    PopText.text = NowPLAttack + " Dam";
                    ADXSoundManager.Instance.PlaySound("sword", CueRef_SE.AcbAsset.Handle, 005, null, false);

                    AddNewLinePos(thisPuz.transform);
                }
            }
        }
    }

    //選択したパズルたちを消す
    void DeletePuz()
    {
        if (ListPuz.Count >= 2)
        {
            if (ListPuz[1].tag == "Mana")
            {
                PLScript.MPAdd(ListPuz.Count);
            }
            else if (ListPuz[1].tag == "Cure")
            {
                PLScript.HPAdd(ListPuz.Count);
            }
            int EnemCount = 0;
            foreach (var item in ListPuz)
            {
                string tag = item.tag;
                if (tag == "Enemy")
                {
                    EnemSc EnemScript = item.GetComponent<EnemSc>();
                    EnemScript.EnemyHp = EnemScript.EnemyHp - NowPLAttack;
                    SpriteRenderer itemSprite = item.GetComponent<SpriteRenderer>();
                    Color PuzColor = itemSprite.color;
                    PuzColor.a = 1;
                    itemSprite.color = PuzColor;
                    EnemCount++;
                    continue;
                }
                else
                {
                    Enab.count--;
                    Destroy(item);
                }
            }
            turn++;
            TurnPoint.text = turn.ToString();
            Invoke(nameof(CallHPDown), 0.2f);
        }
        else
        {
            foreach (var item in ListPuz)
            {
                Color PuzColor = item.GetComponent<SpriteRenderer>().color;
                PuzColor.a = 1;
                item.GetComponent<SpriteRenderer>().color = PuzColor;
            }
        }
        ListPuz.Clear();
        PuzzleLine.positionCount = 0;
    }

    void ReMake(float x)
    {
        for(int i = 0; i< 6; i++)
        {
            int r_kind = UnityEngine.Random.Range(0, 4);
            float r_Scale = UnityEngine.Random.Range(PuzSize_Min, PuzSize_Max);
            float r_Pos = UnityEngine.Random.Range(0f, 1f);
            GameObject puz = new GameObject();
            if (r_kind == 3) //Enemyを引いた時
            {
                EnemSc Enem = Instantiate(Enempuz); //Enemyを生成
                Enem.ChangeLev((PLScript.Level / 2) + 1);   //レベル調整
                Enab.AllEnemies.Add(Enem);
                puz = Enem.gameObject;
            }
            else
            {
                puz = Instantiate(Puz[r_kind]);
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
                float r_Scale = UnityEngine.Random.Range(PuzSize_Min, PuzSize_Max);
                float r_Pos = UnityEngine.Random.Range(0f, 1f);
                int r = 0;
                GameObject puz = new GameObject();
                if (r_kind == 3) //Enemyを引いた時
                {
                    r = UnityEngine.Random.Range(0, 3 + 1); //2/3判定
                    if (r != 0)  //当たったぞ！
                    {
                        EnemSc Enem = Instantiate(Enempuz); //Enemyを晴れて生成
                        Enab.AllEnemies.Add(Enem);
                        puz = Enem.gameObject;
                    }
                }
                else
                {
                    puz = Instantiate(Puz[r_kind]);
                }
                puz.transform.position = new Vector2(-9 + i * 4 + r_Pos, -8 + j * 4);
                puz.transform.localScale = new Vector3(r_Scale, r_Scale, 1);
            }
        }
        for(int i =0; i < 7; i++)
        {
            Lim[i].transform.position = new Vector2(-9 + i * 4, 80);
        }
    }

    //PLScriptのHPDownをコルーチン(Invoke)するための関数
    void CallHPDown()
    {
        PLScript.HPDown();
    }

    //Lineを伸ばすための関数
    void AddNewLinePos(Transform tr)
    {
        PuzzleLine.positionCount++;
        PuzzleLine.SetPosition(PuzzleLine.positionCount - 1, tr.position);
    }
}