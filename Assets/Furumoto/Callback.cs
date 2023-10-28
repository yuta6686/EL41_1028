using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Callback : MonoBehaviour
{
    [SerializeField]
    private Player m_player;
    [SerializeField]
    private CinemachineVirtualCamera m_camera;
    [SerializeField]
    private Sprash _sprash;

    public void JumpCallback()
    {
        m_player.ChangeJumpState();
        AudioManager.Instance.PlaySe("ƒWƒƒƒ“ƒv‰¹", false);
    }

    public void FinishCallback()
    {
        m_player.ChangeEndState();
        m_camera.Priority = 100;

        _sprash.SetEffect();
        AudioManager.Instance.PlaySe("’……‰¹", false);
    }
}
