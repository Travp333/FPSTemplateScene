using UnityEngine;

public class camAnim : MonoBehaviour
{
    [SerializeField]
    Movement move;
    Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //basically you are inputting a movement key and you are moving
        if (move.movementAction.ReadValue<Vector2>().magnitude > 0 && move.body.linearVelocity.magnitude > .01f)
        {
            anim.SetBool("Moving", true);
            if (move.movementAction.ReadValue<Vector2>().x > 0)
            {
                anim.SetBool("MovingLeft", true);
                //moving left?
            }
            else
            {
                anim.SetBool("MovingLeft", false);
            }
            if (move.movementAction.ReadValue<Vector2>().x < 0)
            {
                anim.SetBool("MovingRight", true);
                //moving right?
            }
            else
            {
                anim.SetBool("MovingRight", false);
            }
        }
        else
        {
            anim.SetBool("Moving", false);
            anim.SetBool("MovingLeft", false);
            anim.SetBool("MovingRight", false);
        }

    }
}
