using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public List<GameObject> obj = new List<GameObject>();
    public List<GameObject> characters = new List<GameObject>();
    public List<GameObject> nonActiveChar = new List<GameObject>();
 

    public int population;
    public int populationActive = 0;
    public int dead = 0;

    public float spawnRate = 3f;
    public bool ablSpwn = true;

    public GameObject CharacterPrefab;

    public GameObject spwnLoc;
        

    // Start is called before the first frame update
    void Start()
    {
        nonActiveChar = createPool(population);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (populationActive < population && ablSpwn == true && nonActiveChar.Count >=1)
        {
            ablSpwn = false;
            StartCoroutine(startSpawning());
        }
    }

    IEnumerator startSpawning()
    {
        
        populationActive++;

        int tempInt = Random.Range(0, nonActiveChar.Count);
        GameObject tempChar = nonActiveChar[tempInt];
        tempChar.transform.position = spwnLoc.transform.position;
        tempChar.SetActive(true);
        tempChar.GetComponent<AIController>().setState(Ncharacterstate.Normal);
        tempChar.GetComponent<AIController>().unDead();

        nonActiveChar.Remove(tempChar);
        characters.Add(tempChar);

        yield return new WaitForSeconds(spawnRate);
        ablSpwn = true;
    }

    public List<GameObject> createPool(int popul)
    {
        List<GameObject> temp = new List<GameObject>();

        for (int i = 0; i < popul; i++)
        {
            temp.Add(characterCreation());
        }

        return temp;
    }

    public GameObject characterCreation()
    {
        GameObject tempC = Instantiate(CharacterPrefab,gameObject.transform.position,Quaternion.identity);
        tempC.transform.parent = gameObject.transform;

        tempC.GetComponent<AIController>().setObjList(objListCreation());
        tempC.GetComponent<AIController>().spawnerPointer = gameObject;

        return tempC;
    }

    public List<GameObject> objListCreation()
    {
        List<GameObject> tempL = new List<GameObject>();

        int tempObjN = Random.Range(1, 5);

        for (int i = 0; i < tempObjN; i++)
        {
            int t = Random.Range(1, obj.Count);
            tempL.Add(obj[t]);
        }

        tempL.Add(spwnLoc);

        return tempL;
    }

    public void cleanUpTime(GameObject sleeping)
    {
        if (characters.Exists(x => sleeping))
        {
            characters.Remove(sleeping);
            //nonActiveChar.Add(sleeping);
            populationActive--;
        }        
    }

    public void cleanUpTimeDied(GameObject dying)
    {
        population--;
        populationActive--;
        dead++;
        if (characters.Exists(x => dying))
        {
            characters.Remove(dying);
            //nonActiveChar.Add(dying);
        }     
    }
    public void addToNon(GameObject non)
    {
        nonActiveChar.Add(non);
    }

}

