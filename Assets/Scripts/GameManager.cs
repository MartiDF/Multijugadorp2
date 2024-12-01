using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int index = 0;
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] List<GameObject> ubicacions = new List<GameObject>();

    public Player InstanciarJugador()
    {
        GameObject jugador = GameObject.Instantiate(prefabs[index], ubicacions[index].transform.position, ubicacions[index].transform.rotation);
        index++;
        return jugador.GetComponent<Player>();
    }
}
