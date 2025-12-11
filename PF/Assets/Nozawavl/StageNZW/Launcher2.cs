using UnityEngine;

public class Launcher2 : MonoBehaviour
{
    [Header("ˆÚ“®”ÍˆÍÝ’èiInspector‚ÅÝ’èj")]
    public float minX = 0f;        // ZŽ²‚ÌˆÚ“®”ÍˆÍiÅ¬j
    public float maxX = 10f;       // ZŽ²‚ÌˆÚ“®”ÍˆÍiÅ‘åj
    public float moveSpeed = 3f;   // ZŽ²ˆÚ“®ƒXƒs[ƒh

    [Header("Lightning ”­ŽËÝ’è")]
    public GameObject lightningPrefab; // ”­ŽË‚·‚éLightning
    public Transform firePoint;        // ”­ŽËˆÊ’u

    [Header("’âŽ~ŠÔŠuiƒ‰ƒ“ƒ_ƒ€Ý’èj")]
    public float minStopInterval = 3f; // Å¬’âŽ~ŠÔŠu
    public float maxStopInterval = 6f; // Å‘å’âŽ~ŠÔŠu

    private float stopInterval;        // Œ»Ý‚Ì’âŽ~ŠÔŠui–ˆ‰ñƒ‰ƒ“ƒ_ƒ€Œˆ’èj
    private bool movingForward = true; // ZŽ²³•ûŒü‚ÖˆÚ“®’†‚©H
    private bool isStopped = false;    // ’âŽ~ó‘Ô‚©H
    private float timer = 0f;          // ’âŽ~ƒ^ƒCƒ}[

    void Start()
    {
        // Å‰‚Ì’âŽ~ŽžŠÔ‚àƒ‰ƒ“ƒ_ƒ€‚ÅÝ’è
        stopInterval = Random.Range(minStopInterval, maxStopInterval);
    }

    void Update()
    {
        // ’âŽ~ó‘Ô‚Ìˆ—
        if (isStopped)
        {
            timer += Time.deltaTime;

            // ’âŽ~‚µ‚Ä 1 •bŒã‚É Lightning ”­ŽË
            if (timer >= 1f)
            {
                FireLightning();

                timer = 0f;
                isStopped = false;

                // ŽŸ‚Ì’âŽ~ŠÔŠu‚àƒ‰ƒ“ƒ_ƒ€‚ÅÝ’èi–ˆ‰ñ•Ï‚í‚éj
                stopInterval = Random.Range(minStopInterval, maxStopInterval);
            }

            return; // ’âŽ~’†‚ÍˆÚ“®‚µ‚È‚¢
        }

        // ZŽ²‚Ì‰•œˆÚ“®
        MoveXAxis();

        // ŽžŠÔŒo‰ß‚Å’âŽ~‚Ö
        timer += Time.deltaTime;
        if (timer >= stopInterval)
        {
            timer = 0f;
            isStopped = true;
        }
    }

    // ZŽ²•ûŒü‚Ì‰•œˆÚ“®
    void MoveXAxis()
    {
        Vector3 pos = transform.position;

        if (movingForward)
        {
            pos.x += moveSpeed * Time.deltaTime;

            if (pos.x >= maxX)
            {
                pos.x    = maxX;
                movingForward = false;
            }
        }
        else
        {
            pos.x -= moveSpeed * Time.deltaTime;

            if (pos.x <= minX)
            {
                pos.x = minX;
                movingForward = true;
            }
        }

        transform.position = pos;
    }

    // Lightning ‚ð”­ŽË
    void FireLightning()
    {
        Instantiate(lightningPrefab, firePoint.position, firePoint.rotation);
    }
}
