//																		//
//	 ╔═════════════════════════════╡C#╞═════════════════════════════╗	//
//	 ║                PlayerMovement by Juan Callejas               ║	//
//	 ╠══════════════════════════════════════════════════════════════╣	//
//	 ║	Script to control player visuals like directional rotation,	║	//
//	 ║	squish and stretch, and player movement, including			║	//
//	 ║	character specific abilities like dashing, swimming, and	║	//
//	 ║	wall grabbing.												║	//
//	 ╚══════════════════════════════════════════════════════════════╝	//
//																		//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Characters {
	[Tooltip("Has all abilities enabled")]
	Debug,
	[Tooltip("Can dash vertically, and hang onto walls")]
	Scout,
	[Tooltip("Can dash horizontally, and swim in water")]
	Sailor
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAudio))]
public class PlayerMovement : MonoBehaviour {

	new private Rigidbody2D rigidbody;
	[HideInInspector]
	public PhysicsMaterial2D steppingOn;
	new private PlayerAudio audio;
	public Animator anim;
	private List<Undoable> undosUntilLastCheckpoint = new List<Undoable>();
	private List<Undoable> undosGLOBAL = new List<Undoable>();
	private ContactPoint2D[] contacts = new ContactPoint2D[0];
	private Vector2 vel = new Vector2(0, 0);
	private float lastJumpTime = -1;
	private float lastDashTime = -1;
	private float lastWallgrabTime;
	private float lastWallgrabContactXPos;
	private float lastYVel;
	private float charZRot;
	private float prevVerticalAxisValue;
	private bool onJump;
	private int slipNSliding;
	private int lastRigidbodyVelOrientation;

	public ParticleSystem confettiParticleSystem;



	[Header("General")]
	[Tooltip(
		"Debug:\tHas all abilities enabled.\n" +
		"Scout:\tCan dash vertically, and hang onto walls.\n" +
		"Sailor:\tCan dash horizontally, and swim in water."
	)]
	public Characters charType = Characters.Debug;
	[Range(-1, 1)]
	public int rigidbodyVelOrientation = 1;
	public LayerMask groundLayers;
	public PhysicsMaterial2D icePhysMat;

	[Header("Keybinds")]
	public string inputHorizontalAxisName = "Horizontal";
	public string inputVerticalAxisName = "Vertical";
	//public string inputJumpButtonName = "Jump";
	//public string inputDashButtonName = "Dash";

	[Header("Movement Parameters")]
	[Range(-1, 1)]
	public float horizontalInput;
	[Range(-1, 1)]
	public float verticalInput;
	public Vector2 raycastStartPos = new Vector2(.36f, -0.46f);
	public float raycastCheckDist = 0.15f;
	public float maxVelHori = 5;
	public float addVelHori = 0.5f;
	public float addVelHoriAerial = 0.5f;
	public float dampVelHori = 1.2f;
	public float dampVelHoriAerial = 1.01f;
	[Tooltip("CoyoteTime™")]
	public float coyoteTime = 0.1f;
	[Tooltip("How early can a jump keypress be to still be considered a jump")]
	public float jumpFrameDuration = 0.1f;
	public float jumpForce = 10;
	public float jumpHoldGravMulti = 1.8f;
	public float jumpReleaseGravMulti = 4.5f;
	public bool isGrounded;

	[Header("Dash Parameters")]
	public bool isInNoDashZone;
	public bool dashAvailable;
	public Vector2Int dashingToward;
	[Range(0, 1)]
	public float dashInputDeadzone = 0.5f;
	public float dashBraceFreeze = 0.15f;
	public float dashDuration = 0.2f;
	public float dashVelocity = 15;
	public float postDashXDampening = 0.1f;
	public float postDashYDampening = 0.15f;
	public bool isBracing;
	public bool isDashing;

	[Header("Dash Visuals")]
	public TrailRenderer[] dashTrails;
	public float dashLengthIdle = 0.1f;
	public float dashLengthDashing = 2;

	/*
	[Header("Water Interaction")]
	public float scoutHoriDrag;
	public float waterDamping;
	public float scoutWaterGrav;
	public float waterExitVelocityBoost;
	*/

