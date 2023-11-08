using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachineManager : MonoBehaviour
{
    
    public State currentState;
    public bool isThereNoise = false;
    public float noiseTimer = 0.5f;
    public GameManager gameManager;
    public Vector3 lastSoundPosition;


    void Update() {
        RunStateMachine();

        if(isThereNoise) {
            noiseTimer -= Time.deltaTime;
        }
        if(noiseTimer <= 0) {
            isThereNoise = false;
            noiseTimer = 0.5f;
        }
    }


    private void RunStateMachine(){
        State nextState = currentState?.RunCurrentState();

        if(nextState != null) {
            SwitchToNextState(nextState);
        }
    }

    private void SwitchToNextState(State nextState) {
        currentState = nextState;
    }
}
