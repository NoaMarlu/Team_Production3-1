using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public float fastForwardNum = 2;
    public float GameTimer = 0;
    public float GameTime=0;

    private SheepSpawner sheepSpawner;

    void Start()
    {
        sheepSpawner = GameObject.FindWithTag("Spawner").GetComponent<SheepSpawner>();
    }
    void Update()
    {

        FastForward();

        GameTimer += Time.deltaTime;
        if (GameTimer > GameTime)
        {
            if(GameTime!=0 && sheepSpawner.isNotDieSheep()==false)
            {
                sheepSpawner.IsLoopSpawn();
                GameTimer = 0; 
            }
        }

    }

    //早送り
    void FastForward()
    {
        //R1を押している間のみ早送り
        if (Input.GetKey(KeyCode.JoystickButton5)||Input.GetKey(KeyCode.LeftShift)){ Time.timeScale = fastForwardNum;  }
        if (Input.GetKeyUp(KeyCode.JoystickButton5)|| Input.GetKeyUp(KeyCode.LeftShift)) { Time.timeScale = 1; }
    }
    public float GetGameTimer() {  return GameTimer; }
    public void GameTimerReset() { GameTimer=0; }
    public float GetGameTime() { return GameTime; }
    public void SetGameTime(float num) { GameTime = num; }

}
