using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneNameToLoad;

    public void LoadScene()
    {
        SoundManager.Instance.TriggerSoundEvent("buttonClick");
        if (sceneNameToLoad != null) {
            if (sceneNameToLoad == "MainMenu") {
                FindObjectOfType<TankSpawner>().SaveGame();
            }
        }
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
