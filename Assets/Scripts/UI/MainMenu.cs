using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneTransition.SwitchToScene("MainScene");
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}
