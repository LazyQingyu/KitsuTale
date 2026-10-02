using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField] private string player_layer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(player_layer))
        {
            // Si l'objet qui entre en collision est sur le layer du joueur
            // Récupère le script Player et tue le joueur
            collision.GetComponent<Player>()?.Kill(); // "collision.GetComponent<Player>()?" équivaut à faire if(collision.GetComponent<Player>() != null) {...}
        }
    }
}
