using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishCatcher : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "fish")
        {
            if (!GameManager.instance.Hooked_A_Fish)
            {
                GameManager.instance.Hooked_A_Fish = true;
                GameManager.instance.catchedFish = collision.gameObject;
                collision.gameObject.GetComponent<FishMovements>().enabled = false;
                collision.gameObject.GetComponent<Animator>().enabled = false;
                collision.gameObject.tag = "catchedFish";

            }
        }
    }
}
