using UnityEngine;
using System.Collections;
using GameSparks.Api;

public class TestUserAutoAuthenticate : MonoBehaviour {
	public string username = "joetest01";
	public string password = "test";

	void Start () {
		Debug.Log("TestUserAutoAuthenticate Start");
		GameSparksManager.Instance().OnGSAvailable += (bool _isAvailable) => {
			Debug.Log("Beginning auto-authenticate with user " + username);
			GameSparksManager.Instance().Authenticate(username, password, 
				(authResponse) => Debug.Log("Authenticated."), 
				(error) => Debug.Log(error.errorMessage.ToString()));
		};
	}

}