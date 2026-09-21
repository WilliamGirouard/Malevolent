using UnityEngine;

/// <summary>
/// Caméra 2D qui suit le joueur en douceur.
/// À mettre sur la Main Camera, puis glisse le joueur dans "Cible".
/// </summary>
[DefaultExecutionOrder(-100)] // la caméra s'installe avant tout le reste
public class CameraSuivi : MonoBehaviour
{
    [SerializeField] private Transform cible;
    [SerializeField] private Vector2 decalage = new(0f, 1f); // décalage par rapport au joueur
    [SerializeField, Min(0f)] private float lissage = 0.15f; // 0 = instantané, plus grand = plus lent
    [SerializeField] private bool suivreVertical = true;     // décoche pour verrouiller la hauteur

    private Vector3 vitesse;
    private float hauteurInitiale;

    private void Start()
    {
        hauteurInitiale = transform.position.y;

        // Place la caméra directement sur le joueur au démarrage (évite le saut des couches parallax)
        if (cible != null)
        {
           transform.position = Objectif(); 
        }
            
    }

    private void LateUpdate()
    {
        if (cible == null)
        {
          return;  
        } 

        transform.position = Vector3.SmoothDamp(transform.position, Objectif(), ref vitesse, lissage);
    }

    private Vector3 Objectif()
    {
        float y = suivreVertical ? cible.position.y + decalage.y : hauteurInitiale;
        return new Vector3(cible.position.x + decalage.x, y, transform.position.z);
    }
}