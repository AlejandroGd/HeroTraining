using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Code taken from the YouTube video ‘How to fade Between Scenes in Unity’ by Brackeys (2014). Available at: https://www.youtube.com/watch?v=0HwZQt94uHQ */

public class Fading : MonoBehaviour
{
    [SerializeField] Texture2D fadeOutTexture;
    [SerializeField] float fadeSpeed = 0.0f;


    private float alpha = 1f;
    private int fadeDir = -1;

    private void OnGUI()
    {
        alpha += fadeDir * fadeSpeed + Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return fadeSpeed;
    }

    private void OnLevelWasLoaded(int level)
    {
        BeginFade(-1);
    }    
}