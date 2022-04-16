//Copyright (C) 2022 Shatrujit Aditya Kumar, All Rights Reserved
using UnityEngine;

public class ResetAtEnd : MonoBehaviour
{
    CarMotorScript resetTarget;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if the trigger object is the car
        if (collision.gameObject.TryGetComponent<CarMotorScript>(out resetTarget))
        {
            //If it is, call it's publicc reset method
            resetTarget.ResetCar();
        }
    }
}
