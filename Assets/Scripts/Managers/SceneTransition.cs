using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransition : MonoBehaviour
{
    public bool is_showing = false;
    public CanvasGroup canvas_group;
    public float transition_duration = 0.5f;


    void Awake()
    {
        // Cache par défaut l'écran de transition en mettant son alpha à 0
        canvas_group.alpha = 0;
        
    }

    public void Show()
    {
        if (is_showing) return; // Si l'écran est déjà affiché, on ignore (évite de rejouer le fade in)

        // Utilise la librairie DOTween pour effectuer un fade graduel
        canvas_group.DOFade(1, transition_duration);
        // Garde en mémoire que l'écran est actif
        is_showing = true;
    }

    public void Hide()
    {
        if (!is_showing) return; // Si l'écran n'est pas affiché, on ignore (évite de rejouer le fade out)

        // Utilise la librairie DOTween pour effectuer un fade graduel
        canvas_group.DOFade(0,transition_duration);
        // Garde en mémoire que l'écran est inactif
        is_showing = false;
    }

}
