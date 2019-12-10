using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PunishmentButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Image panelImage;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        panelImage = gameObject.GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        panelImage.color = new Color(0.95f, 0.2f, 0.2f); //Red-ish;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        panelImage.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //This happens only once per battle, no overhead if FindObjectOfType is used here.
        FindObjectOfType<BattleManagerStateMachine>().ShouldProcessPunishment = true;
        gameObject.SetActive(false);
    }
}