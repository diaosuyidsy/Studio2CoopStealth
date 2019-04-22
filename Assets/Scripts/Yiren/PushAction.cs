using TMPro;
using UnityEngine;
using UnityEngine.Events;



namespace Invector.vCharacterController.vActions
{
    [vClassHeader("Ladder Action", "Use the vTriggerLadderAction on your ladder mesh.", iconName = "ladderIcon")]
    public class PushAction : vActionListener
    {
        #region public variables
        public float pushSpeed = 2;
        public string actionTag = "PushTrigger";
        public RewiredInputWrapper verticallInput = new RewiredInputWrapper("Move Vertical");
        public RewiredInputWrapper horicallInput = new RewiredInputWrapper("Move Horizontal");
        public RewiredInputWrapper enterInput = new RewiredInputWrapper("Jump");
        public RewiredInputWrapper exitInput = new RewiredInputWrapper("B");     
        public bool debugMode;

        public UnityEvent OnEnterLadder;
        public UnityEvent OnExitLadder;

        #endregion

        #region protected variables    

        protected vThirdPersonInput tpInput;
        protected vTriggerLadderAction ladderAction;
        protected vTriggerLadderAction ladderActionTemp;
        protected float speed;
        protected bool isUsingLadder;
        protected bool isExitingLadder;
        protected bool triggerEnterOnce;
        protected bool triggerExitOnce;
        protected bool isTouchingPushObject;

        #endregion

        private void Awake()
        {
            actionStay = true;
            actionExit = true;
        }

        void Start()
        {
            tpInput = GetComponent<vThirdPersonInput>(); 
            verticallInput.playerID = tpInput.playerId;
            horicallInput.playerID = tpInput.playerId;
            enterInput.playerID = tpInput.playerId;
            exitInput.playerID = tpInput.playerId;
        }


        void Update()
        {
            //AutoEnterLadder(); 
            EnterLadderInput();            
            ExitLadderInput(); 
        }

        void OnAnimatorMove()
        {
            if (!isUsingLadder) return;
            UseLadder();
            if (!tpInput.cc.customAction)
            {
                // enable movement using root motion
                transform.rotation = tpInput.cc.animator.rootRotation;
            }

            float targetDis = Vector3.Distance(Vector3.Scale(new Vector3(1, 0, 1), transform.position),
                Vector3.Scale(new Vector3(1, 0, 1), ladderActionTemp.transform.parent.position));
            if (tpInput.cc.baseLayerInfo.IsName("Push") && targetDis < GetComponent<Collider>().bounds.extents.x + ladderActionTemp.transform.parent.GetComponent<Collider>().bounds.extents.x -0.1f )
            {
                return;
            }
            transform.position = tpInput.cc.animator.rootPosition;

            
        }//****************************************************//

        void EnterLadderInput()
        {
            if (ladderAction == null || tpInput.cc.customAction || tpInput.cc.isJumping || !tpInput.cc.isGrounded) return;
           
            if (enterInput.GetButtonDown() && !isUsingLadder && !ladderAction.autoAction)
                TriggerEnterLadder();
            
        } //****************************************************//

        void TriggerEnterLadder()
        {
            if (debugMode) Debug.Log("Enter Ladder");

            OnEnterLadder.Invoke();
            triggerEnterOnce = true;                              
            isUsingLadder = true;
            tpInput.cc.animator.SetInteger("ActionState", 1);     // set actionState 1 to avoid falling transitions            
            tpInput.enabled = false;                              // disable vThirdPersonInput
            tpInput.cc.enabled = false;                           // disable vThirdPersonController, Animator & Motor 
            tpInput.cc.DisableGravityAndCollision();              // disable gravity & turn collision trigger
            tpInput.cc._rigidbody.velocity = Vector3.zero;
            tpInput.cc.isGrounded = false;                       
            tpInput.cc.animator.SetBool("IsGrounded", false);    
            ladderAction.OnDoAction.Invoke();

            ladderActionTemp = ladderAction;
            
            if (!string.IsNullOrEmpty(ladderAction.playAnimation))
                tpInput.cc.animator.CrossFadeInFixedTime(ladderAction.playAnimation, 0.1f);     // trigger the action animation clip                           
        }

