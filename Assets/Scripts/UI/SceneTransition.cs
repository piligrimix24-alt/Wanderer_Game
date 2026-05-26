using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    private static bool shouldPlayAnimation = true;

    private Animator _animator;
    private AsyncOperation loadingSceneOperation;
    void Awake()
    {
        Instance = this;
        _animator = GetComponent<Animator>();
        if (shouldPlayAnimation) _animator.SetTrigger("SceneEnd");
        //_animator.SetTrigger("SceneEnd");
    }
    public static void SwitchToScene(string sceneName)
    {
        Instance._animator.SetTrigger("SceneStart");
        Instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        Instance.loadingSceneOperation.allowSceneActivation = false;
    }
    public void OnAnimationOver()
    {
        shouldPlayAnimation = true;
        loadingSceneOperation.allowSceneActivation = true;
    }
}
