using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<Player>(out _))
        {
            Player.Instance.UpdateRespawnPoint(transform.position);
            Debug.Log("Respawn Updated!");
            _animator.SetTrigger("RespawnActivated");
        }
    }
}
