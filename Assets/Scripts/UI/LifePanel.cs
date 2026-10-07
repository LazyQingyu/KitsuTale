using UnityEngine;
using System.Collections.Generic;



public class LifePanel : MonoBehaviour
{
    public List<RectTransform> listLifeIcon;
    void Start()
    {
        Player.isDead += UpdateLife ;
        InitializedLife();
    }

    void OnDestroy()
    {
        Player.isDead -= UpdateLife;
    }
    void InitializedLife()
    {
        UpdateLife(GameManager.instance.lifeSaved);
    }
    public void UpdateLife( int life)
    {
        int currentLife = life;
        foreach(RectTransform lifeIcon in listLifeIcon)
        {
            if(currentLife > 0)
            {
                lifeIcon.gameObject.SetActive(true);
                currentLife--;
            }
            else
            {
                lifeIcon.gameObject.SetActive(false);
            }
        }
    }
}
