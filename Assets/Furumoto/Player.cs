using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Cinemachine;

public class Player : MonoBehaviour
{
    public enum State
    {
        Skiing = 0,
        Jump,
        End,

        Length,
    }


    [SerializeField]
    private Animator m_animator;

    // 時間経過による加速
    [SerializeField]
    private float m_skiiTimeAccel = 0.2f;

    // 移動の移動加速度の加速
    [SerializeField]
    private float m_slideAccelSpeed = 0.5f;

    // 左右移動の最高加速度
    [SerializeField]
    private float m_slideSpeedMax = 1.0f;

    // 現在左右に移動している方向と逆方向に入力した場合の減速度
    [SerializeField]
    private float m_reverseSlideDecaySpeed = 4.0f;

    [SerializeField]
    private CinemachineVirtualCamera m_jumpCamera;

    private State m_state = State.Skiing;

    private Action[] m_action;

    // 重量当たりの加速
    private float m_skiiMassAccel = 0.1f;

    // 現在の付着物の合計重量
    private float m_depositsMass = 0.0f;

    // 滑る速度
    private float m_skiiSpeed = 0.0f;

    // 左右移動の加速度
    private Vector2 m_slideAccel = Vector2.zero;

    // 左右移動の速度
    private Vector2 m_slidePosition = Vector2.zero;

    private InputAction m_moveInput_Left;
    private InputAction m_moveInput_Right;
    private InputAction m_moveInput_Front;
    private InputAction m_moveInput_Back;
    private int m_moveInputValue_Left;
    private int m_moveInputValue_Right;
    private int m_moveInputValue_Front;
    private int m_moveInputValue_Back;


    private void Start()
    {
        m_action = new Action[(int)State.Length] { SkiingAction, JumpAction, EndAction };

        m_animator.SetFloat("Speed", m_skiiSpeed);

        m_moveInput_Left = new InputAction(binding: "<Keyboard>/a");
        m_moveInput_Left.performed += ctx => m_moveInputValue_Left = (int)ctx.ReadValue<float>();
        m_moveInput_Left.canceled += ctx => m_moveInputValue_Left = 0;
        m_moveInput_Left.Enable();
        m_moveInput_Right = new InputAction(binding: "<Keyboard>/d");
        m_moveInput_Right.performed += ctx => m_moveInputValue_Right = (int)ctx.ReadValue<float>();
        m_moveInput_Right.canceled += ctx => m_moveInputValue_Right = 0;
        m_moveInput_Right.Enable();
        m_moveInput_Front = new InputAction(binding: "<Keyboard>/w");
        m_moveInput_Front.performed += ctx => m_moveInputValue_Front = (int)ctx.ReadValue<float>();
        m_moveInput_Front.canceled += ctx => m_moveInputValue_Front = 0;
        m_moveInput_Front.Enable();
        m_moveInput_Back = new InputAction(binding: "<Keyboard>/s");
        m_moveInput_Back.performed += ctx => m_moveInputValue_Back = (int)ctx.ReadValue<float>();
        m_moveInput_Back.canceled += ctx => m_moveInputValue_Back = 0;
        m_moveInput_Back.Enable();
    }

    private void Update()
    {
        m_action[(int)m_state]();
    }

    private void SkiingAction()
    {
        // 時間経過による加速
        m_skiiSpeed += m_skiiTimeAccel * Time.deltaTime;

        m_animator.SetFloat("Speed", m_skiiSpeed);
        m_animator.SetFloat("RotateSpeed", m_skiiSpeed * 2);

        // 左右移動
        float input = -m_moveInputValue_Left + m_moveInputValue_Right;
        float accel = m_slideAccelSpeed * input * Time.deltaTime;
        if (m_slideAccel.x > 0.0f && accel < 0.0f)
        {
            m_slideAccel -= new Vector2(m_reverseSlideDecaySpeed * Time.deltaTime, 0.0f);
        }
        else if (m_slideAccel.x < 0.0f && accel > 0.0f)
        {
            m_slideAccel += new Vector2(m_reverseSlideDecaySpeed * Time.deltaTime, 0.0f);
        }
        m_slideAccel.x += accel;
        m_slideAccel.x = Mathf.Clamp(m_slideAccel.x, -m_slideSpeedMax, m_slideSpeedMax);
        m_slidePosition += m_slideAccel * Time.deltaTime;
        if (m_slidePosition.x < -1.0f)
        {
            m_slidePosition = new Vector2(-1.0f, 0.0f);
            m_slideAccel = Vector2.zero;
        }
        else if (m_slidePosition.x > 1.0f)
        {
            m_slidePosition = new Vector2(1.0f, 0.0f);
            m_slideAccel = Vector2.zero;
        }
        m_animator.SetFloat("Slide", m_slidePosition.x);
    }

