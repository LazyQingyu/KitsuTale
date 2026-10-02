using UnityEngine;
using UnityEngine.Rendering;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private GameObject setting_panel;

    void OnEnable()
    {
        // Désactive le panel de setting quand le panel de pause devient actif (évite de mettre le jeu en pause et de tomber directement sur le panel de setting)
        setting_panel.SetActive(false);
    }

    public void OnResume()
    {
        // Dit au GameManager de reprendre le jeu 
        GameManager.instance.ResumeGame();

        // Désactive le panel et le gameobject
        setting_panel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnRestart()
    {
        Level.current_level.RestartLevel();
    }

    public void OnSetting()
    {
        // Ouvre le panel de settings
        setting_panel.SetActive(true);
    }

    public void OnMainmenu()
    {
        GameManager.instance.ChangeScene(0);
    }

    public void OnQuit()
    {
        GameManager.instance.QuitGame();
    }
}
