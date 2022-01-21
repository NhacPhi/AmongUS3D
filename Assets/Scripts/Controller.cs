using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Animator m_animator;
    CharacterController m_character;
    public GameObject m_CharacterRoot;

    public float PlayerSpeed = 5.0f;
    public float RuningSpeed = 7.0f;
    public float MouseSentitivity = 60f;

    float m_HorizontalAngle;

    // Check player in ground
    public float m_SpeedAtJump = 1.2f;
    float m_GroundedTimer;
    float jumpSpeed = 5f;
    bool Grounded;

    float VelocityVetical = 0;

    float Vertical = 0;
    float Horizontal = 0;
    Vector3 Move;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_animator = m_CharacterRoot.gameObject.GetComponent<Animator>();

        m_character = GetComponent<CharacterController>();

        // Ground
        Grounded = true;

        // Movement
        //m_VerticalAngle = 0.0f;
        m_HorizontalAngle = transform.localEulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.eulerAngles = Vector3.zero;
        Vertical = Input.GetAxis("Vertical");
        if(Vertical<0)
        {
            Vertical = 0;
        }
        Horizontal = Input.GetAxis("Horizontal");

        m_animator.SetFloat("VelocityY", Vertical);
        m_animator.SetFloat("VelocityX", Horizontal);

        bool wasGrounded = Grounded;

        bool loosedGrounding = false;
        if (!m_character.isGrounded)
        {
            if (!Grounded)
            {
                m_GroundedTimer += Time.deltaTime;
                if (m_GroundedTimer >= 0.5f)
                {
                    //Debug.Log("LOL");
                    loosedGrounding = true;
                    Grounded = false;
                }
            }
        }
        else
        {
            m_GroundedTimer = 0.0f;
            Grounded = true;
        }
        if (Grounded && Input.GetKeyDown(KeyCode.Space))
        {
            m_animator.SetTrigger("Jump");
            StartCoroutine(StartJump());
        }
        Move = Vector3.zero;
        Move = new Vector3(Horizontal,0, Vertical);

        if (Move.sqrMagnitude > 0)
        {
            Move.Normalize();
            //Weapon.Walk = true;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Vertical!=0)
        {
            Move = Move * Time.deltaTime * RuningSpeed;
            //m_animator.SetBool("IsRun", true);
            m_animator.SetFloat("VelocityY", 2);
        }
        else
        {
            Move = Move * Time.deltaTime * PlayerSpeed;
            //m_animator.SetBool("IsRun", false);
        }

        Move = transform.TransformDirection(Move);

        m_character.Move(Move);

        // handle gravity
        VelocityVetical = VelocityVetical - 9.8f * Time.deltaTime;
        if (VelocityVetical < -9.8f)
        {
            VelocityVetical = -9.8f;
        }
        Vector3 verticalSpeed = new Vector3(0, VelocityVetical * Time.deltaTime, 0);
        m_character.Move(verticalSpeed);

        // Player rotation by mouse

        // Axis Horizontal with CharactorRoot
        float turnPlayer = Input.GetAxis("Mouse X") * MouseSentitivity*Time.deltaTime;
        m_HorizontalAngle = m_HorizontalAngle + turnPlayer;

        if (m_HorizontalAngle > 360) m_HorizontalAngle -= 360.0f;
        if (m_HorizontalAngle < 0) m_HorizontalAngle += 360.0f;

        Vector3 currentAngles = transform.localEulerAngles;
        currentAngles.y = m_HorizontalAngle;
        transform.localEulerAngles = currentAngles;

        // Reset 
        m_CharacterRoot.gameObject.transform.localPosition = Vector3.zero;
        m_CharacterRoot.gameObject.transform.localEulerAngles = Vector3.zero;
    }
    IEnumerator StartJump()
    {
        yield return new WaitForSeconds(0.5f);
        VelocityVetical = jumpSpeed;
        Debug.Log("Jump");
        Grounded = false;
    }
}
