using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachineManager : MonoBehaviour
{
    
    public State currentState;
    public bool isThereNoise = false;
    public float noiseTimer = 1;
    public GameManager gameManager;
    public Vector3 gavinPosition;


    void Update() {
        RunStateMachine();


        if(isThereNoise) {
            noiseTimer -= Time.deltaTime;
        }
        if(noiseTimer <= 0) {
            isThereNoise = false;
            noiseTimer = 1;
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
   public void NewNoise(float loudness, Vector3 gavinPosition) {

        this.gavinPosition = gavinPosition;

        if(Vector3.Distance(gavinPosition, transform.position) < loudness) {
            isThereNoise = true;
        }

    }
}
