using UnityEngine;
using System.Collections;

public class RotationFrictionController : MonoBehaviour
{
    public PhysicsMaterial baseMaterial; // Inspectorで設定するベースのPhysicsMaterial

    private PhysicsMaterial instanceMaterial; // 複製した物理マテリアル
    private float originalDynamicFriction; // 元の動的摩擦値を保存
    private float originalStaticFriction;  // 元の静的摩擦値を保存

    private bool isCoroutineRunning = false; // コルーチンが動いているかの判定

    void Start()
    {
        var col = GetComponent<Collider>();
        if (col != null && baseMaterial != null)
        {
            // baseMaterialを複製してCollider.materialに割り当てる
            instanceMaterial = Instantiate(baseMaterial);
            col.material = instanceMaterial;

            // 元の摩擦値を保存しておく
            originalDynamicFriction = instanceMaterial.dynamicFriction;
            originalStaticFriction = instanceMaterial.staticFriction;

            // ゲーム開始から指定した時間で摩擦を変更するコルーチンを開始
            StartCoroutine(ScheduleFrictionChanges());
        }
        else
        {
            Debug.LogWarning("ColliderまたはbaseMaterialがInspectorで設定されていません。");
        }
    }

    private IEnumerator ScheduleFrictionChanges()
    {
        // 25秒後に摩擦値を3秒間1にする
        yield return new WaitForSeconds(25f);
        yield return ChangeFrictionTemporarily();

        // さらに27秒間待機
        yield return new WaitForSeconds(27f);
        yield return ChangeFrictionTemporarily();

        // さらに27秒間待機
        yield return new WaitForSeconds(27f);
        yield return ChangeFrictionTemporarily();

        // さらに27秒間待機
        yield return new WaitForSeconds(27f);
        yield return ChangeFrictionTemporarily();
    }

    private IEnumerator ChangeFrictionTemporarily()
    {
        if (isCoroutineRunning) yield break; // 多重起動防止

        isCoroutineRunning = true;

        // 動的・静的摩擦を1に設定
        instanceMaterial.dynamicFriction = 1f;
        instanceMaterial.staticFriction = 1f;

        // 3秒間その状態を維持
        yield return new WaitForSeconds(3f);

        // 元の摩擦値に戻す
        instanceMaterial.dynamicFriction = originalDynamicFriction;
        instanceMaterial.staticFriction = originalStaticFriction;

        isCoroutineRunning = false;
    }
}

// このスクリプトはゲーム開始から25秒、55秒、85秒、115秒のタイミングで、
// 3秒間動的摩擦と静的摩擦を1に高めて、
// 3秒経過後に元の摩擦数値に戻します。
// baseMaterialにはInspectorから物理マテリアルを割り当ててください。
// Collider.materialに複製インスタンスを割り当てているため、
// 変更が即時に有効で安全です。