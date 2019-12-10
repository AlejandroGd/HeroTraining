using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    Vector3 camPos;

    private void Start()
    {        
        camPos = GameObject.Find("Main Camera").GetComponent<Camera>().transform.position;    
    }

    public void PlaySound()
    { 
        AudioSource.PlayClipAtPoint(clip, camPos);       
    }
}