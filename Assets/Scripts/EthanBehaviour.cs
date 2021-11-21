using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EthanBehaviour : MonoBehaviour
{
    [SerializeField]
    private Path path;
    [SerializeField]
    private Transform self;
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private NavMeshAgent navMesh;
    [SerializeField]
    private Animator selfAnimator;

    [System.NonSerialized]
    public bool isEnable = false;

    private Vector3 previousPointPosition;
    private float timer = 0.0f;
    private float currentSpeed;
    private int indexWaypoint = 0;
    private int indexLinkPoint = 1;
    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        currentSpeed = speed;
    }

    public void StartLiving()
    {
        isEnable = true;
        gameObject.SetActive(true);
        self.localPosition = new Vector3(0.0f, self.localPosition.y, 0.0f);
        previousPointPosition = self.position;
        currentSpeed = speed;
    }

    public void Die()
    {
        // Update animation
        selfAnimator.SetBool("IsDead", true);
        currentSpeed = 0.0f;
    }

    // Test if the dying animations is ended in order to destroy the object
    private void IsDieAnimationFinished()
    {
        // Reset to the pool
        if (selfAnimator.GetBehaviours<AnimationFinishedBehaviour>()[0].animationIsFinished)
        {
            selfAnimator.SetBool("IsDead", false);
            isEnable = false;
            indexLinkPoint = 1;
            indexWaypoint = 0;
            gameObject.SetActive(false);
        }
    }

    // Following path algorithm
    private void SetPathPosition()
    {
        // Define timer for MoveTowards function
        timer += speed * Time.deltaTime;

        // If ethan has not arrived to the end of the current link that he is crossing
        if (indexLinkPoint < path.links[indexWaypoint].pathPoints.Count)
        {
            // If ethan has not arrived to the next corner of the current link
            if (self.position != path.links[indexWaypoint].pathPoints[indexLinkPoint])
            {
                // Move towards the next corner
                self.position = Vector3.MoveTowards(previousPointPosition, path.links[indexWaypoint].pathPoints[indexLinkPoint],
                    timer);
                self.LookAt(path.links[indexWaypoint].pathPoints[indexLinkPoint]);
            }
            else
            {
                // Reset timer + define the next corner to go
                timer = 0;
                previousPointPosition = path.links[indexWaypoint].pathPoints[indexLinkPoint];
                ++indexLinkPoint;
            }
        }
        else
        {
            // Reset timer + define new link to cross
            timer = 0;
            ++indexWaypoint;
            if (indexWaypoint >= path.links.Count) return;
            previousPointPosition = path.links[indexWaypoint].pathPoints[0];
            indexLinkPoint = 1;
        }
    }

    private bool PlayerIsNear()
    {
        Collider[] near = Physics.OverlapSphere(self.position, 1.0f);
        for (int i = 0; i < near.Length; ++i) {
            if (near[i].CompareTag("PlayerController"))
                return true;
        }
        return false;
    }

    IEnumerator Attack()
    {
        // Define a little timer to avoid the player to be dead when he is closed an Ethan
        yield return new WaitForSeconds(0.5f);
        Collider[] hits = Physics.OverlapSphere(self.position, 1.0f);
        // Select a random attack animation
        int randomAttackIndex = Random.Range(0, 6);
        selfAnimator.SetFloat("RandomAttack", randomAttackIndex);

        // Update animator
        selfAnimator.SetTrigger("Attack");

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("PlayerController"))
            {
                GameManager.instance.GameOver();
            }
        }

        // Start cooldown between two attacks
        canAttack = false;
        yield return new WaitForSeconds(2.5f);
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable && !GameManager.instance.isEndGame)
        {
            // While Ethan does not finish the current path
            if (indexWaypoint < path.links.Count)
            {
                selfAnimator.SetFloat("Speed", currentSpeed);
                SetPathPosition();
            }
            else
            {
                // Go to the player automatically
                navMesh.SetDestination(PlayerController.instance.self.position);
                navMesh.speed = currentSpeed;
            }

            // If player is near => attack
            if (PlayerIsNear() && canAttack)
            {
                self.LookAt(PlayerController.instance.self.position);
                StartCoroutine(Attack());              
            }

            // Check if ethan is dead
            IsDieAnimationFinished();
        }
            
    }
}
