using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.ServiceLocator;
using UnityEngine.SceneManagement;

public class AppRoot : ServiceLocator
{
    private IEnumerator Start()
    {
        Application.targetFrameRate = GameConfig.DEFAULT_TARGET_FRAMERATE;
        InitService();

        DontDestroyOnLoad(this.gameObject);

        yield return null;

        SceneManager.LoadScene(firstSceneName);
    }

    [SerializeField]
    private string firstSceneName;
}
