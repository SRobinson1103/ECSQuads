using UnityEngine;

public class SceneDebugger : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Listing all GameObjects in the scene:");
        foreach (GameObject obj in FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None))
        {
            Debug.Log($"GameObject: {obj.name}, Active: {obj.activeInHierarchy}");
        }
    }
}
    