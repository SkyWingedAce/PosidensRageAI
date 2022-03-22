using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : AI_State
{
    public float distance;

    public int seconds = 2;

    public NavMeshAgent agent;

    public List<GameObject> objList = new List<GameObject>();
    public List<GameObject> travledList = new List<GameObject>();

    public GameObject spawnerPointer;
    //public Ncharacterstate currentState;
    public Vector3 destination;

    /* below are values needed to interface withsprite master and change sprites
     * 0 =norm
     * 1 =walk
     * 2 =scard
     * 3 =work
     * 4 =dead
     * */

    [SerializeField] private spriteMaster spriteMaster;

    [SerializeField] private GameObject scareB;

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = Ncharacterstate.Normal;
    }

    public override void ProcessStatus(Ncharacterstate currState)
    {
        switch (currentState)
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
                    if (occupied == false)
                    {
                        occupied = true;
                        if (objList.Count > 0)
                        {
                            StartCoroutine(moveAgentNormal());
                        }
                    }
                    break;
                }
            case Ncharacterstate.Scared:
                {
                    if (scare == false)
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
                    distance = Vector3.Distance(transform.position, agent.destination);

                    if (distance <= 1.5f && occupied == false)
                    {
                        occupied = true;
                        StartCoroutine(walk());
                    }

                    break;
                }
        }
    }

    public override IEnumerator dyingTime()
    {
       Vector3 pos = transform.position;
       Instantiate(scareB, pos,Quaternion.identity);
       spriteMaster.updateSprite(4);
       spawnerPointer.GetComponent<SpawnerScript>().cleanUpTimeDied(gameObject);

       yield return new WaitForSeconds(3);

       gameObject.SetActive(false);  
    }

    public override IEnumerator sleepTime()
    {
        spawnerPointer.GetComponent<SpawnerScript>().cleanUpTime(gameObject);
        
        yield return new WaitForSeconds(1);
        
        gameObject.SetActive(false);
        spawnerPointer.GetComponent<SpawnerScript>().addToNon(gameObject);
    }

    public override IEnumerator waiting()
    {
        spriteMaster.updateSprite(3);

        yield return new WaitForSeconds(3);

        if (objList.Count > 0 && sleep == false && dead == false)
        {
            currentState = Ncharacterstate.Normal;
        }
        occupied = false;      
    }

    public override IEnumerator moveAgentNormal()
    {
        spriteMaster.updateSprite(1);
        getnewDestination();
        agent.isStopped = false;
        Debug.DrawRay(destination, Vector3.up, Color.blue, 1.0f);
        occupied = false;
        currentState = Ncharacterstate.Walking;

        yield return new WaitForSeconds(1);
    }

    public override IEnumerator walk()
    {
            agent.ResetPath();
            agent.isStopped = true;

            travledList.Add(objList[0]);
            objList.RemoveAt(0);
            //play animation here
            
            
            yield return new WaitForSeconds(1);

        if (objList.Count == 0)
        {
            Debug.Log(objList.Count);
            objList.AddRange(travledList);
            travledList.Clear();
            currentState = Ncharacterstate.Sleep;
            occupied = false;
        }
        else
        {
            currentState = Ncharacterstate.Still;
            occupied = false;
        }
        
    }

    public override IEnumerator scared()
    {
        spriteMaster.updateSprite(2);
        agent.isStopped = true;
        yield return new WaitForSeconds(3);
        StartCoroutine(moveAgentNormal());
        agent.isStopped = false;

    }

    public void setObjList(List<GameObject> tempL)
    {
        for (int i = 0; i < tempL.Count; i++)
        {
            objList.Add(tempL[i]);
        }
    }

    public void setState(Ncharacterstate charStat)
    {
        currentState = charStat;
    }

    public void unDead()
    {
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
        dead = false;
        sleep = false;
    }

    private void getnewDestination()
    {
        destination = objList[0].transform.position;
        agent.SetDestination(destination);
    }

    public void changeColor(Material mat)
    {
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }


}


