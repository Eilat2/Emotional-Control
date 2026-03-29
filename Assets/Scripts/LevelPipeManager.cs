using UnityEngine;

public class LevelPipeManager : MonoBehaviour
{
    public PipeConnector neutralPipe;
    public PipeConnector joyPipe;
    public PipeConnector ragePipe;

    public GameObject waterSpray;
    public GameObject fireObject;
    public DoorController doorController;

    public void CheckPuzzleState()
    {
        Debug.Log("Checking puzzle...");
        Debug.Log("Neutral = " + (neutralPipe != null ? neutralPipe.flowingCorrectly.ToString() : "NULL"));
        Debug.Log("Joy = " + (joyPipe != null ? joyPipe.flowingCorrectly.ToString() : "NULL"));
        Debug.Log("Rage = " + (ragePipe != null ? ragePipe.flowingCorrectly.ToString() : "NULL"));

        bool allCorrect =
            neutralPipe != null && neutralPipe.flowingCorrectly &&
            joyPipe != null && joyPipe.flowingCorrectly &&
            ragePipe != null && ragePipe.flowingCorrectly;

        if (allCorrect)
        {
            Debug.Log("כל הצינורות חוברו נכון!");

            if (waterSpray != null)
                waterSpray.SetActive(true);

            if (fireObject != null)
                fireObject.SetActive(false);

            if (doorController != null)
                doorController.OpenDoor();
        }
    }
}