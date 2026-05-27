using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] audioClip;



    void Start()
    {
        PlayerPrefs.DeleteAll();
        audioSource = GetComponent<AudioSource>();
        Prefs();
    }

    void Update()
    {
        // 入力処理
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
        {
            audioSource.PlayOneShot(audioClip[0]);
            SceneManager.LoadScene("StageSelect");
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.P))
        {
            SceneManager.LoadScene("OneHundredStage");
        }
    }

    void Prefs()
    {
        PlayerPrefs.SetInt("STAGE2_Count",0);
        PlayerPrefs.SetInt("STAGE2_Star",0);
        PlayerPrefs.SetInt("STAGE4_Count",0);
        PlayerPrefs.SetInt("STAGE4_Star",0);
        PlayerPrefs.SetInt("CurrentStage", 0);
        PlayerPrefs.SetInt("isAnimation", 0);
        PlayerPrefs.Save();
    }


}