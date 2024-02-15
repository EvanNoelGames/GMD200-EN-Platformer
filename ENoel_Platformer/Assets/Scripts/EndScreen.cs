using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public PlayerManager playerManager;

    public Image star1;
    public Image star2;
    public Image star3;

    public GameObject retryButton, continueButton, lastSelectedButton;

    public string nextLevel;

    private TextMeshProUGUI star1Text;
    private TextMeshProUGUI star2Text;
    private TextMeshProUGUI star3Text;

    private void Awake()
    {
        star1Text = star1.GetComponentInChildren<TextMeshProUGUI>();
        star2Text = star2.GetComponentInChildren<TextMeshProUGUI>();
        star3Text = star3.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        // do not let the mouse deselect a menu icon
        if (EventSystem.current.currentSelectedGameObject != null && GetComponent<Canvas>().enabled)
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }

        if (EventSystem.current.currentSelectedGameObject == null && GetComponent<Canvas>().enabled)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }


        if (playerManager.GetStars() != 3)
        {
            continueButton.SetActive(false);
        }

        // figure out what color to display each star, and what the text under each star should say
        if (playerManager.GetCollectedStars() == 3)
        {
            star1Text.gameObject.SetActive(false);
            star2Text.gameObject.SetActive(false);
            star3Text.gameObject.SetActive(false);

            star1.color = Color.yellow;
            star2.color = Color.yellow;
            star3.color = Color.yellow;
        }
        else if (playerManager.GetCollectedStars() == 2)
        {
            star1Text.gameObject.SetActive(false);
            star2Text.gameObject.SetActive(false);
            star3Text.SetText(((((int)playerManager.bronzeTime) / 60).ToString()) + ":" + ((((int)playerManager.bronzeTime)) % 60).ToString("D2"));

            star1.color = Color.yellow;
            star2.color = Color.yellow;
            if (playerManager.timeMedal == PlayerManager.time.gold || playerManager.timeMedal == PlayerManager.time.silver || playerManager.timeMedal == PlayerManager.time.bronze)
            {
                star3.color = Color.green;
            }
        }
        else if (playerManager.GetCollectedStars() == 1)
        {
            star1Text.gameObject.SetActive(false);
            star2Text.SetText(((((int)playerManager.bronzeTime) / 60).ToString()) + ":" + ((((int)playerManager.bronzeTime)) % 60).ToString("D2"));
            star3Text.SetText(((((int)playerManager.silverTime) / 60).ToString()) + ":" + ((((int)playerManager.silverTime)) % 60).ToString("D2"));

            star1.color = Color.yellow;
            if (playerManager.timeMedal == PlayerManager.time.gold || playerManager.timeMedal == PlayerManager.time.silver)
            {
                star2.color = Color.green;
                star3.color = Color.green;
            }
            else if (playerManager.timeMedal == PlayerManager.time.bronze)
            {
                star2.color = Color.green;
            }
        }
        else if (playerManager.GetCollectedStars() == 0)
        {
            star3Text.SetText(((((int)playerManager.goldTime) / 60).ToString()) + ":" + ((((int)playerManager.goldTime)) % 60).ToString("D2"));
            star2Text.SetText(((((int)playerManager.silverTime) / 60).ToString()) + ":" + ((((int)playerManager.silverTime)) % 60).ToString("D2"));
            star1Text.SetText(((((int)playerManager.bronzeTime) / 60).ToString()) + ":" + ((((int)playerManager.bronzeTime)) % 60).ToString("D2"));
            if (playerManager.timeMedal == PlayerManager.time.gold)
            {
                star1.color = Color.green;
                star2.color = Color.green;
                star3.color = Color.green;
            }
            else if (playerManager.timeMedal == PlayerManager.time.silver)
            {
                star1.color = Color.green;
                star2.color = Color.green;
            }
            else if (playerManager.timeMedal == PlayerManager.time.bronze)
            {
                star1.color = Color.green;
            }
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Next()
    {
        SceneManager.LoadScene(nextLevel);
    }
}
