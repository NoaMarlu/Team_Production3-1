using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ★追加：アニメーションがついているオブジェクトの SpriteRenderer をここに登録する
    public SpriteRenderer targetSpriteRenderer;

    public float fastForwardNum = 2;
    public float GameTimer = 0;
    public float GameTime = 0;

    private SheepSpawner sheepSpawner;
    private AudioSource audioSource;
    public AudioClip[] audioClip;
    public float[] SEVolume;

    /*StartAnimation*/
    public bool isStartAnimation = true;
    private float animeTime = 2.8f;
    private float animeTimer = 0;
    public GameObject animationObj;
    private GameObject animationInstance;

    /*FastForward*/
    private bool isFast=false;

    void Start()
    {
        animationInstance=Instantiate(animationObj);
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip[1]);
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();

        sheepSpawner.sheepCount = 1;

        // ★追加：ゲーム開始時は画像を非表示（見えない状態）にしておく
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.enabled = false;
        }

    }
    void Update()
    {
        StartAnimation();
        GameReset();

        if (isStartAnimation) return;

        FastForward();
        GameTimer += Time.deltaTime;
        if (GameTimer > GameTime)
        {
            if (GameTime != 0 && sheepSpawner.isNotDieSheep() == false)
            {
                sheepSpawner.IsLoopSpawn();
                GameTimer = 0;
            }
        }

    }

    // 早送り
    void FastForward()
    {
        // R1か左Shiftを押している間のみ早送り
        if (Input.GetAxis("RT") > 0.5f || Input.GetKey(KeyCode.LeftShift))
        {
             Time.timeScale = fastForwardNum;

            if (!isFast)
            {
                if (targetSpriteRenderer != null)
                {
                    targetSpriteRenderer.enabled = true;
                }
                audioSource.PlayOneShot(audioClip[0]);
                isFast = true;
            }
        }

        // ★追加：離した瞬間に画像を非表示（enabled = false）にする
        if (Input.GetAxis("RT") <=0|| Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1;
            isFast = false;

            if (targetSpriteRenderer != null)
            {
                targetSpriteRenderer.enabled = false;
            }
        }
    }
    public float GetGameTimer() { return GameTimer; }
    public void GameTimerReset() { GameTimer = 0; }
    public float GetGameTime() { return GameTime; }
    public void SetGameTime(float num) { GameTime = num; }
    //開始時のレディGO!アニメーション
    void StartAnimation()
    {
        if (isStartAnimation != true) return;
        animeTimer += Time.deltaTime;
        if (animeTimer > animeTime)
        {
            Destroy(animationInstance);
            isStartAnimation = false;
        }
    }
    void GameReset()
    {
        //リセット
        if (Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

}