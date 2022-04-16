//Copyright (C) 2022 Shatrujit Aditya Kumar, All Rights Reserved
using UnityEngine;

public class CarMotorScript : MonoBehaviour
{
    //References to the front and rear wheel joints
    [SerializeField] WheelJoint2D frontWheelJoint, rearWheelJoint;
    [SerializeField] float speedMultiplier = 1, //Multiplied with input to get motr speed
                           jumpForce = 1, //Impulse added on jump key
                           rotationSpeed = 5, //Speed with which the car rotates
                           groundingDistance = 0.5f; //Distance of the wheel centre from the ground to be considered grounded

    //Layer containing the objects that make up the track
    [SerializeField] LayerMask trackLayer;

    Vector3 startingPostion, //Cache starting position of the car to reset to later
            rotationAxis; //Axis about which the car rotates

    float speed, //Speed to set to the motors
          inputInverted; //Cache input axis value * -1

    bool shouldJump, //Whether car should jump in the next fixed update
         isGrounded; //Whether the car is on the ground

    Rigidbody2D carRigidbody; //Reference to the car's rigid body
    GameObject frontWheel, rearWheel; //References to the front and rear wheel object, for ground check

    JointMotor2D carMotor; //Cached motor values to be applied to both wheel joints

    // Start is called before the first frame update
    void Start()
    {
        //Initialize internal variables
        speed = 0;
        isGrounded = true;
        carRigidbody = gameObject.GetComponent<Rigidbody2D>();
        startingPostion = transform.position;
        rotationAxis = new Vector3(0, 0, 1);

        carMotor = new JointMotor2D
        {
            motorSpeed = 0,
            maxMotorTorque = rearWheelJoint.motor.maxMotorTorque
        };

        //Get refernces to the front and rear wheel game objects
        frontWheel = frontWheelJoint.connectedBody.gameObject;
        rearWheel = rearWheelJoint.connectedBody.gameObject;

        //Set the wheel joint's motors
        UpdateMotorSpeed(speed);
    }

    // Update is called once per frame
    void Update()
    {
        //Get horizontal axis (times -1) and calculate speed
        inputInverted = -Input.GetAxis("Horizontal");
        speed = inputInverted * speedMultiplier;

        //Check for jump key input
        shouldJump = Input.GetButton("Jump");
    }

    private void FixedUpdate()
    {
        //Check if we're grounded, cast rays from both wheel centres in the car's down direction (-1 * transform.up)
        //Check against a predetermined distance and layermask
        if (Physics2D.Raycast(frontWheel.transform.position, -transform.up, groundingDistance, trackLayer.value) || Physics2D.Raycast(rearWheel.transform.position, -transform.up, groundingDistance, trackLayer.value))
        {
            //Set is grounded and draw debug rays in green
            isGrounded = true;
            Debug.DrawRay(frontWheel.transform.position, -transform.up * groundingDistance, Color.green);
            Debug.DrawRay(rearWheel.transform.position, -transform.up * groundingDistance, Color.green);
        }
        else
        {
            //Set is grounded and draw debug rays in red if not grounded
            isGrounded = false;
            Debug.DrawRay(frontWheel.transform.position, -transform.up * groundingDistance, Color.red);
            Debug.DrawRay(rearWheel.transform.position, -transform.up * groundingDistance, Color.red);
        }

        //Update motor speed
        UpdateMotorSpeed(speed);

        if (!isGrounded)
        {
            //If not grounded, let the player rotate in the air
            transform.Rotate(rotationAxis, inputInverted * rotationSpeed * Time.fixedDeltaTime);

            //If velocity is almost zero and we're not grounded, car is probably flipped so reset the rotation
            if (Mathf.Approximately(carRigidbody.velocity.sqrMagnitude, 0f))
            {
                ResetRotation();
            }
        }
        //If we're grounded and we get a jumpt input, apply an impulse in the up direction
        else if (shouldJump)
            carRigidbody.AddForce(jumpForce * transform.up, ForceMode2D.Impulse);
    }

    //Update motor speed based on input and apply to the wheel joints
    void UpdateMotorSpeed(float newSpeed)
    {
        carMotor.motorSpeed = newSpeed;
        frontWheelJoint.motor = carMotor;
        rearWheelJoint.motor = carMotor;
    }

    //Called by the end level trigger to reset our position, rotation, and speed
    public void ResetCar()
    {
        transform.position = startingPostion;
        UpdateMotorSpeed(0);
        ResetRotation();
    }

    //Reset rotation to identity
    void ResetRotation() {
        transform.rotation = Quaternion.identity;
    }
}
