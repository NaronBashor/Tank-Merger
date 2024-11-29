using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void OnRestartGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
