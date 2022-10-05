using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15.0f;
    public int minSwipeRecognition = 500;
    public AudioClip crashSound;
    
    private AudioSource ballAudio;
    private bool isTravelling;
    private bool gameJustStarted;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;
    private Vector2 swipePositionLastFrame;
    private Vector2 swipePositionCurrentFrame;
    private Vector2 currentSwipe;

    private Color solvedColour;


    private void Start()
    {
        solvedColour = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solvedColour;
        ballAudio = GetComponent<AudioSource>();
        gameJustStarted = false;
    }



    private void FixedUpdate()
    {
        if (isTravelling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - Vector3.up / 2, 0.2f);
        int i = 0;
        while (i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColoured)
            {
                ground.ChangeColour(solvedColour);
            }
            i++;
        }

        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTravelling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;

            }
        }

        if (isTravelling)
            return;
        
        // swipe codes.
        if (Input.GetMouseButton(0))
        {
            swipePositionCurrentFrame = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePositionLastFrame != Vector2.zero)
            {
                currentSwipe = swipePositionCurrentFrame - swipePositionLastFrame;

                if (currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                // up/down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    //go up/down
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                //left/right
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    //go left/right
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }

            }

            swipePositionLastFrame = swipePositionCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePositionLastFrame = Vector2.zero;
            swipePositionCurrentFrame = Vector2.zero;
        }
        
    }

    // partly swipe part also.
    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;
        float maxHitDistance = 100f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxHitDistance))
        {
            nextCollisionPosition = hit.point;
        }

        isTravelling = true;
        gameJustStarted = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        

        if (other.gameObject.CompareTag("Wall") && !isTravelling && gameJustStarted)
        {
            ballAudio.PlayOneShot(crashSound, 1.0f);
        }
    }
}
