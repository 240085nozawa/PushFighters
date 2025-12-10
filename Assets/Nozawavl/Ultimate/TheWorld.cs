using UnityEngine;
using System.Collections;

public class TheWorld : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("発動者のタグ（例：Player1）")]
    public string ownerTag;

    [Header("停止時間（秒）")]
    public float stopDuration = 5f;

    private bool isActive = false;
    private GameObject owner; // 自分自身を記憶
    private GameTimer gameTimer;

    void Start()
    {
        owner = this.gameObject; // 自身を登録
        // シーン上のGameTimerを探す
        gameTimer = FindObjectOfType<GameTimer>();
    }

    //void Update()
    //{
    //    // Spaceキーで発動
    //    if (Input.GetKeyDown(KeyCode.X))
    //    {
    //        Activate();
    //    }
    //}

    public void Activate()
    {
        if (isActive) return;

        Debug.Log("【The World】時よ止まれ…！！");
        isActive = true;
        StartCoroutine(StopTimeForOthers());
    }

    private IEnumerator StopTimeForOthers()
    {
        // ?? タイマー停止
        if (gameTimer != null)
            gameTimer.isStopped = true;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // 発動者は除外（タグ一致 or 参照一致）
            if (player == owner) continue;
            if (!string.IsNullOrEmpty(ownerTag) && player.CompareTag(ownerTag)) continue;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // PlayerControllerの行動停止
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.isTimeStopped = true;
            }
        }

        yield return new WaitForSeconds(stopDuration);

        // ?? タイマー再開
        if (gameTimer != null)
            gameTimer.isStopped = false;


        // 復帰処理
        foreach (GameObject player in players)
        {
            if (player == owner) continue;
            if (!string.IsNullOrEmpty(ownerTag) && player.CompareTag(ownerTag)) continue;

            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.isTimeStopped = false;
            }
        }

        Debug.Log("【The World】時は動き出す…");
        isActive = false;
    }
    //private void Update()
    //{
    //    // Update() は毎フレーム呼ばれる
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("Special thew 発動！");
    //        // 実際の攻撃処理などを書く
    //    }
    //}
}
