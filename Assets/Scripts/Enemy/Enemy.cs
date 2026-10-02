using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    enum MoveType
    {
        Forward,
        AvoidDrop,
        Patrol
    }

    [SerializeField] private MoveType moveType;
    [SerializeField] private float speed = 10;
    [SerializeField] private float gravity = -18f;
    [SerializeField] private bool facing_left;
    [SerializeField] private bool is_flying;
    [SerializeField] private float ground_check_dist = 0.15f;
    [SerializeField] private float pit_check_dist = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject kill_fX;
    [SerializeField] private Vector2[] patrol_pos;
    [SerializeField] private float distance_before_point_change = 0.2f;

    private int target_patrol_point_index;
    private Vector2 move_dir;
    private Vector2 start_pos;


    private Rigidbody2D body;

    void Start()
    {
        // Récupère le rigidbody
        body = GetComponent<Rigidbody2D>();
        // Définit la startPosition (utilisée pour le mode patrol)
        start_pos = transform.position;
        if (moveType == MoveType.Patrol && patrol_pos.Length <=1)
        {
            // Si le movetype est patrol et qu'il y a moins de 2 positions, envoie un warning et passe le movetype en avoid drop pour éviter les erreurs
            Debug.LogWarning("Not engouth patrol point to be in patrol point switching to Avoid Drop",this);
            moveType = MoveType.AvoidDrop;
        }
    }

    void FixedUpdate()
    {
        CheckWall();

        // Si le movetype est avoidDrop, vérifie si l'ennemi se dirige vers un trou
        if (moveType == MoveType.AvoidDrop)
        {
            CheckPit();
        }

        // Récupère la vélocité précédente
        move_dir = body.linearVelocity;

        if( moveType == MoveType.Patrol) // Logique de mouvement pour le déplacement en patrouille
        {
            // Vérifie la distance à la target actuelle
            CheckDistanceToTarget();

            // Calcule la direction vers le prochain point
            Vector2 dir = patrol_pos[target_patrol_point_index] + start_pos - (Vector2) transform.position;

            // Tourne le sprite si la direction ne correspond pas à l'orientation du sprite
            if (facing_left != (dir.x < 0))
            {
                facing_left = !facing_left;
                sprite.flipX = !sprite.flipX;
            }

            // Applique la vitesse à la direction
            move_dir = dir.normalized * speed;
        }
        else
        {
            // Définit le mouvement horizontal en fonction de l'orientation de l'ennemi
            if (facing_left)
            {
                move_dir.x = -speed;
            }
            else
            {
                move_dir.x = speed;
            }    
        }

        if (!is_flying) // Si l'ennemi ne vole pas, applique la gravité
        {
            if(GroundCheck())
            {
                // Si l'ennemi touche le sol, définit sa vélocité verticale à la gravité pour le coller au sol
                move_dir.y = gravity;
            }
            else
            {
                // Si l'ennemi est dans les airs, augmente sa vitesse verticale avec la gravité
                move_dir.y += gravity * Time.fixedDeltaTime;
            }
        }

        // Applique la vélocité au rigidbody 
        body.linearVelocity = move_dir;

    }

    // Vérifie la distance de l'ennemi à la target actuelle
    void CheckDistanceToTarget()
    {
        if (Vector2.Distance(transform.position,patrol_pos[target_patrol_point_index] + start_pos) <= distance_before_point_change)
        {
            // Si la target est assez proche, passe à la target suivante dans la liste
            target_patrol_point_index = (target_patrol_point_index + 1 ) % patrol_pos.Length;
        }
    }

    // Vérifie si l'ennemi touche le sol
    bool GroundCheck()
    {
        // Vérifie s'il existe un collider au pied de l'ennemi
        Collider2D walkColider = Physics2D.OverlapCircle(transform.position,ground_check_dist,groundLayer);

        if (walkColider != null)
        {
            // S'il y a un collider, il touche le sol
            return true;
        }
        else
        {
            // Sinon l'ennemi est dans les airs
            return false;
        }
    }

    // Vérifie s'il y a un trou devant l'ennemi
    void CheckPit()
    {
        // Choisit la direction en fonction de l'orientation de l'ennemi
        Vector2 dir = facing_left? Vector2.left : Vector2.right; // Équivaut à écrire "if(facing_left) {dir = VEctor2.left} else {dir = Vector2.right}"

        // Fait un raycast vers le sol devant l'ennemi
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + dir * pit_check_dist, Vector2.down,1,groundLayer);

        if (!hit)
        {
            // S'il n'y a pas de collider, il y a un trou : tourne l'ennemi et flip le sprite renderer
            facing_left = !facing_left;
            sprite.flipX = !sprite.flipX;
        }
    }

    void CheckWall()
    {
        // Choisit la direction en fonction de l'orientation de l'ennemi
        Vector2 dir = facing_left? Vector2.left : Vector2.right;

        // Fait un raycast devant l'ennemi en fonction de sa direction
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + (Vector2.up * 0.2f), dir,1,groundLayer);

        if (hit)
        {
            // Si le raycast touche, il y a un mur : tourne l'ennemi et flip le sprite renderer
            facing_left = !facing_left;
            sprite.flipX = !sprite.flipX;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // L'objet qui est entré dans le trigger est le joueur

            // Récupère le script du joueur
            Player player = collision.GetComponent<Player>();
            // Vérifie s'il est au sol
            if (!player.grounded)
            {
                // S'il est dans les airs, il a écrasé l'ennemi
                // Tue l'ennemi
                Kill();
                // Ajoute une pièce et du score au player
                player.AddCoin();
                // Fait rebondir le joueur
                player.Bounce();
            }  
            else
            {
                // Le joueur n'était pas en saut, tue le joueur
                player.Kill();
            }
        }
    }


    void Kill()
    {
        // Si le prefab fx de mort existe, spawn une instance sur l'ennemi
        if (kill_fX)
        {
            Instantiate(kill_fX,transform.position + Vector3.up * 0.5f,Quaternion.identity);
        }

        // Détruit l'ennemi
        Destroy(gameObject);
    }

    // Fonction appelée par Unity dans l'éditeur pour afficher des gizmos supplémentaires
    void OnDrawGizmos()
    {
        // Si le movetype est patrol et qu'il y a au moins un point dans la liste de patrouille
        if (moveType == MoveType.Patrol && patrol_pos.Length > 0)
        {
            // Passe la couleur des gizmos en rouge
            Gizmos.color = Color.red;
            
            // Pour chaque point, dessine une sphère à la position
            for (int i = 0; i < patrol_pos.Length; i++)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3) patrol_pos[i],distance_before_point_change);
            }
        }
    }


}
