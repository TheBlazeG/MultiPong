using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Rigidbody2D rb;
    private float horizontal;
    private float vertical;
    public float speed = 0.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        //if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow))
        //if(Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow))
        //if(Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow)||Input.GetKeyDown(KeyCode.Space))

        horizontal = Input.GetAxis("Horizontal")*speed;
        horizontal = Input.GetAxis("Vertical")*speed;
    }

    private void FixedUpdate()
    {
        //rb.AddForce(Vector2(horizontal, vertical));
    }
}
