using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GameSparks.Api.Responses;
using GameSparks.Core;

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
		// we are going to setup a callback for the session terminated message //
		GameSparks.Api.Messages.SessionTerminatedMessage.Listener += (_message) => {
			Debug.LogWarning("GSM| Player Has Been Logged Out Due To Concurrent Login...");
		};
	}
	
	#region AUTHENTICATION CALLS
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

	public void GiveXp(int _amount, logevent_callback _callback){

		Debug.Log ("Attempting To Give Xp...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("giveXp")
			.SetEventAttribute("amount", _amount)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.LogWarning ("GSM| XP Granted...");
					if (_callback != null) {
						_callback (response);
					}
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}

	public delegate void levelAndExp_callback(int _level, int _exp);

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


	public delegate void resetPassword_onSucess_callback(string _newPassword);
	public delegate void resetPassword_onFailed_callback(bool _newPasswordNull, bool _invalidPassword, string _failedPassword, string _strength);
	public void ResetPassword(string old_password, string new_password, resetPassword_onSucess_callback _onSucess, resetPassword_onFailed_callback _onFailed){
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
						

						if(response.Errors.GetString("@resetPassword") == "new-password-null"){
							_onFailed(true, false, null, null);
						}else{
							GSData errors = response.Errors.GetGSData("@resetPassword");
							// get the strength of the password and the submitted password //
							if(errors.GetString("message") == "old-password-invalid"){ // the old-password didnt match
								_onFailed(false, true, null, null);
							}else { // failed a strength test
								string failedPassword = errors.GetString("failed-password");
								string strength = errors.GetString("strength");
								_onFailed(false, false, failedPassword, strength);
							}
						}
					}
				}
			});
	}


	public void RegisterParentEmail(string parent_email){
		Debug.Log ("GSM| Submitting Parent Email...");
		new GameSparks.Api.Requests.LogEventRequest().SetEventKey("submitParentEmail")
			.SetEventAttribute("parent_email", parent_email)
			.Send ((response) => {
				if (!response.HasErrors) {
					Debug.Log ("GSM| Parent Email Pending...");
				} else {
					Debug.LogWarning ("GSM| Error \n " + response.Errors.JSON);	
				}
			});
	}

	#endregion

}
