using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{

    public GameObject GameUIPanel = null;
    public GameObject EndOfGamePanel = null;
    public TextMeshProUGUI ObjectiveText = null;
    public TextMeshProUGUI AmmoText = null;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI EndOfGameText = null;
    public TextMeshProUGUI playAgainText = null;
    public GameObject restartGameButton;

    public GameObject creditsSection;

    public void startGame()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().canMove = true;
        GameObject[] skeletons = GameObject.FindGameObjectsWithTag("Skeleton");
        foreach(GameObject g in skeletons)
        {
            g.GetComponent<NPCTankController>().canMove = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Destroy(this.gameObject);

    }
    public void quitGame()
    {
        Application.Quit();
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