	[Header("Walljump Interaction")]
	public float minCollisionXDistance = 0.4f;
	public float wallJumpHoriVel = 10;
	[Tooltip("CoyoteTime™")]
	public float wallJumpCoyoteTime = 0.2f;

	[Header("Visual")]
	/*
	public Color SailorCol;
	public Color ScoutCol;
	public Material bodyMat;
	*/
	public float offCenterAngle = 115;
	public float offCenterSpinSpeed = 2;
	public GameObject PlayerSprite;
	public float vertSquishFac = 0.025f;
	public float vertSquishMin = 1;
	public float vertSquishMax = 1.5f;
	[Range(0.5f, 1.5f)]
	public float currentSquishFactor;
	[Range(-10, 10)]
	public float currentXVel;

	/*
	//Respawn behaviour no longer necessary
	[Header("Respawn")]
	public float preRespawnDelay;
	public float lerpOutOfBoundsDuration;
	public float lerpToCheckpointDuration;
	public float travelToCheckpointZDepth;
	*/

	// Start is called before the first frame update
	void Start() {

		rigidbody = GetComponent<Rigidbody2D>();
		//anim = PlayerMesh.GetComponent<Animator>();
		audio = GetComponent<PlayerAudio>();

		/*
		//Set placeholder char color depending on charType
		if (charType == Characters.Sailor)
			bodyMat.SetColor("_BaseColor", SailorCol);
		else if (charType == Characters.Scout)
			bodyMat.SetColor("_BaseColor", ScoutCol);
		else
			bodyMat.SetColor("_BaseColor", Color.white);
		*/

	}

