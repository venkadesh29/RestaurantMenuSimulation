using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerNPC : MonoBehaviour
{
    public Transform[] chairs; // Array of chair transforms
    private Transform currentChair; // The chair the customer is occupying
    private NavMeshAgent navMeshAgent;
    private bool isSitting = false;
    private bool isEating = false; // Track if the customer is eating

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(Roam());
    }

    private IEnumerator Roam()
    {
        while (true)
        {
            if (!isSitting)
            {
                Transform nearestChair = FindNearestEmptyChair();
                if (nearestChair != null)
                {
                    // Move to the nearest chair
                    navMeshAgent.SetDestination(nearestChair.position);
                    yield return new WaitUntil(() => navMeshAgent.remainingDistance < 0.5f);

                    // Sit down at the chair
                    currentChair = nearestChair;
                    isSitting = true;
                    currentChair.GetComponent<Chair>().OnCustomerSit(); // Mark the chair as occupied
                    Debug.Log(name + " is sitting at " + currentChair.name);

                    // Order food after sitting
                    OrderFood();

                    // Wait for the food to be served
                    yield return new WaitForSeconds(3f); // Simulate time for food to arrive
                    // Randomize eating time for more realistic behavior
                    float eatingTime = Random.Range(4f, 7f); // Randomize eating duration
                    // Start eating
                    StartEating();
                    yield return new WaitForSeconds(eatingTime); // Wait for random eating duration

                    // After eating, leave the chair
                    LeaveChair();
                }
                else
                {
                    Debug.Log(name + " found no empty chairs."); // Log if no chairs are found
                }
            }

            yield return new WaitForSeconds(5f); // Wait before roaming again
        }
    }

    private Transform FindNearestEmptyChair()
    {
        Transform nearestChair = null;
        float minDistance = float.MaxValue;

        foreach (Transform chair in chairs)
        {
            Chair chairComponent = chair.GetComponent<Chair>();
            if (chairComponent != null && !chairComponent.IsOccupied)
            {
                float distance = Vector3.Distance(transform.position, chair.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestChair = chair;
                }
            }
        }

        return nearestChair; // Return the nearest empty chair found
    }

    private void OrderFood()
    {
        // Notify the nearest waiter NPC to serve food
        WaiterNPC[] waiters = FindObjectsOfType<WaiterNPC>();
        WaiterNPC nearestWaiter = null;
        float minDistance = float.MaxValue;

        foreach (WaiterNPC waiter in waiters)
        {
            float distance = Vector3.Distance(transform.position, waiter.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestWaiter = waiter;
            }
        }

        if (nearestWaiter != null)
        {
            nearestWaiter.SetTarget(transform); // The customer as the target
            Debug.Log(name + " has ordered food from " + nearestWaiter.name); // Log the ordering action
        }
        else
        {
            Debug.Log(name + " could not find a waiter to order food."); // Log if no waiters found
        }
    }

    private void StartEating()
    {
        isEating = true;
        Debug.Log(name + " is eating."); // Log the eating action
        // Trigger eating animation
        Animator animator = GetComponent<Animator>();
        //animator.SetTrigger("eat"); // Ensure you have a trigger in the Animator for eating
    }

    public void LeaveChair()
    {
        if (currentChair != null)
        {
            currentChair.GetComponent<Chair>().OnCustomerLeave(); // Mark the chair as empty
            Debug.Log(name + " has left the chair.");
        }
        isSitting = false; // Allow the customer to roam again
        currentChair = null; // Clear the occupied chair

        // Move the customer away from the chair
        MoveAwayFromChair();
    }

    private void MoveAwayFromChair()
    {
        // Choose a random direction to move away
        Vector3 randomDirection = Random.insideUnitSphere * 5; // Move away 5 units
        randomDirection += transform.position; // Set new position relative to the current position

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 5.0f, NavMesh.AllAreas); // Find a valid position on the NavMesh
        navMeshAgent.SetDestination(hit.position); // Move to the new position

        Debug.Log(name + " is moving away to a new position.");
    }
}
