using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [NonSerialized] public static GameManager current;

    [Header("Live Game Info")]
    // At the top so it's easy to track
    public bool gamePaused;
    [SerializeField] private int playerHealth = 1;
    [SerializeField] private int coinsCollected;
    public bool ballInDoor;

    // Keep these for other scripts to reference
    [NonSerialized] public CharacterMovement playerScript;
    [NonSerialized] public GameObject playerObject;

    [Header("Scene UI")]
    // UI for each scene
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject levelUI;

    [Header("Level UI Screens")]
    // Panels to show the game state in each level scene
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] GameObject pauseScreen;

    [Header("Level UI Elements")]
    [SerializeField] TextMeshProUGUI coinsCollectedText;
    //[SerializeField] TextMeshProUGUI playerHealthText;



    // Start is called before the first frame update
    void Awake()
    {
        if (current != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            current = this;
            DontDestroyOnLoad(gameObject);
        }
        SceneSetUp(SceneManager.GetActiveScene().buildIndex);
    }

    // Load the level
    public void EnterLevel()
    {
        SceneManager.LoadScene("Level_1");
    }

    // Enable correct UI for the next scene and locate player
    private void SceneSetUp(int i)
    {
        if(i == 0) // scene is main menu
        {
            menuUI.SetActive(true);
            levelUI.SetActive(false);
        }
        else // scene is a level
        {
            menuUI.SetActive(false);
            levelUI.SetActive(true);

            // find player
            playerObject = GameObject.FindGameObjectWithTag("Player");
            playerScript = playerObject.GetComponent<CharacterMovement>();

            // Set all UI screens in the level false
            winScreen.SetActive(false);
            loseScreen.SetActive(false);
            pauseScreen.SetActive(false);

            // coin amount resets each level
            coinsCollected = 0;
            RefreshLevelUI();
        }
    }

    // prepare scene 
    private void OnLevelWasLoaded()
    {
        Time.timeScale = 1;
        gamePaused = false;
        ballInDoor = false;

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            menuUI.SetActive(true);
            levelUI.SetActive(false);
        }
        else
        {
            menuUI.SetActive(false);
            levelUI.SetActive(true);

            // find player
            playerObject = GameObject.FindGameObjectWithTag("Player");
            playerScript = playerObject.GetComponent<CharacterMovement>();

            // Set all UI screens in the level false
            winScreen.SetActive(false);
            loseScreen.SetActive(false);
            pauseScreen.SetActive(false);

            // coin amount resets each level
            coinsCollected = 0;
            if (playerHealth < 1) playerHealth = 1;
            RefreshLevelUI();
        }
    }

    // Close the game
    public void Quit()
    {
        Application.Quit();
    }

    // Player has died
    public void LoseGame()
    {
        gamePaused = true;
        loseScreen.SetActive(true);
    }

    // Player has won the level
    public void WinGame()
    {
        gamePaused = true;
        winScreen.SetActive(true);
    }

    // back to main menu scene
    public void LoadMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Pause the game -- time won't move, pause screen is showing
    public void PauseGame()
    {
        if(!gamePaused)
        {
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            gamePaused = true;
        }
        else
        {
            Time.timeScale = 1.0f;
            pauseScreen.SetActive(false);
            gamePaused = false;
        }
    }

    // Start the level again
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CollectCoin()
    {
        coinsCollected++;
        // if player has collected a multiple of 5 coins, increase player health
        RefreshLevelUI();
    }

    public void RefreshLevelUI()
    {
        coinsCollectedText.text = coinsCollected.ToString();
        //playerHealthText.text = playerHealth.ToString();
    }
}
