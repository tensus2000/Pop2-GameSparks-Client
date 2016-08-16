using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System.Text.RegularExpressions;

public class GameSparksManager : MonoBehaviour {


	#region Singleton
	/// <summary>
	/// a refrence to the game-state manager object used for the singleton
	/// </summary>
	private static GameSparksManager instance = null;
	/// <summary>
	/// This method returns the current instance of this class.
	/// The singelton ensures that only one instance of the game-state manager exists.
	/// </summary>
	/// <returns>an instance of the Game-State Manager</returns>
	public static GameSparksManager Instance()
	{
		if(instance != null){	return instance; }
		Debug.LogError("GSM | GameSparks Not Initialized...");
		return instance;
	}
	#endregion

	void Awake()
	{
		
		if(instance == null){ // when the first GSM is activated, it should be null, so we create the reference

			Debug.Log ("GSM | Singleton Initilized...");
			instance = this;
			DontDestroyOnLoad(this.gameObject); // gamesparks manager persists throughout game
		}
		else {
			// if we load into a level that has the gamesparks manager present, it should be destroyed //
			// there can be only one! //
			Debug.Log ("GSM | Removed Duplicate...");
			Destroy(this.gameObject);
		}
	}


	// Use this for initialization
	void Start () {

		// This is a callback which is triggered when gamesparks connects and disconnects //
		// Note: on disconnect, this needs a request to timeout before it will know that the socket has closed //
		// i.e. we cannot tell the SDK the socket is closed if it is closed (since there is no connection) //
		GS.GameSparksAvailable += ((bool _isAvail) => {
			if (_isAvail) {
				Debug.LogWarning ("GameSparks Connected...");
			}else{
				Debug.LogWarning ("GameSparks Disconnected...");
			}
		});


		// THE FOLLOWING ARE EXAMPLE OF HOW WE CAN DETECT DIFFERNT TYPES OF SOCKET-MESSAGES COMING FROM THE SERVER //
		// THESE CAN BE CALLED FROM ANYWHER, THEY DONT NEED TO BE CALLED FROM THE GAMESPARKSMANAGER.cs //

		// ------- SESSION TERMINATED MESSAGE RECIEVED ------- //
		GameSparks.Api.Messages.SessionTerminatedMessage.Listener += (_message) => {
			// there is no other pertenant data returned here //
			// contact GameSparks if you need extra information to be returned from this message //
			Debug.LogWarning("GSM| Player Has Been Logged Out Due To Concurrent Login...");
		};
		// ----- GLOBAL INBOX MESSAGE RECIEVED ------ //
		GameSparks.Api.Messages.ScriptMessage_globalUserMessage.Listener += (_message) => {
			// there are 3 important things returned in this message //
			// [1] - the header. This is the 'title' or  'subject' of the message //
			// [2] - the body, which is the main text of the message (html can be included if you use the rich-text option in the unity GUI system //
			// [3] - the payload. This is JSON/GSData which can be checked to trigger events in the client //
			// [additional] - we can get the character id, and message id from these messages also //
			string header = _message.Data.GetString("header");
			string body = _message.Data.GetString("body");
			GSData payload = _message.Data.GetGSData("payload");
			string characterID = _message.Data.GetString("character_id");
			string messageID = _message.MessageId;
			// The message ID is very important, is it is used to tell which message to dismiss later //
			// therefore, when drawing message details you should cache the message ID for later use //
			Debug.LogWarning("GSM| New Global Message  \n"+_message.JSONString);
			Debug.LogWarning("GSM| Header: "+header);
			Debug.LogWarning("GSM| Body: "+body);
			Debug.LogWarning("GSM| Chacacter ID: "+characterID);
			Debug.LogWarning("GSM| Message ID: "+messageID);
		};
	}
	
