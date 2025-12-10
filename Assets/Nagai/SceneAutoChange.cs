using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAutoChange : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // 移動先のシーン名
    [SerializeField] private float delay = 4f;     // シーン移動までの時間（秒）

    void Start()
    {
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private System.Collections.IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("次のシーン名が設定されていません！");
        }
    }
}
