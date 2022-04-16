//Copyright (C) 2022 Shatrujit Aditya Kumar, All Rights Reserved
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //Reference to the player controlled car
    [SerializeField] CarMotorScript playerCar;

    //Cache the offset between the camera and car
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {   
        //Get the refernce to the car if the serialized field is null
        if(playerCar == null)
        {
            playerCar = FindObjectOfType<CarMotorScript>();
        }

        //Calculate the offset between the camera and the car
        offset = transform.position - playerCar.transform.position;
    }

    void FixedUpdate()
    {
        //Maintain a constant pre-calculated offset between the camera and the car
        transform.position = playerCar.transform.position + offset;
    }
}
