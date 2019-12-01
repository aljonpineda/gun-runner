using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ScreenTransitions : MonoBehaviour
{
    public Animator transitionAnim;
    public string sceneName;
    public Drill drill;

    private void Update()
    {
        if (drill.IsDrillStarted)
        {
            StartCoroutine(WatchDrill());
            
        }
    }

    IEnumerator WatchDrill()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }
}
