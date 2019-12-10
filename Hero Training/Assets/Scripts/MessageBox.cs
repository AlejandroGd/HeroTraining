using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;    

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    public void UpdateMessage(string message)
    {
        this.message.text = message;
    }
}