        void UseLadder()
        {
            // update the base layer to know what animations are being played
            tpInput.cc.LayerControl();
            tpInput.cc.ActionsControl();
            // update camera movement
            tpInput.CameraInput();

            // go up or down
            tpInput.cc.input.y = verticallInput.GetAxis();
            tpInput.cc.input.x = horicallInput.GetAxis();
            Transform pushParent = ladderActionTemp.transform.parent.transform;
            Vector3 objDir = (pushParent.position - ladderActionTemp.transform.position).normalized;
            Vector3 moveDir = new Vector3(tpInput.cc.input.x,0,tpInput.cc.input.y);
            speed = Vector3.Dot(moveDir, objDir);
            speed = Mathf.Clamp(speed, 0f, 1f);
            //speed = Mathf.Clamp(tpInput.cc.input.y, -1f, 1f);
            tpInput.cc.animator.SetFloat("InputVertical", speed * pushSpeed, 0.25f, Time.deltaTime);
            
            
            // enter ladder behaviour           
            if (tpInput.cc.baseLayerInfo.IsName("EnterPush"))
            {
                // disable ingame hud
                if (ladderActionTemp != null) ladderActionTemp.OnPlayerExit.Invoke();

                if (ladderActionTemp.useTriggerRotation)
                {
                    // smoothly rotate the character to the target
                    transform.rotation = Quaternion.Lerp(transform.rotation, ladderActionTemp.matchTarget.transform.rotation, tpInput.cc.animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    
                }

                if (ladderActionTemp.matchTarget != null)
                {
                    if (debugMode) Debug.Log("Match Target...");
                    // use match target to match the Y and Z target 
                    tpInput.cc.MatchTarget(ladderActionTemp.matchTarget.transform.position, ladderActionTemp.matchTarget.transform.rotation, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), ladderActionTemp.startMatchTarget, ladderActionTemp.endMatchTarget);
                    
                }
            }

            // exit ladder behaviour
            //isExitingLadder = tpInput.cc.baseLayerInfo.IsName("ExitLadderTop") || tpInput.cc.baseLayerInfo.IsName("ExitLadderBottom");

            /*if (isExitingLadder && tpInput.cc.baseLayerInfo.normalizedTime >= 0.8f)
            {
                // after playing the animation we reset some values
                ResetPlayerSettings();
            }*/
        }

        public void ExitLadderInput()
        {
            if (!isUsingLadder) return;
            //if (tpInput.cc.baseLayerInfo.IsName("EnterLadderTop") || tpInput.cc.baseLayerInfo.IsName("EnterLadderBottom")) return; 
           /*if (transform.Find("push") != null)
            {
                if (tpInput.cc.baseLayerInfo.IsName("Push") && (transform.Find("push").transform.position - transform.position).magnitude >5)
                {
                    if (debugMode) Debug.Log("Quick Exit");
                    RemovePushObjFromParent();
                    ResetPlayerSettings();
                }
            }*/
            
            
            if (tpInput.cc.baseLayerInfo.IsName("Push") && ladderActionTemp.transform.parent.GetComponent<PushObj>().isFall)
            {
                if (debugMode) Debug.Log("Quick Exit");
                RemovePushObjFromParent();
                ResetPlayerSettings();
                
            }
            
            if (tpInput.cc.baseLayerInfo.IsName("Push") && exitInput.GetButtonDown())
            {
                if (debugMode) Debug.Log("Quick Exit");
                RemovePushObjFromParent();
                ResetPlayerSettings();
                
            }
            
            /*if (ladderAction == null)
            {
                print("PushOver");
                // exit ladder at any moment by pressing the cancelInput
                if (tpInput.cc.baseLayerInfo.IsName("Push") && exitInput.GetButtonDown())
                {
                    if (debugMode) Debug.Log("Quick Exit");
                    ResetPlayerSettings();
                }
            }*/

        }

        void RemovePushObjFromParent()
        {
            ladderActionTemp.transform.parent.GetComponent<PushObj>().StopMove();
        }

        void ResetPlayerSettings()
        {
            if (debugMode) Debug.Log("Reset Player Settings");
            speed = 0f;
            ladderAction = null;
            isUsingLadder = false;
            OnExitLadder.Invoke();
            triggerExitOnce = false;
            triggerEnterOnce = false;
            tpInput.cc._capsuleCollider.isTrigger = false;
            tpInput.cc._rigidbody.useGravity = true;            
            tpInput.cc.animator.SetInteger("ActionState", 0);            
            tpInput.cc.enabled = true;
            tpInput.enabled = true;
            tpInput.cc.gameObject.transform.eulerAngles = new Vector3(0f, tpInput.cc.gameObject.transform.localEulerAngles.y, 0f);
        }

        public override void OnActionStay(Collider other)
        {
            if (other.gameObject.CompareTag(actionTag))
            {
                CheckForTriggerAction(other);
            }
        }

        public override void OnActionExit(Collider other)
        {
            if (other.gameObject.CompareTag(actionTag))
            {
                // disable ingame hud
                if (ladderAction != null) ladderAction.OnPlayerExit.Invoke();
                ladderAction = null;
            }
        }

        void CheckForTriggerAction(Collider other)
        {
            // assign the component - it will be null when he exit the trigger area
            var _ladderAction = other.GetComponent<vTriggerLadderAction>();
            if (!_ladderAction)
            {
                return;
            }
            // check the maxAngle too see if the character can do the action
            var dist = Vector3.Distance(transform.forward, _ladderAction.transform.forward);

            if (isUsingLadder && _ladderAction != null)
                ladderAction = _ladderAction;
            else if (dist <= 0.8f && !isUsingLadder)
            {
                ladderAction = _ladderAction;
                ladderAction.OnPlayerEnter.Invoke();
            }
            else
            {
                if (ladderAction != null) ladderAction.OnPlayerExit.Invoke();
                ladderAction = null;
            }
        }
    }
}
