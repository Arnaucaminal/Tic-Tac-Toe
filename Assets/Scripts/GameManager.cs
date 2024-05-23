using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isCubeTurn = true;
    public TextMeshProUGUI label;
    public Cell[] cells;
    public GameObject restartButton;
    public GameObject backToMenuButton;
    private bool modeAI;
    private bool thinking = false; // Variable per controlar si la IA està pensant
    public bool gameEnded = false; // Variable per controlar si el joc ha acabat

    public AudioClip clipWin;
    public AudioClip clipDraw;

    void Start()
    {
        ChangeTurn();
        restartButton.SetActive(false);
        backToMenuButton.SetActive(false);
        int flag = PlayerPrefs.GetInt("AI", 1);
        modeAI = flag == 1;
    }

    public void CheckWinner()
    {
        bool isDraw = true;

        // Revisa las filas
        for (int i = 0; i < 9; i += 3)
        {
            if (cells[i].status != CellType.EMPTY && cells[i].status == cells[i + 1].status && cells[i + 1].status == cells[i + 2].status)
            {
                DeclareWinner(cells[i].status);
                return;
            }
            if (cells[i].status == CellType.EMPTY || cells[i + 1].status == CellType.EMPTY || cells[i + 2].status == CellType.EMPTY) isDraw = false;
        }

        // Revisa las columnas
        for (int i = 0; i < 3; i++)
        {
            if (cells[i].status != CellType.EMPTY && cells[i].status == cells[i + 3].status && cells[i + 3].status == cells[i + 6].status)
            {
                DeclareWinner(cells[i].status);
                return;
            }
        }

        // Revisa las diagonales
        if (cells[0].status != CellType.EMPTY && cells[0].status == cells[4].status && cells[4].status == cells[8].status)
        {
            DeclareWinner(cells[0].status);
            return;
        }

        if (cells[2].status != CellType.EMPTY && cells[2].status == cells[4].status && cells[4].status == cells[6].status)
        {
            DeclareWinner(cells[2].status);
            return;
        }

        if (isDraw)
        {
            label.text = "It's a draw!";
            SetupGameFinished(false);
            return;
        }
    }

    void DeclareWinner(CellType status)
    {
        if (status == CellType.SPHERE)
        {
            label.text = "Player is the winner";
        }
        else
        {
            label.text = "AI is the winner";
        }

        SetupGameFinished(true);

        // El joc ha acabat, desactiva les cel·les
        gameEnded = true;
        foreach (Cell cell in cells)
        {
            cell.enabled = false;
        }
    }

    bool CheckWin(CellType player)
    {
        // Revisar filas
        for (int i = 0; i < 9; i += 3)
        {
            if (cells[i].status == player && cells[i + 1].status == player && cells[i + 2].status == player)
            {
                return true;
            }
        }
        // Revisar columnas
        for (int i = 0; i < 3; i++)
        {
            if (cells[i].status == player && cells[i + 3].status == player && cells[i + 6].status == player)
            {
                return true;
            }
        }
        // Revisar diagonales
        if ((cells[0].status == player && cells[4].status == player && cells[8].status == player) ||
            (cells[2].status == player && cells[4].status == player && cells[6].status == player))
        {
            return true;
        }
        return false;
    }

    bool CheckDraw()
    {
        foreach (Cell cell in cells)
        {
            if (cell.status == CellType.EMPTY)
            {
                return false;
            }
        }
        return true;
    }

    void SetupGameFinished(bool winner)
    {
        restartButton.SetActive(true);
        backToMenuButton.SetActive(true);

        if (winner)
        {
            GetComponent<AudioSource>().PlayOneShot(clipWin);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(clipDraw);
        }
    }

    public void ChangeTurn()
    {
        if (gameEnded) // Si el joc ha acabat, no permetis canvis de torn
            return;

        isCubeTurn = !isCubeTurn;
        if (isCubeTurn)
        {
            label.text = "Ai Turn(Cube)";
        }
        else
        {
            label.text = "Player Turn(Sphere)";
        }
    }

    int GetBestMove()
    {
        int bestMove = -1;
        int bestScore = int.MinValue;

        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].status == CellType.EMPTY)
            {
                cells[i].status = CellType.CUBE;
                int score = MiniMax(CellType.SPHERE, 0);
                cells[i].status = CellType.EMPTY;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        return bestMove;
    }

    int MiniMax(CellType player, int depth)
    {
        if (CheckWin(CellType.CUBE)) return 1;
        if (CheckWin(CellType.SPHERE)) return -1;
        if (CheckDraw()) return 0;

        int bestScore = (player == CellType.CUBE) ? int.MinValue : int.MaxValue;

        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].status == CellType.EMPTY)
            {
                cells[i].status = player;

                if (player == CellType.CUBE)
                {
                    int score = MiniMax(CellType.SPHERE, depth + 1);
                    bestScore = Mathf.Max(score, bestScore);
                }
                else
                {
                    int score = MiniMax(CellType.CUBE, depth + 1);
                    bestScore = Mathf.Min(score, bestScore);
                }

                cells[i].status = CellType.EMPTY;
            }
        }

        return bestScore;
    }

    void MakeAIMoveWithDelay()
    {
        thinking = true; // Marca que la IA està pensant
        int bestMove = GetBestMove();

        
        if (bestMove >= 0 && bestMove < cells.Length && cells[bestMove].status == CellType.EMPTY)
        {
            cells[bestMove].onClick(); // La IA realitza la jugada
            thinking = false; // La IA ha acabat de pensar
        }
    }

    void Update()
    {
        if (modeAI && isCubeTurn && !thinking) // Només fa la jugada si és el torn de la IA i no està pensant
        {
            Invoke("MakeAIMoveWithDelay", 3.0f); // Espera 3 segons abans de cridar la funció de la IA
            thinking = !thinking;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
