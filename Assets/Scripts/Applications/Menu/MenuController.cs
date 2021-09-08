using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public string bgmId;
    public bool freeze;

    private void Awake()
    {
        sfx = AppRoot.Instance.GetService<SfxController>();
        menuCallbacks = new List<System.Action>();
        menuCallbacks.Add(StartEnteringTheGame);
        menuCallbacks.Add(Quit);
        splash = true;
    }

    private void Start()
    {
        var bgm = AppRoot.Instance.GetService<BgmController>();
        bgm.Play(AppRoot.Instance.GetService<AudioManager>().GetBgm(bgmId), true, 6);
        StartCoroutine(KeyboardCatch());
        currentOption = 0;
    }

    private IEnumerator KeyboardCatch()
    {
        while (true)
        {
            if (splash)
            {
                if (Input.anyKeyDown)
                {
                    splash = false;
                    ShowOptions();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) && currentOption > 0)
                {
                    currentOption--;
                }

                if (Input.GetKeyDown(KeyCode.DownArrow) && currentOption < menuOptions.Length - 1)
                {
                    currentOption++;
                }

                if (Input.GetKeyDown(KeyCode.Return) && !freeze)
                {
                    menuCallbacks[currentOption]();
                    freeze = true;
                }
            }

            yield return null;

            var visual = menuOptions[currentOption];
            var pos = visual.rectTransform.position;
            var src = selector.position;

            src.y = Mathf.Lerp(src.y, pos.y, Time.fixedDeltaTime * 10);
            selector.position = src;
        }
    }

    private void ShowOptions()
    {
        GetComponent<Animator>().SetTrigger("ShowOption");
        sfx.Play("fx-confirm");
    }

    private void StartEnteringTheGame()
    {
        Debug.Log("Startgame");
        GetComponent<Animator>().SetTrigger("StartGame");
        cameraAnimator.SetTrigger("GameStart");
        var bgm = AppRoot.Instance.GetService<BgmController>();
        bgm.Stop(true, 3f);
        sfx.Play("fx-start");
        sfx.Play("fx-confirm");
    }

    private void Credits()
    {
        GetComponent<Animator>().SetTrigger("ShowCredits");
        cameraAnimator.SetTrigger("Credits");
    }

    private void Quit()
    {
        Application.Quit();
    }

    public void SwitchScene()
    {
        SceneManager.LoadScene("Cut_Prologue");
    }

    [SerializeField]
    private Text[] menuOptions;
    private int currentOption;
    private List<System.Action> menuCallbacks;

    [SerializeField]
    private RectTransform selector;

    [SerializeField]
    private Animator cameraAnimator;

    private bool splash;

    private SfxController sfx;
}
