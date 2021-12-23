using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameStates {
    Active,
    Over,
    Victory
}

public class gameController : MonoBehaviour {
    public GameStates GameState;
    [Header("Minimap")]
    public GameObject Minimap;
    [Header("Game Start Variables")]
    public Text StartCounter;
    [SerializeField] int startWindow = 3; // time at the start before sand rises; in seconds
    [Header("Variables")]
    public int TimeLimit = 60; // in seconds
    public GameObject[] Levels = new GameObject[1];
    [Header("User Interface")]
    public GameObject GameOverUI;
    public GameObject GameVictoryUI;

    private int currentLevel;
    private float startCount; // in seconds
    private float time = 0f; // in seconds
    private float progress = 0f;
    
    # region Setters and Getters
    public float Progress {
        set {progress = value;} get {return progress;}
    }
    # endregion

    private void InitializeGame(int selection) {
        // make current level active in the game scene
        for (int i = 0; i < Levels.Length; i++)
            Levels[i].SetActive(false);
        Levels[selection].SetActive(true);
        // set state and reset timer
        time = 0f;
        GameState = GameStates.Active;

        startCount = startWindow;
        StartCounter.gameObject.SetActive(true);
    }

    void Start() {
        InitializeGame(levelSelection.Level);
    }

    private bool GameStartCount() {
        if (startCount > 0) {
            startCount -= 0.02f; // 50 * 0.02f = 1 second; update count
            
            // update count value; hide at 0
            if (startCount <= 0)
                StartCounter.gameObject.SetActive(false);
            else
                StartCounter.text = "" + Mathf.CeilToInt(startCount);

            // update visual of counter, zoom out
            StartCounter.fontSize = 180 + (int)((startCount - Mathf.FloorToInt(startCount)) * 100);

            return true;
        }
        else return false;
    }

    public void GameVictory() {
        GameState = GameStates.Victory;
    }

    public void GameOver() {
        GameState = GameStates.Over;
    }

    public void NextStage() {
        if (levelSelection.Level + 1 >= Levels.Length) {
            levelSelection.Level = 0; // return to first stage
            SceneManager.LoadScene("SampleScene");
        }
        else {
            levelSelection.Level += 1;
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("Title");
    }

    public void RetryStage() {
        SceneManager.LoadScene("SampleScene");
    }

    void FixedUpdate() {
        switch (GameState) {
        case GameStates.Active:
            // update that occurs 50 times a second
            if (time < TimeLimit + startWindow)
                time += 0.02f;

            if (time > startWindow)
                progress = ((time - startWindow) / TimeLimit) * 100;
            //Debug.Log(progress + "%");
            break;
        case GameStates.Over:
            break;
        case GameStates.Victory:
            break;
        }
    	
        // only show minimap when the game isn't counting down; only show minimap if the game is active
        if (!GameStartCount() && GameState == GameStates.Active)
            Minimap.SetActive(true);
        else
            Minimap.SetActive(false);
        // show appropriate UI
        if (GameState == GameStates.Over)
            GameOverUI.SetActive(true);
        else
            GameOverUI.SetActive(false);
        //
        if (GameState == GameStates.Victory)
            GameVictoryUI.SetActive(true);
        else
            GameVictoryUI.SetActive(false);
    }

    void Update() {
        if (GameState == GameStates.Active) {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene("Title");
        }
    }
}
