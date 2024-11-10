using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed = 5.0f;
    public float angularSpeed = 200.0f;
    public KeyMoveType keyMoveType = KeyMoveType.ARROR;

    private Vector3 heading;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private GameObject headingIndicator;

    // Start is called before the first frame update
    void Start()
    {
        this.heading = Vector3.up;
        this.rb = this.GetComponent<Rigidbody2D>();
        this.headingIndicator = this.transform.Find("HeadingIndicator").gameObject;
    }

    public enum KeyMoveType
    {
        ARROR,
        WASD,
        IJKL
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = false;

        switch (this.keyMoveType) {
            case KeyMoveType.ARROR:
                // turning
                if (Input.GetKey(KeyCode.LeftArrow)) {
                    this.Rotate(this.angularSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.RightArrow)) {
                    this.Rotate(-this.angularSpeed * Time.deltaTime);
                }

                // moving
                if (Input.GetKey(KeyCode.UpArrow)) {
                    this.Move(this.speed);
                }
                if (Input.GetKey(KeyCode.DownArrow)) {
                    this.Move(-this.speed);
                }

                break;
            case KeyMoveType.WASD:
                // turning
                if (Input.GetKey(KeyCode.A)) {
                    this.Rotate(this.angularSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D)) {
                    this.Rotate(-this.angularSpeed * Time.deltaTime);
                }

                // moving
                if (Input.GetKey(KeyCode.W)) {
                    this.Move(this.speed);
                }
                if (Input.GetKey(KeyCode.S)) {
                    this.Move(-this.speed);
                }

                break;
            case KeyMoveType.IJKL:
                // turning
                if (Input.GetKey(KeyCode.J)) {
                    this.Rotate(this.angularSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.L)) {
                    this.Rotate(-this.angularSpeed * Time.deltaTime);
                }

                // moving
                if (Input.GetKey(KeyCode.I)) {
                    this.Move(this.speed);
                }
                if (Input.GetKey(KeyCode.K)) {
                    this.Move(-this.speed);
                }

                break;
        }

        if (!isMoving) {
            this.rb.velocity = Vector2.zero;
        }
    }

    private void Move(float speed)
    {
        this.rb.velocity = this.heading * speed;
        this.isMoving = true;
    }

    private void Rotate(float angle)
    {
        this.heading = Quaternion.Euler(0, 0, angle) * this.heading;
        this.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.heading);
    }
}
