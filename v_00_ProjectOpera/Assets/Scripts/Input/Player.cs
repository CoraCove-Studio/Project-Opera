using UnityEngine;

public class Player : MonoBehaviour
{
    private void OnEnable()
    {
        InputManager.Instance.OnVertMove += HandleVertMovement;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnVertMove -= HandleVertMovement;
    }

    private void HandleVertMovement(float movementInput)
    {
        print(movementInput);
    }

}
