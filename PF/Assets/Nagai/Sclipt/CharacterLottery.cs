using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterLottery : MonoBehaviour
{
    [Header("キャラ画像")]
    [SerializeField] private Sprite[] characterIcons;

    [Header("表示スロット")]
    [SerializeField] private Image[] slots;

    [Header("回転間隔")]
    [SerializeField] private float interval = 0.1f;

    [Header("回転エフェクト")]
    [SerializeField] private GameObject rollingEffectPrefab;
    [SerializeField] private Transform effectParent;

    [Header("移動先シーン")]
    [SerializeField] private string[] gameScenes;

    [Header("SE")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startPressSE;
    [SerializeField] private AudioClip loopSE;

    [Header("SE音量設定")]
    [Range(0f, 100f)]
    [SerializeField] private float seVolume = 1.5f; // ★ここで音量を上げられる★

    private Coroutine rollCoroutine;
    private GameObject rollingEffectInstance;

    private bool started = false;
    private bool isRolling = false;
    private bool canStop = false;
    private bool stopping = false;

    void Update()
    {
        bool press = false;

        // ⌨️ キーボード
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame)
            {
                press = true;
            }
        }

        // 🎮 コントローラー（南ボタン）
        if (Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            press = true;
        }

        if (!press) return;

        if (!started)
        {
            StartLotteryCoroutine();
        }
        else if (canStop && !stopping)
        {
            StopLottery();
        }
    }

    public void StartLotteryCoroutine()
    {
        if (!started)
            StartCoroutine(StartLottery());
    }

    IEnumerator StartLottery()
    {
        started = true;

        // 🔊 開始SE
        if (audioSource && startPressSE)
            audioSource.PlayOneShot(startPressSE, seVolume);

        yield return new WaitForSeconds(2f);

        // 🔊 ループSE
        if (audioSource && loopSE)
        {
            audioSource.clip = loopSE;
            audioSource.volume = seVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        StartRoll();
    }

    void StartRoll()
    {
        isRolling = true;
        canStop = false;

        if (rollingEffectPrefab && effectParent)
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
                slots[i].sprite = shuffled[i];

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
        stopping = true;
        isRolling = false;

        // 🔇 SE停止
        if (audioSource)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }

        if (rollCoroutine != null)
            StopCoroutine(rollCoroutine);

        if (rollingEffectInstance)
            Destroy(rollingEffectInstance);

        SaveResult();
        StartCoroutine(WaitAndMoveScene());
    }

    IEnumerator WaitAndMoveScene()
    {
        yield return new WaitForSeconds(1.0f);
        MoveToRandomScene();
    }

    void SaveResult()
    {
        SkillSelectionData.playerCount = slots.Length;

        for (int i = 0; i < slots.Length; i++)
        {
            int charId = GetCharacterIndex(slots[i].sprite);
            int skillId = GetSkillFromCharacter(charId);

            switch (i)
            {
                case 0: SkillSelectionData.p1Character = charId; SkillSelectionData.p1Skill = skillId; break;
                case 1: SkillSelectionData.p2Character = charId; SkillSelectionData.p2Skill = skillId; break;
                case 2: SkillSelectionData.p3Character = charId; SkillSelectionData.p3Skill = skillId; break;
                case 3: SkillSelectionData.p4Character = charId; SkillSelectionData.p4Skill = skillId; break;
            }
        }
    }

    int GetCharacterIndex(Sprite sprite)
    {
        for (int i = 0; i < characterIcons.Length; i++)
            if (characterIcons[i] == sprite) return i;
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
        if (gameScenes.Length > 0)
        {
            SceneManager.LoadScene(
                gameScenes[Random.Range(0, gameScenes.Length)]
            );
        }
        else
        {
            Debug.LogError("移動先のシーン(Game Scenes)が設定されていません！");
        }
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
