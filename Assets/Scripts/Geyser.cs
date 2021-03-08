using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * Geyser. Animates and pushes players within upwards periodically.
 */

public class Geyser : MonoBehaviour
{
    public float GeyserForce = 10;

    // animation vars
    public AnimationCurve geyserAnim;
    public float geyserAnimScale;
    public float geyserAnimSpeed;
    private float geyserAnimT;


    private void Update()
    {
        // geyser "animation"
        geyserAnimT += Time.deltaTime * geyserAnimSpeed;

        transform.localPosition = Vector3.up * geyserAnim.Evaluate(geyserAnimT) * geyserAnimScale;
        transform.localScale = (Vector3.one * 0.9f) + (Vector3.up * geyserAnim.Evaluate(geyserAnimT) * geyserAnimScale * 2);
    }

    // players in geyser are pushed up
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other && other.CompareTag("Player"))
        {
            other.attachedRigidbody.AddForce(Vector3.up * GeyserForce * geyserAnim.Evaluate(geyserAnimT), ForceMode2D.Force);
        }
    }
}
