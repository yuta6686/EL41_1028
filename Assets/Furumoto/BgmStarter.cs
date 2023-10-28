/*******************************************************************************
*
*	タイトル：	シーン遷移時BGM設定シングルトンスクリプト
*	ファイル：	BgmStarter.cs
*	作成者：	古本 泰隆
*	制作日：    2023/05/31
*
*******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmStarter : SingletonMonoBehaviour<BgmStarter>
{
    [SerializeField, CustomLabel("再生するBGMの音名")]
    private string m_bgmName;

    void Start()
    {
        AudioManager.Instance.StopBgmAll();
        AudioManager.Instance.PlayBgm(m_bgmName, true);

        // この後は不要なので削除する
        Destroy(gameObject);
    }
}
