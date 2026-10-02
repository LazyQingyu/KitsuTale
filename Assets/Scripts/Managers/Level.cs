using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level current_level;

    public int next_scene_index;
    [SerializeField] private GameObject pause_panel;
    [SerializeField] private AudioClip level_music;

    private Vector2 current_spawnpoint;
    public Player player;

    void Awake()
    {
        // Assigne ce niveau en tant que niveau actuel
        current_level = this;
        // Trouve le player du niveau
        player = FindAnyObjectByType<Player>();
    }

    void Start()
    {
        
        if (!player)
        {
            // S'il n'y a pas de player dans le niveau, envoie une erreur et désactive le script
            Debug.LogError("MISING PLAYER");
            enabled = false;
        }

        // Choisit la position du joueur comme premier checkpoint
        current_spawnpoint = player.transform.position;

        // Si le panel de pause existe, désactive le panel
        pause_panel?.SetActive(false);

        // Change la musique pour celle du niveau
        GameManager.instance.ChangeMusic(level_music);

    }

    public void RestartLevel()
    {
        GameManager.instance.ReloadScene();
    }

    public void NextLevel()
    {
        GameManager.instance.ChangeScene(next_scene_index);
    }

    // Pause le jeu et active le panel de pause
    public void PauseLevel()
    {
        GameManager.instance.PauseGame();
        pause_panel.SetActive(true);
    }

    // Téléporte le joueur à la position du checkpoint actuel
    public void LoadCheckpoint()
    {
        player.Teleport(current_spawnpoint);
    }

    // Change la position du checkpoint actuel
    public void NewCheckpoint(Vector2 newpos)
    {
        current_spawnpoint = newpos;
    }

}
