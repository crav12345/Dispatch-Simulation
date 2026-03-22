using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgramRunner : MonoBehaviour
{
    private IExecutionMode _executionMode;

    public IEnumerator LoadProgram(int citySeed, int simulationSeed)
    {
        yield return SceneManager.LoadSceneAsync("SimEnv", LoadSceneMode.Single);

        // TODO: Needs to be configurable. Add replay option. -----------------
        var modeObject = new GameObject("GameMode", typeof(Simulation));
        _executionMode = modeObject.GetComponent<Simulation>();
        // --------------------------------------------------------------------

        _executionMode.Load(citySeed, simulationSeed);
    }

    public IEnumerator RunProgram()
    {
        yield return null;
    }
}
