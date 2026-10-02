
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    Rigidbody2D body2D;
    
    [SerializeField] private float speed = 10f;
    [SerializeField] private float gravity = -18f;
    [SerializeField] private float ground_check_dist = 0.15f;
    [SerializeField] private float jump_force = 10f;
    [SerializeField] private float jump_gravity_multiplier = 0.8f;
    [SerializeField] private float fall_gravity_multiplier = 1.2f;
    [SerializeField] private float coyote_time_duration = 0.5f;
    [SerializeField] private float jump_buffer_time_duration = 1.2f;
    [SerializeField] private LayerMask groundLayer;
    
    private bool is_freeze = false;
    private float gravity_multiplier = 1f;
    private Vector2 move_input;
    private Vector2 move_dir;
    private bool jump_press;
    private bool jump_released;
    private bool jump_hold;
    private bool is_grounded;

    public bool grounded {get {return is_grounded;}}
    public Vector2 MoveDir {get {return move_dir;}}

    // Timers
    private float coyote_time_timer;
    private float jump_buffer_timer;

    public UnityEvent OnJump;
    public UnityEvent OnLand;

    void Awake()
    {
        // Récupère le script Player Input
        playerInput = GetComponent<PlayerInput>();

        // Si le script n'est pas sur le joueur, envoie une erreur et désactive le script
        if (playerInput == null)
        {
            Debug.LogError("Missing an Player Input Component",this);
            enabled = false;
        }

        // Désactive les actions de base et active les actions par défaut
        InputSystem.actions.Disable();
        playerInput.currentActionMap?.Enable();

        // Récupère le rigidbody 2D
        body2D = GetComponent<Rigidbody2D>();

    }

    void Start()
    {
        // Reset les timers et la valeur par défaut du multiplicateur de gravité
        gravity_multiplier = fall_gravity_multiplier;
        coyote_time_timer = -1;
        jump_buffer_timer = -1;
    }

    void FixedUpdate()
    {
        if (is_freeze) return;

        // Récupère la vélocité actuelle
        move_dir = body2D.linearVelocity;

        // Movement Horizontal
        move_dir.x = move_input.x * speed;

        HanddleJumpInput();

        // Vérifie si le joueur touche le sol
        is_grounded = Ground_check();

        // Si le joueur tombe et que le multiplicateur de gravité est différent de la chute, change le multiplicateur de gravité
        if (move_dir.y < 0 && gravity_multiplier != fall_gravity_multiplier)
        {
            gravity_multiplier = fall_gravity_multiplier;
        }
        
        // Applique la gravité
        if(is_grounded)
        {
            // Si le joueur est au sol, met la vélocité verticale à la gravité
            // (s'assure que le joueur est bloqué au sol et ne flotte pas légèrement au-dessus du sol)
            move_dir.y = gravity;
        }
        else
        {
            // Augmente la vélocité en fonction du temps, de la gravité et du multiplicateur de gravité
            move_dir.y += gravity * Time.fixedDeltaTime * gravity_multiplier;
        }

        // Applique la vélocité modifiée au rigidbody.
        body2D.linearVelocity = move_dir;

        // Réduit les timers s'ils sont supérieurs à zéro
        if (coyote_time_timer > 0)
        {
            coyote_time_timer -= Time.fixedDeltaTime;
        }

        if (jump_buffer_timer > 0)
        {
            jump_buffer_timer -= Time.fixedDeltaTime;
        }
    }

    bool Ground_check()
    {
        // Si le joueur est en train de monter (vélocité Y > 1), ignore le check et return false
        if (move_dir.y > 1f)
        {
            return false;
        }

        // Vérifie s'il existe un collider au pied du joueur
        Collider2D walkColider = Physics2D.OverlapCircle(transform.position,ground_check_dist,groundLayer);

        if (walkColider != null)
        {
            // Le joueur touche le sol

            if (!is_grounded)
            {
                // S'il ne touchait pas le sol à la frame précédente, le joueur vient d'atterrir
                OnLand?.Invoke();
            }

            // Met le multiplicateur de gravité à celui de chute
            gravity_multiplier = fall_gravity_multiplier;
            return true;
        }
        else
        {
            // Le joueur ne touche pas le sol
            if(is_grounded)
            {

                // S'il touchait le sol à la frame précédente, le joueur vient de quitter le sol
                // Commence le coyote time timer (Voir TDD pour explication du coyote time)
                coyote_time_timer = coyote_time_duration;
            }

            return false;
        }
    }

    void HanddleJumpInput()
    {
        
        if (jump_press)
        {
            // Le bouton de jump vient d'être appuyé

            if (is_grounded || coyote_time_timer > 0)
            {
                // Si le joueur touche le sol ou le coyote time timer est actif, saute.
                Jump();
            }
        }
        
        if (jump_buffer_timer <= 0)
        {
            // Si le jump buffer timer atteint 0, ne considère plus que le bouton de saut a été appuyé.
            jump_press = false;
        }

        if (jump_released && move_dir.y > 0 )
        {
            // Si le bouton de saut est relâché et que le joueur monte, met le multiplicateur de gravité à celui de chute
            gravity_multiplier = fall_gravity_multiplier;
        }

        // Ne considère plus que le bouton de saut a été relâché cette frame.
        jump_released = false;
    }

    

    public void Jump()
    {
        // Ajoute la force du saut à la vélocité verticale
        move_dir.y = jump_force;

        // Change le multiplicateur de gravité à celui de saut
        gravity_multiplier = jump_gravity_multiplier;
        
        // Arrête les timers
        coyote_time_timer = -1;
        jump_buffer_timer = -1;

        // Appelle les fonctions de saut
        OnJump?.Invoke();
        
    }

    

    // Permet de forcer un saut même si le perso ne touche pas le sol.
    public void ForceJump()
    {
        // Change la vélocité du rigidbody
        body2D.linearVelocityY = jump_force;
        
        // Change la vélocité du joueur (évite les problèmes si la fonction est appelée au milieu d'une frame)
        move_dir.y = jump_force;

        if (jump_hold)
        {
            // Si le bouton de saut est appuyé lors du Force Jump, change le multiplicateur de saut (permet de sauter plus haut)
            gravity_multiplier = jump_gravity_multiplier;
        }

        // Appelle les fonctions de saut
        OnJump?.Invoke();

    }

    
    // Permet de stopper le player et les input
    public void SetFreeze(bool value)
    {
        is_freeze = value;

        if (value)
        {
            // Stoppe le player et sa physique
            body2D.bodyType = RigidbodyType2D.Static;
            body2D.linearVelocity = Vector2.zero;
            // Stop input
            playerInput.enabled = false;
        }
        else
        {
            // Réactive la physique du joueur
            body2D.bodyType = RigidbodyType2D.Dynamic;
            // Réactive les inputs
            playerInput.enabled = true;
        }
    }

#region Inputs
    // Input de direction
    public void Move(InputAction.CallbackContext ctx)
    {
        move_input = ctx.ReadValue<Vector2>();
    }

    // Input de saut
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) // Si le saut vient d'être appuyé cette frame
        {
            jump_press = true;
            jump_hold = true;

            if (!is_grounded)
            {
                // Si le joueur n'est pas sur le sol, commence le timer de jump buffer (voir TDD)
                jump_buffer_timer = jump_buffer_time_duration;
            }
        }

        if (ctx.canceled) // Si le saut vient d'être relâché cette frame
        {
            jump_released = true;
            jump_hold = false;
        }
    }

#endregion

}
