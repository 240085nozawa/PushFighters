// GimmickManager.cs
using System.Collections.Generic;
using UnityEngine;

public class GimmickManager : MonoBehaviour
{
    [Header("FloorColorManager をアサインしてください")]
    public FloorColorManager floorColorManager;

    [Header("Scene上のギミックコンポーネントをここへ登録")]
    public List<MonoBehaviour> gimmickComponents = new List<MonoBehaviour>();

    private List<IGimmick> gimmicks = new List<IGimmick>();
    private IGimmick currentGimmick = null;

    private void Awake()
    {
        // MonoBehaviour → IGimmick 抽出
        gimmicks.Clear();
        foreach (var mb in gimmickComponents)
        {
            if (mb == null) continue;

            if (mb is IGimmick g)
            {
                gimmicks.Add(g);
            }
            else
            {
                Debug.LogWarning($"GimmickManager: {mb.name} は IGimmick を実装していません");
            }
        }
    }

    private void OnEnable()
    {
        if (floorColorManager != null)
        {
            floorColorManager.OnPatternActivated += OnPatternActivated;
            floorColorManager.OnPatternDeactivated += OnPatternDeactivated;
        }
    }

    private void OnDisable()
    {
        if (floorColorManager != null)
        {
            floorColorManager.OnPatternActivated -= OnPatternActivated;
            floorColorManager.OnPatternDeactivated -= OnPatternDeactivated;
        }
    }

    private void OnPatternActivated(List<int> tileNumbers)
    {
        if (gimmicks.Count == 0)
        {
            Debug.LogWarning("GimmickManager: 登録されたギミックがありません");
            return;
        }

        currentGimmick = gimmicks[Random.Range(0, gimmicks.Count)];
        Debug.Log($"GimmickManager: 発動 → {currentGimmick.GetName()}");

        currentGimmick.Activate(tileNumbers);
    }

    private void OnPatternDeactivated()
    {
        if (currentGimmick != null)
        {
            Debug.Log($"GimmickManager: 終了 → {currentGimmick.GetName()}");
            currentGimmick.Deactivate();
            currentGimmick = null;
        }
    }
}