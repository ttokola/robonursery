using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	public float speed;
	public SyncClient client;
	public Camera cam;
	public RenderTexture renderHere;
	public AudioListener audio;

	private Thread listeningThread;
	private Rigidbody rb;
	private Vector3 position;
	private String message;
	private String spesifications = "Motors: [moveHorizontal(-1,1); moveVertical(-1,1)], Sensors: [Location(-inf,inf;-inf,inf;-inf,inf)]";
	private RenderTexture rt;
	private int imageSize = 300;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		client.StartClient();

		// Listen to the server
		listeningThread = new Thread(client.Listen);
		listeningThread.Start();

		// Disable the camera so that it can be manually rendered
		cam.enabled = false;

		// Create the RenderTexture
		rt = new RenderTexture(imageSize, imageSize, 16, RenderTextureFormat.ARGB32);
	}

	// Update is called once per frame
	void Update () {


		// RENDER IMAGE
		RenderTexture currentRT = rt;
		cam.targetTexture = rt;
		RenderTexture.active = cam.targetTexture;

		cam.Render();
		// 2x2, ARGB32 format image with mipmaps
		Texture2D image = new Texture2D(imageSize, imageSize, TextureFormat.ARGB32, true);
		image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
		image.Apply();
		RenderTexture.active = currentRT;
		byte[] rawData = image.GetRawTextureData();

		// GET AUDIO
		float[] right = AudioListener.GetOutputData(1024, 0);
		float[] left = AudioListener.GetOutputData(1024, 1);

		var rightBytes = new byte[right.Length * 4];
		Buffer.BlockCopy(right, 0, rightBytes, 0, rightBytes.Length);

		var leftBytes = new byte[left.Length * 4];
		Buffer.BlockCopy(left, 0, leftBytes, 0, leftBytes.Length);


		// // SEND IMAGE
		client.SendBytes(rawData, client.videoSocket);
		client.Send("DONE", client.videoSocket);
		Debug.Log(rawData.Length.ToString());

		// // SEND AUDIO
		client.SendBytes(rightBytes, client.audioCH1Socket);
		client.SendBytes(leftBytes, client.audioCH2Socket);

	}

	void FixedUpdate() {

		position = transform.position;

		message = client.getMessage();

		switch (message) {
			case "SPESIFICATIONS":
				client.Send(spesifications, client.controlSocket);
				break;
			case "END SIMULATION":
				client.Send("SIMULATION ENDED", client.controlSocket);
				client.StopClient();
				Application.Quit();
				break;
			default:
				if (!String.IsNullOrEmpty(message) && message != "SPESIFICATIONS") {
					float[] commands = parseCommand(message);

					moveVertical(commands[0], 300);
					moveHorizontal(commands[1], 300);
				}

				// SEND SENSOR DATA
				client.Send(position.ToString(), client.controlSocket);
				break;
		}
	}

	void OnApplicationQuit() {
		client.StopClient();
		listeningThread.Abort();
	}


	public float[] parseCommand(string command) {
		string[] movementStrings = command.Split(';');

		float moveX;
		float moveY;


		Single.TryParse(movementStrings[0], out moveX);
		Single.TryParse(movementStrings[1], out moveY);
		float[] movement = new float[2] {moveX, moveY};

		return movement;
	}

	// MOTORS AND SENSORS
	private void moveVertical(float amount, int speed) {
		Vector3 movement = new Vector3 (amount, 0.0f, 0.0f);
		rb.AddForce(movement * speed);
	}

	private void moveHorizontal(float amount, int speed) {
		Vector3 movement = new Vector3 (0.0f, 0.0f, amount);
		rb.AddForce(movement * speed);
	}

}
