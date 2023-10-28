using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callback : MonoBehaviour
{
    [SerializeField]
    private Player m_player;

    public void JumpCallback()
    {
        m_player.ChangeJumpState();
    }

    public void FinishCallback()
    {
        m_player.ChangeEndState();
    }
}
