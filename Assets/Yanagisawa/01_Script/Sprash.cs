using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class Sprash : MonoBehaviour
{               
    public float _finishScaleY;

    [SerializeField]
    float _duration = 1.0f;

    [SerializeField]
    private Ease _ease;

    [SerializeField]
    private Vector3 _offset = new Vector3(0.0f, -0.65f, 0.0f);

    [SerializeField]
    VisualEffect _splashEffect;

    // Start is called before the first frame update
    void Start()
    {        
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetEffect(Vector3 position,float finishScaleY)
    {
        transform.position = position + _offset;
        _finishScaleY = finishScaleY;
        SetEffect();
    }

    // ボタン用（テスト）なので使わないでね
    public void SetEffect()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScaleY(_finishScaleY, _duration).SetEase(_ease).OnComplete(() => { ScreenCapture.CaptureScreenshot("Assets/screenshot.png"); }));
        sequence.Append(transform.DOScaleY(1.0f, _duration)).SetEase(_ease);
        _splashEffect.SendEvent("OnPlay");
    }

    public void OnStart()
    {
        _splashEffect.SendEvent("OnPlay");
    }

    public void OnStop()
    {
        _splashEffect.SendEvent("OnStop");
    }
}
