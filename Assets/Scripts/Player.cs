using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IKitchenObjectParent
{

    public static Player Instance { get; private set; }


    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;


    private float speed = 6.0f;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private bool isWalking;
    private KitchenObjects kitchenObject;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

        gameInput.OnInteraction += GameInput_OnInteraction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;

    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameHandler.Instance.IsGamePlaying()) return; //if the game is not playing player cannot interact with objcts
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);

        }
    }

    private void GameInput_OnInteraction(object sender, System.EventArgs e){
        if (!GameHandler.Instance.IsGamePlaying()) return; //if the game is not playing player cannot interact with objcts

        if (selectedCounter != null) {
          selectedCounter.Interact(this);
     
        }
    
    }
   
    private void Update(){

     HandleMovement();
     HandleInteractions();
    
    }

   public bool IsWalking(){
        return isWalking;
   }

     private void HandleInteractions(){

          Vector2 inputVector = gameInput.GameInputVectorNormalized();

          Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

          if(moveDir != Vector3.zero){

               lastInteractDir = moveDir;

          }

          

          float interactDistance = 1.0f;
          if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)){
              if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {

                    //has ClearCounter
                    if (baseCounter != selectedCounter){

                         SetSelectedCounter(baseCounter);

                    }
              } else {

               SetSelectedCounter(null);

              }
          } else {

               SetSelectedCounter(null);
            

          }
          
     }


    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GameInputVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = speed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
    private void SetSelectedCounter(BaseCounter selectedCounter){
     this.selectedCounter = selectedCounter;
     OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs{
          selectedCounter =   selectedCounter
     });
   }

    public Transform GetKitchenObjectFollowTransform()
    {
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObjects kitchenObjects)
    {
        this.kitchenObject = kitchenObjects;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }


    }
    public KitchenObjects GetKitchenObjects()
    {
        return kitchenObject;
    }
    public void ClearKitchenObjects()
    {
        kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return (kitchenObject != null);
    }

}
