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

    private void Start()
    {
        GenerateGrid();
    }

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

    private void PlaceLions()
    {
        int placed = 0;
        while (placed < numberOfLions)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            Tile tile = grid[x, y];

            if (!tile.isLion)
            {
                tile.isLion = true;
                placed++;
            }
        }
    }

    private void CalculateAdjacents()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = grid[x, y];

                if (tile.isLion)
                {
                    tile.adjacentLions = -1; // Juste pour marquer un lion
                    continue;
                }

                int count = 0;

                // Vérifie les 8 cases autour
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int checkX = x + dx;
                        int checkY = y + dy;

                        if (IsInBounds(checkX, checkY) && grid[checkX, checkY].isLion)
                        {
                            count++;
                        }
                    }
                }

                tile.adjacentLions = count;
            }
        }
    }

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
    /// Centre la caméra au centre de la grille
    /// </summary>
    private void CenterCamera()
    {
        float centerX = (width - 1) / 2f;
        float centerY = (height - 1) / 2f;

        Camera.main.transform.position = new Vector3(centerX, centerY, -10f); // Z = -10 pour caméra ortho
    }

}
