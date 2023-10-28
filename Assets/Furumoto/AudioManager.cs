/*******************************************************************************
*
*	�^�C�g���F	�I�[�f�B�I�Ǘ��V���O���g���X�N���v�g
*	�t�@�C���F	AudioManager.cs
*	�쐬�ҁF	�Ö{ �ח�
*	������F    2023/05/22
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
        // �I�[�f�B�I�t�@�C��
        public AudioClip m_Clip = null;

        // �{�����[��
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
        // ����(�t�@�C�����Ƃ͕�)
        public string m_Name = "";

        // �I�[�f�B�I�t�@�C��
        public AudioClip m_Clip = null;

        // �{�����[��
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

    [SerializeField, CustomLabel("BGM����"), Range(0, 1)]
    private float m_bgmVolume = 1.0f;

    [SerializeField, CustomLabel("SE����"), Range(0, 1)]
    private float m_seVolume = 1.0f;

    [SerializeField, CustomLabel("BGM�̓����Đ��\��"), Range(0, 3)]
    private int m_bgmPlaybackNum = 5;

    [SerializeField, CustomLabel("SE�̓����Đ��\��"), Range(0, 30)]
    private int m_sePlaybackNum = 5;

    [SerializeField, CustomLabel("�~���[�g(BGM)")]
    private bool m_isMuteBgm = false;

    [SerializeField, CustomLabel("�~���[�g(SE)")]
    private bool m_isMuteSe = false;

    // ���g�p��AudioSource
    private List<AudioSource> m_bgmSources = new List<AudioSource>();
    private List<AudioSource> m_seSources = new List<AudioSource>();

    // �g�p����AudioSource
    private List<UsingSource> m_bgmUsingSources = new List<UsingSource>();
    private List<UsingSource> m_seUsingSources = new List<UsingSource>();

    // �I�[�f�B�I�f�[�^
    // �L�[   string�^     ����(�t�@�C�����Ƃ͕�)
    // �v�f   AudioInfo�^  �����
    private Dictionary<string, AudioInfo> m_bgmInfo = new Dictionary<string, AudioInfo>();
    private Dictionary<string, AudioInfo> m_seInfo = new Dictionary<string, AudioInfo>();

    // �C���X�y�N�^�[���͗p���X�g
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

        // ���Z�[�u�f�[�^����擾
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

        // BGM�p��AudioSource�R���|�[�l���g���擾
        for (int i = 0; i < m_bgmPlaybackNum; i++)
        {
            // AudioSource��ǉ�
            m_bgmSources.Add(gameObject.AddComponent<AudioSource>());

            // �����{�����[�����Z�b�g
            m_bgmSources[i].volume = m_bgmVolume;
        }

        // SE�p��AudioSource�R���|�[�l���g���擾
        for (int i = 0; i < m_sePlaybackNum; i++)
        {
            // AudioSource��ǉ�
            m_seSources.Add(gameObject.AddComponent<AudioSource>());

            // �����{�����[�����Z�b�g
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
    //     BGM�֘A
    //=============================================================================

    /// <summary>
    /// BGM�Đ�
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    /// <param name="loop"> ���[�v���邩 </param>
    /// <param name="volume"> �Đ����ʔ{�� </param>
    public void PlayBgm(string name, bool loop, float volume = 1.0f)
    {
        // ���݂��Ȃ����O�Ȃ�I��
        if (!m_bgmInfo.ContainsKey(name)) { return; }

        // �Đ��Ɏg�p����Sources���擾
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
    /// BGM�Đ�
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    /// <param name="source"> �Đ��Ɏg�p����R���|�[�l���g���󂯎�邽�߂̎Q�ƌ^ </param>
    /// <param name="loop"> ���[�v���邩 </param>
    /// <param name="volume"> �Đ����ʔ{�� </param>
    public void PlayBgm(string name, ref AudioSource source, bool loop, float volume = 1.0f)
    {
        // ���݂��Ȃ����O�Ȃ�I��
        if (!m_bgmInfo.ContainsKey(name)) { return; }

        // �Đ�����
        AudioInfo info = m_bgmInfo[name];
        source.clip = info.m_Clip;
        source.loop = loop;
        source.volume = m_bgmVolume * info.m_Volume * volume;
        source.Play();
    }

    /// <summary>
    /// BGM��~
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    public void StopBgm(string name)
    {
        // ���O�����~����BGM����������
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
    /// �SBGM��~
    /// </summary>
    public void StopBgmAll()
    {
        int count = m_bgmUsingSources.Count;

        for (int i = 0; i < count; i++)
        {
            // ��~�Ə�����
            m_bgmUsingSources[0].m_source.Stop();
            m_bgmUsingSources[0].m_source.clip = null;

            // ���g�p���X�g�Ɉړ�
            m_bgmSources.Add(m_bgmUsingSources[0].m_source);
            m_bgmUsingSources.RemoveAt(0);
        }
    }

    /// <summary>
    /// BGM�ꎞ��~
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    public void PauseBgm(string name)
    {
        // ���O����ꎞ��~����BGM����������
        UsingSource source = BgmSourceNameToNumber(name);

        if (source != null)
        {
            source.m_source.Pause();
        }
    }

    /// <summary>
    /// �SBGM�~���[�g
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
    /// �SBGM�~���[�g����
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
    /// BGM�Đ��ĊJ
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    public void ResumeBgm(string name)
    {
        // ���O����ꎞ��~����BGM����������
        UsingSource source = BgmSourceNameToNumber(name);

        if (source != null)
        {
            source.m_source.Play();
        }
    }

    /// <summary>
    /// �w�肵�����O��BGM���Đ����Ă���AudioSource���擾
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    private UsingSource BgmSourceNameToNumber(string name)
    {
        // ���݂��Ȃ����O�Ȃ�I��
        if (!m_bgmInfo.ContainsKey(name)) { return null; }

        AudioClip clip = m_bgmInfo[name].m_Clip;

        // �t�@�C�����r���ĒT��
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
    /// �Đ����̑SBGM�pSource�̉��ʂ�ύX
    /// </summary>
    /// <param name="rate"> ���̉��ʂɑ΂���V�������ʂ̔䗦 </param>
    private void SetBgmUsingSourceVolume(float volume)
    {
        foreach (UsingSource s in m_bgmUsingSources)
        {
            s.m_source.volume = volume * s.m_originVolume;
        }
    }

    /// <summary>
    /// �Đ����̑SBGM�pSource�̉��ʂ�ύX
    /// </summary>
    /// <param name="rate"> ���̉��ʂɑ΂���V�������ʂ̔䗦 </param>
    private void SetBgmUsingSourceVolumeRate(float rate)
    {
        foreach (UsingSource s in m_bgmUsingSources)
        {
            s.m_source.volume *= rate;
        }
    }


    //=============================================================================
    //     SE�֘A
    //=============================================================================

    /// <summary>
    /// SE�Đ�
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    /// <param name="loop"> ���[�v���邩 </param>
    public void PlaySe(string name, bool loop, float volume = 1.0f)
    {
        // ���݂��Ȃ����O�Ȃ�I��
        if (!m_seInfo.ContainsKey(name)) { return; }

        // �Đ��Ɏg�p����Sources���擾
        AudioSource source;
        AudioInfo info = m_seInfo[name];

        // ���g�p��Source�����邩
        if (m_seSources.Count != 0)
        {
            // ����΂�����g��
            source = m_seSources[0];

            m_seSources.RemoveAt(0);
            m_seUsingSources.Add(new UsingSource(source, info.m_Volume));
        }
        else
        {
            // ������Ό��ݍĐ�����SE�̒��ň�ԌÂ�Source����Đ�
            source = m_seUsingSources[0].m_source;
            m_seUsingSources[0].m_originVolume = info.m_Volume;
        }

        source.clip = info.m_Clip;
        source.loop = loop;
        source.volume = m_seVolume * info.m_Volume * volume;
        source.Play();
    }

    /// <summary>
    /// SE�Đ�
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    /// <param name="loop"> ���[�v���邩 </param>
    public void PlaySe(string name, ref AudioSource source, bool loop, float volume = 1.0f)
    {
        // ���݂��Ȃ����O�Ȃ�I��
        if (!m_seInfo.ContainsKey(name)) { return; }

        AudioInfo info = m_seInfo[name];
        source.clip = info.m_Clip;
        source.loop = loop;
        source.volume = m_seVolume * info.m_Volume * volume;
        source.Play();
    }

    /// <summary>
    /// SE��~
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    public void StopSe(string name)
    {
        // ���O�����~����BGM����������
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
    /// �SSE��~
    /// </summary>
    public void StopSeAll()
    {
        // �Đ�����Source�̐���ۑ�
        // (���X�g����폜���Ă�������)
        int count = m_seUsingSources.Count;

        for (int i = 0; i < count; i++)
        {
            // ��~�Ə�����
            m_seUsingSources[0].m_source.Stop();
            m_seUsingSources[0].m_source.clip = null;

            // ���g�p���X�g�Ɉړ�
            m_seSources.Add(m_seUsingSources[0].m_source);
            m_seUsingSources.RemoveAt(0);
        }
    }

    /// <summary>
    /// �SSE�~���[�g
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
    /// �SSE�~���[�g����
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
    /// �w�肵�����O��SE���Đ����Ă���AudioSource���擾
    /// </summary>
    /// <param name="name"> ����(Dictionary�̃L�[���) </param>
    private UsingSource SeSourceNameToNumber(string name)
    {
        // ���݂��Ȃ����O�Ȃ�I��
        if (!m_seInfo.ContainsKey(name)) { return null; }

        AudioClip clip = m_seInfo[name].m_Clip;

        // �t�@�C�����r���ĒT��
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
    /// �Đ����̑SSE�pSource�̉��ʂ�ύX
    /// </summary>
    /// <param name="volume"> �V�������� </param>
    private void SetSeUsingSourceVolume(float volume)
    {
        foreach (UsingSource s in m_seUsingSources)
        {
            s.m_source.volume = volume * s.m_originVolume;
        }
    }

    /// <summary>
    /// �Đ����̑SSE�pSource�̉��ʂ�ύX
    /// </summary>
    /// <param name="rate"> ���̉��ʂɑ΂���V�������ʂ̔䗦 </param>
    private void SetSeUsingSourceVolumeRate(float rate)
    {
        foreach (UsingSource s in m_seUsingSources)
        {
            s.m_source.volume *= rate;
        }
    }

    #endregion
}
