using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();

        // ★追加：ゲーム開始時は画像を非表示（見えない状態）にしておく
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.enabled = false;
        }
    }

    void Update()
    {
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
        if (Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.LeftShift))
        {
            Time.timeScale = fastForwardNum;
        }

        // ★追加：押した瞬間に画像を表示（enabled = true）にする
        if (Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (targetSpriteRenderer != null)
            {
                targetSpriteRenderer.enabled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            audioSource.PlayOneShot(audioClip[0]);
        }

        // ★追加：離した瞬間に画像を非表示（enabled = false）にする
        if (Input.GetKeyUp(KeyCode.JoystickButton5) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1;

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
}