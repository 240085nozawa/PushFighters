using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    [Header("設定")]
    public string battleSceneName = "BattleScene";
    public Image[] characterImages;    // キャラクターの画像リスト

    [Header("UI設定")]
    public TextMeshProUGUI promptText; // 全体メッセージ

    [Header("P1 UI")]
    public RectTransform p1Cursor;     // P1カーソル
    public GameObject p1ReadyText;     // P1「OK!」の表示

    [Header("P2 UI")]
    public RectTransform p2Cursor;     // P2カーソル
    public GameObject p2ReadyText;     // P2「OK!」の表示

    // --- 内部変数 ---
    private int p1Index = 0;
    private int p2Index = 0;

    private bool p1Ready = false;
    private bool p2Ready = false;

    // 入力遅延用 (P1, P2 個別に管理)
    private float p1InputTimer = 0f;
    private float p2InputTimer = 0f;
    private float inputDelay = 0.2f;

    private void Awake()
    {
        // 初期化
        GameData.PlayerCount = 2;
        p1Ready = false;
        p2Ready = false;
    }

    void Start()
    {
        UpdateVisuals();
    }

    void Update()
    {
        // 両方準備完了なら何もしない（あるいはカウントダウン待機）
        if (p1Ready && p2Ready)
        {
            StartGame();
            return;
        }

        HandleP1Input();
        HandleP2Input();
    }

    // --- P1の入力処理 ---
    void HandleP1Input()
    {
        if (p1Ready) // 決定済みならキャンセルのみ受付
        {
            if (Input.GetButtonDown("Cancel")) // Bボタンなど
            {
                p1Ready = false;
                UpdateVisuals();
            }
            return;
        }

        // 決定 (Punch = Aボタン / Enter)
        // ※Input Managerで "Punch" が Joystick1 Button 0 に設定されている前提
        if (Input.GetButtonDown("Punch") || Input.GetKeyDown(KeyCode.Return))
        {
            p1Ready = true;
            UpdateVisuals();
            return;
        }

        // 横移動
        float h = Input.GetAxisRaw("Horizontal");
        if (p1InputTimer > 0) p1InputTimer -= Time.deltaTime;

        if (p1InputTimer <= 0 && Mathf.Abs(h) > 0.5f)
        {
            if (h > 0) p1Index++; else p1Index--;

            // ループ処理
            if (p1Index >= characterImages.Length) p1Index = 0;
            if (p1Index < 0) p1Index = characterImages.Length - 1;

            p1InputTimer = inputDelay;
            UpdateVisuals();
        }
        else if (Mathf.Abs(h) < 0.1f)
        {
            p1InputTimer = 0; // スティックを戻したら即入力可
        }
    }

    // --- P2の入力処理 ---
    void HandleP2Input()
    {
        if (p2Ready)
        {
            if (Input.GetButtonDown("Cancel")) // P2_Cancelがあればベスト
            {
                p2Ready = false;
                UpdateVisuals();
            }
            return;
        }

        // 決定 (P2_Punch = 右コントローラー Aボタン)
        // ※Input Managerで "P2_Punch" が Joystick2 Button 0 に設定されている前提
        if (Input.GetButtonDown("P2_Punch"))
        {
            p2Ready = true;
            UpdateVisuals();
            return;
        }

        // 横移動
        float h = Input.GetAxisRaw("P2_Horizontal");
        if (p2InputTimer > 0) p2InputTimer -= Time.deltaTime;

        if (p2InputTimer <= 0 && Mathf.Abs(h) > 0.5f)
        {
            if (h > 0) p2Index++; else p2Index--;

            // ループ処理
            if (p2Index >= characterImages.Length) p2Index = 0;
            if (p2Index < 0) p2Index = characterImages.Length - 1;

            p2InputTimer = inputDelay;
            UpdateVisuals();
        }
        else if (Mathf.Abs(h) < 0.1f)
        {
            p2InputTimer = 0;
        }
    }

    // --- 表示の更新 ---
    void UpdateVisuals()
    {
        // カーソル位置の更新
        if (characterImages.Length > 0)
        {
            if (p1Cursor != null)
                p1Cursor.position = characterImages[p1Index].transform.position;

            if (p2Cursor != null)
                p2Cursor.position = characterImages[p2Index].transform.position;
        }

        // Readyテキストの表示/非表示
        if (p1ReadyText != null) p1ReadyText.SetActive(p1Ready);
        if (p2ReadyText != null) p2ReadyText.SetActive(p2Ready);

        // ガイドテキスト更新
        if (p1Ready && p2Ready)
        {
            promptText.text = "BATTLE START!";
        }
        else
        {
            promptText.text = "キャラクターを選択してください";
        }
    }

    void StartGame()
    {
        // データを保存
        GameData.Player1CharacterIndex = p1Index;
        GameData.Player2CharacterIndex = p2Index;

        Debug.Log($"P1:{p1Index}, P2:{p2Index} でバトル開始！");

        // シーン遷移
        SceneManager.LoadScene(battleSceneName);
    }
}