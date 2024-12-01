using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera fixedCamera;          // Assigna la c�mera fixa
    public Camera followCamera;         // Assigna la c�mera de seguiment no adaptativa
    public Camera adaptiveCamera;       // Assigna la c�mera de seguiment adaptativa
    public Camera[] splitScreenCameras; // Assigna les c�meres per a pantalla dividida
    public GameObject divider;

    void Start()
    {
        // Llegir la selecci� de c�mera desada
        int cameraType = PlayerPrefs.GetInt("CameraType", 0); // Per defecte 0 (C�mera fixa)

        // Desactivar totes les c�meres primer
        fixedCamera.gameObject.SetActive(false);
        followCamera.gameObject.SetActive(false);
        adaptiveCamera.gameObject.SetActive(false);
        foreach (Camera cam in splitScreenCameras)
        {
            cam.gameObject.SetActive(false);
        }
        divider.gameObject.SetActive(false);

        // Activar la c�mera seleccionada
        switch (cameraType)
        {
            case 0: // C�mera fixa
                fixedCamera.gameObject.SetActive(true);
                break;
            case 1: // C�mera de seguiment no adaptativa
                followCamera.gameObject.SetActive(true);
                break;
            case 2: // C�mera de seguiment adaptativa
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
                Debug.LogWarning("Tipus de c�mera desconegut!");
                break;
        }
    }
}
