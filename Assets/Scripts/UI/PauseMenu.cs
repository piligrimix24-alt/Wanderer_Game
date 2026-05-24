using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool _gameIsOaused = false;
    private PlayerInputActions _playerInputActions;
    public GameObject _pauseMenuUI;
    //==============================================================
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }
    void Update()
    {
        _playerInputActions.UI.Menu.performed += Menu_performed;
    }
    //==============================================================
    public void LoadMenu()
    {
        Debug.Log("Load");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    public void Resume()
    {
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _gameIsOaused = false;
    }
    //==============================================================
    private void Menu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_gameIsOaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    private void Pause()
    {
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _gameIsOaused = true;
    }

}
