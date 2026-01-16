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
    /// 抽選開始
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

    void StopLottery()
    {
        if (!isRolling) return;

        isRolling = false;

        if (rollCoroutine != null)
            StopCoroutine(rollCoroutine);

        if (rollingEffectInstance != null)
            Destroy(rollingEffectInstance);

        SaveResult();
        MoveToRandomScene();
    }

    /// <summary>
    /// 抽選結果を保存（キャラID＋スキルID）
    /// </summary>
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

    /// <summary>
    /// Sprite → キャラID
    /// </summary>
    int GetCharacterIndex(Sprite sprite)
    {
        for (int i = 0; i < characterIcons.Length; i++)
        {
            if (characterIcons[i] == sprite)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// キャラID → スキルID（固定）
    /// </summary>
    int GetSkillFromCharacter(int characterId)
    {
        switch (characterId)
        {
            case 0: return 0; // ビーム
            case 1: return 2; // 自爆
            case 2: return 3; // カウンター
            case 3: return 1; // 停止
            default: return -1;
        }
    }

    /// <summary>
    /// ランダムシーン移動
    /// </summary>
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
