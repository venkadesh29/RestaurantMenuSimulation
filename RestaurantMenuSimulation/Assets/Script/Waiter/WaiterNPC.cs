using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WaiterNPC : MonoBehaviour
{
    public float roamingSpeed = 2.0f; // Speed while roaming
    public float servingDistance = 2.0f; // Distance to serve food
    public float roamDuration = 3.0f; // Duration to roam before stopping
    private Vector3 originalPosition; // Original position of the waiter
    private NavMeshAgent navMeshAgent;
    private Transform target; // The target customer
    private bool isServing = false; // Flag to check if the waiter is serving

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position; // Set the original position
        StartCoroutine(RoamRandomly());
    }

    void Update()
    {
        // If serving, move towards the target
        if (isServing && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > servingDistance)
            {
                MoveToTarget();
            }
            else
            {
                ServeFood();
            }
        }
    }

    private IEnumerator RoamRandomly()
    {
        while (true)
        {
            if (!isServing)
            {
                // Choose a random position to roam to
                Vector3 randomPosition = GetRandomPosition();
                navMeshAgent.SetDestination(randomPosition);
                yield return new WaitForSeconds(roamDuration); // Wait before moving again

                // Check if we need to check for food orders while roaming
                yield return new WaitForSeconds(1f); // Small delay to avoid continuous checking
            }
            yield return null; // Wait for the next frame
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Generate a random position within a certain range from the original position
        Vector3 randomDirection = Random.insideUnitSphere * 5f; // 5f units range
        randomDirection += originalPosition; // Add to the original position
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 5.0f, NavMesh.AllAreas); // Find a valid position on the NavMesh
        return hit.position; // Return the random valid position
    }

    private void MoveToTarget()
    {
        navMeshAgent.SetDestination(target.position);
    }

    private void ServeFood()
    {
        navMeshAgent.isStopped = true; // Stop moving
        Debug.Log("Serving food to " + target.name); // Log the serving action

        // Call the customer's LeaveChair method after serving
        CustomerNPC customer = target.GetComponent<CustomerNPC>();
        if (customer != null)
        {
            customer.LeaveChair(); // Notify customer to leave the chair
        }

        // Reset serving state and return to roaming
        isServing = false;
        target = null; // Clear the target
        StartCoroutine(RoamRandomly()); // Resume roaming after serving
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget; // Set a new target for the waiter
        isServing = true; // Mark the waiter as serving
        navMeshAgent.isStopped = false; // Ensure the waiter is moving
    }
}
