using UnityEngine;

/// <summary>
/// À mettre sur l'objet "Sol" (avec son Box Collider 2D).
/// Le sol suit le joueur en X, donc il ne se termine jamais.
/// Ne met PAS ce script sur un objet enfant du joueur ou d'une couche parallax.
/// </summary>
public class SolInfini : MonoBehaviour
{
    [SerializeField] private Transform cible; // le joueur (l'objet avec le Rigidbody2D)

    private float yInitial;
    private float zInitial;

    private void Start()
    {
        yInitial = transform.position.y;
        zInitial = transform.position.z;

        if (cible == null)
        {
            var joueur = FindFirstObjectByType<JoueurMouvement>();
            if (joueur != null) cible = joueur.transform;
        }
    }

    // FixedUpdate : même rythme que la physique, donc pas de saccades sous les pieds du joueur
    private void FixedUpdate()
    {
        if (cible == null) return;

        transform.position = new Vector3(cible.position.x, yInitial, zInitial);
    }
}