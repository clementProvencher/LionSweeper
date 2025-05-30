using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Contrôle général du jeu : gestion de l’état de la partie.
/// Singleton accessible depuis les tuiles.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton statique

    private GridManager gridManager;

    public GameObject gameOverPanel;   // Assigné dans l’inspecteur
    public GameObject victoryPanel;

    private bool gameEnded = false;

    private bool revealingNeighboors = false; // Flag pour empêcher une boucle infini de révélation de voisins

    private void Awake()
    {
        // Assure qu'on a une seule instance du GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Si une autre instance existe, on la détruit
        }
    }

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    /// <summary>
    /// Appelé quand une tuile est cliquée.
    /// </summary>
    public void OnTileClicked(Tile tile)
    {
        if (gameEnded || tile.isFlagged) return;

        if (!tile.isRevealed)
        {
            tile.Reveal();

            if (tile.isLion)
            {
                GameOver();
                return;
            }

            if (tile.adjacentLions == 0)
            {
                gridManager.FloodReveal(tile.x, tile.y);
            }
        }
        else
        {
            // Clic sur une tuile révélée avec un chiffre
            if (tile.adjacentLions > 0 && !revealingNeighboors)
            {
                revealingNeighboors = true;
                gridManager.TryAutoReveal(tile);
                revealingNeighboors = false;
            }
        }

        CheckWin();
    }


    private void GameOver()
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);
        gridManager.RevealAllLions();
    }

    private void CheckWin()
    {
        foreach (Tile t in FindObjectsOfType<Tile>())
        {
            if (!t.isRevealed && !t.isLion)
                return;
        }

        // Victoire
        gridManager.RevealAllLions();
        gameEnded = true;
        victoryPanel.SetActive(true);
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
}