	// Update is called once per frame
	void FixedUpdate() {

		verticalInput = Input.GetAxis(inputVerticalAxisName);
		horizontalInput = Input.GetAxis(inputHorizontalAxisName);

		UpdateGrounded();

		vel = rigidbody.velocity;
		currentXVel = vel.x;

		//Update the orientation only if x-vel is far enough from 0
		if (Input.GetAxisRaw(inputHorizontalAxisName) > 0.1f)
			rigidbodyVelOrientation = 1;
		else if (Input.GetAxisRaw(inputHorizontalAxisName) < -0.1f)
			rigidbodyVelOrientation = -1;

		
		//Only update player orientation (here) if the player is not submerged
		if (charType == Characters.Scout || !isDashing)
			PlayerSprite.transform.localRotation = Quaternion.Lerp(
				PlayerSprite.transform.localRotation, //LerpFrom (currentRot)
				Quaternion.Euler(                   //LerpTo
					0,
					offCenterAngle * rigidbodyVelOrientation,           //Use precalculated orientation
					0
				),
				0.1f * offCenterSpinSpeed           //Lerp using speed specified in inspector
			);

		//If the player changes the direction they're facing, play a footstep sound effect
		if (lastRigidbodyVelOrientation != rigidbodyVelOrientation && isGrounded && !isDashing && !isBracing)
			audio.playRandomStep(steppingOn);
		lastRigidbodyVelOrientation = rigidbodyVelOrientation;

		//Handle horizontal movement (but not if the player is a scout and they are underwater), including
		if (System.Math.Abs(Input.GetAxisRaw(inputHorizontalAxisName)) > 0.01f) {
			//Add horizontal velocity, using different valued depending on whether one is grounded or not
			float velToAdd = Input.GetAxisRaw(inputHorizontalAxisName) * (isGrounded ? addVelHori : addVelHoriAerial);
			if (Input.GetAxisRaw(inputHorizontalAxisName) > 0)
				vel.x += Mathf.Clamp(velToAdd, 0, Mathf.Max(maxVelHori - vel.x, 0));
			else
				vel.x += Mathf.Clamp(velToAdd, Mathf.Min(-maxVelHori - vel.x, 0), 0);
		} else if (slipNSliding <= 0) {
			//If idle, dampen the player's velocity (again, different values if grounded)
			vel.x /= 1 + ((isGrounded ? dampVelHori : dampVelHoriAerial) * Time.fixedDeltaTime);
		}

		//Coyote time implementation // detect jump input
		if (isGrounded && Time.time - lastJumpTime < jumpFrameDuration) {
			vel.y = jumpForce;
			lastJumpTime = -jumpFrameDuration;//Reset the last jump time so the player may not register a jump multiple times a frame (you can't be too careful)
			isGrounded = false;
			anim.SetTrigger("jump");
		}

		//Jump higher if holding jump, smol jump if jump is only tapped
		bool jump = Input.GetAxisRaw(inputVerticalAxisName) > 0.5f;
		if (jump && rigidbody.velocity.y > 0) {
			rigidbody.gravityScale = jumpHoldGravMulti;
		} else {
			rigidbody.gravityScale = jumpReleaseGravMulti;
		}

		// ========== Water interaction behaviours ========== //
		//Assuming water/swimming won't be used, none of this code is necessary (I think)
		/*
		if (swimDetector.inWater) {

			if (charType == Characters.Scout)

				rigidbody.gravityScale = scoutWaterGrav;

			else {

				// ========== Swimming (Basically walking code but vertical motion) ========== //
				rigidbody.gravityScale = 0;
				//Add horizontal velocity, using different valued depending on whether one is grounded or not
				float velToAdd = Input.GetAxisRaw("Vertical") * (addVelHoriAerial);
				if (Input.GetAxisRaw("Vertical") > 0)
					vel.y += Mathf.Clamp(velToAdd, 0, Mathf.Max(maxVelHori - vel.y, 0));
				else
					vel.y += Mathf.Clamp(velToAdd, Mathf.Min(-maxVelHori - vel.y, 0), 0);

				// ========== Character rotation while swimming (facing forwards, rotate on Z axis) ========== //
				PlayerMesh.transform.localRotation = Quaternion.Lerp(
					PlayerMesh.transform.localRotation, //LerpFrom (currentRot)
					Quaternion.Euler(                   //LerpTo
						Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg * -1,     //Use calculated X rotation
						90,                                                 //Use 90 for Y axis for proper orientation
						90                                                  //Use 90 for Z axis for proper orientation
					),
					0.1f * offCenterSpinSpeed           //Lerp using speed specified in inspector
				);

			}

			vel /= 1 + waterDamping;

		}
		*/

		// ========== Apply all velocity modifications ========== //
		if (!rigidbody.isKinematic) {
			lastYVel = rigidbody.velocity.y;
			rigidbody.velocity = vel;
		}

		//QualitySettings.vSyncCount = 0;
		//Application.targetFrameRate = 144;

	}

