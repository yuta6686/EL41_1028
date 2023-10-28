/*******************************************************************************
*
*	タイトル：	オーディオ管理シングルトンスクリプト
*	ファイル：	AudioManager.cs
*	作成者：	古本 泰隆
*	制作日：    2023/05/22
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    #region support class

    public class AudioInfo
    {
        // オーディオファイル
        public AudioClip m_Clip = null;

        // ボリューム
        public float m_Volume = 1.0f;

        public AudioInfo(AudioClip clip, float volume)
        {
            m_Clip = clip;
            m_Volume = volume;
        }
    }

    [System.Serializable]
    public class AudioInfoInspector
    {
        // 音名(ファイル名とは別)
        public string m_Name = "";

        // オーディオファイル
        public AudioClip m_Clip = null;

        // ボリューム
        public float m_Volume = 1.0f;
    }

    public class UsingSource
    {
        public AudioSource m_source;

        public float m_originVolume;

        public UsingSource(AudioSource source, float volume)
        {
            m_source = source;
            m_originVolume = volume;
        }
    }

    #endregion

    #region field

    [SerializeField, CustomLabel("BGM音量"), Range(0, 1)]
    private float m_bgmVolume = 1.0f;

    [SerializeField, CustomLabel("SE音量"), Range(0, 1)]
    private float m_seVolume = 1.0f;

    [SerializeField, CustomLabel("BGMの同時再生可能数"), Range(0, 3)]
    private int m_bgmPlaybackNum = 5;

    [SerializeField, CustomLabel("SEの同時再生可能数"), Range(0, 30)]
    private int m_sePlaybackNum = 5;

    [SerializeField, CustomLabel("ミュート(BGM)")]
    private bool m_isMuteBgm = false;

    [SerializeField, CustomLabel("ミュート(SE)")]
    private bool m_isMuteSe = false;

    // 未使用のAudioSource
    private List<AudioSource> m_bgmSources = new List<AudioSource>();
    private List<AudioSource> m_seSources = new List<AudioSource>();

    // 使用中のAudioSource
    private List<UsingSource> m_bgmUsingSources = new List<UsingSource>();
    private List<UsingSource> m_seUsingSources = new List<UsingSource>();

    // オーディオデータ
    // キー   string型     音名(ファイル名とは別)
    // 要素   AudioInfo型  音情報
    private Dictionary<string, AudioInfo> m_bgmInfo = new Dictionary<string, AudioInfo>();
    private Dictionary<string, AudioInfo> m_seInfo = new Dictionary<string, AudioInfo>();

    // インスペクター入力用リスト
    [SerializeField]
    private List<AudioInfoInspector> m_bgmInfoInspector = new List<AudioInfoInspector>();
    [SerializeField]
    private List<AudioInfoInspector> m_seInfoInspector = new List<AudioInfoInspector>();

    #endregion

    #region property

    public float m_BgmVolume
    {
        get { return m_bgmVolume; }
        set
        {
            if (m_bgmVolume == 0.0f)
            {
                SetBgmUsingSourceVolume(value);
            }
            else
            {
                float rate = value / m_bgmVolume;
                SetBgmUsingSourceVolumeRate(rate);
            }
            m_bgmVolume = Mathf.Clamp01(value);
        }
    }

    public float m_SeVolume
    {
        get { return m_seVolume; }
        set
        {
            if (m_seVolume == 0.0f)
            {
                SetSeUsingSourceVolume(value);
            }
            else
            {
                float rate = value / m_seVolume;
                SetSeUsingSourceVolumeRate(rate);
            }
            m_seVolume = Mathf.Clamp01(value);
        }
    }

    public bool m_IsMuteBgm
    {
        get { return m_isMuteBgm; }
        set
        {
            m_isMuteBgm = value;

            if (value)
            {
                MuteBgmAll();
            }
            else
            {
                UnmuteBgmAll();
            }
        }
    }

    public bool m_IsMuteSe
    {
        get { return m_isMuteSe; }
        set
        {
            m_isMuteSe = value;

            if (value)
            {
                MuteSeAll();
            }
            else
            {
                UnmuteSeAll();
            }
        }
    }

    #endregion

    #region function

    protected override void Awake()
    {
        base.Awake();

        // ★セーブデータから取得
        //m_bgmVolume = SaveDataManager.Instance.m_SaveData.Value.Option.BgmVolume * 0.1f;
        //m_seVolume = SaveDataManager.Instance.m_SaveData.Value.Option.SeVolume * 0.1f;

        foreach (AudioInfoInspector info in m_bgmInfoInspector)
        {
            m_bgmInfo.Add(info.m_Name, new AudioInfo(info.m_Clip, info.m_Volume));
        }
        foreach (AudioInfoInspector info in m_seInfoInspector)
        {
            m_seInfo.Add(info.m_Name, new AudioInfo(info.m_Clip, info.m_Volume));
        }

        // BGM用にAudioSourceコンポーネントを取得
        for (int i = 0; i < m_bgmPlaybackNum; i++)
        {
            // AudioSourceを追加
            m_bgmSources.Add(gameObject.AddComponent<AudioSource>());

            // 初期ボリュームをセット
            m_bgmSources[i].volume = m_bgmVolume;
        }

        // SE用にAudioSourceコンポーネントを取得
        for (int i = 0; i < m_sePlaybackNum; i++)
        {
            // AudioSourceを追加
            m_seSources.Add(gameObject.AddComponent<AudioSource>());

            // 初期ボリュームをセット
            m_seSources[i].volume = m_seVolume;
        }
    }

    private void Update()
    {
        int i = 0;
        while (i < m_bgmUsingSources.Count)
        {
            if (!m_bgmUsingSources[i].m_source.isPlaying)
            {
                m_bgmSources.Add(m_bgmUsingSources[i].m_source);
                m_bgmUsingSources.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        i = 0;
        while (i < m_seUsingSources.Count)
        {
            if (!m_seUsingSources[i].m_source.isPlaying)
            {
                m_seSources.Add(m_seUsingSources[i].m_source);
                m_seUsingSources.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    //=============================================================================
    //     BGM関連
    //=============================================================================

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    /// <param name="loop"> ループするか </param>
    /// <param name="volume"> 再生音量倍率 </param>
    public void PlayBgm(string name, bool loop, float volume = 1.0f)
    {
        // 存在しない名前なら終了
        if (!m_bgmInfo.ContainsKey(name)) { return; }

        // 再生に使用するSourcesを取得
        AudioSource source;
        AudioInfo info = m_bgmInfo[name];

        if (m_bgmSources.Count != 0)
        {
            source = m_bgmSources[0];

            m_bgmSources.RemoveAt(0);
            m_bgmUsingSources.Add(new UsingSource(source, info.m_Volume));
        }
        else
        {
            source = m_bgmUsingSources[0].m_source;
            m_bgmUsingSources[0].m_originVolume = info.m_Volume;
        }

        source.clip = info.m_Clip;
        source.loop = loop;
        source.volume = m_bgmVolume * info.m_Volume * volume;
        source.Play();
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    /// <param name="source"> 再生に使用するコンポーネントを受け取るための参照型 </param>
    /// <param name="loop"> ループするか </param>
    /// <param name="volume"> 再生音量倍率 </param>
    public void PlayBgm(string name, ref AudioSource source, bool loop, float volume = 1.0f)
    {
        // 存在しない名前なら終了
        if (!m_bgmInfo.ContainsKey(name)) { return; }

        // 再生処理
        AudioInfo info = m_bgmInfo[name];
        source.clip = info.m_Clip;
        source.loop = loop;
        source.volume = m_bgmVolume * info.m_Volume * volume;
        source.Play();
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    public void StopBgm(string name)
    {
        // 名前から停止するBGMを検索する
        UsingSource source = BgmSourceNameToNumber(name);

        if (source != null)
        {
            source.m_source.Stop();
            source.m_source.clip = null;

            m_bgmSources.Add(source.m_source);
            m_bgmUsingSources.Remove(source);
        }
    }

    /// <summary>
    /// 全BGM停止
    /// </summary>
    public void StopBgmAll()
    {
        int count = m_bgmUsingSources.Count;

        for (int i = 0; i < count; i++)
        {
            // 停止と初期化
            m_bgmUsingSources[0].m_source.Stop();
            m_bgmUsingSources[0].m_source.clip = null;

            // 未使用リストに移動
            m_bgmSources.Add(m_bgmUsingSources[0].m_source);
            m_bgmUsingSources.RemoveAt(0);
        }
    }

    /// <summary>
    /// BGM一時停止
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    public void PauseBgm(string name)
    {
        // 名前から一時停止するBGMを検索する
        UsingSource source = BgmSourceNameToNumber(name);

        if (source != null)
        {
            source.m_source.Pause();
        }
    }

    /// <summary>
    /// 全BGMミュート
    /// </summary>
    private void MuteBgmAll()
    {
        foreach (AudioSource s in m_bgmSources)
        {
            s.mute = true;
        }
        foreach (UsingSource s in m_bgmUsingSources)
        {
            s.m_source.mute = true;
        }
    }

    /// <summary>
    /// 全BGMミュート解除
    /// </summary>
    private void UnmuteBgmAll()
    {
        foreach (AudioSource s in m_bgmSources)
        {
            s.mute = false;
        }
        foreach (UsingSource s in m_bgmUsingSources)
        {
            s.m_source.mute = false;
        }
    }

    /// <summary>
    /// BGM再生再開
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    public void ResumeBgm(string name)
    {
        // 名前から一時停止するBGMを検索する
        UsingSource source = BgmSourceNameToNumber(name);

        if (source != null)
        {
            source.m_source.Play();
        }
    }

    /// <summary>
    /// 指定した名前のBGMを再生しているAudioSourceを取得
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    private UsingSource BgmSourceNameToNumber(string name)
    {
        // 存在しない名前なら終了
        if (!m_bgmInfo.ContainsKey(name)) { return null; }

        AudioClip clip = m_bgmInfo[name].m_Clip;

        // ファイルを比較して探す
        foreach (UsingSource s in m_bgmUsingSources)
        {
            if (s.m_source.clip == clip)
            {
                return s;
            }
        }

        return null;
    }

    /// <summary>
    /// 再生中の全BGM用Sourceの音量を変更
    /// </summary>
    /// <param name="rate"> 元の音量に対する新しい音量の比率 </param>
    private void SetBgmUsingSourceVolume(float volume)
    {
        foreach (UsingSource s in m_bgmUsingSources)
        {
            s.m_source.volume = volume * s.m_originVolume;
        }
    }

    /// <summary>
    /// 再生中の全BGM用Sourceの音量を変更
    /// </summary>
    /// <param name="rate"> 元の音量に対する新しい音量の比率 </param>
    private void SetBgmUsingSourceVolumeRate(float rate)
    {
        foreach (UsingSource s in m_bgmUsingSources)
        {
            s.m_source.volume *= rate;
        }
    }


    //=============================================================================
    //     SE関連
    //=============================================================================

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    /// <param name="loop"> ループするか </param>
    public void PlaySe(string name, bool loop, float volume = 1.0f)
    {
        // 存在しない名前なら終了
        if (!m_seInfo.ContainsKey(name)) { return; }

        // 再生に使用するSourcesを取得
        AudioSource source;
        AudioInfo info = m_seInfo[name];

        // 未使用のSourceがあるか
        if (m_seSources.Count != 0)
        {
            // あればそれを使う
            source = m_seSources[0];

            m_seSources.RemoveAt(0);
            m_seUsingSources.Add(new UsingSource(source, info.m_Volume));
        }
        else
        {
            // 無ければ現在再生中のSEの中で一番古いSourceから再生
            source = m_seUsingSources[0].m_source;
            m_seUsingSources[0].m_originVolume = info.m_Volume;
        }

        source.clip = info.m_Clip;
        source.loop = loop;
        source.volume = m_seVolume * info.m_Volume * volume;
        source.Play();
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    /// <param name="loop"> ループするか </param>
    public void PlaySe(string name, ref AudioSource source, bool loop, float volume = 1.0f)
    {
        // 存在しない名前なら終了
        if (!m_seInfo.ContainsKey(name)) { return; }

        AudioInfo info = m_seInfo[name];
        source.clip = info.m_Clip;
        source.loop = loop;
        source.volume = m_seVolume * info.m_Volume * volume;
        source.Play();
    }

    /// <summary>
    /// SE停止
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    public void StopSe(string name)
    {
        // 名前から停止するBGMを検索する
        UsingSource source = SeSourceNameToNumber(name);

        if (source != null)
        {
            source.m_source.Stop();
            source.m_source.clip = null;

            m_seSources.Add(source.m_source);
            m_seUsingSources.Remove(source);
        }
    }

    /// <summary>
    /// 全SE停止
    /// </summary>
    public void StopSeAll()
    {
        // 再生中のSourceの数を保存
        // (リストから削除していくため)
        int count = m_seUsingSources.Count;

        for (int i = 0; i < count; i++)
        {
            // 停止と初期化
            m_seUsingSources[0].m_source.Stop();
            m_seUsingSources[0].m_source.clip = null;

            // 未使用リストに移動
            m_seSources.Add(m_seUsingSources[0].m_source);
            m_seUsingSources.RemoveAt(0);
        }
    }

    /// <summary>
    /// 全SEミュート
    /// </summary>
    private void MuteSeAll()
    {
        foreach (AudioSource s in m_seSources)
        {
            s.mute = true;
        }
        foreach (UsingSource s in m_seUsingSources)
        {
            s.m_source.mute = true;
        }
    }

    /// <summary>
    /// 全SEミュート解除
    /// </summary>
    private void UnmuteSeAll()
    {
        foreach (AudioSource s in m_seSources)
        {
            s.mute = false;
        }
        foreach (UsingSource s in m_seUsingSources)
        {
            s.m_source.mute = false;
        }
    }

    /// <summary>
    /// 指定した名前のSEを再生しているAudioSourceを取得
    /// </summary>
    /// <param name="name"> 音名(Dictionaryのキー情報) </param>
    private UsingSource SeSourceNameToNumber(string name)
    {
        // 存在しない名前なら終了
        if (!m_seInfo.ContainsKey(name)) { return null; }

        AudioClip clip = m_seInfo[name].m_Clip;

        // ファイルを比較して探す
        foreach (UsingSource s in m_seUsingSources)
        {
            if (s.m_source.clip == clip)
            {
                return s;
            }
        }

        return null;
    }

    /// <summary>
    /// 再生中の全SE用Sourceの音量を変更
    /// </summary>
    /// <param name="volume"> 新しい音量 </param>
    private void SetSeUsingSourceVolume(float volume)
    {
        foreach (UsingSource s in m_seUsingSources)
        {
            s.m_source.volume = volume * s.m_originVolume;
        }
    }

    /// <summary>
    /// 再生中の全SE用Sourceの音量を変更
    /// </summary>
    /// <param name="rate"> 元の音量に対する新しい音量の比率 </param>
    private void SetSeUsingSourceVolumeRate(float rate)
    {
        foreach (UsingSource s in m_seUsingSources)
        {
            s.m_source.volume *= rate;
        }
    }

    #endregion
}
