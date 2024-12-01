using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public TMP_Dropdown cameraDropdown; // Referència al Dropdown de TextMeshPro
    public string gameSceneName = "Joc"; // Nom de l'escena del joc

    public void StartGame()
    {
        // Guarda la selecció de càmera
        int cameraType = cameraDropdown.value; // Valor seleccionat al Dropdown
        PlayerPrefs.SetInt("CameraType", cameraType);
        PlayerPrefs.Save();

        // Carrega l'escena del joc
        SceneManager.LoadScene(gameSceneName);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuInicial"); // Substitueix "MenuScene" pel nom de l'escena del menú
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!"); // Això només funciona a l'executable, no a l'editor
    }
}
