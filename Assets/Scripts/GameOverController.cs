using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {
    public Camera MainCamera;

    public Text Explanation;
    public Text ScoreLabel;
    public Text Score;
    public Text HighScoreLabel;
    public Text HighScore;
    public Text ResetText;

    public AudioSource SoundPlayer;

    public AudioClip Blip;
    public AudioClip NewHighScore;

    public int State = 0;
    public float StateTime = 0.75f;

    private int current_screen_w;
    private int current_screen_h;
    private float next_state_time;

    // Use this for initialization
    void Start () {
        InitScreen();
        next_state_time = Time.time + StateTime;
    }
	
	// Update is called once per frame
	void Update () {
        if (current_screen_w != Screen.width | current_screen_h != Screen.height)
        {
            InitScreen();
        }
        if (State < 7 & Time.time > next_state_time)
        {
            switch (State)
            {
                case 0:
                    State++;
                    Explanation.gameObject.SetActive(true);
                    SoundPlayer.PlayOneShot(Blip);
                    next_state_time = Time.time + StateTime;
                    break;
                case 1:
                    State++;
                    ScoreLabel.gameObject.SetActive(true);
                    SoundPlayer.PlayOneShot(Blip);
                    next_state_time = Time.time + StateTime;
                    break;
                case 2:
                    State++;
                    Score.text = string.Format("{0}", ScoreManager.Instance.Score);
                    Score.gameObject.SetActive(true);
                    SoundPlayer.PlayOneShot(Blip);
                    next_state_time = Time.time + StateTime;
                    break;
                case 3:
                    State++;
                    HighScoreLabel.gameObject.SetActive(true);
                    SoundPlayer.PlayOneShot(Blip);
                    next_state_time = Time.time + StateTime;
                    break;
                case 4:
                    State++;
                    if (PlayerPrefs.HasKey("highscore"))
                    {
                        HighScore.text = string.Format("{0}", PlayerPrefs.GetInt("highscore"));
                    } else
                    {
                        PlayerPrefs.SetInt("highscore", 0);
                        HighScore.text = "0";
                    }
                    HighScore.gameObject.SetActive(true);
                    SoundPlayer.PlayOneShot(Blip);
                    if (PlayerPrefs.GetInt("highscore") >= ScoreManager.Instance.Score)
                    {
                        State++;
                    }
                    next_state_time = Time.time + StateTime;
                    break;
                case 5:
                    State++;
                    HighScore.text = string.Format("{0}", ScoreManager.Instance.Score);
                    PlayerPrefs.SetInt("highscore", ScoreManager.Instance.Score);
                    SoundPlayer.PlayOneShot(NewHighScore);
                    next_state_time = Time.time + StateTime;
                    break;
                case 6:
                    State++;
                    ResetText.gameObject.SetActive(true);
                    SoundPlayer.PlayOneShot(Blip);
                    break;
            }
        }
        if (State == 7)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            } else if (Input.anyKeyDown)
            {
                ScoreManager.Instance.Reset();
                SceneManager.LoadScene("main");
            }
        }
    }

    void InitScreen()
    {
        current_screen_h = Screen.height;
        current_screen_w = Screen.width;
        float multiplier = current_screen_h / 480.0f;
        if (multiplier >= 1)
        {
            multiplier = Mathf.Floor(multiplier);
        }
        MainCamera.orthographicSize = (current_screen_h / multiplier) / 2;
        Debug.Log(current_screen_h);
        Debug.Log(multiplier);
    }
}
