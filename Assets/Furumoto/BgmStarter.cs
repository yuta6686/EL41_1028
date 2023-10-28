/*******************************************************************************
*
*	�^�C�g���F	�V�[���J�ڎ�BGM�ݒ�V���O���g���X�N���v�g
*	�t�@�C���F	BgmStarter.cs
*	�쐬�ҁF	�Ö{ �ח�
*	������F    2023/05/31
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmStarter : SingletonMonoBehaviour<BgmStarter>
{
    [SerializeField, CustomLabel("�Đ�����BGM�̉���")]
    private string m_bgmName;

    void Start()
    {
        AudioManager.Instance.StopBgmAll();
        AudioManager.Instance.PlayBgm(m_bgmName, true);

        // ���̌�͕s�v�Ȃ̂ō폜����
        Destroy(gameObject);
    }
}
