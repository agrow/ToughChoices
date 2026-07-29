using UnityEngine;
using System;

/*
There are several animations that an NPC could have. Each of the following strings corresponds to the name of the animation that 
can play. Each NPC has an "Animation" component whose field is one of these names. Depending on the context, the same NPC could
cycle through multiple animations, or they may only perform one for the whole game
- Idle
- Running
- Walking
- Jump
- Talking1
- Talking2
- Nod
- SitToStand
- SittingTalking1
- SittingTalking2
- JogForward
- GoalkeeperIdle1
- GoalkeeperIdle2
- OffensiveIdle
*/

public class NPCAnimations : MonoBehaviour
{
    public string animation;

    private Animator animator;
    private float crossfade = 0.0f;
    private float timer;
    private bool animationStarted = false;

    private string idle = "Idle";
    private string running = "Running";
    private string walking = "Walking";
    private string jump = "Jump";
    private string talking1 = "Talking1";
    private string talking2 = "Talking2";
    private string nod = "Nod";
    private string sittostand = "SitToStand";
    private string sittingtalking1 = "SittingTalking1";
    private string sittingtalking2 = "SittingTalking2";
    private string sittingidle = "SittingIdle";
    private string jogforward = "JogForward";
    private string goalkeeperidle1 = "GoalkeeperIdle1";
    private string goalkeeperidle2 = "GoalkeeperIdle2";
    private string offensiveidle = "OffensiveIdle";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        timer = UnityEngine.Random.Range(0.0f, 3.0f);
        checkAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (!animationStarted)
        {
            checkAnimation();
        }
    }

    private void checkAnimation()
    {
        if (String.Compare(idle, animation) == 0)
        {
            animator.CrossFade(idle, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(running, animation) == 0)
        {
            animator.CrossFade(running, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(walking, animation) == 0)
        {
            animator.CrossFade(walking, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(jump, animation) == 0)
        {
            animator.CrossFade(jump, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(talking1, animation) == 0)
        {
            animator.CrossFade(talking1, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(talking2, animation) == 0)
        {
            animator.CrossFade(talking2, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(nod, animation) == 0)
        {
            animator.CrossFade(nod, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(sittostand, animation) == 0)
        {
            animator.CrossFade(sittostand, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(sittingtalking1, animation) == 0)
        {
            animator.CrossFade(sittingtalking1, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(sittingtalking2, animation) == 0)
        {
            animator.CrossFade(sittingtalking2, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(sittingidle, animation) == 0)
        {
            animator.CrossFade(sittingidle, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(jogforward, animation) == 0)
        {
            animator.CrossFade(jogforward, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(goalkeeperidle1, animation) == 0)
        {
            animator.CrossFade(goalkeeperidle1, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(goalkeeperidle2, animation) == 0)
        {
            animator.CrossFade(goalkeeperidle2, crossfade);
            animationStarted = true;
        }
        else if (String.Compare(offensiveidle, animation) == 0)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                animator.CrossFade(offensiveidle, 0.2f);
                animationStarted = true;
            }
        }
        else
        {
            Debug.Log("false");
        }
    }
}
