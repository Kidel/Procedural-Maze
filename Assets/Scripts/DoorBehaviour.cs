using UnityEngine;

public class DoorBehaviour : MonoBehaviour {
    private bool open = false;

    private Vector3 startMarker;
    private Vector3 endMarker;
    public float speed = 2;

    void Start()
    {
        startMarker = transform.position;
        endMarker = transform.position - new Vector3(0,9,0);
    }

    void FixedUpdate()
    {
        if (open)
        {
            transform.position = Vector3.Lerp(transform.position, endMarker, Time.deltaTime * speed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, startMarker, Time.deltaTime * speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            open = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            open = false;
    }

    
}
