using UnityEngine;

/// <summary>
/// Représente une seule tuile sur la grille de jeu.
/// Peut contenir un lion (mine), un chiffre, ou être vide.
/// </summary>
public class Tile : MonoBehaviour
{
    // Position de la tuile dans la grille
    public int x;
    public int y;

    // État logique de la tuile
    public bool isRevealed = false;  // Est-ce que cette tuile a été cliquée ?
    public bool isLion = false;      // Est-ce que cette tuile contient un lion ?
    public bool isFlagged = false;   // Est-ce que cette tuile est marquée comme potentiel lion ?
    public int adjacentLions = 0;    // Nombre de lions autour (0 à 8)

    // Référence au SpriteRenderer pour changer l’apparence visuelle
    private SpriteRenderer sr;

    // Sprites assignables depuis l’inspecteur
    public Sprite hiddenSprite;      // Sprite quand la tuile est cachée
    public Sprite revealedSprite;    // Sprite générique pour une tuile révélée (peu utilisé ici)
    public Sprite lionSprite;        // Sprite spécial pour les lions
    public Sprite flagSprite;        // Sprite pour une tuile suspectée lion
    public Sprite[] numberSprites;   // Sprites numérotés (0 à 8), à afficher si pas de lion

    private void Awake()
    {
        // On récupère le SpriteRenderer pour pouvoir changer le sprite plus tard
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.IsGameEnded()) return;

        // Clic gauche : révéler
        if (Input.GetMouseButtonDown(0) && !isRevealed && !isFlagged)
        {
            Reveal();
            GameManager.Instance.OnTileClicked(this);
        }

        // Clic droit : poser/enlever un drapeau
        if (Input.GetMouseButtonDown(1) && !isRevealed)
        {
            ToggleFlag();
        }
    }

    public void ToggleFlag()
    {
        isFlagged = !isFlagged;

        // Change le sprite
        if (isFlagged)
        {
            sr.sprite = flagSprite;
        }
        else
        {
            sr.sprite = hiddenSprite;
        }
    }

    /// <summary>
    /// Affiche visuellement ce qu’il y a sur la tuile.
    /// </summary>
    public void Reveal()
    {
        if (isRevealed || isFlagged) return;

        isRevealed = true;

        if (isLion)
        {
            // Si c’est un lion, on montre le sprite du lion
            sr.sprite = lionSprite;
        }
        else if(adjacentLions > 0)
        {
            // Sinon, on montre le chiffre correspondant au nombre de lions autour
            sr.sprite = numberSprites[adjacentLions - 1];
        }
        else
        {
            sr.sprite = revealedSprite;
        }
    }

    /// <summary>
    /// Remet la tuile dans son état initial (utile pour relancer une partie).
    /// </summary>
    public void ResetTile()
    {
        isRevealed = false;
        isLion = false;
        adjacentLions = 0;
        sr.sprite = hiddenSprite;
    }
}
