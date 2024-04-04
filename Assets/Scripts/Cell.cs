using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    EMPTY,
    SPHERE,
    CUBE
}

public class Cell : MonoBehaviour
{
    public CellType status;
    public GameManager gameManager;
    public GameObject sphere;
    public GameObject cube;

    void Start()
    {
        sphere.SetActive(false);
        cube.SetActive(false);
        status = CellType.EMPTY;
    }

    void OnMouseDown()
    {
        onClick();
    }

    public void onClick()
    {
        if (cube.activeSelf == true || sphere.activeSelf == true)
        {
            return;
        }

        if (gameManager.isCubeTurn == true)
        {
            status = CellType.CUBE;
            cube.SetActive(true);
            sphere.SetActive(false);
            gameManager.ChangeTurn();
        }
        else
        {
            status = CellType.SPHERE;
            sphere.SetActive(true);
            cube.SetActive(false);
            gameManager.ChangeTurn();
        }

        gameManager.CheckWinner();
    }
}