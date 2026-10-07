using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private int life;
    private const int LIFEMAXIMUM = 5;
    private PlayerController _player_control;
    private PlayerAnim _player_anim;
    public UnityEvent<int> OnScoreChanged;

    public delegate void DelegateIsDead(int life);
    public static DelegateIsDead isDead;

    public bool grounded {get{return _player_control.grounded;}}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Récupère les scripts du player
        _player_control = GetComponent<PlayerController>();
        _player_anim = GetComponent<PlayerAnim>();
        score = GameManager.instance.scoreSaved;
        life = GameManager.instance.lifeSaved;
    }

    public void Kill()
    {
        // Joue l'animation de mort
        _player_control.SetFreeze(true);
        _player_anim.SetDeathAnim(true);
        life--;
        isDead?.Invoke(life);
        if(life < 0)
        {
            StartCoroutine(GameOVerRespawn());
        }
        else
        {
            StartCoroutine(Respawn()); 
        }

    }

    public void SetScore()
    {
        score = 0;
    }
    public int GetScore()
    {
        return score;
    }

    public void SetLife()
    {
        life = 3;
    }
    public int GetLife()
    {
        return life;
    }

    public void AddCoin()
    {
        score += 100;
        OnScoreChanged?.Invoke(score);
    }

    public void AddLife()
    {
        if (life < LIFEMAXIMUM)
        {
            life++;
        }
    }

    public void Bounce()
    {
        _player_control.ForceJump();
    }

    public void OnPauseInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Level.current_level.PauseLevel();
        }
    }


    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.5f);
        
        GameManager.instance.screen_transition.Show();
        yield return new WaitForSeconds(GameManager.instance.screen_transition.transition_duration); // Attend que l'écran termine son fade in
        
        // Charge le checkpoint
        Level.current_level.LoadCheckpoint();

        // Arrête l'animation de mort
        _player_anim.SetDeathAnim(false);


        GameManager.instance.screen_transition.Hide();
        yield return new WaitForSeconds(GameManager.instance.screen_transition.transition_duration); // Attend que l'écran termine son fade in
        

        // Réactive le contrôle
        _player_control.SetFreeze(false);
    }

       public IEnumerator GameOVerRespawn()
    {
        yield return new WaitForSeconds(0.5f);
        
        GameManager.instance.screen_transition.Show();
        yield return new WaitForSeconds(GameManager.instance.screen_transition.transition_duration); // Attend que l'écran termine son fade in
        
        // Charge le checkpoint
        GameManager.instance.GameOver();

    }

    public void Teleport(Vector2 position)
    {
        transform.position = position;
    }
}
