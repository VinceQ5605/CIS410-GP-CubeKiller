using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float jumpCoolDown; // time after jumping until the next jump is available
    public float height;
    public float jumpDistance;
    [Range(0, 30)]
    public float groupingDistance;
    [Range(0, 30)]
    public float rotationSpeed;
    [Range(0, 30)]
    public float freezeRotationTime = 1f;
    [Range(0f, 1f)]
    public float directionChangeProbability;
    public int maxHealth;

    private GameObject baseObject;
    private bool isBusy = false; // is this enemy busy doing a coroutine?
    private bool oriented = false; // the cube is rotated to align with the grid
    private Vector3 baseLocation;  //! change to base.transform.position once we have a base object
    private Vector3 previousPosition;
    private Vector3 positionBeforePreviousJump; // Test position against the position before the previous jump. If too close, increase probability to randomize jumpdirection.
    private int numberOfJumpsWithoutMovingMuch = 0; // to determine how likely it is to randomize jump direction.
    private string[] jumpableTags; // objects tagged with one of these indicate that the cube is able to jump off of it
    private new Rigidbody rigidbody;
    private new Transform transform;
    private List<GameObject> onTopOfTheseObjects;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        baseLocation = baseObject.transform.position;
        positionBeforePreviousJump = Vector3.zero;
        jumpableTags = new string[] { "Ground",  "Barrier"};
        JumpDirection(baseLocation - transform.position);
        //StartCoroutine(Wait(Random.Range(.2f,3f))); // delay for a random amount of time
        onTopOfTheseObjects = new List<GameObject>();
        rigidbody.freezeRotation = true; // this should not be necessary
        currentHealth = maxHealth;
    }

    public void SetBase(GameObject baseObject)
    {
        this.baseObject = baseObject;
    }


    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y < -.9) // if the other object struck this cube's bottom face
            {
                GameObject other = contact.otherCollider.gameObject;
                if (!onTopOfTheseObjects.Contains(other))
                {
                    onTopOfTheseObjects.Add(other);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        onTopOfTheseObjects.Remove(collision.gameObject);
    }

    bool GroundedTest()
    // returns true if this cube is on the ground/barrier. If on top of another cube, checks if that cube is grounded.
    {
        foreach (GameObject obj in onTopOfTheseObjects)
        {
            foreach (string tag in jumpableTags)
            {
                if (obj.CompareTag(tag))
                {
                    return true;
                }
            }
            if (obj.CompareTag("Enemy"))
            {
                EnemyController otherEnemyController = obj.GetComponent<EnemyController>();
                return otherEnemyController.GroundedTest();
            }
        }
        return false;
    }

    Vector3 JumpDirection(Vector3 direction)
    // calculates the normal vector of the input after zeroing out the y-component
    // may randomly decide to go an orthogonal direction
    {
        float x = direction.x;
        float z = direction.z;
        direction = new Vector3(x, 0f, z);
        direction.Normalize();
        Vector3 right = Vector3.Cross(direction, Vector3.up);
        bool thereIsAnEnemyToTheRight = false;
        bool thereIsAnEnemyToTheLeft = false;
        Ray ray = new Ray(transform.position, right);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit))
        {
            GameObject other = raycastHit.collider.gameObject;
            if (other.CompareTag("Enemy") && raycastHit.distance < groupingDistance)
            {
                thereIsAnEnemyToTheRight = true;
            }
        }
        ray = new Ray(transform.position, -1 * right);
        if (Physics.Raycast(ray, out raycastHit))
        {
            GameObject other = raycastHit.collider.gameObject;
            if (other.CompareTag("Enemy") && raycastHit.distance < groupingDistance)
            {
                thereIsAnEnemyToTheLeft = true;
            }
        }

        if (((transform.position - positionBeforePreviousJump).sqrMagnitude < .5) && (numberOfJumpsWithoutMovingMuch <  5))
        {
            numberOfJumpsWithoutMovingMuch += 1;
        }
        else if (numberOfJumpsWithoutMovingMuch > 0)
        {
            numberOfJumpsWithoutMovingMuch -= 1;
        }
        positionBeforePreviousJump = transform.position;

        float randomNumber = Random.Range(0f, 1f);
        if (randomNumber > (1f - .1*numberOfJumpsWithoutMovingMuch))
        {
            direction = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * Vector3.forward; // randomize direction
        }
        else if (randomNumber < directionChangeProbability)
        {
            if (thereIsAnEnemyToTheRight && !thereIsAnEnemyToTheLeft)
            {
                direction = right;
            }
            else if (thereIsAnEnemyToTheLeft && !thereIsAnEnemyToTheRight)
            {
                direction = -1 * right;
            }
        }

        return direction;
    }

    void FixedUpdate()
    // Update is called once per frame
    {
        // if busy doing one of the coroutines, skip this FixedUpdate
        if (isBusy)
        {
            return;
        }
        // if moving, wait for a second and try again
        if (transform.position != previousPosition)
        {
            previousPosition = transform.position;
            StartCoroutine(Wait(.4f)); // wait before checking again
            return;
        }


        if (!oriented) // note: oriented may be false even if already aligned to the grid. This is not a problem.
        {
            StartCoroutine(Orient(1));
            return;
        }


        // if something is directly above, wait
        if (RaycastTest(Vector3.up, new string[] { "Enemy" }, 2f) > 0)
        {
            StartCoroutine(Wait(3f)); // wait for 3 seconds before attempting to jump
            return;
        }

        Vector3 jumpDirection = JumpDirection(baseLocation - transform.position); // randomizes the direction in which to jump

        // do the jump!
        rigidbody.AddForce(200 * height * Vector3.up); // jump straight up
        StartCoroutine(DelayedDash(jumpDirection, .6f)); // dash in the direction determined in JumpDirection
        StartCoroutine(Wait(jumpCoolDown + Random.Range(0f, .3f)));
    }


    public void Hit(Vector3 direction)
    {
        //Disorient();
        rigidbody.AddForce(10 * direction);
        currentHealth--;
        if (currentHealth == 0)
            Die();
    }
    public void Die()
    {
        this.gameObject.SetActive(false);
    }

    public void Disorient()
    // called when the cube is hit by a grenade blast
    {
        oriented = false;
        rigidbody.freezeRotation = false;
    }

    private int RaycastTest(Vector3 direction, string[] tags, float maxDistance)
    // Casts 5 rays. Returns the number of rays that hit a collider (with one of the tags) within maxDistance.
    {
        Vector3[] originPoints = new Vector3[5]; // the center and four corners of the top/bottom face of the cube
        originPoints[0] = transform.position + .5f * direction;
        originPoints[1] = transform.position + .5f * (direction + Vector3.forward + Vector3.right);
        originPoints[2] = transform.position + .5f * (direction + Vector3.forward - Vector3.right);
        originPoints[3] = transform.position + .5f * (direction - Vector3.forward + Vector3.right);
        originPoints[4] = transform.position + .5f * (direction - Vector3.forward - Vector3.right);

        int count = 0; // counts the number of 
        foreach (Vector3 origin in originPoints)
        {
            Ray ray = new Ray(origin, direction);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                GameObject other = raycastHit.collider.gameObject;
                foreach (string tag in tags)
                {
                    if (other.CompareTag(tag))
                    {
                        if (raycastHit.distance <= maxDistance)
                        {
                            count++;
                            break;
                        }
                    }
                }
            }
        }
        return count;
    }

    /*IEnumerator Hold(float time)
    // don't do anything for a set time
    {
        yield return new WaitForSeconds(time);
        isBusy = false;
    }*/

    IEnumerator Orient(int orientCount)
    // aligns rotation with the grid (with the smallest rotation possible)
    // count is the number of times orient has been called in succession. If every 10 times, do a minihop 
    {
        isBusy = true;
        // Test dot products to see which of the six cube faces is most aligned with the y and x axes
        float maxDotY = 0; // meaningless initial values to make the editor happy
        Vector3 maxY = Vector3.zero;
        float maxDotX = 0;
        Vector3 maxX = Vector3.zero;
        int maxIndexX = -1;
        Vector3[] listOfDirections = { transform.forward, transform.up, transform.right, -transform.forward, -transform.up, -transform.right };
        for (int i = 0; i < 6; i++)
        {
            float dotY = Vector3.Dot(listOfDirections[i], Vector3.up);
            if (dotY > maxDotY)
            {
                maxDotY = dotY;
                maxY = listOfDirections[i];
            }
            float dotX = Vector3.Dot(listOfDirections[i], Vector3.right);
            if (dotX > maxDotX)
            {
                maxDotX = dotX;
                maxX = listOfDirections[i];
                maxIndexX = i;
            }
        }

        float rotSpeed = rotationSpeed;
        // figure out the sign of the angle of rotation (clockwise vs. counterclockwise)
        if (Vector3.Dot(Vector3.Cross(maxY, maxX), Vector3.right) < 0)
            rotSpeed = -rotSpeed;

        // rotate around the cube's "maxY" direction so that cube's "maxX" actually faces right
        int rotationCount = 0;
        int maxRotCount = 5 * orientCount;
        while (rotationCount < maxRotCount)
        {
            listOfDirections = new[] { transform.forward, transform.up, transform.right, -transform.forward, -transform.up, -transform.right };
            if (Vector3.Dot(listOfDirections[maxIndexX], Vector3.right) > .9999) // one full tick off at rotSpeed = 30 should have dot product value .999986
            {
                break;
            }

            transform.Rotate(maxY, rotSpeed * Time.deltaTime);
            rotationCount++;
            yield return null;
        }
        if (rotationCount == maxRotCount)
        {
            if (orientCount % 10 == 0)
                rigidbody.AddForce(10 * height * Vector3.up); // minihop

            StartCoroutine(Orient(orientCount + 1)); // try again (in case of flipping over mid rotation)
        }
        else // success!
        {
            transform.rotation = Quaternion.identity;
            rigidbody.freezeRotation = true;
            oriented = true;
            isBusy = false;
        }
    }
/*
    IEnumerator FreezeRotation(float time)
    {
        rigidbody.freezeRotation = true;
        yield return new WaitForSeconds(time);
        rigidbody.freezeRotation = false;
    }*/

    IEnumerator DelayedDash(Vector3 jumpDirection, float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        rigidbody.AddForce(50 * jumpDistance * jumpDirection);
    }

    IEnumerator Wait(float time)
    {
        isBusy = true;
        yield return new WaitForSeconds(time);
        isBusy = false;
    }
}
