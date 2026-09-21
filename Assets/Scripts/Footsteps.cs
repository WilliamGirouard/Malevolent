using UnityEngine;

/// <summary>
/// Bruits de pas du joueur.
/// Mode automatique : un pas à intervalle régulier quand il marche ou court.
/// Mode synchro : appelle JouerPas() depuis des Animation Events dans les clips.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Footsteps : MonoBehaviour
{
    [SerializeField] private JoueurMouvement mouvement;

    [Header("Sons")]
    [SerializeField] private AudioClip[] sonsPas;
    [SerializeField, Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private Vector2 variationPitch = new(0.9f, 1.1f); // légère variation pour éviter la répétition

    [Header("Rythme automatique")]
    [SerializeField] private bool synchroAvecAnimation = false; // true = seuls les Animation Events jouent les pas
    [SerializeField] private float intervalleMarche = 0.5f;
    [SerializeField] private float intervalleCourse = 0.3f;
    [SerializeField] private float seuilCourse = 4.5f; // vitesse à partir de laquelle il court

    private AudioSource source;
    private float minuteur;
    private int dernierIndex = -1;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;

        if (mouvement == null)
            mouvement = GetComponentInParent<JoueurMouvement>();
    }

    private void Update()
    {
        if (synchroAvecAnimation || mouvement == null) return;

        float vitesse = Mathf.Abs(mouvement.VitesseActuelle.x);
        bool marche = mouvement.EstAuSol && !mouvement.EstAccroupi && vitesse > 0.1f;

        if (!marche)
        {
            minuteur = 0f; // le premier pas part tout de suite quand il recommence à marcher
            return;
        }

        minuteur -= Time.deltaTime;

        if (minuteur <= 0f)
        {
            JouerPas();
            minuteur = vitesse > seuilCourse ? intervalleCourse : intervalleMarche;
        }
    }

    // Public : peut être appelée par un Animation Event
    public void JouerPas()
    {
        if (sonsPas == null || sonsPas.Length == 0) return;
        if (mouvement != null && (!mouvement.EstAuSol || mouvement.EstAccroupi)) return;

        int index = Random.Range(0, sonsPas.Length);

        // Évite de jouer deux fois le même son de suite
        if (sonsPas.Length > 1)
        {
            while (index == dernierIndex)
                index = Random.Range(0, sonsPas.Length);
        }

        dernierIndex = index;

        source.pitch = Random.Range(variationPitch.x, variationPitch.y);
        source.PlayOneShot(sonsPas[index], volume);
    }
}