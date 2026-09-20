using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Déplacement 2D : marche, course (Shift), accroupi (flèche bas / S) et saut (flèche haut).
/// À mettre sur le joueur (Rigidbody2D + Collider2D requis).
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class JoueurMouvement : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float vitesseMarche = 3f;
    [SerializeField] private float vitesseCourse = 6f;

    [Header("Saut")]
    [SerializeField] private float forceSaut = 9f;
    [SerializeField] private LayerMask coucheSol;

    [Header("État")]
    public bool peutBouger = true; // à passer à false pendant un QTE, une cachette, etc.

    // Lus par le script d'animation
    public bool EstAuSol { get; private set; }
    public bool EstAccroupi { get; private set; }
    public Vector2 VitesseActuelle => rb.linearVelocity; // Unity 5/2022/2023 : remplace par rb.velocity

    private Rigidbody2D rb;
    private Collider2D col;
    private float direction;
    private bool court;
    private bool sautDemande;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        direction = 0f;
        court = false;
        EstAccroupi = false;

        Keyboard clavier = Keyboard.current;
        if (clavier == null || !peutBouger) return;

        // Droite : flèche droite ou D
        if (clavier.rightArrowKey.isPressed || clavier.dKey.isPressed)
            direction += 1f;

        // Gauche : flèche gauche, A (QWERTY) ou Q (AZERTY)
        if (clavier.leftArrowKey.isPressed || clavier.aKey.isPressed || clavier.qKey.isPressed)
            direction -= 1f;

        // Accroupi : flèche bas ou S (seulement au sol)
        bool basAppuye = clavier.downArrowKey.isPressed || clavier.sKey.isPressed;
        EstAccroupi = basAppuye && EstAuSol;

        // Course : Shift (impossible accroupi)
        court = clavier.leftShiftKey.isPressed && !EstAccroupi;

        // Saut : flèche haut, W ou Z (impossible accroupi)
        bool sautAppuye = clavier.upArrowKey.wasPressedThisFrame
                       || clavier.wKey.wasPressedThisFrame
                       || clavier.zKey.wasPressedThisFrame;

        if (sautAppuye && !EstAccroupi)
            sautDemande = true;

        // Retourne le joueur (et la lampe de poche si c'est un enfant)
        if (direction != 0f)
        {
            Vector3 echelle = transform.localScale;
            echelle.x = Mathf.Abs(echelle.x) * Mathf.Sign(direction);
            transform.localScale = echelle;
        }
    }

    private void FixedUpdate()
    {
        EstAuSol = VerifierSol();

        // Accroupi = immobile (il peut quand même se tourner)
        float vitesse = vitesseMarche;
        if (EstAccroupi) vitesse = 0f;
        else if (court) vitesse = vitesseCourse;

        Vector2 v = rb.linearVelocity; // Unity 5/2022/2023 : remplace par rb.velocity
        v.x = direction * vitesse;

        if (sautDemande && EstAuSol)
            v.y = forceSaut;

        sautDemande = false;
        rb.linearVelocity = v;         // Unity 5/2022/2023 : remplace par rb.velocity
    }

    private bool VerifierSol()
    {
        Bounds b = col.bounds;
        Vector2 centre = new Vector2(b.center.x, b.min.y - 0.02f);
        Vector2 taille = new Vector2(b.size.x * 0.9f, 0.05f);
        return Physics2D.OverlapBox(centre, taille, 0f, coucheSol) != null;
    }
}