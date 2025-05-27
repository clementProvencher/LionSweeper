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

        tile.Reveal();

        if (tile.isLion)
        {
            GameOver();
            return;
        }

        if (tile.adjacentLions == 0)
        {
            FloodReveal(tile.x, tile.y);
        }

        CheckWin();
    }

    private void GameOver()
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);
        RevealAllLions();
    }

    private void CheckWin()
    {
        foreach (Tile t in FindObjectsOfType<Tile>())
        {
            if (!t.isRevealed && !t.isLion)
                return;
        }

        gameEnded = true;
        victoryPanel.SetActive(true);
    }

    private void RevealAllLions()
    {
        foreach (Tile t in FindObjectsOfType<Tile>())
        {
            if (t.isLion)
            {
                t.Reveal();
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }

    /// <summary>
    /// Révèle récursivement les tuiles vides connectées.
    /// Utilise une approche itérative pour éviter les appels récursifs infinis.
    /// </summary>
    private void FloodReveal(int startX, int startY)
    {
        Queue<Tile> toCheck = new Queue<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();

        Tile startTile = gridManager.GetTileAt(startX, startY);
        toCheck.Enqueue(startTile);
        visited.Add(startTile);

        while (toCheck.Count > 0)
        {
            Tile current = toCheck.Dequeue();

            current.Reveal();

            if (current.adjacentLions == 0)
            {
                // Explore les 8 voisines
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = current.x + dx;
                        int ny = current.y + dy;

                        // Ignore le centre
                        if (dx == 0 && dy == 0) continue;

                        Tile neighbor = gridManager.GetTileAt(nx, ny);
                        if (neighbor == null) continue;

                        if (!neighbor.isRevealed && !neighbor.isLion && !visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            toCheck.Enqueue(neighbor);
                        }
                    }
                }
            }
        }
    }
}
