using UnityEngine;
using TMPro;

public class StartGame : MonoBehaviour
{
    public TMP_Dropdown sizeDropdown; // Le choix de taille
    public GridManager gridManager;   // Référence vers ton script de gestion de grille

    /// <summary>
    /// Fonction qui créer un jeu de taille sélectionnée
    /// </summary>
    public void OnStartButtonClicked()
    {
        string selectedOption = sizeDropdown.options[sizeDropdown.value].text;

        switch (selectedOption.ToLower())
        {
            case "smol (8x8)":
                gridManager.GenerateGrid(8, 8, 10);
                break;
            case "mid (16x16)":
                gridManager.GenerateGrid(16, 16, 40);
                break;
            case "beeg (24x24)":
                gridManager.GenerateGrid(24, 24, 99);
                break;
            case "titanus (32x32)":
                gridManager.GenerateGrid(32, 32, 200);
                break;
            default:
                Debug.LogWarning("Option de taille inconnue : " + selectedOption);
                break;
        }

        // Cache le choix de taille
        this.gameObject.SetActive(false);
    }
}
