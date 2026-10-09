using System.Linq.Expressions;
using UnityEngine;

public class TeleporterTrigger : MonoBehaviour
{
    Player player;
    void Start()
    {
        player = Level.current_level.player;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {   
        Debug.Log("Teleporter trigger");
        if (collision.CompareTag("Player"))
        {
            player.Teleport(new Vector2(-12.3f,-3.95f));
        }
    }


}
