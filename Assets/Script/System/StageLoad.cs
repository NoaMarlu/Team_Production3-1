using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoad : MonoBehaviour
{
    public string sceneName;

    private AudioSource audioS;
    public AudioClip resultSE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioS = GetComponent<AudioSource>();
        audioS.PlayOneShot(resultSE);
    }

    // Update is called once per frame
    void Update()
    {
        {
            if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                if (BGM.Instance != null)
                {
                    BGM.Instance.PlayBGM();
                }
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
