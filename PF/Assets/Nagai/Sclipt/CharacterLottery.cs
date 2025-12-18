using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterLottery : MonoBehaviour
{
    [Header("キャラ画像（最低4つ）")]
    [SerializeField] private Sprite[] characterIcons;

    [Header("表示スロット（左から1P〜4P）")]
    [SerializeField] private Image[] slots;

    [Header("回転間隔（小さいほど速い）")]
    [SerializeField] private float interval = 0.1f;

    [Header("回転中エフェクトPrefab")]
    [SerializeField] private GameObject rollingEffectPrefab;
    [SerializeField] private Transform effectParent;

    [Header("移動先シーン候補")]
    [SerializeField] private string[] gameScenes;

    private GameObject rollingEffectInstance;
    private Coroutine rollCoroutine;

    private bool isRolling = false;
    private bool canStop = false;

    void Start()
    {
        isRolling = false;
        canStop = false;
    }

    void Update()
    {
        if (!isRolling || !canStop) return;

        bool stopInput =
            Keyboard.current != null &&
            (
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame
            );

        if (stopInput)
        {
            StopLottery();
        }
    }

    /// <summary>
    /// 抽選開始（UIが消えた後に呼ぶ）
    /// </summary>
    public void StartLottery()
    {
        if (isRolling) return;

        if (characterIcons == null || characterIcons.Length < slots.Length)
        {
            Debug.LogError("❌ キャラ画像が足りない");
            return;
        }

        isRolling = true;
        canStop = false;

        // ★ 回転エフェクト生成
        if (rollingEffectPrefab != null && effectParent != null)
        {
            rollingEffectInstance = Instantiate(
                rollingEffectPrefab,
                effectParent.position,
                Quaternion.identity,
                effectParent
            );
        }

        rollCoroutine = StartCoroutine(Roll());
        StartCoroutine(EnableStopAfterDelay());
    }

    IEnumerator Roll()
    {
        while (isRolling)
        {
            List<Sprite> shuffled = new List<Sprite>(characterIcons);
            Shuffle(shuffled);

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                    slots[i].sprite = shuffled[i];
            }

            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator EnableStopAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);
        canStop = true;
    }

    void StopLottery()
    {
        if (!isRolling) return;

        isRolling = false;

        if (rollCoroutine != null)
            StopCoroutine(rollCoroutine);

        // ★ 回転エフェクト削除
        if (rollingEffectInstance != null)
        {
            Destroy(rollingEffectInstance);
            rollingEffectInstance = null;
        }

        SaveResult();
        MoveToRandomScene();
    }

    /// <summary>
    /// 抽選結果を保存
    /// </summary>
    void SaveResult()
    {
        // slots[i].sprite をそのまま使えばOK
        Debug.Log("🎯 抽選結果保存");
        // ここで CharacterSelectionData / SkillSelectionData に代入
    }

    /// <summary>
    /// ランダムにゲームシーンへ移動
    /// </summary>
    void MoveToRandomScene()
    {
        if (gameScenes == null || gameScenes.Length == 0)
        {
            Debug.LogError("❌ 移動先シーンが設定されていない");
            return;
        }

        string sceneName = gameScenes[Random.Range(0, gameScenes.Length)];
        Debug.Log("➡ シーン移動: " + sceneName);

        SceneManager.LoadScene(sceneName);
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
