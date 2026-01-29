using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterLottery2P : MonoBehaviour
{
    [Header("キャラ画像（最低2つ）")]
    [SerializeField] private Sprite[] characterIcons;

    [Header("表示スロット（1P・2P）")]
    [SerializeField] private Image[] slots; // 2

    [Header("回転間隔")]
    [SerializeField] private float interval = 0.1f;

    [Header("遷移先ゲームシーン（2P用）")]
    [SerializeField] private string[] gameSceneNames;

    [Header("カウントダウンUI（3・2・1）")]
    [SerializeField] private GameObject count3;
    [SerializeField] private GameObject count2;
    [SerializeField] private GameObject count1;

    private bool isRolling;
    private bool canStop;
    private bool isStopping;
    private Coroutine rollCoroutine;

    // =============================
    // 最初に全部非表示
    // =============================
    void Start()
    {
        if (count3) count3.SetActive(false);
        if (count2) count2.SetActive(false);
        if (count1) count1.SetActive(false);
    }

    public void StartLottery()
    {
        if (isRolling) return;

        if (characterIcons.Length < slots.Length)
        {
            Debug.LogError("❌ キャラ画像が足りません（2P）");
            return;
        }

        isRolling = true;
        canStop = false;
        isStopping = false;

        rollCoroutine = StartCoroutine(Roll());
        StartCoroutine(EnableStopAfterDelay());
    }

    void Update()
    {
        if (!isRolling || !canStop || isStopping) return;

        bool press = false;

        // キーボード
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame)
            {
                press = true;
            }
        }

        // コントローラーB
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                press = true;
            }
        }

        if (press)
        {
            StopLottery();
        }
    }

    IEnumerator Roll()
    {
        while (isRolling)
        {
            List<Sprite> shuffled = new List<Sprite>(characterIcons);
            Shuffle(shuffled);

            slots[0].sprite = shuffled[0];
            slots[1].sprite = shuffled[1];

            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator EnableStopAfterDelay()
    {
        yield return new WaitForSeconds(0.3f);
        canStop = true;
    }

    public void StopLottery()
    {
        if (!isRolling) return;

        isStopping = true;
        isRolling = false;

        if (rollCoroutine != null)
            StopCoroutine(rollCoroutine);

        // ===== キャラ保存 =====
        SkillSelectionData.playerCount = 2;
        SkillSelectionData.p1Character = GetId(slots[0].sprite);
        SkillSelectionData.p2Character = GetId(slots[1].sprite);
        SkillSelectionData.p3Character = -1;
        SkillSelectionData.p4Character = -1;

        // ===== カウントダウン開始 =====
        StartCoroutine(CountDownAndMove());
    }

    IEnumerator CountDownAndMove()
    {
        if (count3) count3.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (count3) count3.SetActive(false);

        if (count2) count2.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (count2) count2.SetActive(false);

        if (count1) count1.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (count1) count1.SetActive(false);

        MoveToRandomScene();
    }

    void MoveToRandomScene()
    {
        if (gameSceneNames == null || gameSceneNames.Length == 0)
        {
            Debug.LogError("❌ 遷移先シーン未設定（2P）");
            return;
        }

        string next = gameSceneNames[Random.Range(0, gameSceneNames.Length)];
        SceneManager.LoadScene(next);
    }

    int GetId(Sprite sprite)
    {
        return System.Array.IndexOf(characterIcons, sprite);
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}
