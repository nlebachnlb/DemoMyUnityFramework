using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    [SerializeField] private string sceneName;
    [SerializeField] private float delay;

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
