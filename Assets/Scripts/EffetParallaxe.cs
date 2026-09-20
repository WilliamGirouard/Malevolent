using UnityEngine;

/// <summary>
/// À mettre sur chaque couche de fond (GameObject avec un SpriteRenderer).
/// suiviHorizontal : 1 = fixe à l'écran (très loin), 0 = bouge avec le sol,
/// valeur négative = plus rapide que le sol (avant-plan).
/// </summary>
[DefaultExecutionOrder(100)] // les couches bougent après la caméra
public class EffetParallaxe : MonoBehaviour
{
    [Header("Caméra")]
    [SerializeField] private Transform cameraCible;

    [Header("Suivi de la caméra")]
    [SerializeField, Range(-0.5f, 1f)] private float suiviHorizontal = 0.85f;
    [SerializeField, Range(0f, 1f)] private float suiviVertical = 0.90f;

    [Header("Déplacement automatique très lent")]
    [SerializeField] private Vector2 vitesseAutomatique = new(0.01f, 0f);

    [Header("Boucle infinie (horizontale)")]
    [SerializeField] private bool boucleInfinie = true;

    [Header("Taille à l'écran")]
    [SerializeField] private bool remplirHauteurEcran = true;
    [SerializeField, Min(1f)] private float margeVerticale = 1.1f; // > 1 = laisse de la place si la caméra bouge en Y

    private Vector3 positionInitiale;
    private Vector3 positionCameraInitiale;
    private Vector2 decalageAutomatique;
    private float largeurTuile;

    private void Start()
    {
        if (cameraCible == null && Camera.main != null)
            cameraCible = Camera.main.transform;

        // Doit être fait AVANT d'enregistrer la position initiale
        if (remplirHauteurEcran)
            AjusterALaHauteurEcran();

        positionInitiale = transform.position;

        if (cameraCible != null)
            positionCameraInitiale = cameraCible.position;

        if (boucleInfinie)
            ConfigurerBoucle();
    }

    private void AjusterALaHauteurEcran()
    {
        if (cameraCible == null) return;

        var sr = GetComponent<SpriteRenderer>();
        var cam = cameraCible.GetComponent<Camera>();

        if (sr == null || sr.sprite == null || cam == null || !cam.orthographic) return;

        float hauteurEcran = cam.orthographicSize * 2f * margeVerticale;
        float echelle = hauteurEcran / sr.sprite.bounds.size.y;

        transform.localScale = new Vector3(echelle, echelle, 1f);

        // Centre la couche verticalement sur la caméra
        transform.position = new Vector3(
            transform.position.x,
            cameraCible.position.y,
            transform.position.z
        );
    }

    private void ConfigurerBoucle()
    {
        var sr = GetComponent<SpriteRenderer>();

        if (sr == null || sr.sprite == null)
        {
            Debug.LogWarning($"{name} : pas de SpriteRenderer/sprite, la boucle est désactivée.", this);
            boucleInfinie = false;
            return;
        }

        float largeurSprite = sr.sprite.bounds.size.x;

        // Répète le sprite 3 fois pour qu'il n'y ait jamais de trou
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(largeurSprite * 3f, sr.size.y);

        largeurTuile = largeurSprite * transform.lossyScale.x;
    }

    private void LateUpdate()
    {
        if (cameraCible == null) return;

        Vector3 mouvementCamera = cameraCible.position - positionCameraInitiale;
        decalageAutomatique += vitesseAutomatique * Time.deltaTime;

        float y = positionInitiale.y
                + mouvementCamera.y * suiviVertical
                + decalageAutomatique.y;

        if (boucleInfinie)
        {
            // Position de la couche par rapport à la caméra
            float relatif = (positionInitiale.x - positionCameraInitiale.x)
                          - mouvementCamera.x * (1f - suiviHorizontal)
                          + decalageAutomatique.x;

            // Garde la couche à +/- une demi-tuile de la caméra
            relatif = Mathf.Repeat(relatif + largeurTuile / 2f, largeurTuile) - largeurTuile / 2f;

            transform.position = new Vector3(cameraCible.position.x + relatif, y, positionInitiale.z);
        }
        else
        {
            transform.position = new Vector3(
                positionInitiale.x + mouvementCamera.x * suiviHorizontal + decalageAutomatique.x,
                y,
                positionInitiale.z
            );
        }
    }
}