using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

    public Camera MainCamera;

    public GameObject MusicPlayerPrefab;

    private int current_screen_w;
    private int current_screen_h;
    // Use this for initialization
    void Start () {
        InitScreen();
        if (GameObject.FindGameObjectsWithTag("Music").Length == 0)
        {
            GameObject music_player = Instantiate(MusicPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DontDestroyOnLoad(music_player);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (current_screen_w != Screen.width | current_screen_h != Screen.height)
        {
            InitScreen();
        }
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("main");
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
