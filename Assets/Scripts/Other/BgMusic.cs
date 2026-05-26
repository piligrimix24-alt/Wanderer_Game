using UnityEngine;

public class BgMusic : MonoBehaviour
{
    [SerializeField] private Collider2D area;
    [SerializeField] private GameObject player;

    private void Update()
    {
        Vector3 closestPoint = area.ClosestPoint(player.transform.position);
        transform.position = closestPoint;
    }
}
