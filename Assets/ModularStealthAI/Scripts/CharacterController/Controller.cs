using Unity.VisualScripting;
using UnityEngine;

public class Controller : MonoBehaviour
{
    CharacterController character;
    Input Input;

    public float walkingSpeed = 10f;

    void Awake()
    {
        Input = new Input();
    }

    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        Input.Character.Enable();
    }

    void OnDisable()
    {
        Input.Character.Disable();
    }

    void Update()
    {
        Vector2 move = Input.Character.Movement.ReadValue<Vector2>();

        MoveCharacter(move);
    }

    void MoveCharacter(Vector2 movement)
    {
        Vector3 move = new Vector3(movement.x, 0f, movement.y);

        if (move.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        character.SimpleMove(move * walkingSpeed);
    }
}
