using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public AudioSource getStar;

    public enum time
    {
        gold,
        silver,
        bronze,
        none
    }

    private int stars = 0;
    private int collectedStars = 0;
    public float goldTime = 6f;
    public float silverTime = 9f;
    public float bronzeTime = 12f;

    public time timeMedal;

    private float timer = 0f;

    private bool playing = true;

    public bool Playing => playing;

    public TextMeshProUGUI timerText;

    public Canvas endScreen;

    public Image star1;
    public Image star2;
    public Image star3;

    private Color clearYellow = new Color(230f, 255f, 0f, 30f);

    private void Awake()
    {
        // set up colors
        clearYellow.a = star1.color.a;
        star1.color = Color.green;
        star2.color = Color.green;
        star3.color = Color.green;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // update the timer text
        timerText.SetText(((((int)timer) / 60).ToString()) + ":" + ((((int)timer)) % 60).ToString("D2"));

        // update the stars UI
        if (playing)
        {
            if (stars == 2)
            {
                if (timer > bronzeTime + 1)
                {
                    star3.color = clearYellow;
                }
                else
                {
                    star3.color = Color.green;
                }
            }
            else if (stars == 1)
            {
                if (timer > silverTime + 1)
                {
                    star3.color = clearYellow;
                }
                else
                {
                    star3.color = Color.green;
                }
                if (timer > bronzeTime + 1)
                {
                    star2.color = clearYellow;
                }
                else
                {
                    star2.color = Color.green;
                }
            }
            else if (stars == 0)
            {
                if (timer > goldTime + 1)
                {
                    star3.color = clearYellow;
                }
                if (timer > silverTime + 1)
                {
                    star2.color = clearYellow;
                }
                if (timer > bronzeTime + 1)
                {
                    star1.color = clearYellow;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (playing)
        {
            timer += Time.deltaTime;
        }
    }

    public void AddStar()
    {
        stars++;
        if (stars == 1)
        {
            star1.color = Color.yellow;
        }
        else if (stars == 2)
        {
            star2.color = Color.yellow;
        }
        else if (stars == 3)
        {
            star3.color = Color.yellow;
        }
    }

    public int GetStars()
    {
        return stars;
    }

    public int GetCollectedStars()
    {
        return collectedStars;
    }

    public void Goal()
    {
        playing = false;
        collectedStars = stars;

        // figure out what time the player got (how many stars their time will give them)
        if (timer < goldTime + 1)
        {
            timeMedal = time.gold;
        }
        else if (timer < silverTime + 1)
        {
            timeMedal = time.silver;
        }
        else if (timer < bronzeTime + 1)
        {
            timeMedal = time.bronze;
        }
        else
        {
            timeMedal = time.none;
        }

        // add stars accordingly
        if (stars != 3)
        {
            if (timeMedal == time.gold)
            {
                stars = 3;
            }
            else if (timeMedal == time.silver)
            {
                stars += 2;
            }
            else if (timeMedal == time.bronze)
            {
                stars += 1;
            }
        }
        endScreen.gameObject.SetActive(true);
    }

    public void StarSound()
    {
        getStar.Play();
    }
}
