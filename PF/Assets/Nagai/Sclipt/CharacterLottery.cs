using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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

    private bool isRolling = false;
    private bool canStop = false;
    private Coroutine rollCoroutine;

    void Start()
    {
        Debug.Log("CharacterLottery Start");
        isRolling = false;
        canStop = false;
    }

    /// <summary>
    /// 抽選開始（UIが消えた後に呼ぶ）
    /// </summary>
    public void StartLottery()
    {
        Debug.Log("▶ StartLottery called. Active = " + gameObject.activeInHierarchy);

        if (isRolling) return;

        if (characterIcons == null || characterIcons.Length < slots.Length)
        {
            Debug.LogError("❌ キャラ画像が足りない");
            return;
        }

        isRolling = true;
        canStop = false;

        rollCoroutine = StartCoroutine(Roll());
        StartCoroutine(EnableStopAfterDelay());
    }

    void Update()
    {
        if (!isRolling || !canStop) return;

        // 🎯 キーボードのみ
        bool stopInput =
            Keyboard.current != null &&
            (
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame
            );

        if (stopInput)
        {
            Debug.Log("▶ Stop input detected (Keyboard)");
            StopLottery();
        }
    }

    IEnumerator Roll()
    {
        Debug.Log("▶ Roll coroutine START");

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

        Debug.Log("▶ Roll coroutine END");
    }

    IEnumerator EnableStopAfterDelay()
    {
        yield return new WaitForSeconds(0.3f); // 同フレーム停止防止
        canStop = true;
        Debug.Log("▶ Stop enabled");
    }

    void StopLottery()
    {
        if (!isRolling) return;

        Debug.Log("▶ StopLottery called");

        isRolling = false;

        if (rollCoroutine != null)
            StopCoroutine(rollCoroutine);
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
