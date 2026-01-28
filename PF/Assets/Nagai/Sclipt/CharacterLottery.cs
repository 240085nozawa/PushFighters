using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Input System必須
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

    [Header("効果音設定")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startPressSE;    // スペース押した瞬間のSE
    [SerializeField] private AudioClip loopSE;          // 2秒後ループSE

    private GameObject rollingEffectInstance;
    private Coroutine rollCoroutine;

    private bool isRolling = false;
    private bool canStop = false;
    private bool started = false;      // スタート済みフラグ
    private bool stopping = false;     // 停止フラグ

    void Start()
    {
        isRolling = false;
        canStop = false;
        started = false;
    }

    void Update()
    {
        // ---------------------------------------------------------
        // 入力チェック (キーボード & ゲームパッド)
        // ---------------------------------------------------------
        bool press = false;

        // ⌨️ キーボード
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                press = true;
            }
        }

        // 🎮 ゲームパッド (Aボタン / 南ボタン) ★ここを追加！
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                press = true;
            }
        }

        // 入力がなければ何もしない
        if (!press) return;

        // ---------------------------------------------------------
        // 処理実行
        // ---------------------------------------------------------
        if (!started)
        {
            StartLotteryCoroutineFromUpdate();
        }
        else if (!stopping && canStop)
        {
            StopLottery();
        }
    }

    /// <summary>
    /// UIから呼ばれる抽選開始コルーチン
    /// </summary>
    public void StartLotteryCoroutine()
    {
        StartCoroutine(StartLotteryCoroutineInternal());
    }

    /// <summary>
    /// Updateから呼ばれる抽選開始コルーチン
    /// </summary>
    void StartLotteryCoroutineFromUpdate()
    {
        StartCoroutine(StartLotteryCoroutineInternal());
    }

    IEnumerator StartLotteryCoroutineInternal()
    {
        started = true;

        // 1つ目のSE
        if (audioSource != null && startPressSE != null)
        {
            audioSource.PlayOneShot(startPressSE);
        }

        // 2秒待機
        yield return new WaitForSeconds(2f);

        // ループSE開始
        if (audioSource != null && loopSE != null)
        {
            audioSource.clip = loopSE;
            audioSource.loop = true;
            audioSource.Play();
        }

        // ルーレット開始
        StartRoll();
    }

    void StartRoll()
    {
        if (characterIcons == null || characterIcons.Length < slots.Length)
        {
            Debug.LogError("❌ キャラ画像が足りない");
            return;
        }

        isRolling = true;
        canStop = false;

        // 回転エフェクト生成
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

    // 必要に応じて外部から呼べるように public にしておきます
    public void StopLottery()
    {
        stopping = true;

        // ループSE停止
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        isRolling = false;

        if (rollCoroutine != null)
            StopCoroutine(rollCoroutine);

        if (rollingEffectInstance != null)
            Destroy(rollingEffectInstance);

        SaveResult();
        MoveToRandomScene();
    }

    void SaveResult()
    {
        SkillSelectionData.playerCount = slots.Length;

        SavePlayerData(0, slots[0].sprite);
        SavePlayerData(1, slots[1].sprite);

        if (slots.Length >= 3)
            SavePlayerData(2, slots[2].sprite);

        if (slots.Length >= 4)
            SavePlayerData(3, slots[3].sprite);

        Debug.Log("🎯 キャラ＆スキル保存完了");
    }

    void SavePlayerData(int playerIndex, Sprite sprite)
    {
        int charId = GetCharacterIndex(sprite);
        int skillId = GetSkillFromCharacter(charId);

        switch (playerIndex)
        {
            case 0:
                SkillSelectionData.p1Character = charId;
                SkillSelectionData.p1Skill = skillId;
                break;
            case 1:
                SkillSelectionData.p2Character = charId;
                SkillSelectionData.p2Skill = skillId;
                break;
            case 2:
                SkillSelectionData.p3Character = charId;
                SkillSelectionData.p3Skill = skillId;
                break;
            case 3:
                SkillSelectionData.p4Character = charId;
                SkillSelectionData.p4Skill = skillId;
                break;
        }
    }

    int GetCharacterIndex(Sprite sprite)
    {
        for (int i = 0; i < characterIcons.Length; i++)
        {
            if (characterIcons[i] == sprite)
                return i;
        }
        return -1;
    }

    int GetSkillFromCharacter(int characterId)
    {
        switch (characterId)
        {
            case 0: return 0;
            case 1: return 2;
            case 2: return 3;
            case 3: return 1;
            default: return -1;
        }
    }

    void MoveToRandomScene()
    {
        if (gameScenes == null || gameScenes.Length == 0)
        {
            Debug.LogError("❌ 移動先シーンが未設定");
            return;
        }

        string sceneName = gameScenes[Random.Range(0, gameScenes.Length)];
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