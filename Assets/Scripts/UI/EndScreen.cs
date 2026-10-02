using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public AudioClip end_screen_music;

    void Start()
    {
        GameManager.instance.ChangeMusic(end_screen_music);
    }
    public void Quit()
    {
        GameManager.instance.QuitGame();
    }

    public void MainMenu()
    {
        GameManager.instance.ChangeScene(0);
    }
}