	private void Update() {

		//Set the dash trail parameters according to the player's status
		foreach (TrailRenderer trail in dashTrails) {

			trail.emitting = (dashAvailable || isDashing) && !isInNoDashZone && !rigidbody.isKinematic;

			trail.time = Mathf.Lerp(trail.time, isDashing ? dashLengthDashing : dashLengthIdle, Time.deltaTime);

		}

		UpdateGrounded();

		//Draw ground detecting rays (blue)
		Debug.DrawRay((Vector2)transform.position + raycastStartPos, Vector3.down * raycastCheckDist, Color.blue);
		Debug.DrawRay((Vector2)transform.position + raycastStartPos * (Vector3.up + Vector3.left), Vector3.down * raycastCheckDist, Color.blue);

		//Draw wall-grabbing bounds (red)
		Debug.DrawRay(transform.position + Vector3.left * minCollisionXDistance + (Vector3.down / 2), Vector2.up, Color.red);
		Debug.DrawRay(transform.position + Vector3.right * minCollisionXDistance + (Vector3.down / 2), Vector2.up, Color.red);

		//Indicate grounded state (green if grounded, red if not)
		Debug.DrawRay(transform.position + Vector3.up + Vector3.left, Vector3.right * 2, isGrounded ? Color.green : Color.red);

		// ========== Animation behaviours ========== //
		anim.SetBool("isRunning", Mathf.Abs(Input.GetAxisRaw(inputHorizontalAxisName)) > 0.5f);
		anim.SetBool("isFalling", vel.y < 0.5f);
		anim.SetBool("isGrounded", isGrounded);

		//Store the value of OnButtonDown jump during Update()
		onJump = (Input.GetAxisRaw(inputVerticalAxisName) > prevVerticalAxisValue) && Input.GetAxisRaw(inputVerticalAxisName) > 0.5f;
		prevVerticalAxisValue = Input.GetAxisRaw(inputVerticalAxisName);
		if (onJump) {
			lastJumpTime = Time.time;
			Debug.Log("onJump!");
		}

		//Check if fell down
		Level currentLevel = GamePhaseManager.instance.levels[GamePhaseManager.instance.currentLevel];
		if (transform.position.y < currentLevel.transform.position.y + currentLevel.killHeight)
        {
			SpawnConfetti();
			Die();			
		}

		// ========== Dashing behaviours ========== //
		/*
		if (dashAvailable && Input.GetButtonDown(inputDashButtonName) && !isInNoDashZone) {

			Vector2Int dir = Vector2Int.zero;
			Vector2 inputs = new Vector2(
				Input.GetAxisRaw(inputHorizontalAxisName),
				Input.GetAxisRaw(inputVerticalAxisName)
			);

			//Make sure to avoid using dash with no velocity
			//by assigning defaults if the current values are
			//not valid (0)
			if (Mathf.Abs(inputs.x) < dashInputDeadzone) {
				inputs.x = rigidbodyVelOrientation;
				Debug.Log("Horizontal input is inside deadzone, defaulting to " + rigidbodyVelOrientation);
			}
			if (Mathf.Abs(inputs.y) < dashInputDeadzone)
				inputs.y = 1;

			if (charType == Characters.Sailor || charType == Characters.Debug)
				dir += Vector2Int.right * (inputs.x >= 0 ? 1 : -1);
			if (charType == Characters.Scout || charType == Characters.Debug)
				dir += Vector2Int.up * (inputs.y >= 0 ? 1 : -1);

			BeginDash(dir);
			dashAvailable = false;
		}
		*/

		// ========== Squish and Stretch behaviours ========== //
		currentSquishFactor = Mathf.Lerp(currentSquishFactor, Mathf.Min(Mathf.Pow(rigidbody.velocity.y / 2, 2f) * vertSquishFac + vertSquishMin, vertSquishMax), Time.deltaTime * 5f);

		PlayerSprite.transform.localScale = new Vector3(
			1 / currentSquishFactor,
			currentSquishFactor,
			1 / currentSquishFactor
		);


		// ========== Behaviours when exiting water ==========//
		//No swimming behaviours are used anymore
		/*
		if (lastInWater && !swimDetector.inWater) {
			//give them a speed boost when exiting water to give a "bobbing" effect
			rigidbody.velocity += Vector2.up * waterExitVelocityBoost;
			dashAvailable = true; //Refund player dash when exiting water
		}
		lastInWater = swimDetector.inWater;
		*/

		// ========== Walljump behaviours ========== //
		//On jump, wall-hop on direction opposite of the wall being grabbed
		/*
		if (!isGrounded && Time.time - lastJumpTime < jumpFrameDuration && Time.time - lastWallgrabTime < wallJumpCoyoteTime) {
			lastWallgrabTime = -wallJumpCoyoteTime;
			rigidbody.velocity =
				(Vector3.up * jumpForce) +
				(Vector3.right * wallJumpHoriVel * ((lastWallgrabContactXPos - transform.position.x) > 0 ? -1 : 1));
		}
		*/

#if UNITY_EDITOR

		//Log pressed joystick buttons for easy keybinds
		for (int i = 0; i < 20; i++) {
			if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "JoystickButton" + i)))
				Debug.Log("joystickbutton" + i);
		}

		// ========== Devmode on-the-fly char change ========== //
		if (Input.GetButtonDown("CheatCharChange") && Application.isEditor) //Only allow cheatcode inside editor, not in builds
			CheatCharChange();

