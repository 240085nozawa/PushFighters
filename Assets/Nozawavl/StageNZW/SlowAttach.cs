using UnityEngine;
using System.Collections;

public class SlowAttach : MonoBehaviour
{
    private PlayerController pc;
    private float originalSpeed;
    private Coroutine routine;

    public void ApplySlow(PlayerController target, float multiplier, float duration)
    {
        pc = target;

        // moveSpeed の元値を保存
        originalSpeed = pc.moveSpeed;

        // すでに SlowRoutine 動いてたら止める（重複防止）
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(SlowRoutine(multiplier, duration));
    }

    private IEnumerator SlowRoutine(float multiplier, float duration)
    {
        // 遅くする
        pc.moveSpeed = pc.moveSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        // 元の速度に戻す
        pc.moveSpeed = originalSpeed;

        // コンポーネント自体を削除（キレイに終わる）
        Destroy(this);
    }
}
