using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovements : MonoBehaviour
{
    public int movedirection;
    public float speed;
    void Start()
    {
        
    }

    void Update()
    {
        MoveFish();
        destroyIfBoundaryExceeds();
    }
    void MoveFish()
    {
        this.transform.position = new Vector3(this.transform.position.x + (speed*Time.deltaTime)*movedirection, this.transform.position.y, this.transform.position.z);
    }
    void destroyIfBoundaryExceeds()
    {
        if(this.transform.position.x < -11 || this.transform.position.x > 11)
        {
            Destroy(this.gameObject);
        }
    }
}
