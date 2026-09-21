using System.Collections;
using UnityEngine;

/// <summary>
/// Joue des pistes d'ambiance au hasard, l'une après l'autre, avec fondu et pause entre elles.
/// À mettre sur un objet vide "Ambiance" (un Audio Source est ajouté automatiquement).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AmbianceRandom : MonoBehaviour
{
    [Header("Pistes")]
    [SerializeField] private AudioClip[] pistes;
    [SerializeField, Range(0f, 1f)] private float volume = 0.4f;

    [Header("Transitions")]
    [SerializeField, Min(0f)] private float dureeFondu = 3f; // fondu d'entrée et de sortie
    [SerializeField] private Vector2 pauseEntrePistes = new(5f, 20f); // silence aléatoire entre deux pistes (min, max)
    [SerializeField, Min(0f)] private float delaiDepart = 0f;

    [Header("Démarrage")]
    [SerializeField] private bool jouerAuDebut = true;

    private AudioSource source;
    private Coroutine boucle;
    private int dernierIndex = -1;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f; // 2D : même volume partout
    }

    private void Start()
    {
        if (jouerAuDebut)
            Demarrer();
    }

    public void Demarrer()
    {
        if (boucle == null && pistes != null && pistes.Length > 0)
        {
            boucle = StartCoroutine(Boucle());
        }

    }

    // Arrête l'ambiance en douceur (par exemple pendant une poursuite ou à la fin du jeu)
    public void Arreter()
    {
        if (boucle != null)
        {
            StopCoroutine(boucle);
            boucle = null;
        }

        StartCoroutine(FonduSortieEtStop());
    }

    private IEnumerator Boucle()
    {
        if (delaiDepart > 0f)
        {
            yield return new WaitForSeconds(delaiDepart);
        }

        while (true)
        {
            AudioClip piste = ChoisirPiste();

            source.clip = piste;
            source.volume = 0f;
            source.Play();

            yield return Fondu(0f, volume, dureeFondu);

            // Attend jusqu'à ce qu'il reste "dureeFondu" secondes à la piste
            float debutFonduSortie = Mathf.Max(0f, piste.length - dureeFondu);
            while (source.isPlaying && source.time < debutFonduSortie)
            {
                yield return null;
            }

            yield return Fondu(source.volume, 0f, dureeFondu);
            source.Stop();

            // Silence entre les pistes
            float pause = Random.Range(pauseEntrePistes.x, pauseEntrePistes.y);
            if (pause > 0f)
            {
                yield return new WaitForSeconds(pause);
            }

        }
    }

    private AudioClip ChoisirPiste()
    {
        int index = Random.Range(0, pistes.Length);

        // Évite de rejouer la même piste deux fois de suite
        if (pistes.Length > 1)
        {
            while (index == dernierIndex)
                index = Random.Range(0, pistes.Length);
        }

        dernierIndex = index;
        return pistes[index];
    }

    private IEnumerator Fondu(float de, float vers, float duree)
    {
        if (duree <= 0f)
        {
            source.volume = vers;
            yield break;
        }

        float temps = 0f;

        while (temps < duree)
        {
            temps += Time.deltaTime;
            source.volume = Mathf.Lerp(de, vers, temps / duree);
            yield return null;
        }

        source.volume = vers;
    }

    private IEnumerator FonduSortieEtStop()
    {
        yield return Fondu(source.volume, 0f, dureeFondu);
        source.Stop();
    }
}