using UnityEngine;

public class FoodOrderCanvas : MonoBehaviour
{
    public PlayerOrder playerOrder; // Reference to the player order script

    // Call this when a food button is clicked
    public void OnFoodButtonClick(int foodIndex)
    {
        // Order food via the player's order script
        playerOrder.OrderFood(foodIndex);
    }
}
