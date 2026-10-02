using UnityEngine;

public class CreditPanelManager : MonoBehaviour
{
    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
}
