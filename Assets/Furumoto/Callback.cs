using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callback : MonoBehaviour
{
    [SerializeField]
    private Player m_player;

    [SerializeField]
    private Sprash _sprash;

    public void JumpCallback()
    {
        m_player.ChangeJumpState();
    }

    public void FinishCallback()
    {
        m_player.ChangeEndState();
        _sprash.SetEffect();
    }
}
