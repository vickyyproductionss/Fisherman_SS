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
                GameManager.instance.readyToCatchFish = true;
                GameManager.instance.fishOnTarget = collision.gameObject;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameManager.instance.readyToCatchFish = false;
    }
}
