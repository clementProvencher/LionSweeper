using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Gère la génération de la grille de jeu et la pose des lions.
/// </summary>
public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public int numberOfLions = 10;

    public GameObject tilePrefab;

    private Tile[,] grid;

    /// <summary>
    /// Change les dimentions de la grille
    /// </summary>
    /// <param name="width">Largeur de la grille</param>
    /// <param name="height">hauteur de la grille</param>
    /// <param name="numberOfLions">le nombre de mines</param>
    public void SetGridSize(int width, int height, int numberOfLions)
    {
        if (width <= 0 || height <= 0 || numberOfLions <= 0)
            throw new ArgumentOutOfRangeException("Tous les paramètres doivent être superieur à 0");

        this.width = width;
        this.height = height;
        this.numberOfLions = numberOfLions;
    }

    /// <summary>
    /// Génère la grille selon les dimentions
    /// </summary>
    public void GenerateGrid()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Création de la tuile à la position x, y
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                tileObj.name = $"Tile_{x}_{y}";
                tileObj.transform.parent = this.transform;

                // Configuration de la tuile
                Tile tile = tileObj.GetComponent<Tile>();
                tile.x = x;
                tile.y = y;

                grid[x, y] = tile;
            }
        }

        PlaceLions();
        CalculateAdjacents();
        CenterCamera();
    }

    /// <summary>
    /// Fusion des fonctions GenerateGrid et SetGridSize
    /// Change les dimentions de la grille avant de la créer
    /// </summary>
    /// <param name="width">Largeur de la grille</param>
    /// <param name="height">hauteur de la grille</param>
    /// <param name="numberOfLions">le nombre de mines</param>
    public void GenerateGrid(int width, int height, int numberOfLions)
    {
        SetGridSize(width, height, numberOfLions);
        GenerateGrid();
    }

    /// <summary>
    /// Place les mines dans la grille
    /// </summary>
    private void PlaceLions()
    {
        int placed = 0;
        while (placed < numberOfLions)
        {
            int x = UnityEngine.Random.Range(0, width);
            int y = UnityEngine.Random.Range(0, height);

            Tile tile = grid[x, y];

            if (!tile.isLion)
            {
                tile.isLion = true;
                placed++;
            }
        }
    }

    /// <summary>
    /// Retourne la liste des tuiles adjacentes à une tuile donnée (au maximum 8).
    /// </summary>
    /// <param name="tile">La tuile dont on veut les voisins.</param>
    /// <returns>Liste des tuiles adjacentes.</returns>
    public List<Tile> GetAdjacentTiles(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = tile.x + dx;
                int ny = tile.y + dy;

                if (IsInBounds(nx, ny))
                {
                    neighbors.Add(grid[nx, ny]);
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Calcule le nombre de lions adjacents à chaque tuile.
    /// </summary>
    private void CalculateAdjacents()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = grid[x, y];

                if (tile.isLion)
                {
                    tile.adjacentLions = -1; // Marque une tuile avec un lion
                    continue;
                }

                int count = 0;

                foreach (Tile neighbor in GetAdjacentTiles(tile))
                {
                    if (neighbor.isLion)
                        count++;
                }

                tile.adjacentLions = count;
            }
        }
    }


    /// <summary>
    /// Trouve si un couple de coordonnnées existe dans la grille
    /// </summary>
    /// <param name="x">la coordonnée en x</param>
    /// <param name="y">la coordonnée en y</param>
    /// <returns>Si la paire de coordonnée est dans la grille</returns>
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    /// <summary>
    /// Retourne la tuile à des coordonnées.
    /// </summary>
    public Tile GetTileAt(int x, int y)
    {
        if (IsInBounds(x, y))
            return grid[x, y];
        return null;
    }

    /// <summary>
    /// Révèle tous les lions
    /// </summary>
    public void RevealAllLions()
    {
        foreach (Tile t in FindObjectsOfType<Tile>())
        {
            if (t.isLion)
            {
                t.Reveal();
            }
        }
    }


    /// <summary>
    /// Centre la caméra au centre de la grille
    /// </summary>
    private void CenterCamera()
    {
        float centerX = (width - 1) / 2f;
        float centerY = (height - 1) / 2f;

        Camera.main.transform.position = new Vector3(centerX, centerY, -10f); // Z = -10 pour caméra ortho
    }


    /// <summary>
    /// Révèle récursivement les tuiles vides connectées.
    /// Utilise une approche itérative pour éviter les appels récursifs infinis.
    /// </summary>
    public void FloodReveal(int startX, int startY)
    {
        Queue<Tile> toCheck = new Queue<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();

        Tile startTile = GetTileAt(startX, startY);
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

                        Tile neighbor = GetTileAt(nx, ny);
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

    /// <summary>
    /// Tente de révéler automatiquement les cases autour si le bon nombre de drapeaux est posé.
    /// </summary>
    public void TryAutoReveal(Tile centerTile)
    {
        var neighbors = GetAdjacentTiles(centerTile);
        int flags = neighbors.Count(t => t.isFlagged);

        if (flags == centerTile.adjacentLions)
        {
            foreach (var neighbor in neighbors)
            {
                GameManager.Instance.OnTileClicked(neighbor);
            }
        }
    }

}