    private void JumpAction()
    {
        // 前後左右移動
        float input = -m_moveInputValue_Left + m_moveInputValue_Right;
        float accel = m_slideAccelSpeed * input * Time.deltaTime;
        if (m_slideAccel.x > 0.0f && accel < 0.0f)
        {
            m_slideAccel -= new Vector2(m_reverseSlideDecaySpeed * Time.deltaTime, 0.0f);
        }
        else if (m_slideAccel.x < 0.0f && accel > 0.0f)
        {
            m_slideAccel += new Vector2(m_reverseSlideDecaySpeed * Time.deltaTime, 0.0f);
        }
        m_slideAccel.x += accel;
        m_slideAccel.x = Mathf.Clamp(m_slideAccel.x, -m_slideSpeedMax, m_slideSpeedMax);


        input = m_moveInputValue_Front - m_moveInputValue_Back;
        accel = m_slideAccelSpeed * input * Time.deltaTime;
        if (m_slideAccel.y > 0.0f && accel < 0.0f)
        {
            m_slideAccel -= new Vector2(0.0f, m_reverseSlideDecaySpeed * Time.deltaTime);
        }
        else if (m_slideAccel.y < 0.0f && accel > 0.0f)
        {
            m_slideAccel += new Vector2(0.0f, m_reverseSlideDecaySpeed * Time.deltaTime);
        }
        m_slideAccel.y += accel;
        m_slideAccel.y = Mathf.Clamp(m_slideAccel.y, -m_slideSpeedMax, m_slideSpeedMax);


        m_slidePosition += m_slideAccel * Time.deltaTime;
        if (m_slidePosition.x < -1.0f)
        {
            m_slidePosition = new Vector2(-1.0f, m_slidePosition.y);
            m_slideAccel = new Vector2(0.0f, m_slideAccel.y);
        }
        else if (m_slidePosition.x > 1.0f)
        {
            m_slidePosition = new Vector2(1.0f, m_slidePosition.y);
            m_slideAccel = new Vector2(0.0f, m_slideAccel.y);
        }
        if (m_slidePosition.y < -1.0f)
        {
            m_slidePosition = new Vector2(m_slidePosition.x, -1.0f);
            m_slideAccel = new Vector2(m_slideAccel.x, 0.0f);
        }
        else if (m_slidePosition.y > 1.0f)
        {
            m_slidePosition = new Vector2(m_slidePosition.x, 1.0f);
            m_slideAccel = new Vector2(m_slideAccel.x, 0.0f);
        }

        m_animator.SetFloat("Slide", m_slidePosition.x);
        m_animator.SetFloat("Around", m_slidePosition.y);
    }

    public void EndAction()
    {

    }

    // オブジェクトがくっつく処理
    public void Adhesion(float mass, Transform deposit)
    {
        m_depositsMass += mass;
        m_skiiSpeed += m_skiiMassAccel * mass;

        // 親子関係設定
        deposit.parent = transform.GetChild(1);
    }

    // オブジェクトが離れる処理
    public void Unadhesion(float mass, Transform deposit)
    {
        m_depositsMass -= mass;

        // 親子関係解除
        Destroy(deposit.gameObject);
    }

    public void ChangeJumpState()
    {
        m_state = State.Jump;

        m_slidePosition = Vector2.zero;

        m_animator.SetFloat("Height", Mathf.Clamp(m_skiiSpeed - 10.0f * 0.5f, -1.0f, 1.0f));
        m_animator.SetFloat("Slide", 0.0f);
        m_animator.SetFloat("Speed", 1.0f);

        m_jumpCamera.Priority = 5;
    }

    public void ChangeEndState()
    {
        m_state = State.End;

        m_animator.SetFloat("RotateSpeed", 0.0f);
        m_jumpCamera.Priority = 5;
    }
}
