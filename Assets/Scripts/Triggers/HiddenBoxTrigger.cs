using System;
using UnityEngine;

public class HiddenBoxTrigger : MonoBehaviour
{
    [SerializeField] private string player_layer;
    [SerializeField] private SpriteRenderer hiddenBox;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(player_layer))
        {
            // Si l'objet qui entre en collision est sur le layer du joueur
            // Récupère le script Player et tue le joueur
            hiddenBox.enabled = true; // "collision.GetComponent<Player>()?" équivaut à faire if(collision.GetComponent<Player>() != null) {...}
        }
    }
}
