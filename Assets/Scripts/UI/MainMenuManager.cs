using UnityEngine;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject main_memu_panel;
    [SerializeField] private GameObject credit_panel;
    [SerializeField] private GameObject setting_panel;
    [SerializeField] private AudioClip menu_music;
    [SerializeField] private int start_level_index;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // S'assure que tous les panels sont désactivés
        ClosePanels();
        // Active le main panel
        main_memu_panel.SetActive(true);
        // Change la musique pour celle du main menu
        GameManager.instance.ChangeMusic(menu_music);
    }

    public void OnPlayPessed()
    {
        // Charge la première scène
        GameManager.instance.ChangeScene(start_level_index);
    }

    public void OnSettingPressed()
    {
        ClosePanels();
        setting_panel.SetActive(true);
    }

    public void OnCreditPressed()
    {
        ClosePanels();
        credit_panel.SetActive(true);
    }

    private void ClosePanels()
    {   
        setting_panel.SetActive(false);
        credit_panel.SetActive(false);
    }

    public void OnQuitPressed()
    {
        GameManager.instance.QuitGame();
    }
}
