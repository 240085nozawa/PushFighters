using UnityEngine;

public class AnimSwitch : MonoBehaviour
{
    public Animator animator;  // Inspector‚ÅAnimator‚ðƒhƒ‰ƒbƒO
    float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 60f)
        {
            animator.SetBool("Switch", true);
        }
    }
}
