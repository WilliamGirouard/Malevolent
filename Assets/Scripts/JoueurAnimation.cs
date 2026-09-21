using UnityEngine;

/// <summary>
/// Envoie l'état du joueur à l'Animator.
/// À mettre sur l'objet qui a l'Animator (le joueur ou son enfant sprite).
/// Les noms des paramètres doivent être identiques dans l'Animator (majuscules comprises).
/// </summary>
[RequireComponent(typeof(Animator))]
public class JoueurAnimation : MonoBehaviour
{
    [SerializeField] private JoueurMouvement mouvement; 

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (mouvement == null)
            mouvement = GetComponentInParent<JoueurMouvement>();
    }

    private void Update()
    {
        if (mouvement == null) return;

        Vector2 vitesse = mouvement.VitesseActuelle;

        animator.SetFloat("Vitesse", Mathf.Abs(vitesse.x)); // vitesse horizontale (toujours positive)
        animator.SetFloat("VitesseY", vitesse.y);           // > 0 monte, < 0 tombe
        animator.SetBool("AuSol", mouvement.EstAuSol);
        animator.SetBool("Accroupi", mouvement.EstAccroupi);
    }
}