using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadARMode()
    {
        SceneManager.LoadScene("ARScene");
    }

    public void LoadNonARMode()
    {
        SceneManager.LoadScene("NonARScene");
    }
}
