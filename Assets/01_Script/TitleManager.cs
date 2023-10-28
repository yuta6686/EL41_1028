using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject _startButton;
    [SerializeField] GameObject _endButton;

    [SerializeField] Fade _fade;

    [SerializeField] AudioSource _audio;
    [SerializeField] AudioClip _clip;

    public bool _isStartSelected;
    float time;

    bool _sAnim;
    bool _eAnim;

    // Start is called before the first frame update
    void Start()
    {
        _isStartSelected = true;
        time = 0.0f;

        _sAnim = false;
        _eAnim = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
		{
            _isStartSelected = false;
            time = 0.0f;

            _eAnim = true;
            _sAnim = false;
            _startButton.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

            _audio.PlayOneShot(_clip);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
            _isStartSelected = true;
            time = 0.0f;

            _sAnim = true;
            _eAnim = false;
            _endButton.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

            _audio.PlayOneShot(_clip);
        }

        if(Input.GetKeyDown(KeyCode.Return))
		{
            if(_isStartSelected)
			{
                _fade.StartFadeIn();
            }
			else
			{
                
			}
		}

        ButtonAnimation();

        SelectedAnimation();

        time+=0.01f;
    }

    void ButtonAnimation()
	{
        if(_isStartSelected)
		{
            _startButton.transform.localScale = new Vector3(4.0f,4.0f,4.0f);
            _startButton.transform.localPosition = new Vector3(_startButton.transform.position.x,Mathf.Sin(time), _startButton.transform.position.z);
            
            
            _endButton.transform.localScale = new Vector3(3.0f,3.0f,3.0f);
            _endButton.transform.localPosition = new Vector3(_endButton.transform.position.x, 0.0f, _endButton.transform.position.z);

        }
		else
		{
            _startButton.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            _startButton.transform.localPosition = new Vector3(_startButton.transform.position.x, 0.0f, _startButton.transform.position.z);

            _endButton.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);
            _endButton.transform.localPosition = new Vector3(_endButton.transform.position.x, Mathf.Sin(time), _endButton.transform.position.z);
        }
	}

    void SelectedAnimation()
	{
        if(_isStartSelected && _sAnim)
		{
            _startButton.transform.Rotate(Mathf.Abs(Mathf.Sign(time * 2)) * 4,0.0f,0.0f);
            if(time >= 1.57f)
			{
                _sAnim = false;
                _startButton.transform.rotation = new Quaternion(0.0f,0.0f,0.0f,0.0f);
			}
		}
        else if(!_isStartSelected && _eAnim)
		{
            _endButton.transform.Rotate(Mathf.Abs(Mathf.Sign(time * 2)) * 4, 0.0f, 0.0f);
            if (time >= 1.57f)
            {
                _eAnim = false;
                _endButton.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }
	}
}
