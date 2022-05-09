using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool Hooked_A_Fish = false;
    public List<GameObject> fishSwimming;
    public List<GameObject> fishIdle;
    public List<Transform> spawnPos;
    public GameObject fishParent;
    public List<Transform> RopePoints;
    public LineRenderer rope;
    [SerializeField]
    int ropeMoveSpeed;
    public GameObject catchedFish;
    public GameObject bucket;
    public static GameManager instance;
    int catchedFishCount = 0;
    public TMP_Text popUpMessage;
    public TMP_Text fishCount;
    public int ropeLength;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        StartCoroutine(SpawnFishes());
        updateFishCount(0);
    }

    // Update is called once per frame
    void Update()
    {
        manageHook();
        renderRope();
        moveCatchedFish();
        unhookFish();
    }
    void moveCatchedFish()
    {
        if(Hooked_A_Fish)
        {
            catchedFish.transform.position = RopePoints[2].position;
        }
    }
    void unhookFish()
    {
        if(Hooked_A_Fish)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (catchedFish.transform.position.y > bucket.transform.position.y)
                {
                    StartCoroutine(moveFishToBucket(catchedFish));
                    catchedFish.transform.localScale = new Vector3(catchedFish.transform.localScale.x/3, catchedFish.transform.localScale.x / 3, catchedFish.transform.localScale.x / 3);
                    catchedFishCount++;
                    updateFishCount(catchedFishCount);
                    Hooked_A_Fish = false;
                }
                else
                {
                    StartCoroutine(showMessage("Take hook over boat and\npress left shift to collect fish.", 2));
                }
            }
        }
    }
    void updateFishCount(int count)
    {
        fishCount.text = count.ToString() + " fishes collected.";
    }
    IEnumerator showMessage(string message, int time)
    {
        popUpMessage.text = message;
        yield return new WaitForSeconds(time);
        popUpMessage.text = "";
    }
    IEnumerator moveFishToBucket(GameObject fish)
    {
        fish.transform.parent = bucket.transform;
        fish.transform.position = Vector3.Lerp(fish.transform.position, bucket.transform.position, Time.deltaTime);
        yield return new WaitForEndOfFrame();
        if(fish.transform.position != bucket.transform.position)
        {
            StartCoroutine(moveFishToBucket(catchedFish));
        }
    }
    void manageHook()
    {
        

        if(Input.GetKey(KeyCode.A))
        {
            Vector3 destination = new Vector3(RopePoints[1].transform.position.x - ropeMoveSpeed * Time.deltaTime, RopePoints[1].transform.position.y, RopePoints[1].transform.position.z);
            float ropeLen1 = Mathf.Abs((RopePoints[1].position - RopePoints[0].position).magnitude);
            float ropeLen2 = Mathf.Abs((destination - RopePoints[0].position).magnitude);
            if((ropeLen1 < ropeLength && ropeLen2 < ropeLength) || (ropeLen2 < ropeLen1))
            {
                RopePoints[1].transform.position = destination;
            }
            else if(ropeLen1 < ropeLength && ropeLen2> ropeLength)
            {
                StartCoroutine(showMessage("maximum rope length", 2));
            }
            
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 destination = new Vector3(RopePoints[1].transform.position.x, RopePoints[1].transform.position.y - ropeMoveSpeed * Time.deltaTime, RopePoints[1].transform.position.z);
            float ropeLen1 = Mathf.Abs((RopePoints[1].position - RopePoints[0].position).magnitude);
            float ropeLen2 = Mathf.Abs((destination - RopePoints[0].position).magnitude);
            if ((ropeLen1 < ropeLength && ropeLen2 < ropeLength) || (ropeLen2 < ropeLen1))
            {
                RopePoints[1].transform.position = destination;
            }
            else if (ropeLen1 < ropeLength && ropeLen2 > ropeLength)
            {
                StartCoroutine(showMessage("maximum rope length", 2));
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            if(RopePoints[1].position.y < RopePoints[0].transform.position.y)
            {
                Vector3 destination = new Vector3(RopePoints[1].transform.position.x, RopePoints[1].transform.position.y + ropeMoveSpeed * Time.deltaTime, RopePoints[1].transform.position.z);
                float ropeLen1 = Mathf.Abs((RopePoints[1].position - RopePoints[0].position).magnitude);
                float ropeLen2 = Mathf.Abs((destination - RopePoints[0].position).magnitude);
                if ((ropeLen1 < ropeLength && ropeLen2 < ropeLength) || (ropeLen2 < ropeLen1))
                {
                    RopePoints[1].transform.position = destination;
                }
                else if (ropeLen1 < ropeLength && ropeLen2 > ropeLength)
                {
                    StartCoroutine(showMessage("maximum rope length", 2));
                }
            }
            else
            {
                StartCoroutine(showMessage("maximum rope height", 2));
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 destination = new Vector3(RopePoints[1].transform.position.x + ropeMoveSpeed * Time.deltaTime, RopePoints[1].transform.position.y, RopePoints[1].transform.position.z);
            float ropeLen1 = Mathf.Abs((RopePoints[1].position - RopePoints[0].position).magnitude);
            float ropeLen2 = Mathf.Abs((destination - RopePoints[0].position).magnitude);
            if ((ropeLen1 < ropeLength && ropeLen2 < ropeLength) || (ropeLen2 < ropeLen1))
            {
                RopePoints[1].transform.position = destination;
            }
            else if (ropeLen1 < ropeLength && ropeLen2 > ropeLength)
            {
                StartCoroutine(showMessage("maximum rope length", 2));
            }
        }
    }
    void renderRope()
    {
        rope.SetPosition(0, RopePoints[0].position);
        rope.SetPosition(1, RopePoints[1].position);
        if(Hooked_A_Fish)
        {
            if(RopePoints[1].position.y > bucket.transform.position.y)
            {
                StartCoroutine(showMessage("Press left shift to collect fish.",2));
            }
        }
    }
    IEnumerator SpawnFishes()
    {
        int spawnFishOrNot = 1;
        int randomFish = Random.Range(0,fishSwimming.Count);
        int randomFishPos = Random.Range(0,spawnPos.Count);
        int randomFishDirection = 1;
        if(randomFishPos > 3)
        {
            randomFishDirection = -1;
        }
        for(int i =0; i < fishParent.transform.childCount; i++)
        {
            if(spawnPos[randomFishPos].position.y != fishParent.transform.GetChild(i).transform.position.y || fishParent.transform.GetChild(i).GetComponent<FishMovements>().movedirection == randomFishDirection)
            {
                spawnFishOrNot *= 1;
            }
            else
            {
                spawnFishOrNot *= -1;
            }
        }
        if(spawnFishOrNot == 1)
        {
            GameObject fish = Instantiate(fishSwimming[randomFish], spawnPos[randomFishPos].position, Quaternion.identity);
            if (randomFishPos > 3)
            {
                fish.GetComponent<FishMovements>().movedirection = -1;
                fish.GetComponent<FishMovements>().speed = 1;
            }
            else
            {
                fish.GetComponent<FishMovements>().movedirection = 1;
                fish.GetComponent<FishMovements>().speed = 1;
                fish.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            fish.transform.parent = fishParent.transform;
        }
        yield return new WaitForSeconds(3);
        StartCoroutine(SpawnFishes());
    }
}
