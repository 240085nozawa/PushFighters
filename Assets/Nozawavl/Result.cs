using UnityEngine;
using UnityEngine.SceneManagement;
public class Result : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class ResultManager : MonoBehaviour
    {
        public void OnReturnToSelect()
        {
            //SpecialManager.Instance.ResetSpecials();
            SceneManager.LoadScene("SelectScene");
        }
    }
}
