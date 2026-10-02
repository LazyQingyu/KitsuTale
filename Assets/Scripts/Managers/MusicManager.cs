using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	[SerializeField] private AudioSource music_source_a;
	[SerializeField] private AudioSource music_source_b;
	[SerializeField] private float crossfade_duration = 1f;

	private AudioSource current_music_source;
	private Coroutine crossfade_coroutine;

	public void ChangeMusic(AudioClip new_music)
	{
		// S'il n'y a pas de nouvelle musique ou si la nouvelle musique est la même que l'ancienne, return
		if (new_music == null || (current_music_source != null && current_music_source.clip == new_music))
		{
			return;
		}

		// Choisit la prochaine source de musique
		AudioSource next_music_source = current_music_source == music_source_a
			? music_source_b
			: music_source_a;

		// S'il n'y a pas de nouvelle source, envoie une erreur et return
		if (next_music_source == null)
		{
			Debug.LogError("MusicManager requires two AudioSource references.", this);
			return;
		}

		// Arrête le fade actuel s'il y en a un
		if (crossfade_coroutine != null)
		{
			StopCoroutine(crossfade_coroutine);
		}

		// Passe l'audio source actuel en ancien audiosource
		AudioSource previous_music_source = current_music_source;
		
		// Set up le nouvel audio source
		next_music_source.clip = new_music;
		next_music_source.volume = 0f;
		next_music_source.loop = true;
		next_music_source.Play();
		
		// Passe le nouvel audio source en current
		current_music_source = next_music_source;

		// Commence et stocke la coroutine du fade
		crossfade_coroutine = StartCoroutine(Crossfade(previous_music_source, next_music_source));
	}

	private IEnumerator Crossfade(AudioSource previous_music_source, AudioSource next_music_source)
	{
		// Initialise les paramètres du timer
		float elapsed_time = 0f;
		float duration = Mathf.Max(0f, crossfade_duration);

		// Tant que le timer tourne
		while (elapsed_time < duration)
		{
			// Augmente le timer
			elapsed_time += Time.unscaledDeltaTime;

			// Récupère la progression actuelle en float entre 0 et 1
			float progress = duration == 0f ? 1f : elapsed_time / duration;

			// Baisse progressivement le volume de la source audio précédente en fonction de la progression
			if (previous_music_source != null)
			{
				previous_music_source.volume = 1f - progress;
			}

			// Augmente progressivement le volume de la nouvelle source audio en fonction de la progression
			next_music_source.volume = progress;
			yield return null;
		}

		// Stoppe l'audio source précédent et met son volume à 0
		if (previous_music_source != null)
		{
			previous_music_source.Stop();
			previous_music_source.volume = 0f;
		}

		// S'assure que la nouvelle audio source est bien au maximum de volume
		next_music_source.volume = 1f;

		// Clear la coroutine
		crossfade_coroutine = null;
	}
}
