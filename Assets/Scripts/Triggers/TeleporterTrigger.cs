using System.Linq.Expressions;
using UnityEngine;

public class TeleporterTrigger : MonoBehaviour
{
    Player player;
    public GameObject next_teleportation_point;
    void Start()
    {
        player = Level.current_level.player;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {   
        Debug.Log("Teleporter trigger");
        if (collision.CompareTag("Player"))
        {
            player.Teleport(next_teleportation_point.transform.position);
        }
    }


}