	#region AUTHENTICATION CALLS
	/// <summary>
	/// Auth callback.
	/// Can be used just to trigger some code when a request comes through (either sucessful or unsucessful)
	/// </summary>
	public delegate void auth_callback(AuthenticationResponse _resp);
	/// <summary>
	/// Logs the player into the gamesparks platform
	/// </summary>
	/// <param name="_userName">User name.</param>
	/// <param name="_password">Password.</param>
	public void Authenticate(string _userName, string _password, auth_callback _onLoginSucess, auth_callback _onLoginFailed){
		Debug.Log(_userName+"|"+_password);
		Debug.Log ("GSM| Attempting Player Authentication....");
		new GameSparks.Api.Requests.AuthenticationRequest ()
			.SetPassword (_password)
			.SetUserName (_userName)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log ("GSM| Authentication Sucessful \n" + response.DisplayName);
				if(_onLoginSucess != null){
					_onLoginSucess(response);
				}
			} else {
				Debug.LogWarning ("GSM| Error Authenticating Player \n " + response.Errors.JSON);
				if(_onLoginFailed != null){
					_onLoginFailed(response);
				}
			}
		});

	}

	public delegate void reg_callback(RegistrationResponse _resp);
	/// <summary>
	/// Register the specified _userName, _displayName and _password.
	/// </summary>
	/// <param name="_userName">User name.</param>
	/// <param name="_displayName">Display name.</param>
	/// <param name="_password">Password.</param>
	public void Register(string _userName, string _displayName, string _password, reg_callback _onLoginSucess, reg_callback _onLoginFailed){
		Debug.Log ("GSM| Attempting Registration...");
		new GameSparks.Api.Requests.RegistrationRequest ()
			.SetUserName (_userName)
			.SetDisplayName (_displayName)
			.SetPassword (_password)
			.Send ((response) => {
				if(!response.HasErrors){
					Debug.Log ("GSM| Registration Sucessful \n" + response.UserId);
					if(_onLoginSucess != null){
						_onLoginSucess(response);
					}
				}else{
					Debug.LogWarning ("GSM| Error Registering Player \n " + response.Errors.JSON);
					if(_onLoginFailed != null){
						_onLoginFailed(response);
					}
				}

		});
	}
	#endregion



	#region INVENTORY CALLS
	/// <summary>
	/// Logevent callback.
	/// </summary>
	public delegate void logevent_callback(LogEventResponse _resp);
	/// <summary>
	/// Gets the inventory.
	/// </summary>
	public void getInventory(logevent_callback _callback)
	{
		Debug.Log ("GSM| Fetching Inventory Items...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("getInventory")
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.Log("GSM| Inventory Found...");
				if(_callback != null){
					_callback(response);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);
			}
		});
	}


	public void dropItem(int _inv_id, int _scene_id, int _x, int _y, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Drop Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("dropItem")
			.SetEventAttribute ("inventoryItemId", _inv_id)
			.SetEventAttribute ("sceneId", _scene_id)
			.SetEventAttribute ("x", _x)
			.SetEventAttribute ("y", _y)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item Dropped...");
				if (_callback != null) {
					_callback (response);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
			}
		});

	}


	public void pickUpItem(int _item_id, int _scene_id, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Drop Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("pickUpItem")
			.SetEventAttribute ("placedItemId", _item_id)
			.SetEventAttribute ("sceneId", _scene_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item Picked Up...");
				if (_callback != null) {
					_callback (response);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
			}
		});
	}


	public void moveItem(int _item_id, int _slot_id, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Move Item...");
		new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("moveItem")
			.SetEventAttribute ("inventoryItemID", _item_id)
			.SetEventAttribute ("destinationSlot", _slot_id)
			.Send ((response) => {
			if (!response.HasErrors) {
				Debug.LogWarning ("GSM| Item Moved...");
				if (_callback != null) {
					_callback (response);
				}
			} else {
				Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
			}
		});
	}


	public void equipItem(int _item_id, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Equip Item...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("equipItem")
			.SetEventAttribute("inventoryItemID", _item_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Item Equipped...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}


	/// <summary>
	/// Uses the item.
	/// </summary>
	/// <param name="_item_id">Item identifier.</param>
	/// <param name="_callback">Callback.</param>
	public void useItem(int _item_id, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Use Item...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("useItem")
			.SetEventAttribute("inventoryItemID", _item_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Item Used...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}

	#endregion


	#region SCENE CALLS

	public void getSceneState(int _island_id, int _scene_id, logevent_callback _callback)
	{
		Debug.Log ("Fecthing Scenes ...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getSceneState")
			.SetEventAttribute("island_id", _island_id)
			.SetEventAttribute("scene_id", _scene_id)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Scenes Retrieved...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}


	public void setSceneState(int _island_id, int _scene_id, GSRequestData _scene_data, logevent_callback _callback)
	{
		Debug.Log ("Attempting To Set Scene State ...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("setSceneState")
			.SetEventAttribute("island_id", _island_id)
			.SetEventAttribute("scene_id", _scene_id)
			.SetEventAttribute("states", _scene_data)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| Scene Set...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}


	#endregion 


	#region Player Details
	/// <summary>
	/// Level and exp callback.
	/// Having the giveXP and GetXp calls return the same data, it means you can assign one call to this delegate
	/// and it will automatically re-draw the new xp and level values instead of requesting them again.
	/// </summary>
	public delegate void levelAndExp_callback(int _level, int _exp);
	/// <summary>
	/// Adds XP to the player and checks that the player has reached enough XP to gain a level.
	/// 
	/// </summary>
	/// <param name="_amount">Amount.</param>
	/// <param name="_callback">Callback.</param>
	public void GiveXp(int _amount, levelAndExp_callback _callback){
		Debug.Log ("Attempting To Give Xp...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("giveXp")
			.SetEventAttribute("amount", _amount)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| XP Granted...");
					// first we get the json where the level and experiance is //
					GSData resp = response.ScriptData.GetGSData("player_details");
					// then we pass the level and xp into the callback //
					_callback(resp.GetInt("level").Value, resp.GetInt("experience").Value);
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}


	/// <summary>
	/// Gets the level and experiance.
	/// </summary>
	/// <param name="_callback">Callback, returns the level and experience</param>
	public void GetLevelAndExperiance(levelAndExp_callback _callback){

		Debug.Log ("Retrieving Player Level & Experiance...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getLevelAndExperience")
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Returned Level & XP...");
					// first we get the json where the level and experiance is //
					GSData resp = response.ScriptData.GetGSData("player_details");
					// then we pass the level and xp into the callback //
					_callback(resp.GetInt("level").Value, resp.GetInt("experience").Value);
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}



	/// <summary>
	/// Reset password on sucess callback.
	/// </summary>
	public delegate void resetPassword_onSucess_callback(string _newPassword);
	/// <summary>
	/// Reset password on failed callback.
	/// There are several types of error that can be returned.
	/// If the old-password was invalid, if the new-password was invalid.
	/// In the later case, the strength of the password will also be retuned.
	/// This can be removed later.
	/// </summary>
	public delegate void resetPassword_onFailed_callback(bool _invalidPassword, string _failedPassword, string _strength);
	/// <summary>
	/// This function will send a old and new password to the server.
	/// It will validate the old password and check the strength of the new password 
	/// </summary>
	/// <param name="old_password">Old password.</param>
	/// <param name="new_password">New password.</param>
	/// <param name="_onSucess">A callback for when the request is sucessful. Is nullable</param>
	/// <param name="_onFailed">A callback for when the request has failed. Is nullable</param>
	public void ResetPassword(string old_password, string new_password, resetPassword_onSucess_callback _onSucess, resetPassword_onFailed_callback _onFailed){
		if (old_password != string.Empty && new_password != string.Empty) {
			Debug.Log ("Retrieving Player Level & Experiance...");
			new GameSparks.Api.Requests.LogEventRequest().SetEventKey("resetPassword")
				.SetEventAttribute("old_password", old_password)
				.SetEventAttribute("new_password", new_password)
				.Send ((response) => {
					if (!response.HasErrors) {
						Debug.Log ("GSM| Password Changed...");
						if (_onSucess != null) {
							_onSucess (response.ScriptData.GetString("@resetPassword"));
						}
					} else {
						Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
						if (_onFailed != null) {
							GSData errors = response.Errors.GetGSData("@resetPassword");
							// get the strength of the password and the submitted password //
							if(errors.GetString("message") == "old-password-invalid"){ // the old-password didnt match
								_onFailed(true, null, null);
							}else { // failed a strength test
								string failedPassword = errors.GetString("failed-password");
								string strength = errors.GetString("strength");
								_onFailed(false, failedPassword, strength);
							}
						}
					}
				});
		} else {
			Debug.LogWarning ("GSM| old-password or new-password empty...");
		}
	}


	/// <summary>
	/// Registers the parent email.
	/// Does not return anything for the moment.
	/// Contact GameSparks if a response is needed.
	/// </summary>
	/// <param name="parent_email">parent_email, this is validated server-side aslo</param>
	public void RegisterParentEmail(string parent_email){
		Debug.Log ("GSM| Submitting Parent Email...");
		if (IsValidEmail (parent_email)) {
			new GameSparks.Api.Requests.LogEventRequest ().SetEventKey ("submitParentEmail")
				.SetEventAttribute ("parent_email", parent_email)
				.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Parent Email Pending...");
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
		} else {
			Debug.LogWarning ("GSM| Invalid Email: "+parent_email);
		}
	}
	#endregion


	#region System
	/// <summary>
	/// Getserverversion callback.
	/// this is used to make sure that the user has a response from the server before trying to use the version-string.
	/// </summary>
	public delegate void getserverversion_callback(string _version);
	/// <summary>
	/// Requests the current version from ther server.
	/// </summary>
	/// <param name="_callback">Callback, returns version string</param>
	public void GetServerVersion(getserverversion_callback _callback){
		Debug.Log ("GSM| Fetching Server Version...");

		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getServerVersion")
			.Send ((response) => {
				if (!response.HasErrors) {
					string version = response.ScriptData.GetString("version");
					if(version != null){
						Debug.Log ("GSM| Server v"+version);
						_callback(version);
					}else{
						Debug.LogError("GSM| Server Error! [server version returned 'null']");
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}
	#endregion


	///////////////////////////////////////
	// ********* MISC FUNCTIONS ******** //
	///////////////////////////////////////

	/// <summary>
	/// USes a regex to check the email is valid before sending it to the server.
	/// </summary>
	/// <param name="_callback">email</param>
	private bool IsValidEmail(string _email)
	{
		string expresion;
		expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
		if (Regex.IsMatch(_email, expresion))
		{
			if (Regex.Replace(_email, expresion, string.Empty).Length == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}


}
