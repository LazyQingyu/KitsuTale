using UnityEngine;
using UnityEngine.Rendering;

public class DebugCanvas : MonoBehaviour
{
    
    public void Restart()
    {
        GameManager.instance.ReloadScene();
    }

    public void NextLevel()
    {
        GameManager.instance.ChangeScene(Level.current_level.next_scene_index);
    }

    public void MainMenu()
    {
        GameManager.instance.ChangeScene(0);
    }

    public void CheckPoint()
    {
        Level.current_level.LoadCheckpoint();
    }
}
