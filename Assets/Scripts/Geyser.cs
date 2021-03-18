using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * Geyser. Animates and pushes players within upwards periodically.
 */

public class Geyser : MonoBehaviour
{
    
    public ParticleSystem geyserParticleSystem; // attached geyser particle system

    public float GeyserForce = 10;
    public float speedCap;

    // animation vars
    public AnimationCurve geyserAnim;
    public float geyserAnimScale;
    public float geyserAnimSpeed;
    private float geyserAnimT;

    private float geyserHeightInBlocks; 


    private void Update()
    {
        // geyser "animation"
        geyserAnimT += Time.deltaTime * geyserAnimSpeed;

        transform.localPosition = Vector3.up * geyserAnim.Evaluate(geyserAnimT) * geyserAnimScale;
        transform.localScale = (Vector3.one * 0.9f) + (Vector3.up * geyserAnim.Evaluate(geyserAnimT) * geyserAnimScale * 2);

        GeyserParticleSystemGrowth(); // function for checking the geyser's height every frame and adjusting the particle system accordingly
    }

    // players in geyser are pushed up
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other && other.CompareTag("Player"))
        {
            if (other.attachedRigidbody.velocity.y < speedCap)
                other.attachedRigidbody.AddForce(Vector3.up * GeyserForce * geyserAnim.Evaluate(geyserAnimT), ForceMode2D.Force);
        }
    }

    void GeyserParticleSystemGrowth()
    {  // This is my function for controlling the particle system's 'Start Lifetime' and 'Start Speed'

        geyserHeightInBlocks = (geyserAnim.Evaluate(geyserAnimT)) * (geyserAnimScale) * 2; // this formula gets the geyser's height in blocks

        // The following is my code for adjusting the geyser particle system in response to the height of the geyser hitbox.

        ParticleSystem.MainModule particleSystemMain = geyserParticleSystem.main; // allows the MainModule of the GeyserParticleSystem to be modified

        if (geyserHeightInBlocks < 1) // if the geyser hitbox is less than one block high then it just stays short
        {
            particleSystemMain.startLifetime = 1.00f;
            particleSystemMain.startSpeed = 0.25f;
        }

        if (geyserHeightInBlocks >= 1) // when the geyser hitbox is higher than one block high then it begins to scale with height
        {
            particleSystemMain.startLifetime = 1.25f + 0.5f * geyserHeightInBlocks;
            particleSystemMain.startSpeed = 1.35f + 0.65f * geyserHeightInBlocks;
        }
    }
}
