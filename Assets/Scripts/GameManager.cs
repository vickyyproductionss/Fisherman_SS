using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    int ropeMoveSpeed;
    public int ropeLength;
    Vector3 initialHookPos;
    public LineRenderer rope;
    public GameObject bucket;
    int catchedFishCount = 0;
    public TMP_Text fishCount;
    public GameObject fishParent;
    public TMP_Text popUpMessage;
    public bool readyToCatchFish;
    public GameObject catchedFish;
    public GameObject fishOnTarget;
    public List<GameObject> bounds;
    public List<Transform> spawnPos;
    public List<GameObject> fishIdle;
    public List<Transform> RopePoints;
    public bool Hooked_A_Fish = false;
    public static GameManager instance;
    public List<GameObject> fishSwimming;
    public AudioSource clickSound;
    public GameObject Counters;
    int a = 0;
    int s = 0;
    int d = 0;
    int w = 0;
    int leftShift = 0;
    int space = 0;
    bool aDown;
    bool sDown;
    bool dDown;
    bool wDown;
    bool spacePressed;
    void updateCounts()
    {
        Counters.transform.GetChild(1).GetComponent<TMP_Text>().text = "A : " + a;
        Counters.transform.GetChild(2).GetComponent<TMP_Text>().text = "S : " + s;
        Counters.transform.GetChild(3).GetComponent<TMP_Text>().text = "D : " + d;
        Counters.transform.GetChild(4).GetComponent<TMP_Text>().text = "W : " + w;
        //Counters.transform.GetChild(5).GetComponent<TMP_Text>().text = "Left Shift : " + leftShift;
        Counters.transform.GetChild(5).GetComponent<TMP_Text>().text = "Space : " + space;
    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        initialHookPos = RopePoints[1].position;
        StartCoroutine(SpawnFishes());
        updateFishCount(0);
        updateCounts();
    }

    // Update is called once per frame
    void Update()
    {
        manageHook();
        renderRope();
        moveCatchedFish();
        //unhookFish();
        hookTheTargetFish();
    }
    void hookTheTargetFish()
    {
        if(readyToCatchFish)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                clickSound.Play();
                space++;
                updateCounts();
                Hooked_A_Fish = true;
                GameManager.instance.catchedFish = fishOnTarget;
                fishOnTarget.GetComponent<FishMovements>().enabled = false;
                fishOnTarget.GetComponent<Animator>().enabled = false;
                fishOnTarget.tag = "catchedFish";
            }
        }
    }
    
    void moveCatchedFish()
    {
        if(Hooked_A_Fish && Input.GetKey(KeyCode.Space))
        {
            clickSound.Play();
            spacePressed = true;
            //space++;
            catchedFish.transform.position = RopePoints[2].position;
        }
        else if(spacePressed)
        {
            spacePressed = false;
            updateCounts();
            if (RopePoints[1].position.y > bounds[1].transform.position.y && RopePoints[1].position.x > bounds[0].transform.position.x && RopePoints[1].position.x < bounds[2].transform.position.x)
            {
                StartCoroutine(moveFishToBucket(catchedFish));
                catchedFish.transform.localScale = new Vector3(catchedFish.transform.localScale.x / 3, catchedFish.transform.localScale.x / 3, catchedFish.transform.localScale.x / 3);
                catchedFishCount++;
                updateFishCount(catchedFishCount);
                Hooked_A_Fish = false;
                StartCoroutine(freeHookToInitialPos(initialHookPos, RopePoints[1]));
            }
            else
            {
                StartCoroutine(showMessage("Fish slipped due to less height.", 2));
                fishOnTarget.GetComponent<FishMovements>().enabled = true;
                fishOnTarget.GetComponent<Animator>().enabled = true;
                Hooked_A_Fish = false;
            }
        }
    }
    void unhookFish()
    {
        if(Hooked_A_Fish)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                clickSound.Play();
                leftShift++;
                updateCounts();
                if (RopePoints[1].position.y > bounds[1].transform.position.y && RopePoints[1].position.x > bounds[0].transform.position.x && RopePoints[1].position.x < bounds[2].transform.position.x)
                {
                    StartCoroutine(moveFishToBucket(catchedFish));
                    catchedFish.transform.localScale = new Vector3(catchedFish.transform.localScale.x/3, catchedFish.transform.localScale.x / 3, catchedFish.transform.localScale.x / 3);
                    catchedFishCount++;
                    updateFishCount(catchedFishCount);
                    Hooked_A_Fish = false;
                    StartCoroutine(freeHookToInitialPos(initialHookPos, RopePoints[1]));
                }
                else
                {
                    StartCoroutine(showMessage("Take hook over boat and\nrelease space button to collect fish.", 2));
                    Hooked_A_Fish = false;
                    fishOnTarget.GetComponent<FishMovements>().enabled = true;
                    fishOnTarget.GetComponent<Animator>().enabled = true;
                }
            }
        }
    }
    IEnumerator freeHookToInitialPos(Vector3 to, Transform hook)
    {
        hook.position = Vector3.Lerp(hook.position, to, Time.deltaTime*ropeMoveSpeed);
        yield return new WaitForEndOfFrame();
        if(Mathf.Abs((to - hook.position).magnitude)>0.5f)
        {
            StartCoroutine(freeHookToInitialPos(to, hook));
        }
    }
    void updateFishCount(int count)
    {
        fishCount.text = "Score : " + count.ToString();
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
        if(Mathf.Abs((fish.transform.position - bucket.transform.position).magnitude) > 0.1f)
        {
            StartCoroutine(moveFishToBucket(catchedFish));
        }
    }
    void manageHook()
    {
        if(Input.GetKey(KeyCode.A))
        {
            if(!aDown)
            {
                aDown = true;
                a++;
                updateCounts();
            }
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
        if(Input.GetKeyUp(KeyCode.A))
        {
            clickSound.Play();
            aDown = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            clickSound.Play();
            sDown = false;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            clickSound.Play();
            dDown = false;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            clickSound.Play();
            wDown = false;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if(!sDown)
            {
                sDown = true;
                s++;
                updateCounts();
            }
            
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
            if (!wDown)
            {
                wDown = true;
                w++;
                updateCounts();
            }
            if (RopePoints[1].position.y < RopePoints[0].transform.position.y)
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
            if (!dDown)
            {
                dDown = true;
                d++;
                updateCounts();
            }
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
            if(RopePoints[1].position.y > bounds[1].transform.position.y && RopePoints[1].position.x > bounds[0].transform.position.x && RopePoints[1].position.x < bounds[2].transform.position.x)
            {
                StartCoroutine(showMessage("Release space to drop fish.",2));
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
