using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool _gameIsPaused = false;
    private PlayerInputActions _playerInputActions;
    public GameObject _pauseMenuUI;
    private bool _isInitialized;
    //==============================================================
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }
    private void Start()
    {
        _playerInputActions.UI.Menu.performed += Menu_performed;
        _isInitialized = true;
        Resume();
    }
    private void OnDestroy()
    {
        if (_playerInputActions != null && _isInitialized)
        {
            _playerInputActions.UI.Menu.performed -= Menu_performed;
            _playerInputActions.Disable();
            _playerInputActions.Dispose();
            _playerInputActions = null;
        }
    }
    //==============================================================
    public void LoadMenu()
    {
        Debug.Log("Load");
        Time.timeScale = 1f;
        if (_playerInputActions != null)
        {
            _playerInputActions.Disable();
            _playerInputActions.Dispose();
            _playerInputActions = null;
        }
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
        _gameIsPaused = false;
    }
    //==============================================================
    private void Menu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_gameIsPaused)
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
        _gameIsPaused = true;
    }
}
