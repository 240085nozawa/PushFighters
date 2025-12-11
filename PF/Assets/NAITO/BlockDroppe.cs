using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDropper : MonoBehaviour
{
    // 4つのグループをInspectorでドラッグしてセット
    public Transform group1, group2, group3, group4;
    private List<GameObject>[] blocksGroups = new List<GameObject>[4];

    private List<GameObject> blinkingBlocks = new List<GameObject>(); // 今点滅してるブロック
    private List<Material> blinkingMaterials = new List<Material>();  // 点滅用マテリアル
    private List<Color> originalColors = new List<Color>();           // 各元色（ベースカラー）
    private List<Color> originalEmissionColors = new List<Color>();   // 各元Emission色

    private float gameTime;
    private float dropInterval;
    private float timer;
    private float blinkDuration = 5f;         // 5秒間点滅
    private float blinkTimer;
    private bool isBlinking = false;          // 点滅中フラグ

    public Color blinkColor = Color.red;      // 点滅時のEmission色
    public float blinkInterval = 0.5f;
    public Transform dropHole;

    void Start()
    {
        blocksGroups[0] = new List<GameObject>();
        blocksGroups[1] = new List<GameObject>();
        blocksGroups[2] = new List<GameObject>();
        blocksGroups[3] = new List<GameObject>();

        CollectAllBlocks(group1, blocksGroups[0]);
        CollectAllBlocks(group2, blocksGroups[1]);
        CollectAllBlocks(group3, blocksGroups[2]);
        CollectAllBlocks(group4, blocksGroups[3]);

        gameTime = 0f;
        dropInterval = 10f;
        timer = 0f;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        timer += Time.deltaTime;

        // 60秒まで10秒、60秒以降5秒間隔
        if (gameTime > 60f)
            dropInterval = 5f;
        else
            dropInterval = 10f;

        // 落下5秒前から点滅開始
        if (!isBlinking && timer >= dropInterval - blinkDuration)
        {
            timer = dropInterval - blinkDuration;
            StartBlinkingBlocks(2);
        }

        if (isBlinking)
        {
            blinkTimer += Time.deltaTime;
            HandleBlinking();

            if (blinkTimer >= blinkDuration)
            {
                StopBlinkingBlocks();
                for (int i = 0; i < blinkingBlocks.Count; i++)
                {
                    StartCoroutine(FallAndDisappear(blinkingBlocks[i]));
                }
                timer = 0f;
            }
        }
        else if (timer >= dropInterval)
        {
            timer = 0f;
            // 点滅省略時2個即落下（残りブロックがblinkDuration未満でタイミングがずれる場合用）
            DropRandomBlocksFromGroups(2);
        }
    }

    // 再帰で指定グループ内のブロック集め
    void CollectAllBlocks(Transform parent, List<GameObject> resultList)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains("pCube"))
            {
                resultList.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                CollectAllBlocks(child, resultList);
            }
        }
    }

    // 2個分点滅対象を各グループからランダム抽選
    void StartBlinkingBlocks(int count)
    {
        blinkingBlocks.Clear();
        blinkingMaterials.Clear();
        originalColors.Clear();
        originalEmissionColors.Clear();

        for (int i = 0; i < count; i++)
        {
            // ランダムグループ番号
            int groupIndex = Random.Range(0, 4);
            List<GameObject> groupBlocks = blocksGroups[groupIndex];

            int safeLoop = 0;
            while (groupBlocks.Count == 0 && safeLoop < 4)
            {
                groupIndex = (groupIndex + 1) % 4;
                groupBlocks = blocksGroups[groupIndex];
                safeLoop++;
            }
            if (groupBlocks.Count == 0) continue;

            // グループ中でランダム抽選
            int blockIndex = Random.Range(0, groupBlocks.Count);
            GameObject block = groupBlocks[blockIndex];
            groupBlocks.RemoveAt(blockIndex);

            blinkingBlocks.Add(block);

            Material mat = block.GetComponent<Renderer>().material;
            blinkingMaterials.Add(mat);

            // ベースカラーとEmissionカラーを保存
            originalColors.Add(mat.color);
            if (mat.HasProperty("_EmissionColor"))
            {
                originalEmissionColors.Add(mat.GetColor("_EmissionColor"));
                // 念のためEmission有効化
                mat.EnableKeyword("_EMISSION");
            }
            else
            {
                // プロパティが無い場合のフォールバック
                originalEmissionColors.Add(Color.black);
            }
        }
        blinkTimer = 0f;
        isBlinking = true;
    }

    // 複数のブロックを赤⇔元Emission色で点滅
    void HandleBlinking()
    {
        float t = blinkTimer % (blinkInterval * 2f);

        for (int i = 0; i < blinkingMaterials.Count; i++)
        {
            Material mat = blinkingMaterials[i];

            // ベースカラーは触らない
            mat.color = originalColors[i];

            if (mat.HasProperty("_EmissionColor"))
            {
                Color target = (t < blinkInterval) ? blinkColor : originalEmissionColors[i];
                mat.SetColor("_EmissionColor", target);
            }
        }
    }

    void StopBlinkingBlocks()
    {
        for (int i = 0; i < blinkingMaterials.Count; i++)
        {
            Material mat = blinkingMaterials[i];

            // ベースカラーを元に戻す
            mat.color = originalColors[i];

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", originalEmissionColors[i]);
            }
        }
        isBlinking = false;
    }

    // 5秒かけて落下・非表示
    IEnumerator FallAndDisappear(GameObject block)
    {
        Vector3 startPos = block.transform.position;
        Vector3 endPos = new Vector3(startPos.x, -10f, startPos.z);
        float duration = 3f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            block.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        block.transform.position = endPos;
        block.SetActive(false);
    }

    // 点滅をスキップして即落下（点滅タイミング外や全消化対応用）
    void DropRandomBlocksFromGroups(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int groupIndex = Random.Range(0, 4);
            List<GameObject> groupBlocks = blocksGroups[groupIndex];

            int safeLoop = 0;
            while (groupBlocks.Count == 0 && safeLoop < 4)
            {
                groupIndex = (groupIndex + 1) % 4;
                groupBlocks = blocksGroups[groupIndex];
                safeLoop++;
            }
            if (groupBlocks.Count == 0) continue;

            int blockIndex = Random.Range(0, groupBlocks.Count);
            GameObject block = groupBlocks[blockIndex];
            groupBlocks.RemoveAt(blockIndex);

            StartCoroutine(FallAndDisappear(block));
        }
    }
}
