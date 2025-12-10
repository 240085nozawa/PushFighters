using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FloorColorManager : MonoBehaviour
{
    [Header("デフォルト状態のマテリアル")]
    public Material defaultMaterial;

    [Header("色が変わるまでの秒数 (例:10秒)")]
    public float changeInterval = 10f;

    [Header("点滅開始は colorChangeTime - blinkDuration")]
    public float blinkDuration = 2f;

    [Header("点滅スピード (例:0.15秒)")]
    public float blinkSpeed = 0.15f;

    [Header("色確定後に維持する秒数 (例:2秒)")]
    public float activeDuration = 2f;


    // =============================
    // パターン定義
    // =============================
    [System.Serializable]
    public class TilePattern
    {
        public string patternName;          // パターン名
        public List<int> tileNumbers;       // このパターンに属する番号
        public Material applyMaterial;      // このパターンで使用するマテリアル
    }

    [Header("色変化パターン一覧（自由に追加可能）")]
    public List<TilePattern> patterns = new List<TilePattern>();


    // =============================
    // ギミック発動イベント
    // =============================
    public delegate void PatternActivated(List<int> tiles);
    public event PatternActivated OnPatternActivated;

    public delegate void PatternDeactivated();
    public event PatternDeactivated OnPatternDeactivated;


    // =============================
    // 内部用
    // =============================
    private Dictionary<int, Renderer> tileMap = new Dictionary<int, Renderer>();
    private TilePattern currentPattern = null;

    private float timer = 0f;
    private bool blinking = false;
    private bool colorActive = false;


    // ==========================================
    // 初期化
    // ==========================================
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (int.TryParse(child.name, out int num))
            {
                Renderer ren = child.GetComponent<Renderer>();
                if (ren)
                {
                    tileMap[num] = ren;
                    ren.material = defaultMaterial;
                }
            }
        }

        Debug.Log($"登録タイル数: {tileMap.Count}");
    }


    // ==========================================
    // メインループ
    // ==========================================
    void Update()
    {
        timer += Time.deltaTime;

        float blinkStart = changeInterval - blinkDuration;

        // -------------------------------
        // ★ 点滅開始
        // -------------------------------
        if (!blinking && !colorActive && timer >= blinkStart)
        {
            SelectRandomPattern();
            StartCoroutine(BlinkRoutine());
            blinking = true;
        }

        // -------------------------------
        // ★ 10秒 → 色確定 & ギミック発動
        // -------------------------------
        if (!colorActive && timer >= changeInterval)
        {
            ApplyFinalColor();
            colorActive = true;
            blinking = false;

            OnPatternActivated?.Invoke(currentPattern.tileNumbers);
        }

        // -------------------------------
        // ★ (10 + activeDuration)秒 → デフォルトへ戻す
        // -------------------------------
        if (colorActive && timer >= (changeInterval + activeDuration))
        {
            RevertToDefault();
            colorActive = false;

            OnPatternDeactivated?.Invoke();

            timer = 0f; // 次サイクルへ
        }
    }


    // ==========================================
    // ★ パターンをランダム選択
    // ==========================================
    private void SelectRandomPattern()
    {
        if (patterns.Count == 0)
        {
            Debug.LogWarning("パターンが設定されていません！");
            return;
        }

        int idx = UnityEngine.Random.Range(0, patterns.Count);
        currentPattern = patterns[idx];

        Debug.Log($"選択されたパターン: {currentPattern.patternName}");
    }


    // ==========================================
    // ★ 点滅（0.15秒周期など）
    // ==========================================
    private IEnumerator BlinkRoutine()
    {
        float elapsed = 0f;
        bool toggle = false;

        while (elapsed < blinkDuration)
        {
            foreach (int num in currentPattern.tileNumbers)
            {
                if (tileMap.ContainsKey(num))
                {
                    tileMap[num].material = toggle ? defaultMaterial : currentPattern.applyMaterial;
                }
            }

            toggle = !toggle;

            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }
    }


    // ==========================================
    // ★ 10秒 → 色最終確定
    // ==========================================
    private void ApplyFinalColor()
    {
        foreach (int num in currentPattern.tileNumbers)
        {
            if (tileMap.ContainsKey(num))
            {
                tileMap[num].material = currentPattern.applyMaterial;
            }
        }
        Debug.Log($"[FloorColorManager] パターン ({currentPattern.patternName}) 色確定");
    }


    // ==========================================
    // ★ デフォルトへ戻す
    // ==========================================
    private void RevertToDefault()
    {
        foreach (Renderer ren in tileMap.Values)
        {
            ren.material = defaultMaterial;
        }

        Debug.Log("[FloorColorManager] デフォルトへ戻した");
    }
}
