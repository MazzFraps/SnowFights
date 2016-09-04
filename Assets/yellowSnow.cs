using UnityEngine;
using System.Collections;

public class yellowSnow : MonoBehaviour
{
    public bool canPickup;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Test122");
        if (collision.tag == "Player")
        {
            Debug.Log("Test1");
            collision.GetComponent<Movement>().yellowSnow = true;
        }
    }

    // OnTriggerExit2D is called when the Collider2D other has stopped touching the trigger (2D physics only)
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Movement>().yellowSnow = false;
        }
    }
}
