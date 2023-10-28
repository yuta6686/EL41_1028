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

    public void JumpCallback()
    {
        m_player.ChangeJumpState();
    }

    public void FinishCallback()
    {
        m_player.ChangeEndState();
        m_camera.Priority = 100;
    }
}
