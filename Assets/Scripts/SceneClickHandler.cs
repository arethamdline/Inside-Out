using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneClickHandler : MonoBehaviour
{
    public string sceneName;

    void OnMouseDown()
    {
        Debug.Log(sceneName);
        SceneManager.LoadScene(sceneName);
    }
}