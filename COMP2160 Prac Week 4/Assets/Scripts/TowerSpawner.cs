using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private Tower tower;

    private PlayerActions playerActions;
    private InputAction mouseMovementAction;
    private InputAction leftClickAction;
    private Vector3 mousePosition;

    [SerializeField] private LayerMask layerMask;

    void Awake()
    {
        playerActions = new PlayerActions();
        mouseMovementAction = playerActions.TowerDefence.MousePosition;
        leftClickAction = playerActions.TowerDefence.LeftClick;
        leftClickAction.performed += OnLeftClick;
    }

    void OnEnable()
    {
        mouseMovementAction.Enable();
        leftClickAction.Enable();
    }

    void OnDisable()
    {
        mouseMovementAction.Disable();
        leftClickAction.Disable();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = new Vector3(mouseMovementAction.ReadValue<Vector2>().x,mouseMovementAction.ReadValue<Vector2>().y, 0);
    }

    void OnLeftClick(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log(hit.collider.gameObject.name);
            Tower newTower = Instantiate(tower);
            newTower.transform.position = hit.point;
            newTower.transform.SetParent(transform);
        }
    }
}
