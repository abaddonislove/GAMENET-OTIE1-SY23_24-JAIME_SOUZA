using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    public FixedTouchField fixedTouchField;
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        rigidbodyFirstPersonController = this.GetComponent<RigidbodyFirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rigidbodyFirstPersonController.JoystickInputAxis.x = joystick.Horizontal;
        rigidbodyFirstPersonController.JoystickInputAxis.y = joystick.Vertical;
        rigidbodyFirstPersonController.mouseLook.lookInputAxis = fixedTouchField.TouchDist;

        animator.SetFloat("Horizontal", joystick.Horizontal);
        animator.SetFloat("Vertical", joystick.Vertical);

        if (Mathf.Abs(joystick.Horizontal) > 0.9 || Mathf.Abs(joystick.Vertical) > 0.9)
        {
            animator.SetBool("IsRunning", true);
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 10;
        }
        else
        {
            animator.SetBool("IsRunning", false);
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed = 5;
        }
    }
}
