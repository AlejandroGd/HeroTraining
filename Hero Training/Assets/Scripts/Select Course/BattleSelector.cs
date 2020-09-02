using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Deals with the functionality on the Battle selection screen.
public class BattleSelector : MonoBehaviour
{
    GameSession gameSession;
    MusicFxController audioPlayer;
       
    [SerializeField] Image prevImage;
    [SerializeField] Image selectedImage;
    [SerializeField] Image nextImage;

    [SerializeField] TextMeshProUGUI battleName;
    [SerializeField] TextMeshProUGUI battleDescription;

    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;

    int currentSelection;

    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        audioPlayer = FindObjectOfType<MusicFxController>();
        InitialiseSelector();
    }

    private void InitialiseSelector()
    {
        if (gameSession.AvailableBattles.Count < 0) return;

        currentSelection = 0;
        UpdateSelection();
    }

    private void UpdateSelection()
    {
        if (gameSession.AvailableBattles.Count < 0) return;

        //Images
        if (currentSelection == 0)
        {
            prevButton.enabled = false;
            prevImage.sprite = null;
        }
        else
        {
            prevButton.enabled = true;
            prevImage.sprite = gameSession.AvailableBattles[currentSelection - 1].BattleSelectionImage;           
        }

        selectedImage.sprite = gameSession.AvailableBattles[currentSelection].BattleSelectionImage;

        if (currentSelection == gameSession.AvailableBattles.Count - 1)
        {
            nextButton.enabled = false;
            nextImage.sprite = null;
        }
        else
        {
            nextButton.enabled = true;
            nextImage.sprite = gameSession.AvailableBattles[currentSelection + 1].BattleSelectionImage;
        }
        

        //Text
        battleName.text = gameSession.AvailableBattles[currentSelection].CourseName;
        battleDescription.text = gameSession.AvailableBattles[currentSelection].CourseDescription;
    }

    public void SelectNextBattle()
    {
        if (currentSelection < gameSession.AvailableBattles.Count - 1)
        {
            audioPlayer.PlayScrollButtonFx();
            currentSelection++;
        }
        UpdateSelection();
    }

    public void SelectPreviousBattle()
    {
        if (currentSelection > 0)
        {
            audioPlayer.PlayScrollButtonFx();
            currentSelection--;
        }
        UpdateSelection();
    }

    public int GetSelectedBattleCourseIndex() { return currentSelection; }
}