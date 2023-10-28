using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    //Image _image;
    Material _material;
    float alpha;

    public bool _fadeIn;

    public bool _isFinished;

    [SerializeField] string _nextScene; 

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<Image>().material;
        //_image = GetComponent<Image>();
        _isFinished = false;
        if(_fadeIn)
		{
            alpha = 1.0f;
		}
		else
		{
            alpha = 0.0f;
		}
    }

    // Update is called once per frame
    void Update()
    {

        if(!_isFinished)
		{
            if(_fadeIn)
			{
                alpha -= 0.005f;

                if (alpha <= 0.0f)
                {
                    alpha = 0.0f;
                    SceneManager.LoadScene(_nextScene);
                }
            }
			else
			{
                alpha += 0.005f;

                if(alpha >= 1.0f)
				{
                    alpha = 1.0f;
                    _isFinished = true;
                    _fadeIn = true;
				}
			}
		}

        _material.SetFloat("_Float",alpha);
        //_image.color = _color;
    }

    public void StartFadeIn()
	{
        _isFinished = false;
	}
}
