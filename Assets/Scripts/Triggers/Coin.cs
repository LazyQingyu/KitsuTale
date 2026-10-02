using Unity.Mathematics;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private string player_layer;
    [SerializeField] private GameObject pickup_Fx;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(player_layer))
        {
            // Si l'objet qui entre en collision est sur le layer du joueur
            // Récupère le script Player et lui ajoute une pièce
            collision.GetComponent<Player>()?.AddCoin();
            // Spawn le fx de pickup
            Instantiate(pickup_Fx,transform.position,quaternion.identity);
            // Détruit la pièce
            Destroy(gameObject);
        }
    }
}
