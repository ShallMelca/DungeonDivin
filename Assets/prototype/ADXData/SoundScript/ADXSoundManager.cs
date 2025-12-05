using CriWare;
using CriWare.Assets;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CriWare.Assets.CriAtomAssetsLoader;

class CueData
{
    public CriAtomExAcb AcbHandle;
    public int CueID;

    public CueData(CriAtomExAcb acbHandle, int cueID)
    {
        AcbHandle = acbHandle;
        CueID = cueID;
    }
}

public class ADXSoundManager: IDisposable
{
    // ExPlayerを管理する変数.
    private Dictionary<string, MyExPlayer> _exPlayers;
    private CriAtomEx3dListener _ex3dListener;    // ExListener
    private Transform _transform;

    private Dictionary<E_Sounds, CueData> _cueMap = new Dictionary<E_Sounds, CueData>();
    private Dictionary<E_Sounds, string> _exPlMap = new Dictionary<E_Sounds, string>();

    // ========================================================================================
    // ADXSoundManagerをシングルトンとするための記述.

    private static ADXSoundManager _instance;

    // ADXSoundManager.Instanceという記述で、どこからでもADXSoundManagerにアクセス可能.
    public static ADXSoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("ADX Load");
                _instance = new ADXSoundManager();
            }
            return _instance;
        }
    }

    // コンストラクタをプライベートにして外部からのインスタンス化を防ぐ
    private ADXSoundManager()
    {
        _exPlayers = new Dictionary<string, MyExPlayer>();
        _ex3dListener = new CriAtomEx3dListener();

        // --------------------------------------------------------------------------------
        // ADXSoundManagerで必要な初期化コードがあれば適宜ここのコードブロックに書くとよい
        //
        // int myFavoriteThings = 100;
        // myFavoriteThings += 10;
        // --------------------------------------------------------------------------------
    }
    // ADXSoundManagerのシングルトンの記述はここまで
    // ========================================================================================

    public void SetCueReference(CueSheetsManager asset)
    {
        Debug.Log($"SetCueReference");
        foreach(var entry in asset.soundDatas.Entries)
        {
            if (entry.AcbReference.AcbAsset == null) continue;

            CriAtomExAcb handle = entry.AcbReference.AcbAsset.Handle;

            int cueId = entry.AcbReference.CueId;

            if (cueId == -1) continue;

            _cueMap[entry.Key] = new CueData(handle, cueId);
            _exPlMap[entry.Key] = entry.PlayerKey;
        }
        Instance.SetCategoryVolume("BGM", 0.5f);
        Instance.SetCategoryVolume("SE", 0.5f);
    }

    // リソースの破棄.
    public void Dispose()
    {
        // すべてのexPlayerを破棄.
        foreach (var exPlayer in _exPlayers.Values)
        {
            exPlayer.Dispose();
        }
        _exPlayers.Clear();

        // Ex3dListenerの破棄.
        _ex3dListener.Dispose();

        GC.SuppressFinalize(this);
    }

    public void Dispose(string key)
    {
        MyExPlayer exPlayer = GetExPlayer(key);
        if (exPlayer != null)
        {
            exPlayer.Dispose();
        }
    }

    private MyExPlayer GetOrCreateExPlayer(string key)
    {
        if (!_exPlayers.ContainsKey(key))
        {
            // keyに対応する名前のExPlayerがまだ存在しないなら作る
            _exPlayers[key] = new MyExPlayer();
        }

        return _exPlayers[key];
    }

    private MyExPlayer GetExPlayer(string key)
    {
        if(_exPlayers.ContainsKey(key))
        {
            return _exPlayers[key];
        }
        else { return null; }
    }

    public bool IsSoundReady(E_Sounds enumSound)
    {
        return _cueMap.ContainsKey(enumSound);
    }

    public void PlaySound(E_Sounds enumSound)
    {
        CueData cueData = _cueMap[enumSound];
        MyExPlayer exPlayer = GetOrCreateExPlayer(_exPlMap[enumSound]);
        exPlayer.SetTransform(null);
        exPlayer.Play(cueData.AcbHandle, cueData.CueID, false);
    }

    //キューの名前から.
    public void PlaySound(string key, CriAtomExAcb cueSheet, string cueName, Transform transform, bool is3D)
    {
        MyExPlayer exPlayer = GetOrCreateExPlayer(key);
        exPlayer.SetTransform(transform);
        exPlayer.Play(cueSheet, cueName, is3D);
    }

    //キューIDから.
    public void PlaySound(string key, CriAtomExAcb cueSheet, int cueId, Transform transform, bool is3D)
    {
        MyExPlayer exPlayer = GetOrCreateExPlayer(key);
        exPlayer.SetTransform(transform);
        exPlayer.Play(cueSheet, cueId, is3D);
    }

    public void StopSound(string key)
    {
        MyExPlayer exPlayer = GetExPlayer(key);
            exPlayer.Stop(true);
            exPlayer.Update();
    }

    public void PauseSound(string key)
    {
        MyExPlayer exPlayer = GetExPlayer(key);
        if (exPlayer != null)
        {
            exPlayer.Pause();
        }
    }

    public void ResumeSound(string key)
    {
        MyExPlayer exPlayer = GetExPlayer(key);
        if (exPlayer != null)
        {
            exPlayer.Resume();
        }
    }

    #region 3DPositioning

    // exPlayerと紐づいているex3dSourceのポジションをアップデートする関数
    // 例えばGameObejct側のUpdate()でこのADXSoundManager.Instance.UpdateSoundPosition()という記述で更新する.
    public void UpdateSoundPosition(string key)
    {
        if (_exPlayers.TryGetValue(key, out MyExPlayer exPlayer))
        {
            exPlayer.Update();
        }
    }

    // リスナーとして使用するTransformを設定(カメラやキャラクターなど)
    public void SetListenerTransform(Transform transform)
    {
        _transform = transform;
    }

    // 紐づいているGameObjectのUpdate()でこのポジションの更新を呼ぶ.
    public void UpdateListenerPosition()
    {
        if (_transform != null)
        {
            _ex3dListener.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
            _ex3dListener.Update();
        }
    }
    #endregion

    public void AISAC(string key, uint aisacControll_Id, float value)
    {
        MyExPlayer exPlayer = GetOrCreateExPlayer(key);
        exPlayer.SetAisacControl(aisacControll_Id, value);
    }

    public void GameValue(uint GameVariable_Id, float value)
    {
        CriAtomEx.SetGameVariable(GameVariable_Id, value);
    }

    public void SetCategoryVolume(string categoryName, float value)
    {
        CriAtom.SetCategoryVolume(categoryName, value);
    }

    public float GetCategoryVolume(string categoryName)
    {
        return CriAtom.GetCategoryVolume(categoryName);
    }

    //debug



    // ========================================================================================
    // ここからMyExPlayerクラスの記述

    // MyExPlayerクラスをADXSoundManagerを介さずに触れないようにprivateにする。(内部クラスとして定義する)
    private class MyExPlayer
    {
        private CriAtomExPlayer _exPlayer;
        private CriAtomEx3dSource _ex3dSource;

        // 音源となるGameObjectの座標.
        private Transform _transform;

        // コンストラクタ. UnityでいうAwake()やStart()と近い、初期化処理.
        public MyExPlayer()
        {
            _exPlayer = new CriAtomExPlayer();
            _ex3dSource = new CriAtomEx3dSource();
        }

        public void SetTransform(Transform transform)
        {
            _transform = transform;
            _exPlayer.Set3dSource(_ex3dSource);
        }

        public void SetAisacControl(uint ID, float value)
        {
            _exPlayer.SetAisacControl(ID, value);
        }

        public void Play(CriAtomExAcb cueSheet, string cueName, bool is3D)
        {
            _exPlayer.SetCue(cueSheet, cueName);

            if (is3D && _transform != null)
            {
                _ex3dSource.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
                _ex3dSource.Update();
            }

            // 再生
            _exPlayer.Start();

            // ---------------------------------------------------------------------------------------------
            // ブロック再生は_exPlayer.Start();の返り値であるCriAtomExPlaybackを介して、
            // exPlayback.SetNextBlockIndex(10);
            // といったように使用するため、必要であれば適宜exPlaybackを使用すると良い。
            // また、アクション機能のブロック遷移先指定アクションを使えば、exPlaybackは不要
            //
            // ExPlaybackを使用したい場合は、関数の返り値の型とともに、以下のように書き換えるとよい。
            // CriAtomExPlayback exPlayback = criAtomExPlayer.Start();
            // return exPlayback;
            // ---------------------------------------------------------------------------------------------

        }

        public void Play(CriAtomExAcb cueSheet, int cueId, bool is3D)
        {
            _exPlayer.SetCue(cueSheet, cueId);

            if (is3D && _transform != null)
            {
                _ex3dSource.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
                _ex3dSource.Update();
            }

            // 再生
            _exPlayer.Start();
        }

        public void Update()
        {
            if (_transform != null)
            {
                _ex3dSource.SetPosition(_transform.position.x, _transform.position.y, _transform.position.z);
                _ex3dSource.Update();
            }
        }


        public void Stop(bool a)
        {
            _exPlayer.Stop(a);
        }

        public void Pause()
        {
            _exPlayer.Pause();
        }

        public void Resume()
        {
            _exPlayer.Resume(CriAtomEx.ResumeMode.PausedPlayback);
        }

        public void Dispose()
        {
            _exPlayer.Dispose();
            _ex3dSource.Dispose();
        }
    }
}
