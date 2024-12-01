using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera fixedCamera;          // Assigna la càmera fixa
    public Camera followCamera;         // Assigna la càmera de seguiment no adaptativa
    public Camera adaptiveCamera;       // Assigna la càmera de seguiment adaptativa
    public Camera[] splitScreenCameras; // Assigna les càmeres per a pantalla dividida
    public GameObject divider;

    void Start()
    {
        // Llegir la selecció de càmera desada
        int cameraType = PlayerPrefs.GetInt("CameraType", 0); // Per defecte 0 (Càmera fixa)

        // Desactivar totes les càmeres primer
        fixedCamera.gameObject.SetActive(false);
        followCamera.gameObject.SetActive(false);
        adaptiveCamera.gameObject.SetActive(false);
        foreach (Camera cam in splitScreenCameras)
        {
            cam.gameObject.SetActive(false);
        }
        divider.gameObject.SetActive(false);

        // Activar la càmera seleccionada
        switch (cameraType)
        {
            case 0: // Càmera fixa
                fixedCamera.gameObject.SetActive(true);
                break;
            case 1: // Càmera de seguiment no adaptativa
                followCamera.gameObject.SetActive(true);
                break;
            case 2: // Càmera de seguiment adaptativa
                adaptiveCamera.gameObject.SetActive(true);
                break;
            case 3: // Pantalla dividida
                foreach (Camera cam in splitScreenCameras)
                {
                    cam.gameObject.SetActive(true);
                }
                divider.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("Tipus de càmera desconegut!");
                break;
        }
    }
}
