using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAudio : MonoBehaviour {

	public float stepAlernatingPitch;
	public float stepDistance;

	public float diveAlternatingPitch;
	public float diveDistance;

	[Header("Empty Parameters")]
	public float volume_Empty;
	public PhysicsMaterial2D physMat_Empty;
	public AudioClip[] footsteps_Empty;
	[Header("Sandy Parameters")]
	public float volume_Sandy;
	public GameObject audioSourcePrefab;
	public PhysicsMaterial2D physMat_Sandy;
	public AudioClip[] footsteps_Sandy;
	[Header("Metal Parameters")]
	public float volume_Metal;
	public PhysicsMaterial2D physMat_Metal;
	public AudioClip[] footsteps_Metal;
	[Header("Dash Parameters")]
	public float volume_Dash;
	public AudioClip dash;
	[Header("Dive")]
	public float volume_Dive;
	public AudioClip dive;

	private static GameObject _audioSourcePrefab;
	private static List<AudioSource> audioSourcePool = new List<AudioSource>();
	private static List<AudioSource> audioSourceActive = new List<AudioSource>();

	private Rigidbody2D rigidbody;
	private PlayerMovement player;
	private PhysicsMaterial2D lastSteppedOn;
	private Vector2 lastStep;
	private Vector2 lastDive;
	private int stepCount;

	// Start is called before the first frame update
	void Start() {

		_audioSourcePrefab = audioSourcePrefab;

		rigidbody = GetComponent<Rigidbody2D>();
		player = GetComponent<PlayerMovement>();

	}

	// Update is called once per frame
	void Update() {

		if (!rigidbody.isKinematic && !player.isDashing) {

			//If we're far away enough from the last step position, play another step
			if (Vector2.Distance(lastDive, transform.position) > diveDistance) {
				lastDive = transform.position;
				PlayClipAtPoint(dive, transform.position, volume_Dive, 1 + (Random.Range(-1, 1) * diveAlternatingPitch));
			}

		}

		//Do not play any footsteps sounds while respawning, dashing, swimming;
		if (rigidbody.isKinematic || player.isDashing || player.isBracing)
			return;

		//If we're far away enough from the last step position, play another step
		if (Vector2.Distance(lastStep, transform.position) > stepDistance || !lastSteppedOn) {
			lastStep = transform.position;
			if (player.steppingOn)
				playRandomStep(player.steppingOn);
		}

		lastSteppedOn = player.steppingOn;

	}

	public void Dash() {

		PlayClipAtPoint(dash, transform.position, volume_Dash, 1);

	}

	public AudioSource playRandomStep(PhysicsMaterial2D material) {

		AudioClip clip = null;
		float vol = 0;
		if (material == physMat_Empty) {
			clip = RandomClipFromArray(footsteps_Empty);
			vol = volume_Empty;
		} else if (material == physMat_Sandy) {
			clip = RandomClipFromArray(footsteps_Sandy);
			vol = volume_Sandy;
		} else if (material == physMat_Metal) {
			clip = RandomClipFromArray(footsteps_Metal);
			vol = volume_Metal;
		} else {//By default fallback to metal footsteps with no volume
			clip = RandomClipFromArray(footsteps_Metal);
			vol = 0;
		}

		stepCount++;

		return PlayClipAtPoint(clip, transform.position, vol, 0.9f + ((stepCount % 2 == 0 ? 1 : -1) * stepAlernatingPitch));

	}

	public static AudioClip RandomClipFromArray(AudioClip[] array) {
		if (array.Length <= 0)
			return null;
		else
			return array[Random.Range(0, array.Length - 1)];
	}

	public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, float vol, float pitch) {

		//Recycle audioSources that are no longer playing
		for (int i = 0; i < audioSourceActive.Count; i++) {
			//Debug.Log(audioSourceActive[i].isPlaying);
			if (!audioSourceActive[i].isPlaying) {
				audioSourcePool.Add(audioSourceActive[i]);
				audioSourceActive.Remove(audioSourceActive[i]);
				i--;
			}
		}

		AudioSource source = null;

		if (audioSourcePool.Count >= 1 && !audioSourcePool[0].isPlaying)
			source = audioSourcePool[0];
		else if (_audioSourcePrefab) {
			source = Instantiate(_audioSourcePrefab).GetComponent<AudioSource>();
		} else {
			source = new GameObject("RecyclableAudioSource").AddComponent<AudioSource>();
		}

		audioSourceActive.Add(source);
		source.clip = clip;
		source.transform.position = pos;
		source.volume = vol;
		source.pitch = pitch;
		source.Play();

		return source;

	}

}