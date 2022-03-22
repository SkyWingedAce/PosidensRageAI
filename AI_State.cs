using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_State : MonoBehaviour
{
    public bool occupied = false;

    public bool dead = false;

    public bool sleep = false;

    public bool scare = false;

    public Ncharacterstate currentState;

    public void FixedUpdate()
    {
        ProcessStatus(currentState);
    }

    public virtual void ProcessStatus(Ncharacterstate currState)
    {
        switch (currState)
        {
            case Ncharacterstate.Dead:
                {
                    if (dead == false)
                    {
                        dead = true;
                        occupied = false;
                        StopAllCoroutines();
                        StartCoroutine(dyingTime());
                    }
                    break;
                }
            case Ncharacterstate.Normal:
                {

                    StartCoroutine(moveAgentNormal());

                    break;
                }
            case Ncharacterstate.Scared:
                {
                    StopAllCoroutines();
                    StartCoroutine(scared());
                    break;
                }
            case Ncharacterstate.Still:
                {
                    if (occupied == false)
                    {
                        occupied = true;
                        StartCoroutine(waiting());
                    }
                    break;
                }
            case Ncharacterstate.Sleep:
                {
                    if (sleep == false)
                    {
                        sleep = true;
                        occupied = false;
                        StopAllCoroutines();
                        StartCoroutine(sleepTime());
                    }
                    break;
                }
            case Ncharacterstate.Walking:
                {
                    StartCoroutine(walk());
                    break;
                }
        }
    }

    public virtual IEnumerator dyingTime()
    {
        yield return new WaitForSeconds(3);
    }

    public virtual IEnumerator sleepTime()
    {
        yield return new WaitForSeconds(3);
    }

    public virtual IEnumerator waiting()
    {
        yield return new WaitForSeconds(3);
    }

    public virtual IEnumerator moveAgentNormal()
    {
        yield return new WaitForSeconds(3);
    }

    public virtual IEnumerator walk()
    {
        yield return new WaitForSeconds(3);
    }

    public virtual IEnumerator scared()
    {
        yield return new WaitForSeconds(3);
    }

}

public enum Ncharacterstate
{
    Still,
    Normal,
    Sleep,
    Scared,
    Dead,
    Walking
}