#endif

		//Exit with esc.
		//Probably don't want to keep that behaviour in SS
		/*
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
		*/

	}

	private void CheatCharChange() {

		//Set charType var.
		if (charType == Characters.Debug)
			charType = Characters.Sailor;
		else if (charType == Characters.Sailor)
			charType = Characters.Scout;
		else if (charType == Characters.Scout)
			charType = Characters.Debug;

		Debug.Log("Cheatcode set charType to " + charType);

		/*
		//Update placeholder colour
		if (charType == Characters.Sailor)
			bodyMat.SetColor("_BaseColor", SailorCol);
		else if (charType == Characters.Scout)
			bodyMat.SetColor("_BaseColor", ScoutCol);
		else
			bodyMat.SetColor("_BaseColor", Color.white);
		*/

	}

	private void UpdateGrounded() {

		RaycastHit2D castA = Physics2D.Raycast((Vector2)transform.position + raycastStartPos, Vector2.down, raycastCheckDist, groundLayers);
		RaycastHit2D castB = Physics2D.Raycast((Vector2)transform.position + raycastStartPos * (Vector3.up + Vector3.left), Vector2.down, raycastCheckDist, groundLayers);

		Debug.DrawRay((Vector2)transform.position + raycastStartPos, Vector2.down * raycastCheckDist, Color.red);
		Debug.DrawRay((Vector2)transform.position + raycastStartPos * (Vector3.up + Vector3.left), Vector2.down * raycastCheckDist, Color.red);

		if (castA || castB) {
			if (!isGrounded)
				currentSquishFactor = Mathf.Clamp(1 / Mathf.Abs(lastYVel * 0.1f), 0.4f, 1); //0.6f;
			isGrounded = true;

			if ((castA && castA.collider.tag == "Hazard") || (castB && castB.collider.tag == "Hazard"))
				return;

			steppingOn = (castA ? castA.collider : castB.collider).sharedMaterial;

			//Do not refund dash if player touched the ground while they were dashing
			//this will allow the dash to be refunded AFTER the player completes the
			//dash, but if they only slightly tap a playform mid-dash, it wont return
			//their dash.
			if (!isDashing && !isBracing)
				dashAvailable = true;
			StopCoroutine("CoyoteNotGrounded");
		} else {
			StartCoroutine("CoyoteNotGrounded");
			steppingOn = null;
		}

	}

	private void BeginDash() {

		BeginDash(new Vector2Int(rigidbodyVelOrientation, 1));

	}

	private void BeginDash(Vector2Int dir) {

		dashingToward = dir;

		audio.Dash();

		StartCoroutine("Dashing");

	}

	public void Die () {
		if (GamePhaseManager.instance && GamePhaseManager.instance.levels != null && GamePhaseManager.instance.levels.Count > 0 && GamePhaseManager.instance.levels[GamePhaseManager.instance.currentLevel])
			transform.position = GamePhaseManager.instance.levels[GamePhaseManager.instance.currentLevel].spawnPoint.position;
		slipNSliding = 0;
	}

    public void SpawnConfetti()
    {
        ParticleSystem deathConfetti = Instantiate(confettiParticleSystem, transform.position + Vector3.up * 10, confettiParticleSystem.transform.rotation);
        deathConfetti.Play();
	}

	/*
	//Respawn behaviour no longer necessary
	public void StartRespawn() {

		//Don't start respawn coroutine if the player is suspended (Ie. if already respawning)
		if (rigidbody.isKinematic)
			return;

		StartCoroutine("Respawn");

	}
	*/

	//Undos are a way of keeping track of actions that should be reverted on death, 
	//I.e. if you die, all the blocks you destroyed since the last checkpoint should
	//be restored, etc.
	public void AddToUndos(Undoable undoable) {
		Debug.Log("Adding undo: " + name);
		undosUntilLastCheckpoint.Add(undoable);
		undosGLOBAL.Add(undoable);
		Debug.Log("Undos since checkpoint: " + undosUntilLastCheckpoint.Count);
	}
	//Should only be called by checkpoint once it is activated
	public void ClearUndos() {
		Debug.LogWarning("clearing " + undosUntilLastCheckpoint.Count + " undos");
		undosUntilLastCheckpoint.Clear();
		Debug.Log("Undos since checkpoint: " + undosUntilLastCheckpoint.Count);
	}
	public void ClearGLOBALUndos() {
		undosGLOBAL.Clear();
	}
	//Should be called on death
	public void ExecuteUndos() {
		Debug.LogWarning("executing " + undosUntilLastCheckpoint.Count + " undos");
		foreach (Undoable undoable in undosUntilLastCheckpoint) {
			undoable.Undo();
		}
		ClearUndos();
	}
	//Should be called on death
	public void ExecuteGLOBALUndos() {
		foreach (Undoable undoable in undosGLOBAL)
			undoable.Undo();
		ClearGLOBALUndos();
	}

    private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.GetComponent<Rigidbody2D>() && collision.gameObject.GetComponent<Rigidbody2D>().sharedMaterial == icePhysMat)
			slipNSliding++;
	}

	private void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.GetComponent<Rigidbody2D>() && collision.gameObject.GetComponent<Rigidbody2D>().sharedMaterial == icePhysMat)
			slipNSliding--;
	}

	public void OnCollisionStay2D(Collision2D collision) {

		//If this is a hazard, respawn the player and cancel method call
		if (collision.gameObject.tag == "Hazard") {
			Die();
			return;
		}

		//Completely ignore all walljump/wallgrab behaviours if the player is a sailor
		if (charType == Characters.Sailor)
			return;

		//Make sure the array size is valid
		if (collision.contactCount != contacts.Length)
			contacts = new ContactPoint2D[collision.contactCount];

		//Load collision points to our array
		collision.GetContacts(contacts);

		//Iterate through collision points and verify if at least one is within valid bound
		foreach (ContactPoint2D contact in contacts) {

			if ((Mathf.Abs(contact.point.x - transform.position.x) > minCollisionXDistance) &&
				(Mathf.Abs(Input.GetAxisRaw(inputHorizontalAxisName)) > 0.1f) &&
				(contact.point.x - transform.position.x < 0 == Input.GetAxisRaw(inputHorizontalAxisName) < 0) &&
				(contact.collider.gameObject.tag == "Grabbable")) {

				//Behaviour during wall-grabbing

				//Stop player from falling
				//if (rigidbody.velocity.y <= 0)
					//rigidbody.velocity = Vector3.zero;

				lastWallgrabTime = Time.time;
				lastWallgrabContactXPos = contact.point.x;

				//If one collision point was valid, stop checking if others are valid to save resources and avoid double-method-calls
				return;

			}

		}

	}

	//Set isGrounded to false after a set delay, to allow player to still jump even if they're late to press jump
	private IEnumerator CoyoteNotGrounded() {

		yield return new WaitForSeconds(coyoteTime);

		isGrounded = false;

	}

	//Handle dashing
	private IEnumerator Dashing() {

		//Don't dash if kinematic;
		if (rigidbody.isKinematic)
			yield break;

		//Use high-jump gravity scale during dash
		rigidbody.gravityScale = jumpHoldGravMulti;

		float dashStart = Time.time;    //Record when the dash started

		isBracing = true; //Player is now bracing for dash

		//Freezeframe at the start of the dash for extra oomph
		while (Time.time < dashStart + dashBraceFreeze) {

			//Back the player up before dashing for less weird movement, unless they're dashing upwards
			if (dashingToward.y < 0.5f)
				rigidbody.velocity = (Vector2)dashingToward * -dashVelocity * 0.25f;
			else
				rigidbody.velocity = Vector3.zero;

			yield return new WaitForEndOfFrame();

			//While bracing, orient the sailor towards the dash direction
			if (charType != Characters.Scout)
				PlayerSprite.transform.localRotation = Quaternion.Lerp(
					PlayerSprite.transform.localRotation, //LerpFrom (currentRot)
					Quaternion.Euler(                   //LerpTo
						Mathf.Atan2(dashingToward.y, dashingToward.x) * Mathf.Rad2Deg * -1,        //Use calculated X rotation
						90,                                                 //Use 90 for Y axis for proper orientation
						90                                                  //Use 90 for Z axis for proper orientation
					),
				0.1f * offCenterSpinSpeed           //Lerp using speed specified in inspector
			);
		}

		isBracing = false;
		isDashing = true;
		dashStart = Time.time;    //Record when the dash started again after the brace freeze

		//Keep setting the velocity if the dash is not supposed to be done
		while (Time.time < dashStart + dashDuration) {

			rigidbody.velocity = (Vector2)dashingToward * dashVelocity;

			//The following code is meant to apply screenshake on dash, but since the original CameraFollower behaviour is not used in this project, it has been disabled 
			/*
			//Constantly set dash shake factor throughout the entire dash (do not let the shake decay mid-dash)
			if (CameraFollower.instance.shakeFactor.magnitude < dashShakeFac)
				CameraFollower.instance.shakeFactor = CameraFollower.AbsoluteVectorValues((Vector2)dashingToward) * dashShakeFac;
			*/

			yield return new WaitForEndOfFrame();

			// ========== Character rotation while dashing (facing forwards, rotate on Z axis) ========== //
			if (charType != Characters.Scout)
				PlayerSprite.transform.localRotation = Quaternion.Lerp(
					PlayerSprite.transform.localRotation, //LerpFrom (currentRot)
					Quaternion.Euler(                   //LerpTo
						Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg * -1,        //Use calculated X rotation
						90,   //Use 90 for Y axis for proper orientation
						90    //Use 90 for Z axis for proper orientation
					),
					0.1f * offCenterSpinSpeed           //Lerp using speed specified in inspector
				);
		}

		//Do not include the velocity falloff window as part of the dash
		isDashing = false;

		while (Time.time < dashStart + (2 * dashDuration)) {
			if (dashingToward.x != 0)
				rigidbody.velocity /= 1 + Mathf.Max(postDashXDampening, 0);
			if (dashingToward.y != 0)
				rigidbody.velocity /= 1 + Mathf.Max(postDashYDampening, 0);
			//Use high-jump gravity scale during dash
			//update the value in case the player taps jump during dash
			rigidbody.gravityScale = jumpHoldGravMulti;
			yield return new WaitForEndOfFrame();

		}
		//Restore non-hold jump grav scale
		rigidbody.gravityScale = jumpReleaseGravMulti;

	}

	//Respawn code is no longer necessary
	/*
	private IEnumerator Respawn() {

		//Disable player physics during respawn
		rigidbody.isKinematic = true;
		rigidbody.velocity = Vector2.zero;

		StopCoroutine("Dashing");
		StopCoroutine("CoyoteNotGrounded");
		isDashing = false;
		isGrounded = false;
		steppingOn = null;

		float time = 0;
		Vector3 startPos = transform.position;
		Vector3 pos = startPos;

		yield return new WaitForSeconds(preRespawnDelay);

		//Iterate and move player out of bounds;
		while (time <= lerpOutOfBoundsDuration) {
			pos.z = Mathfx.Sinerp(startPos.z, travelToCheckpointZDepth, time / lerpOutOfBoundsDuration);
			transform.position = pos;
			yield return null;
			time += Time.deltaTime;
		}
		time = 0;
		//Iterate and move player to the checkpoint;
		while (time <= lerpToCheckpointDuration) {
			pos.x = Mathfx.Hermite(startPos.x, lastCheckpoint.transform.position.x, time / lerpToCheckpointDuration);
			pos.y = Mathfx.Hermite(startPos.y, lastCheckpoint.transform.position.y, time / lerpToCheckpointDuration);
			transform.position = pos;
			yield return null;
			time += Time.deltaTime;
		}
		time = 0;
		//Iterate and move player back in bounds;
		while (time <= lerpOutOfBoundsDuration) {
			pos.z = Mathfx.Clerp(travelToCheckpointZDepth, startPos.z, time / lerpOutOfBoundsDuration);
			transform.position = pos;
			yield return null;
			time += Time.deltaTime;
		}

		pos.z = startPos.z;
		transform.position = pos;

		//Undo all actions since last checkpoint
		ExecuteUndos();

		//Re-enable player physics and reset velocity to avoid sudden burst of speed
		rigidbody.isKinematic = false;
		rigidbody.velocity = Vector2.up * jumpForce;

	}
	*/

}
