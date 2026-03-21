using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgramRunner : MonoBehaviour
{
    public IEnumerator LoadProgram()
    {
        yield return SceneManager.LoadSceneAsync("SimEnv", LoadSceneMode.Single);
    }

    public IEnumerator RunProgram()
    {
        yield return null;
    }
}
