using DG.Tweening;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite_renderer;

    private PlayerController player_control;
    
    [Header("Fx")]
    [SerializeField] private GameObject landParticles;
    [SerializeField] private GameObject jump_particles;
    
    [Header("Sound")]
    [SerializeField] private AudioSource jump_audio_source;
    [SerializeField] private AudioSource step_audio_source;
    [SerializeField] private AudioSource death_audio_source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_control = GetComponent<PlayerController>();
    }

    void Update()
    {
        HandleAnimVariable();
    }

    void HandleAnimVariable()
    {

        // Si le SpriteRenderer existe, flip l'axe X en fonction de la direction du joueur
        if (sprite_renderer != null)
        {
            if (player_control.MoveDir.x > 0) sprite_renderer.flipX = false;
            if (player_control.MoveDir.x < 0) sprite_renderer.flipX = true;
        }

        if (animator == null) return;

        // Passe les valeurs du PlayerController à l'animator
        animator.SetBool("Grounded" , player_control.grounded);
        animator.SetBool("Moving", player_control.MoveDir.x != 0);
        animator.SetFloat("VSpeed", player_control.MoveDir.y);
    }

    public void JumpFx()
    {
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        // spawn particles
        if(jump_particles)
        {
            Instantiate(jump_particles,transform.position,Quaternion.identity);
        }

        // Squash and stretch
        sprite_renderer.transform.localScale = new Vector3(0.8f,1.2f,1);
        sprite_renderer.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.InOutElastic);
        
        // Audio
        if (jump_audio_source) jump_audio_source.Play();
    }

    public void LandFX()
    {
        // Spawn land particles
        Instantiate(landParticles,transform.position,Quaternion.identity);

        // Squash and stretch
        sprite_renderer.transform.localScale = new Vector3(1.2f,0.8f,1);
        sprite_renderer.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.InOutElastic);

        // Audio
        if(step_audio_source) step_audio_source.Play();
    }

    public void SetDeathAnim(bool value)
    {
        animator.SetBool("IsDead",value);
        if (value && death_audio_source) death_audio_source.Play();
    }
}
