using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class pjmoves : MonoBehaviour
{
    private Rigidbody2D rb2D;

    [Header("basic moves")]
    private float InputX;
    private float horizontalmoves = 0f;
    [SerializeField] private float speedmove ;
    [Range (0, 0.1f)][SerializeField] private float motionsmoother ;
    private Vector3 speed = Vector3.zero;
    private bool rightlook = true;

    [Header ("jump")]
    [SerializeField] private float jumpforce;
    [SerializeField] private LayerMask isfloor;
    [SerializeField] private Transform floorcontroller;
    [SerializeField] private Vector3 boxdimension;
    [SerializeField] private bool infloor;
    [SerializeField] private bool jump = false;

    [Header("Animation")]
    private Animator animator;

    [Header("Dash")]
    [SerializeField] private float dashspeed;
    [SerializeField] private float dashtime;
    private float initialgravity;
    private bool canmove = true;

    [Header("walljump")]
    [SerializeField] private Transform wallcontroller;
    [SerializeField] private Vector3 boxwalldimension;
    private bool inwall;
    private bool wallslide;
    [SerializeField] private float slidespeed;
    [SerializeField] private float walljumpforcex;
    [SerializeField] private float walljumpforcey;
    [SerializeField] private float timewalljump;
    private bool jumpingwall;

    // Start is called before the first frame update
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialgravity = rb2D.gravityScale;
        
    }

    // Update is called once per frame
    private void Update()
    {
        InputX = Input.GetAxisRaw("Horizontal");
        horizontalmoves = InputX * speedmove;

        animator.SetFloat("Horizontal",Mathf.Abs (horizontalmoves));

        animator.SetBool("slide",wallslide);

        if (Input.GetKeyDown(KeyCode.W))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && infloor==true)
        {
            StartCoroutine(Dash());
        }

        if (!infloor && inwall && InputX != 0)
        {
            wallslide = true;
        }

        else
        {
            wallslide = false;
        }
    }

    private void FixedUpdate()

    {
        infloor = Physics2D.OverlapBox(floorcontroller.position, boxdimension, 0f, isfloor);

        animator.SetBool("grounded",infloor);

        inwall = Physics2D.OverlapBox(wallcontroller.position, boxwalldimension, 0f, isfloor);

        if (canmove)

        Move(horizontalmoves * Time.fixedDeltaTime,jump);

        jump = false;

        if (wallslide)
        {
            rb2D.velocity=new Vector2(rb2D.velocity.x, Mathf.Clamp(rb2D.velocity.y, -slidespeed,float.MaxValue));
        }
    }
   
    private void Move(float move, bool jump)
    
    {
        if (!jumpingwall)
        {

        }
            Vector3 objectvelocity = new Vector2(move, rb2D.velocity.y);
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, objectvelocity, ref speed, motionsmoother);

        if (move > 0 && !rightlook)
        {
            rotation();
            //lookrotation
        }
        else if (move < 0 && rightlook)
        {
            rotation();
            //lookrotation
        }

        if (jump && infloor && !wallslide)
            {
                infloor = false;
                rb2D.AddForce(new Vector2(0, jumpforce));
            }

        
        if (jump && inwall && wallslide)
            {
                jumpwall();
            }
    }
        private void jumpwall()
        {
            inwall = false;
            rb2D.velocity = new Vector2(walljumpforcex * -InputX, walljumpforcey);
            StartCoroutine(changewalljump());
        }

        IEnumerator changewalljump()
        {
            jumpingwall = true;
            yield return new WaitForSeconds(timewalljump);
            jumpingwall = false;
        }
        
    private void rotation()
    {
            rightlook = !rightlook;

            Vector3 escale = transform.localScale;
            escale.x *= -1;
            transform.localScale = escale;
    
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(floorcontroller.position, boxdimension);
        Gizmos.DrawWireCube(wallcontroller.position, boxwalldimension);
    }
    
    private IEnumerator Dash()
    {
        canmove = false;
        rb2D.gravityScale = 0;
        rb2D.velocity = new Vector2 (dashspeed * transform.localScale.x, 0);
        animator.SetTrigger("dash");
        
        yield return new WaitForSeconds(dashtime);
        
        canmove= true;
        rb2D.gravityScale = initialgravity;

    }


    }
