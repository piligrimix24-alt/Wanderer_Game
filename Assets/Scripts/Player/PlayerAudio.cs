using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip respawnSound;

    private bool _hasPlayedDeathSound = false;
    private void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerTakeHit += OnPlayerTakeHit;
            Player.Instance.OnPlayerDeath += OnPlayerDeath;
            Player.Instance.OnPlayerRespawn += OnPlayerRespawn;
        }
    }
    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerTakeHit -= OnPlayerTakeHit;
            Player.Instance.OnPlayerDeath -= OnPlayerDeath;
            Player.Instance.OnPlayerRespawn -= OnPlayerRespawn;
        }
    }
    //================
    private void OnPlayerTakeHit(object sender, System.EventArgs e)
    {
        if (takeDamageSound != null)
            AudioManager.Instance.PlaySFX(takeDamageSound, 1.1f);
    }
    private void OnPlayerDeath(object sender, System.EventArgs e)
    {
        if (!_hasPlayedDeathSound && deathSound != null)
        {
            _hasPlayedDeathSound = true;
            AudioManager.Instance.PlaySFX(deathSound, 0.5f);
        }
    }

    private void OnPlayerRespawn(object sender, System.EventArgs e)
    {
        _hasPlayedDeathSound = false;
        if (respawnSound != null)
            AudioManager.Instance.PlaySFX(respawnSound, 0.5f);
    }

}