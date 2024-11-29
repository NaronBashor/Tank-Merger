using UnityEngine;

public class SceneTutorial : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject[] tutorialPanels;
    private int currentPanelIndex = 0;

    private const string TutorialCompletedKey = "TutorialCompleted"; // Key to save completion status

    private void Start()
    {
        // Check if the tutorial has been completed
        if (FindAnyObjectByType<TankSpawner>().playerdTutorial) {
            overlay.SetActive(false);
            Debug.Log("Tutorial already completed. Skipping tutorial.");
            return;
        }

        //Debug.Log("Starting tutorial. Total panels: " + tutorialPanels.Length);
        StartSceneTutorial();
    }

    private void StartSceneTutorial()
    {
        ShowPanel(currentPanelIndex);
        Time.timeScale = 0; // Pause the game when the last panel is displayed
    }

    private void ShowPanel(int index)
    {
        overlay.SetActive(true);

        // Ensure all panels are initially hidden
        foreach (GameObject panel in tutorialPanels) {
            panel.SetActive(false);
        }

        // Activate the current panel
        if (index < tutorialPanels.Length) {
            tutorialPanels[index].SetActive(true);
            //Debug.Log("Displaying panel " + index);
        }
    }

    public void OnNextButton()
    {
        // Hide the current panel
        tutorialPanels[currentPanelIndex].SetActive(false);

        // Move to the next panel
        currentPanelIndex++;

        // If we've just reached the last panel, pause and display it
        if (currentPanelIndex == tutorialPanels.Length - 1) {
            ShowPanel(currentPanelIndex);
        }
        // If we've gone past the last panel, resume and complete the tutorial
        else if (currentPanelIndex >= tutorialPanels.Length) {
            Time.timeScale = 1;
            CompleteTutorial();
        } else {
            // Show the next panel if it's not the last
            ShowPanel(currentPanelIndex);
        }
    }

    private void CompleteTutorial()
    {
        //Debug.Log("Tutorial completed.");

        // Mark the tutorial as completed in PlayerPrefs
        //PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        //PlayerPrefs.Save();

        TutorialManager tutorialManager = FindAnyObjectByType<TutorialManager>();
        if (tutorialManager != null) {
            tutorialManager.NextStep(); // Optional: Advances the global tutorial step
            tutorialManager.EndTutorial(); // Ends the tutorial sequence
        }

        overlay.SetActive(false);
        currentPanelIndex = 0; // Reset for potential reuse
    }
}
