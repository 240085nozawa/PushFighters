using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    [Header("表示させたいプレハブ")]
    public GameObject targetPrefab; // ここにAutoDestroyがついたプレハブを登録

    // 指定した座標にオブジェクトを出す関数
    public void SpawnObjectAt(Vector3 position)
    {
        if (targetPrefab != null)
        {
            // プレハブを、指定した位置(position)、回転なし(Quaternion.identity)で生成
            Instantiate(targetPrefab, position, Quaternion.identity);

            Debug.Log($"座標 {position} にオブジェクトを生成しました（3秒後に消えます）");
        }
        else
        {
            Debug.LogWarning("プレハブが設定されていません！");
        }
    }

    // --- テスト用: スペースキーを押すと (0, 2, 0) に出る ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObjectAt(new Vector3(0, 0, 0));
        }
    }
}