using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SceneTransition screen_transition;
    public SoundManager sound_manager;
    public MusicManager music_manager;
    public int scoreSaved;
    public int lifeSaved;

    void Awake()
    {
        if (!instance)
        {
            // S'il n'existe pas d'instance du GameManager, ce script devient l'instance
            instance = this;
            // Empêche le gameobject de se détruire lors du changement de scène
            DontDestroyOnLoad(this);
        }
        else
        {
            // Si une instance existe déjà, envoie un warning dans la console et désactive le gameobject pour éviter les erreurs
            Debug.LogWarning("Two or more Gamemanager in the scene! Disabling new one", this);
            gameObject.SetActive(false);
        }
        lifeSaved = 3;
    }

    void Start()
    {
        Player.isDead += SaveLife;
    }

    void SaveLife(int life)
    {
        lifeSaved = life;
    }
    // Change la scène en fonction du build index
    public void ChangeScene(int build_index)
    {
        // Reprend le jeu (au cas où le jeu était en pause)
        ResumeGame();
        StartCoroutine(LoadScene(build_index));
    }

    // Change la scène en fonction du nom de la scène
    public void ChangeScene(string scene_name) // Avoir deux fonctions avec le même nom mais des paramètres différents permet de créer des variations dans l'utilisation des fonctions
    {

        // Vérifie si le string est vide ou null
        if (string.IsNullOrEmpty(scene_name))
        {
            // Affiche une erreur et quitte la fonction si le string n'est pas valide
            Debug.LogError("Invalid scene name");
            return;
        }
        // Récupère l'index de la scène en fonction du nom
        int scene_index =SceneManager.GetSceneByName(scene_name).buildIndex;
        // Appelle le changement de scène avec le scene index récupéré
        ChangeScene(scene_index);
    }

    public void ReloadScene()
    {
        // Appelle le changement de scène avec l'index de la scène actuelle
        ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ResetPlayerState()
    {
        Level.current_level.player.SetLife();
        Level.current_level.player.SetScore();
    }
    public void GameOver()
    {
        ChangeScene(1);
        lifeSaved = 3;
        scoreSaved = 0;
        ResetPlayerState();
    }
    private IEnumerator LoadScene(int scene_index)
    {
        // Affiche l'écran de transition
        screen_transition.Show();

        // Attend que l'écran s'affiche
        yield return new WaitForSecondsRealtime(screen_transition.transition_duration);

        // Attend que la scène se charge
        yield return SceneManager.LoadSceneAsync(scene_index);

        // Cache l'écran de transition
        screen_transition.Hide();

    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        // #if UNITY_EDITOR va s'exécuter uniquement si le jeu est lancé dans l'éditeur; ce morceau de code sera ignoré automatiquement lors du build
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Enlève le play mode
        #endif

        // Ferme le jeu
        Application.Quit();
    }


    public void ChangeMusic(AudioClip music)
    {
        if (music_manager != null)
        {
            // Si le music manager existe, appelle la fonction pour changer la musique
            music_manager.ChangeMusic(music);
        }
    }
}
