using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public bool IsTutorialActive { get; private set; }
    private int tutorialStep; // Tracks the current step in the tutorial

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartTutorial()
    {
        IsTutorialActive = true;
        tutorialStep = 0; // Reset the tutorial to the beginning
    }

    public void EndTutorial()
    {
        IsTutorialActive = false;
    }

    public int GetTutorialStep() => tutorialStep;

    public void NextStep()
    {
        tutorialStep++;
    }
}
