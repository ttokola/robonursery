using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class SyncClient : MonoBehaviour {

	public Socket controlSocket;
	public Socket videoSocket;
	public Socket audioCH1Socket;
	public Socket audioCH2Socket;
	// Data buffer
	private byte[] bytes = new byte[1024];
	private static Queue<string> messages = new Queue<string>();

	// Use this for initialization
	public void StartClient() {

		// Connect to server
		try{
			// Establish endpoint on localhost and port 9001
			// IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost"); // Do I need this?
			IPAddress ipAddress = System.Net.IPAddress.Parse("127.0.0.1");

			IPEndPoint remoteEp1 = new IPEndPoint(ipAddress, 9004);
			IPEndPoint remoteEp2 = new IPEndPoint(ipAddress, 9005);
			IPEndPoint remoteEp3 = new IPEndPoint(ipAddress, 9006);
			IPEndPoint remoteEp4 = new IPEndPoint(ipAddress, 9007);

			// Create a TCP/IP Sockets
			// Only control socket is listened to. Other sockets are only for sending data.
			controlSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			videoSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			audioCH1Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			audioCH2Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			//Try to connect to the endpoint
			try{
				controlSocket.Connect(remoteEp1);
				videoSocket.Connect(remoteEp2);
				audioCH1Socket.Connect(remoteEp3);
				audioCH2Socket.Connect(remoteEp4);

				Send("SIMULATION READY", controlSocket);

			} catch (Exception e) {
				Debug.Log(e.ToString());
			}

		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	public void StopClient() {
		// CLOSE SOCKETS
		controlSocket.Shutdown(SocketShutdown.Both);
		videoSocket.Shutdown(SocketShutdown.Both);
		audioCH1Socket.Shutdown(SocketShutdown.Both);
		audioCH2Socket.Shutdown(SocketShutdown.Both);
		
		controlSocket.Close();
		videoSocket.Close();
		audioCH1Socket.Close();
		audioCH2Socket.Close();
	}

	public void Listen() {
		// Only control socket is listened to
		while(true) {
			// Add message to queue
			messages.Enqueue(ReceiveMessage(controlSocket));
		}
	}

	public string getMessage() {
		if (messages.Count > 0) {
			return messages.Dequeue();
		} else {
			// No messages
			return null;
		}
	}

	public void SendBytes(byte[] bytes, Socket socket) {
		// Send bytes as is
		try {
			socket.Send(bytes);

		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	public void Send(string message, Socket socket) {
		try {
			// Encode the message into byte array
			byte[] msg = Encoding.ASCII.GetBytes(message);

			// Send data through the socket
			int bytesSent = socket.Send(msg);
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	private string ReceiveMessage(Socket socket) {
		int bytesRec = socket.Receive(bytes);
		return Encoding.ASCII.GetString(bytes, 0, bytesRec);
	}

}
