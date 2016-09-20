using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using GameSparks.Api.Messages;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;


/// <summary>
/// GameSparks Manager Class
/// To setup this class, add it to an empty gameobject and also attach the class GameSparksUnity.cs
/// July 2016, Sean Durkan
/// </summary>
public class GameSparksManager : MonoBehaviour
{

    #region Singleton

    /// <summary>
    ///  A reference to the game-state manager object used for the singleton
    /// </summary>
    private static GameSparksManager instance = null;

    /// <summary>
    /// This method returns the current instance of this class.
    /// The singleton ensures that only one instance of the game-state manager exists.
    /// </summary>
    /// <returns>The instance of GameSparksManager, or null if none is set yet</returns>
    public static GameSparksManager Instance()
    {
        if (instance != null)
        {
            return instance;
        }
        Debug.LogError("GSM | GameSparks Not Initialized...");
        return instance;
    }

    #endregion

    public static int GSVersion = 1;
    /// <summary>
    /// Sets the client version
    /// </summary>
    /// <param name="newVersion">New version.</param>
    public static void SetGSVersion(int newVersion)
    {
        GSVersion = newVersion;
    }

    /// <summary>
    /// Make sure there is exactly one GameSparksManager object.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        { 
            // when the first GSM is activated, it should be null, so we create the reference
            Debug.Log("GSM | Singleton Initialized...");
            instance = this;
            DontDestroyOnLoad(this.gameObject); // gamesparks manager persists throughout game
        }
        else
        {
            // if we load into a level that has the gamesparks manager present, it should be destroyed //
            // there can be only one! //
            Debug.Log("GSM | Removed Duplicate...");
            Destroy(this.gameObject);
        }
    }



    // Use this for initialization
    void Start()
    {
        // This is a callback which is triggered when gamesparks connects and disconnects //
        // Note: on disconnect, this needs a request to timeout before it will know that the socket has closed //
        // i.e. we cannot tell the SDK the socket is closed if it is closed (since there is no connection) //
        GS.GameSparksAvailable += ((bool _isAvail) =>
        {
            if (OnGSAvailable != null)
            {
                OnGSAvailable(_isAvail);
            }
        });
        // These callbacks will send the message-received events back to the class they are being called from
        GameSparks.Api.Messages.ScriptMessage_privateUserMessage.Listener += (message) =>
        {
            if (OnNewPrivateMessage != null)
            {
                OnNewPrivateMessage(new InboxMessage(message.MessageId, message.Data.GetString("sender-name"), message.Data.GetString("from"), message.Data.GetString("header"), message.Data.GetString("body"), message.Data.GetGSData("payload")));
            }
        };
        GameSparks.Api.Messages.ScriptMessage_globalUserMessage.Listener += (message) =>
        {
            if (OnNewGlobalMessage != null)
            {
                OnNewGlobalMessage(new InboxMessage(message.MessageId, "Poptropica", "Poptropica", message.Data.GetString("header"), message.Data.GetString("body"), message.Data.GetGSData("payload")));
            }
        };
        GameSparks.Api.Messages.SessionTerminatedMessage.Listener += (message) =>
        {
            if (OnSessionTerminated != null)
            {
                        
                OnSessionTerminated();
            }
        };
    }

    #region Callbacks For Socket-Messages

    /// <summary>
    /// Occurs when on the GameSparks websocket is opened or closed
    /// </summary>
    public event GSAvailable OnGSAvailable;

    /// <summary>
    /// called when GameSparks SDK has been initilized, i.e. the websocket is authenticated
    /// </summary>
    public delegate void GSAvailable(bool _isAvailable);

    /// <summary>
    /// Occurs when on a new private message is pushed to the websocket
    /// </summary>
    public event MessageDelegate OnNewPrivateMessage, OnNewGlobalMessage;

    /// <summary>
    /// Message delegate. Handles messages from the server
    /// </summary>
    public delegate void MessageDelegate(InboxMessage message);

    /// <summary>
    /// Message event. Occurs when a message hits the client
    /// </summary>
    public delegate void MessageEvent();

    /// <summary>
    /// Occurs when the session is terminated.
    /// The occcurs wither by websocket inactivity (30mins) or the player is disconnected, as they 
    /// connected with the same credientals on another device.
    /// </summary>
    public event MessageEvent OnSessionTerminated;

    #endregion

    /// <summary>
    /// This delegate allows a callback to be used in any request where you want an action to take place
    /// once the request is completed, but there is no specific information you need from the response.
    /// </summary>
    public delegate void onRequestSuccess();

    #region AUTHENTICATION  & REGISTRATION CALLS

    /// <summary>
    /// receives AuthenticationResponse,  an object containing the user's player-data.
    /// list of all player's character Ids, 
    /// The Id of the last character the player used,  
    /// bool if player has parent email registered, 
    /// bool if player is a user migrating from pop1
    /// </summary>
    public delegate void onAuthSuccess(AuthResponse authResponse);

    /// <summary>
    /// Recieves GameSparksError, contains error message enum & status on the if the user was trying a pop1 account
    /// </summary>
    public delegate void onAuthFailed(AuthFailed error);

    /// <summary>
    /// receives, GameSparksError, a class which contains a GameSparksErrorMessage, Enum
    /// invalid_username, invalid_password, request_failed, request_timeout
    /// </summary>
    public delegate void onRequestFailed(GameSparksError error);

    /// <summary>
    /// Authenticate the user with the given username and password
    /// </summary>
    /// <param name="userName">User name</param>
    /// <param name="password">Password</param>
    /// <param name="onSuccess">callback.  Receives AuthenticationResponse with authentication details:
    /// list of character Ids, the Id of the last character logged in, bool if has parent email registered, bool if is a user migrating from pop1</param>
    /// <param name="onAuthFailed">
    /// callback, GameSparksError, contains enum & string errorString,   invalid_username, invalid_password, request_failed, request_timeout</param>
    public void Authenticate(string userName, string password, onAuthSuccess onSuccess, onAuthFailed onAuthFailed)
    {
        Debug.Log("UserName:" + userName + ", Password:" + password);
        Debug.Log("GSM| Attempting Player Authentication....");
        new GameSparks.Api.Requests.AuthenticationRequest()
            .SetPassword(password)
            .SetUserName(userName)
            .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        Debug.Log("GSM| Authentication Successful \n" + response.DisplayName);
                        if (onSuccess != null)
                        {
                            string lastCharacterId = string.Empty;
                            // check that we have a last-character saved //
                            if (response.ScriptData.GetString("last_character") != null)
                            {
                                lastCharacterId = response.ScriptData.GetString("last_character");
                            }
                            bool hasParentEmail = response.ScriptData.GetBoolean("hasParentEmail").GetValueOrDefault(false);

                            string gender = string.Empty;
                            if (response.ScriptData.GetString("gender") != null)
                            {
                                gender = response.ScriptData.GetString("gender");
                            }

                            int age = 0;
                            if (response.ScriptData.GetInt("age") != null)
                            {
                                age = (int)response.ScriptData.GetInt("age");
                            }
            			    AuthResponse auth = new AuthResponse(response.ScriptData.GetStringList("character_list").ToArray(), lastCharacterId, hasParentEmail, gender, age);
            			    Debug.Log("GSM| AuthResponse:");
            			    auth.Print();
                            onSuccess(auth);
                        }
                    }
                    else
                    {
                        if (onAuthFailed != null)
                        {
                            if(response.ScriptData.GetGSData("pop1_data") != null)
                            {
                                onAuthFailed(new AuthFailed(ProcessGSErrors(response.Errors), response.ScriptData.GetGSData("pop1_data")));
                            }
                            else
                            {
                                onAuthFailed(new AuthFailed(ProcessGSErrors(response.Errors), response.ScriptData.GetString("isPop1")));
                            }
                        }
                    }
                });
    }

    /// <summary>
    /// Receives a new suggested username if the username is taken.
    /// Receives a GameSparksError enum by default.
    /// error, - 'username_taken', 'request_failed'
    /// </summary>
    public delegate void onRegFailed(GameSparksErrorMessage error);

    /// <summary>
    /// Receives the new=player's playerID upon registration/
    /// </summary>
    public delegate void onRegSuccess(string userName);

    /// <summary>
    /// Register the specified userName, displayName, password, age, gender.
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="displayName">Display name.</param>
    /// <param name="password">Password.</param>
    /// <param name="age">Age.</param>
    /// <param name="gender">Gender.</param>
    /// <param name="onRequestSuccess">Callback. Called when the registration is successful. Takes the user's playerID</param>
    /// <param name="onRegFailed">callback. Receives GameSparksError object: enum, String, suggested username if the username is taken</param>
    public void Register(string userName, string displayName, string password, int age, string gender, onRegSuccess onRegSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("UserName:" + userName + ", Password:" + password + ", Display Name:" + displayName);
        Debug.Log("GSM| Attempting Registration...");
        new GameSparks.Api.Requests.RegistrationRequest()
            .SetUserName(userName)
            .SetDisplayName(displayName)
            .SetPassword(password)
            .SetScriptData(new GSRequestData().AddString("gender", gender).AddNumber("age", age))
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Registration Successful \n PlayerID:" + response.UserId);
                    if (onRegSuccess != null)
                    {
                        onRegSuccess(response.UserId);
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                    // Sean - 14/9/2016 - This has been commented out to test the CheckUsername() method.
                    // once we know this is working, we can remove this code.
                    // we need to check that these parameters are not null before sending the callback
//                    if (onRegFailed != null && response.Errors.GetString("USERNAME") != null && response.Errors.GetString("USERNAME") == "TAKEN")
//                    {
//                        onRegFailed((GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), response.Errors.GetString("@registration")), response.ScriptData.GetString("suggested-name"));
//                    }
//                    else 
//                            if (onRegFailed != null && response.Errors.GetString("error") != null && response.Errors.GetString("error") == "timeout")
//                    {
//                        // timeout will take 10sec, after which the socket will be closed //
//                        // this is a default function of the GameSparks SDK and cannot be modified, though the duration //
//                        // of the timeout can be changed //
//                        onRegFailed((GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), "request_timeout"));
//                    }
//                    else if (onRegFailed != null)
//                    {
//                        // the final error response, if there is no specific error from the server //
//                        onRegFailed((GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), "request_failed"));
//                    }
                }
            });
    }

    /// <summary>
    /// receives an array of suggested usernames.
    /// </summary>
    public delegate void onCheckUsername(string[] suggestedNames, string availableName);

    /// <summary>
    /// Checks the name request.
    /// </summary>
    /// <returns>The name request.</returns>
    /// <param name="userName">User name.</param>
    /// <param name="n_suggestions">no of suggestions</param>
    /// <param name="onCheckUsername">On check username. callback, array of suggested usernames</param>
    /// <param name="onRequestFailed">callback. Receives GameSparksError object: enum,</param>
    private IEnumerator checkNameRequest(string userName, int n_suggestions, onCheckUsername onCheckUsername, onRequestFailed onRequestFailed)
    {
        WWW checkNameRequest = new WWW("https://preview.gamesparks.net/callback/E300018ZDdAx/checkUsername/zYZXFSavP0ibDV8ep30ylnsJS1EisDFG?username="+userName+"&suggestions="+n_suggestions);
        yield return checkNameRequest;
        // check that the response has data, otherwise the request failed //
        if(checkNameRequest.text == null || checkNameRequest.text == string.Empty)
        {
            onRequestFailed(new GameSparksError(GameSparksErrorMessage.invalid_response));
        }
        else 
        {
            // once we have the response we can parse it to an object from the JSON using gsdata //
            GSRequestData respData = new GSRequestData(checkNameRequest.text);
            if(onRequestFailed != null && respData.GetGSData("errors") != null) // check if we have an error
            {
                onRequestFailed(new GameSparksError(ProcessGSErrors(respData.GetGSData("errors"))));
            }
            else if(onCheckUsername != null && respData.GetGSData("@checkUsername") != null)
            {
                onCheckUsername(respData.GetGSData("@checkUsername").GetStringList("suggested_names").ToArray(), respData.GetGSData("@checkUsername").GetString("available_name"));
            }
        }
    }

    /// <summary>
    /// Checks the username is available on pop2 and pop1
    /// If the username is unavailble, the server will return a number of suggestions
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="n_suggestions">no of suggestions to return</param>
    /// <param name="onCheckUsername">On check username. recieves an array of suggestions and a valid name if available</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError object: enum, String</param>
    public void CheckUsername(string userName, int n_suggestions, onCheckUsername onCheckUsername, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Checking Username: "+userName);
        StartCoroutine(checkNameRequest(userName, n_suggestions, onCheckUsername, onRequestFailed));
    }

    /// <summary>
    /// Logs the player out of GameSparks without closing the websocket
    /// </summary>
    /// <param name="onRequestSuccess">On request success.</param>
    /// <param name="onRequestFailed">On request failed.</param>
    public void Logout(onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Loggin Player Out...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("logout")
            .SetDurable(true)
            .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        if (onRequestSuccess != null)
                        {
                            onRequestSuccess();
                        }
                    }
                    else
                    {
                        if (onRequestFailed != null)
                        {
                            onRequestFailed(new GameSparksError(ProcessGSErrors(response.BaseData)));
                        }
                    }
                });
    }

    #endregion

    #region INVENTORY CALLS

    /// <summary>
    /// Receives the item-ID of the item being removed from the player's inventory 
    /// </summary>
    public delegate void onItemRemoved(string item_id);

    /// <summary>
    /// Removes an item from the player's inventory
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="item_id">Item ID</param>
    /// <param name="onItemRemoved">callback, int, Item ID.</param>
    /// <param name="onRequestFailed">callback, GameSparksError, contains enum & string errorString,  
    /// Errors - invalid_item_id, no_character_record,</param>
    public void RemoveItem(string character_id, string item_id, onItemRemoved onItemRemoved, onRequestFailed onRequestFailed)
    {
        Debug.Log("Attempting To Remove Item...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("removeItem")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("item_id", item_id)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.LogWarning("GSM| Item  Removed...");
                    if (onItemRemoved != null)
                    {
                        onItemRemoved(response.ScriptData.GetNumber("item_id").Value.ToString());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.BaseData)));
                    }
                }
            });
    }

    /// <summary>
    /// The item id of the item added to the character's inventory
    /// </summary>
    public delegate void onItemPickedUp(string item_id);

    /// <summary>
    /// Adds an item to the inventory of the requested character
    /// </summary>
    /// <param name="character_id">Character identifier.</param>
    /// <param name="item_id">Item ID</param>
    /// <param name="scene_id">Scene ID.</param>
    /// <param name="onItemPickedUp">On item picked up.</param>
    /// <param name="onRequestFailed">if a duplicate is added- "player-has-item",
    /// If the scene ID is invalid -  "invalid-scene-id",
    /// If the item-id is invalid - "invalid-item-id"</param>
    public void PickUpItem(string character_id, string item_id, string scene_id, onItemPickedUp onItemPickedUp, onRequestFailed onRequestFailed)
    {
        Debug.Log("Attempting To Pickup Item...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("pickUpItem")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("item_id", item_id)
            .SetEventAttribute("scene_id", scene_id)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.LogWarning("GSM| Item Picked Up...");
                    if (onItemPickedUp != null)
                    {
                        onItemPickedUp(response.ScriptData.GetNumber("item_id").Value.ToString());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.BaseData)));
                    }
                }
            });
    }
   
    /// <summary>
    /// receives the item id equipped
    /// </summary>
    public delegate void onItemEquipped(string item_id);

    /// <summary>
    /// Equips the item.
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="item_id">Item ID</param>
    /// <param name="equip_location">Equip location, a string denoting the location</param>
    /// <param name="onItemEquipped">On item equipped. callback, int, the ID of the item equipped</param>
    /// <param name="onRequestFailed">On request failed, callback, contains enum & string errorString, item_not_found_in_scene,min_level_not_met,max_level_exceeded,_</param>
    public void EquipItem(string character_id, string item_id, string equip_location, onItemEquipped onItemEquipped, onRequestFailed onRequestFailed)
    {
        Debug.Log("Attempting To Equip Item...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("equipItem")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("item_id", item_id)
            .SetEventAttribute("equip_location", equip_location)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.LogWarning("GSM| Item Equipped...");
                    if (onItemEquipped != null && response.ScriptData.GetInt("item_id") != null)
                    {
                        onItemEquipped(response.ScriptData.GetInt("item_id").Value.ToString());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.BaseData)));
                    }
                }
            });
    }

    /// <summary>
    /// receives the id of the item that has been used
    /// </summary>
    public delegate void onItemUsed(string item_id);

    /// <summary>
    /// Uses an item.
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="item_id">Item ID</param>
    /// <param name="onItemUsed">On item used, callback, returns the item ID</param>
    /// <param name="onRequestFailed">On request failed, callback, contains enum & string errorString</param>
    public void UseItem(string character_id, string item_id, onItemUsed onItemUsed, onRequestFailed onRequestFailed)
    {
        Debug.Log("Attempting To Use Item...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("useItem")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("item_id", item_id)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.LogWarning("GSM| Item Used...");
                    if (onItemUsed != null && response.ScriptData.GetInt("item_id") != null)
                    {
                        onItemUsed(response.ScriptData.GetInt("item_id").Value.ToString());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Receives an array of items
    /// </summary>
    public delegate void onGetInventory(Item[] items);

    /// <summary>
    /// Returns the player's inventory list
    /// </summary>
    /// <param name="character_id">Character ID.</param>
    /// <param name="onGetInventory">On get inventory, callback, Item[]</param>
    /// <param name="onRequestFailed">On request failed, callback, contains enum & string errorString, 'no_inventory'</param>
    public void GetInventory(string character_id, onGetInventory onGetInventory, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Fetching Inventory Items...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getInventory")
            .SetEventAttribute("character_id", character_id)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    if (onGetInventory != null)
                    {
                        List<Item> items = new List<Item>();
                        // go through all the items in the response and cache them to be returned by the callback //
                        foreach (GSData item in response.ScriptData.GetGSDataList("item_list"))
                        {
                            items.Add(new Item(item.GetInt("item_id").Value, item.GetString("name"), item.GetString("icon"), item.GetString("equipped"), item.GetString("is_special"), item.GetString("representation")));
                        }
                        onGetInventory(items.ToArray());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    #endregion

    #region SCENES API CALLS

    /// <summary>
    /// Recieves an array of Scene objects
    /// </summary>
    public delegate void onGetScenes(Scene[] scenes);

    /// <summary>
    /// Gets the scenes.
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="island_id">Island ID</param>
    /// <param name="onGetScenes">On get scenes. Receives an array of scenes</param>
    /// <param name="onRequestFailed">On request failed  callback, contains enum & string errorString, invalid_island_id<param>
    public void GetScenes(string character_id, string island_id, onGetScenes onGetScenes, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Fetching Scenes...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getScenes")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("island_id", island_id)
            .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        if (onGetScenes != null && response.ScriptData.GetGSDataList("scenes") != null)
                        {
                            List<Scene> scenes = new List<Scene>();
                            // go through all the items in teh response and cache them to be returned by the callback //
                            foreach (GSData scene in response.ScriptData.GetGSDataList("scenes"))
                            {
                                List<Scene.Connection> cons = new List<Scene.Connection>();
                                foreach(GSData con in scene.GetGSDataList("connections"))
                                {
                                    cons.Add(new Scene.Connection(con.GetInt("scene_id").Value, new Vector2(con.GetGSData("start_location").GetInt("x").Value, con.GetGSData("start_location").GetInt("y").Value)));
                                }
                                scenes.Add(new Scene(scene.GetInt("scene_id").Value, scene.GetInt("island_id").Value, scene.GetString("asset_bundle"), new Vector2(scene.GetGSData("start_location").GetInt("x").Value, scene.GetGSData("start_location").GetInt("x").Value), cons.ToArray()));
                            }
                            onGetScenes(scenes.ToArray());
                        }
                    }
                    else
                    {
                        
                        if (onRequestFailed != null)
                        {
                            onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                        }
                    }
                });
    }

 
    #endregion

    #region SCENES STATE API CALLS

    /// <summary>
    /// Receives a scene-state object
    /// </summary>
    public delegate void onSceneStateFound(SceneState sceneState);

    /// <summary>
    /// Gets the state of the scene.
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="island_id">Island ID</param>
    /// <param name="scene_id">Scene ID</param>
    /// <param name="onSceneStateFound">On scene state found, callback, SceneStat</param>
    /// <param name="onRequestFailed">On request failed, callback, contains enum & string errorString, invalid_scene_id, invalid_island_id</param>
    public void GetSceneState(string character_id, string island_id, string scene_id, onSceneStateFound onSceneStateFound, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Fetching Scenes ...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getSceneState")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("island_id", island_id)
            .SetEventAttribute("scene_id", scene_id)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.LogWarning("GSM| Scenes Retrieved...");
                    if (onSceneStateFound != null && response.ScriptData.GetGSData("state") != null)
                    {
                        onSceneStateFound(GSResponseToSceneState(response.ScriptData.GetGSData("state").GetGSData("scene_state")));
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    public object GSDataToObject(GSData data, Type objType)
    {
        object obj = Activator.CreateInstance(objType);
        foreach(var typeField in objType.GetFields())
        {
            Debug.Log(typeField.FieldType.ToString());
            if(!typeField.IsNotSerialized && typeField.FieldType == typeof(string))
            {
                typeField.SetValue(obj, data.GetString(typeField.Name));
            }
            else if(!typeField.IsNotSerialized && typeField.FieldType == typeof(int))
            {
                typeField.SetValue(obj, (int)data.GetNumber(typeField.Name).Value);    
            }
            else if(!typeField.IsNotSerialized && typeField.FieldType == typeof(bool))
            {
                typeField.SetValue(obj, data.GetBoolean(typeField.Name));    
            }
            else if(!typeField.IsNotSerialized && ( typeField.FieldType == typeof(List<string>) || typeField.FieldType == typeof(string[]) ))
            {
                typeField.SetValue(obj, (typeField.FieldType == typeof(List<string>)) ? (object)data.GetStringList(typeField.Name) : data.GetStringList(typeField.Name).ToArray());  
            }
            else if(!typeField.IsNotSerialized && (typeField.FieldType == typeof(List<int>) || typeField.FieldType == typeof(int[])) )
            {
                typeField.SetValue(obj, (typeField.FieldType == typeof(List<int>)) ? (object)data.GetIntList(typeField.Name) : data.GetIntList(typeField.Name).ToArray());    
            }
            else if(!typeField.IsNotSerialized && (typeField.FieldType == typeof(List<float>) || typeField.FieldType == typeof(float[])) )
            {
                typeField.SetValue(obj, (typeField.FieldType == typeof(List<float>)) ? (object)data.GetFloatList(typeField.Name) : data.GetFloatList(typeField.Name).ToArray());    
            }
            else if(!typeField.IsNotSerialized && (typeField.FieldType == typeof(List<double>) || typeField.FieldType == typeof(double[])) )
            {
                typeField.SetValue(obj, (typeField.FieldType == typeof(List<double>)) ? (object)data.GetDoubleList(typeField.Name) : data.GetDoubleList(typeField.Name).ToArray());    
            }
        }
        return obj;
    }

    public SceneState GSResponseToSceneState(GSData sceneData)
    {
        Debug.LogWarning(sceneData.JSON);
        SceneState newScene = new SceneState();
        foreach(var sceneField in typeof(SceneState).GetFields())
        {
            if(!sceneField.IsNotSerialized && sceneField.FieldType == typeof(string))
            {
                sceneField.SetValue(newScene, sceneData.GetString(sceneField.Name));
            }
            else if(!sceneField.IsNotSerialized && sceneField.FieldType == typeof(int))
            {
                sceneField.SetValue(newScene, (int)sceneData.GetNumber(sceneField.Name).Value);    
            }
            else if(!sceneField.IsNotSerialized && sceneField.FieldType == typeof(bool))
            {
                sceneField.SetValue(newScene, sceneData.GetBoolean(sceneField.Name));    
            }
            else if(!sceneField.IsNotSerialized && sceneField.FieldType == typeof(float))
            {
                sceneField.SetValue(newScene, sceneData.GetFloat(sceneField.Name));    
            }
            else if(!sceneField.IsNotSerialized && sceneField.FieldType == typeof(Dictionary<string, object>))
            {
                Dictionary<string, object> newDic = new Dictionary<string, object>();
                GSData dataList = sceneData.GetGSData("states");
                foreach(var elem in dataList.BaseData)
                {
                    if(sceneData.GetGSData("states").GetGSData(elem.Key) != null)
                    {
                        GSData stateGSData = sceneData.GetGSData("states").GetGSData(elem.Key);
                        newDic.Add(elem.Key, GSDataToObject(sceneData.GetGSData("states").GetGSData(elem.Key), Type.GetType(stateGSData.GetString("type"))));
                    }
                    else
                    {
                        newDic.Add(elem.Key, elem.Value);
                    }
                }
                newScene.states = newDic;
            }   
        }
        newScene.Print();

        return newScene;
    }

    /// <summary>
    /// Sets the state of the scene.
    /// </summary>
    /// <param name="character_id">Character iID</param>
    /// <param name="island_id">Island ID</param>
    /// <param name="scene_id">Scene ID.</param>
    /// <param name="newScene">New scene.</param>
    /// <param name="onRequestSuccess">Callback on request success. No arguments.</param>
    /// <param name="onRequestFailed">Callback on request failed. Receives GameSparksError, contains enum & string errorString, , 'invalid_character_id, invalid_scene_id'</param>
    public void SetSceneState(string character_id, string island_id, string scene_id, SceneState sceneState, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Parsing SceneState to GSData...");

        GSRequestData sceneData = new GSRequestData();
        sceneData.AddObject("scene_state", ObjectToGSData(sceneState));
        Debug.LogWarning(sceneData.JSON);

        Debug.Log("Attempting To Set Scene State ...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("setSceneState")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("island_id", island_id)
            .SetEventAttribute("scene_id", scene_id)
            .SetEventAttribute("state", sceneData)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.LogWarning("GSM| Scene Set...");
                    if (onRequestSuccess != null)
                    {
                        onRequestSuccess();
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }



    /// <summary>
    /// receives the island-ID and scene-ID of a scene just entered
    /// </summary>
    public delegate void onEnterScene(string island_id, string scene_id);

    /// <summary>
    /// Registers the player having entered a scene
    /// </summary>
    /// <param name="character_id">Character ID.</param>
    /// <param name="scene_id">Scene ID.</param>
    /// <param name="onEnterScene">Callback on success. Receives island-ID & scene-ID</param>
    /// <param name="onRequestFailed">Callback on request failed. Receives GameSparksError, contains enum & string errorString, 'invalid_character_id, invalid_scene_id'</param>
    public void EnterScene(string character_id, string scene_id, onEnterScene onEnterScene, onRequestFailed onRequestFailed)
    {
        Debug.Log("Attempting To Enter Scene ...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("enterScene")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("scene_id", scene_id)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.LogWarning("GSM| Entered Scene...");
                    if (onEnterScene != null)
                    {
                        onEnterScene(response.ScriptData.GetNumber("island_id").Value.ToString(), response.ScriptData.GetNumber("scene_id").Value.ToString());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    #endregion

    #region Character API calls

    /// <summary>
    /// Call the HTTP request to the gamesparks endpoint in order to generate a new character name.
    /// HTTP is required because gamesparks needs authentication in order to do log-event requests.
    /// This circumvents that by returning what is in the endpoint.
    /// </summary>
    /// <returns>Generates a new character name</returns>
    /// <param name="count">int - number of names to generate.</param> 
    /// <param name="onCharacterName">Callback on success. Receives the array of character names.</param>
    /// <param name="onRequestFailed">Callback on request failed. Receives GameSparksError, contains enum & string errorString</param>
    private IEnumerator generateNamesRequest(int count, onCharacterName onCharacterName, onRequestFailed onRequestFailed)
    {
        WWW genNamesRequest = new WWW("https://preview.gamesparks.net/callback/E300018ZDdAx/generateName/YR5w9F53GYeMsP8LTBqijeeAsPM66v7J?count=" + count);
        yield return genNamesRequest;
        // once we have the response we can parse it to an object from the JSON using gsdata //
        GSRequestData respData = new GSRequestData(genNamesRequest.text);
        if (respData.GetStringList("names") != null && onCharacterName != null)
        {
            onCharacterName(respData.GetStringList("names").ToArray());
        }
        else if (respData.GetString("error") != null && onRequestFailed != null)
        {
            onRequestFailed(new GameSparksError(ProcessGSErrors(new GSRequestData().AddString("@generateCharacterName", respData.GetString("error")))));
        }
    }

    /// <summary>
    /// Receives the name of the character
    /// </summary>
    public delegate void onCharacterName(string[] names);

    /// <summary>
    /// Generates the name of the character.
    /// Calls a coroutine to make an HTTP request
    /// </summary>
    /// <param name="count">number of names to retrieve</param>
    /// <param name="onCharacterName">Callback on success. Receives the array of character names.</param>
    /// <param name="onRequestFailed">Callback on request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void GenerateCharacterNames(int count, onCharacterName onCharacterName, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Fetching New Character Names  [" + count + "]...");
        if (count <= 0) // there is no default for this callback, so this is to ensure at least one name is returned.
        {
            count = 1;
        }
        StartCoroutine(generateNamesRequest(count, onCharacterName, onRequestFailed));
    }

    /// <summary>
    /// Gives player experience.
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="amount">Amount.</param>
    /// <param name="onLevelAndExperience">Callback on success. Called with string character-name</param>
    /// <param name="onRequestFailed">Callback on request failed. Receives GameSparksError, contains enum & string errorString, request_failed</param>
    public void GiveExperience(string character_id, int amount, onLevelAndExperience onLevelAndExperience, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM|Attempting To Give Xp...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("giveXp")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("amount", amount)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.LogWarning("GSM| XP Granted...");
                    // first we get the json where the level and experience is //
                    GSData resp = response.ScriptData.GetGSData("player_details");
                    // then we pass the level and xp into the callback //
                    onLevelAndExperience(resp.GetInt("level").Value, resp.GetInt("experience").Value);
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Level and exp callback.
    /// Having the giveXP and GetXp calls return the same data, it means you can assign one call to this delegate
    /// and it will automatically re-draw the new xp and level values instead of requesting them again.
    /// </summary>
    public delegate void onLevelAndExperience(int level, int xp);

    /// <summary>
    /// Returns the player-character's level and experiance
    /// </summary>
    /// <param name="character_id">Character ID.</param>
    /// <param name="onLevelAndExperience">Callback on success. Called with level and xp (ints)</param>
    /// <param name="onRequestFailed">Callback on request failed. Receives GameSparksError, contains enum & string errorString,  invalid_character_id</param>
    public void GetLevelAndExperience(string character_id, onLevelAndExperience onLevelAndExperience, onRequestFailed onRequestFailed)
    {
        Debug.Log("Retrieving Player Level & Experience...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getLevelAndExperience")
            .SetEventAttribute("character_id", character_id)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Returned Level & XP...");
                    // first we get the json where the level and experience is //
                    GSData resp = response.ScriptData.GetGSData("player_details");
                    // then we pass the level and xp into the callback //
                    onLevelAndExperience(resp.GetInt("level").Value, resp.GetInt("experience").Value);
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// receives the character ID
    /// </summary>
    public delegate void onGetCharacter(Character character);

    /// <summary>
    /// Gets the character-details given the id
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="onGetCharacter">Callback on success. Receives Character object.</param>
    /// <param name="onRequestFailed">Callback on request failed. Receives GameSparksError, contains enum & string errorString, invalid_character_id</param>
    public void GetCharacter(string character_id, onGetCharacter onGetCharacter, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Fetching Info For Character: " + character_id);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getCharacter")
            .SetEventAttribute("character_id", character_id)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Found Character....");
                    onGetCharacter(new Character(response.ScriptData.GetGSData("character").GetGSData("_id").GetString("$oid"), response.ScriptData.GetGSData("character").GetInt("level").Value, (int)response.ScriptData.GetGSData("character").GetNumber("experience").Value, response.ScriptData.GetGSData("character").GetString("name"), response.ScriptData.GetGSData("character").GetString("gender")));
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// receives a list of characterIDs
    /// </summary>
    public delegate void onGetCharacters(Character[] characters);

    /// <summary>
    /// Gets the details of a list of player-characters
    /// </summary>
    /// <param name="character_ids">List of character IDs</param>
    /// <param name="onGetCharacters">On get characters. callback, Character[]</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void GetCharacters(List<string> character_ids, onGetCharacters onGetCharacters, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Fetching Info For Characters");
        GSRequestData id_list = new GSRequestData().AddStringList("list", character_ids);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getCharacter")
            .SetEventAttribute("character_id", id_list)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Found Characters....");
                    List<Character> charList = new List<Character>();
                    foreach (GSData character in response.ScriptData.GetGSDataList("character"))
                    {
                        charList.Add(new Character(character.GetGSData("_id").GetString("$oid"), character.GetInt("level").Value, character.GetInt("experience").Value, character.GetString("name"), character.GetString("gender")));
                    }
                    onGetCharacters(charList.ToArray());
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Receives the ID of the new character
    /// </summary>
    public delegate void onCharacterCreated(string newCharacterID);

    /// <summary>
    /// Creates the character.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="gender">Gender.</param>
    /// <param name="onCharacterCreated">On character created. callback, returns the ID of the new character</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void CreateCharacter(string name, string gender, onCharacterCreated onCharacterCreated, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Creating New Character: " + name);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("createCharacter")
            .SetEventAttribute("name", name)
            .SetEventAttribute("gender", gender)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Character Created....");
                    if (onCharacterCreated != null)
                    {
                        onCharacterCreated(response.ScriptData.GetString("new-character-id"));
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    #endregion

    #region Player API calls

    /// <summary>
    /// Sends the reset password email http request.
    /// </summary>
    /// <returns>The reset password email request.</returns>
    /// <param name="userName">User name.</param>
    /// <param name="onRequestSuccess">On request success. callback</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString , invalid-username, no-email</param>
    private IEnumerator sendResetPasswordEmailRequest(string userName, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        WWW passwordResetRequest = new WWW("https://preview.gamesparks.net/callback/E300018ZDdAx/passReset/gZpjnyCSkKxNboajk9drAMCn8VnKnAlT?userName="+userName);
        yield return passwordResetRequest;
        // once we have the response we can parse it to an object from the JSON using gsdata //
        GSRequestData respData = new GSRequestData(passwordResetRequest.text);
        if (respData.GetString("@passReset") != null && onRequestSuccess != null)
        {
            onRequestSuccess();
        }
        else if (respData.GetGSData("errors").GetString("@passReset") != null && onRequestFailed != null)
        {
            onRequestFailed(new GameSparksError(ProcessGSErrors(new GSRequestData().AddString("@passReset", respData.GetGSData("errors").GetString("@passReset")))));
        }
    }

    /// <summary>
    /// Sends a reset-password email if the user has a valid email registered
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <param name="onRequestSuccess">On request success. callback</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString, invalid-username, no-email</param>
    public void SendResetPasswordEmail(string userName, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("Sending Password Reset Email...");
        // we need to send off a http-request to the gamesparks backend in order to find the user;s email //
        StartCoroutine(sendResetPasswordEmailRequest(userName, onRequestSuccess, onRequestFailed));
    }
        
    /// <summary>
    /// Registers the parent email.
    /// </summary>
    /// <param name="parentEmail">Parent email.</param>
    /// <param name="onRequestSuccess">On request success. callback</param>
    /// <param name="onRequestFailed">On request failed. callback, Receives GameSparksError, contains enum & string errorString, enum</param>
    public void RegisterParentEmail(string parentEmail, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Submitting Parent Email...");
        if (IsValidEmail(parentEmail))
        {
            new GameSparks.Api.Requests.LogEventRequest().SetEventKey("submitParentEmail")
                .SetEventAttribute("parent_email", parentEmail)
                .SetDurable(true)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        Debug.Log("GSM| Parent Email Pending...");
                        if (onRequestSuccess != null)
                        {
                            onRequestSuccess();
                        }
                    }
                    else
                    {
                        if (onRequestFailed != null)
                        {
                            onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                        }
                    }
                });
        }
        else
        {
            if (onRequestFailed != null)
            {
                onRequestFailed(new GameSparksError(ProcessGSErrors(new GSRequestData().AddString("@submitParentEmail", "invalid_email"))));
            }
        }
    }

    /// <summary>
    /// retreives a list of parent email for this player
    /// </summary>
    public delegate void onGetParentEmailSuccess(ParentEmailStatus[] ParentEmailStatus);

    /// <summary>
    /// Gets the parent email status.
    /// </summary>
    /// <param name="onGetParentEmailSuccess">On get parent email success. callback receives array of ParentEmailStatus objects</param>
    /// <param name="onRequestFailed">On request failed. callback, Receives GameSparksError, contains enum & string errorString</param>
    public void GetParentEmailStatus(onGetParentEmailSuccess onGetParentEmailSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Fetching Parent Email Status...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getParentEmailStatus")
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Found Email Status...");
                    if (onGetParentEmailSuccess != null)
                    {
                        List<ParentEmailStatus> parentEmailList = new List<ParentEmailStatus>();
                        foreach (GSData email in response.ScriptData.GetGSDataList("email_history"))
                        {
                            // not all emails have a verified date if they are not verified //
                            // therefore there are two constructors for this class to make sure there are //
                            // no null-exceptions //
                            if (email.GetNumber("verified_date") != null)
                            {
                                parentEmailList.Add(new ParentEmailStatus(email.GetString("email"), new DateTime(email.GetNumber("date_added").Value), email.GetString("status"), new DateTime(email.GetNumber("verified_date").Value)));
                            }
                            else
                            {
                                parentEmailList.Add(new ParentEmailStatus(email.GetString("email"), new DateTime(email.GetNumber("date_added").Value), email.GetString("status")));
                            }
                        }
                        onGetParentEmailSuccess(parentEmailList.ToArray());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Given the Player's password, this method will verify the password and rest it with a new password.
    /// </summary>
    /// <param name="onRequestSuccess">On request success.</param>
    /// <param name="onRequestFailed">On request failed. GameSparksError - enum</param>
    public void ChangePassword(string old_password, string new_password, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Setting New Password...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("changePassword")
            .SetEventAttribute("old_password", old_password)
            .SetEventAttribute("new_password", new_password)
            .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        Debug.Log("GSM| Password Set...");
                        if (onRequestSuccess != null)
                        {
                            onRequestSuccess();
                        }
                    }
                    else
                    {
                        if (onRequestFailed != null)
                        {
                            onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                        }
                    }
                });
    }

    /// <summary>
    /// Deletes the parent email history.
    /// </summary>
    /// <param name="onRequestSuccess">On request success.</param>
    /// <param name="onRequestFailed">On request failed. GameSparksError - enum</param>
    public void DeleteParentEmail(onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Deleting Parent Email History...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("deleteParentEmail")
            .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        if (onRequestSuccess != null)
                        {
                            onRequestSuccess();
                        }
                    }
                    else
                    {
                        if (onRequestFailed != null)
                        {
                            onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                        }
                    }
                });
    }

    #endregion

    #region System

    /// <summary>
    /// receives the server version for the current snapshot
    /// </summary>
    public delegate void onGetServerVersion(string version, DateTime versionDate);

    /// <summary>
    /// Gets the server version.
    /// </summary>
    /// <param name="onGetServerVersion">On get server version. callback, string, server version</param>
    /// <param name="onRequestFailed">On request failed. callback, Receives GameSparksError, contains enum & string errorString</param>
    public void GetServerVersion(onGetServerVersion onGetServerVersion, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Fetching Server Version...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getServerVersion")
            .SetEventAttribute("currVersion", GSVersion)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    string version = null;
                    DateTime versionDate = DateTime.MinValue; // since dates cannot be null, one way to check the version-date is valid is to use the min-date
                    if(response.ScriptData.GetNumber("version").HasValue && response.ScriptData.GetString("created") != null)
                    {
                        version = response.ScriptData.GetNumber("version").Value.ToString();
                        versionDate = DateTime.Parse(response.ScriptData.GetString("created"));
                    }
                    if (version != null && versionDate != DateTime.MinValue)
                    {
                        Debug.Log("GSM| Server v" + version);
                        onGetServerVersion(version, versionDate);
                    }
                    else
                    {
                        Debug.LogWarning("GSM| Server Error! [server version returned 'null']");
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    #endregion

    #region Inbox System

    /// <summary>
    /// receives an array of inbox messages
    /// </summary>
    public delegate void onGetMessages(InboxMessage[] messages);

    /// <summary>
    /// Gets all the message for this player and their characters
    /// </summary>
    /// <param name="character_id">Character ID.</param>
    /// <param name="type">Type.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="limit">Limit.</param>
    /// <param name="onGetMessages">On get messages. callback, InboxMessage[]</param>
    /// <param name="onRequestFailed">On request failed. callback, Receives GameSparksError, contains enum & string errorString</param>
    public void GetMessages(string character_id, string type, int offset, int limit, onGetMessages onGetMessages, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Fetching Messages For Character - " + character_id);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getMessages")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("type", type)
            .SetEventAttribute("limit", limit)
            .SetEventAttribute("offset", offset)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Retrieved Messages....");
                    if (onGetMessages != null)
                    {
                        List<InboxMessage> messageList = new List<InboxMessage>();
                        foreach (GSData message  in response.ScriptData.GetGSDataList("messages"))
                        {
                            messageList.Add(new InboxMessage(message.GetString("messageId"), message.GetGSData("data").GetString("sender-name"), message.GetGSData("data").GetString("from"), message.GetGSData("data").GetString("header"), message.GetGSData("data").GetString("body"), message.GetGSData("data").GetGSData("payload")));
                        }
                        onGetMessages(messageList.ToArray());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Sends the private message.
    /// </summary>
    /// <param name="header">Header.</param>
    /// <param name="body">Body.</param>
    /// <param name="characterTo">Character ID to send the message to.</param>
    /// <param name="characterFrom">Your character ID</param>
    /// <param name="onRequestSuccess">On request success. callback</param>
    /// <param name="onRequestFailed">On request failed, callback, Receives GameSparksError, contains enum & string errorString</param>
    public void SendPrivateMessage(string header, string body, string characterTo, string characterFrom, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Sending Private Message To " + characterTo);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("sendPrivateMessage")
            .SetEventAttribute("header", header)
            .SetEventAttribute("body", body)
            .SetEventAttribute("payload", new GSRequestData())
            .SetEventAttribute("character_id_to", characterTo)
            .SetEventAttribute("character_id_from", characterFrom)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Message Sent....");
                    if (onRequestSuccess != null)
                    {
                        onRequestSuccess();
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Sends a private message to the character ID given.
    /// </summary>
    /// <param name="header">Header.</param>
    /// <param name="body">Body.</param>
    /// <param name="payload">Payload.</param>
    /// <param name="characterTo">Character to.</param>
    /// <param name="characterFrom">Character from.</param>
    /// <param name="onRequestSuccess">On request success. callback</param>
    /// <param name="onRequestFailed">On request failed. callback, Receives GameSparksError, contains enum & string errorString</param>
    public void SendPrivateMessage(string header, string body, GSRequestData payload, string characterTo, string characterFrom, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Sending Private Message To " + characterTo);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("sendPrivateMessage")
            .SetEventAttribute("header", header)
            .SetEventAttribute("body", body)
            .SetEventAttribute("payload", payload)
            .SetEventAttribute("character_id_to", characterTo)
            .SetEventAttribute("character_id_from", characterFrom)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Message Sent....");
                    if (onRequestSuccess != null)
                    {
                        onRequestSuccess();
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Deletes a message given the message ID
    /// </summary>
    /// <param name="messageID">Message ID.</param>
    /// <param name="onRequestSuccess">On request success.</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void DeleteMessage(string messageID, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Deleting Message " + messageID);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("deleteMessage")
            .SetEventAttribute("message_id", messageID)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Message Deleted....");
                    if (onRequestSuccess != null)
                    {
                        onRequestSuccess();
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    public void ReadMessage(string messageID)
    {

        // THIS IS YET TO BE IMPLEMENTED //


//      Debug.Log ("GSM| Deleting Message "+messageID);
//      new GameSparks.Api.Requests.LogEventRequest().SetEventKey("deleteMessage")
//          .SetEventAttribute("message_id", messageID)
//          .Send ((response) => {
//              if (!response.HasErrors) {
//                  Debug.Log("GSM| Message Deleted....");
//              } else {
//                  Debug.LogError("GSM| Message Not Deleted");
//              }
//          });
    }

    #endregion

    #region Get Available Islands

    /// <summary>
    /// receives array of islands
    /// </summary>
    public delegate void onGetIslands(Island[] islands);

    /// <summary>
    /// Gets the available islands.
    /// </summary>
    /// <param name="character_id">Character identifier.</param>
    /// <param name="getIslands">Get islands. callback Island[]</param>
    /// <param name="_callback">Callback. callback, Receives GameSparksError, contains enum & string errorString</param>
    public void GetAvailableIslands(string character_id, onGetIslands getIslands, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Fetching Available Islands...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getAvailableIslands")
            .SetEventAttribute("character_id", character_id)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Found Islands....");
                    if (getIslands != null && response.ScriptData.GetGSDataList("islands") != null)
                    {
                        List<Island> islandList = new List<Island>();
                        // add this data to the islands
                        foreach (GSData island in response.ScriptData.GetGSDataList("islands"))
                        {
                            List<Island.Gate> gateList = new List<Island.Gate>();
                            // get all the gates //
                            foreach (GSData gate in island.GetGSDataList("gates"))
                            {
                                gateList.Add(new Island.Gate(gate.GetString("gate_type"), gate.GetString("start_date"), gate.GetString("end_date"), 
                                        (gate.GetInt("min_level") != null ? gate.GetInt("min_level").Value : 100),
                                        (gate.GetInt("min_level") != null ? gate.GetInt("max_level").Value : -1),
                                        gate.GetString("product_id")));
                            }
                            islandList.Add(new Island(island.GetInt("island_id").Value, island.GetString("name"), island.GetString("description"), gateList.ToArray(), island.GetStringList("urls").ToArray()));
                        }
                        getIslands(islandList.ToArray());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// receives the id of the island visited
    /// </summary>
    public delegate void onIslandVisited(string island_id);

    /// <summary>
    /// Registers the Player-character having visited the island.
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="island_id">Island ID</param>
    /// <param name="onIslandVisited">On island visited. callback, receives the id of the island visited</param>
    /// <param name="onRequestFailed">On request failed. callback, Receives GameSparksError, contains enum & string errorString</param>
    public void VisitIsland(string character_id, string island_id, onIslandVisited onIslandVisited, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM|  Visiting Island...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("visitIsland")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("island_id", island_id)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Character Visited Island....");
                    if (onIslandVisited != null)
                    {
                        onIslandVisited(response.ScriptData.GetInt("island_id").Value.ToString());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Leaves the island.
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="island_id">Island ID</param>
    /// <param name="onRequestSuccess">On request success. callback </param>
    /// <param name="onRequestFailed">On request failed. callback, Receives GameSparksError, contains enum & string errorString</param>
    public void LeaveIsland(string character_id, string island_id, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM|  Leaving Island...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("leaveIsland")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("island_id", island_id)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Character Left Island....");
                    if (onRequestSuccess != null)
                    {
                        onRequestSuccess();
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// Marks the player-character as having Completed the island.
    /// </summary>
    /// <param name="character_id">Character ID.</param>
    /// <param name="island_id">Island ID.</param>
    /// <param name="onRequestSuccess">On request success. callback</param>
    /// <param name="onRequestFailed">On request failed. callback, Receives GameSparksError, contains enum & string errorString</param>
    public void CompleteIsland(string character_id, string island_id, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM|  Completing Island...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("completeIsland")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("island_id", island_id)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Character Completed Island....");
                    if (onRequestSuccess != null)
                    {
                        onRequestSuccess();
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    #endregion

    #region Character Outfit


    /// <summary>
    /// Sets the outfit in the character's outfit collection
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="outfit">Outfit.</param>
    /// <param name="onRequestSuccess">On request success.</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void SetOutfit(string character_id, Outfit outfit, onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        Debug.Log("GSM| Setting Character Outfit....");
        if (outfit != null)
        {
            GSRequestData outfitData = new GSRequestData();
            List<GSData> adList = new List<GSData>();
            Debug.Log("GSM| Building GSdata...");
            // using the Outfit type we can iterate through all the fields in Outfit and get the variable names //
            foreach(var field in typeof(Outfit).GetFields())
            {
                string varName = field.Name;
                // then we can parse those field-values back to objects to get the names of adornment, and values for any bools, strings or ints //
                if(field.GetValue(outfit) != null && field.GetValue(outfit).GetType().BaseType == typeof(Adornment) && !field.IsNotSerialized)
                {
                    Adornment adorn =  (Adornment)field.GetValue(outfit);
                    adList.Add(new GSRequestData().AddString(varName, adorn.name));
                }
                // colour are a special case which will be stores with r,g,b values on the server. This makes it easier to parse //
                // them back to Color objects when the outfit comes back //
                else if(field.GetValue(outfit) != null && field.GetValue(outfit).GetType() == typeof(Color) && !field.IsNotSerialized)
                {
                    Color color = (Color)field.GetValue(outfit); // get the colour object
                    GSRequestData colJSON = new GSRequestData();
                    colJSON .AddNumber("r", color.r); // pass in the r,g,b,a values as seperate fields
                    colJSON .AddNumber("g", color.g);
                    colJSON .AddNumber("b", color.b);
                    colJSON .AddNumber("a", color.a);
                    outfitData.AddObject(varName, colJSON); // set the colour as gsdata json object
                }
                else if(field.GetValue(outfit) != null && field.GetValue(outfit).GetType() == typeof(bool) && !field.IsNotSerialized)
                {
                    outfitData.AddBoolean(varName, bool.Parse(field.GetValue(outfit).ToString()));
                }
                else if(field.GetValue(outfit) != null && field.GetValue(outfit).GetType() == typeof(int) && !field.IsNotSerialized)
                {
                    outfitData.AddNumber(varName, int.Parse(field.GetValue(outfit).ToString()));
                }
                else if(field.GetValue(outfit) != null && field.GetValue(outfit).GetType() == typeof(string) && !field.IsNotSerialized)
                {
                    outfitData.AddString(varName, field.GetValue(outfit).ToString());
                }
            }
            outfitData.AddObjectList("adornments", adList);

            new GameSparks.Api.Requests.LogEventRequest().SetEventKey("setOutfit")
                .SetEventAttribute("character_id", character_id)
                .SetEventAttribute("outfit", outfitData)
                .SetDurable(true)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        Debug.Log("GSM| Outfit Set....");
                        if (onRequestSuccess != null)
                        {
                            onRequestSuccess();
                        }
                    }
                    else
                    {
                        if (onRequestFailed != null)
                        {
                            onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                        }
                    }
                });

        }
        else
        {
            Debug.LogWarning("GSM| No outfit submitted...");
        }
    }

    /// <summary>
    /// receives a list of adornment-protoypes.
    /// This gives information about the class (type), the variable name (name) and the url.
    /// </summary>
    public delegate void onGetOutfit(OutfitPrototype _outfitPrototype);

    /// <summary>
    /// Returns a list of adornmnets for the outfit.
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="onGetOutfit">On get outfit. receives a list of prototype adornments</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void GetOutfit(string character_id, onGetOutfit onGetOutfit, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Fetching Outfit...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getOutfit")
                .SetEventAttribute("character_id", character_id)
                .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Outfit Retrieved....");

                    OutfitPrototype outfitPrototype = new OutfitPrototype();
                    List<GSData> outfitList = response.ScriptData.GetGSDataList("outfit");
                    
                    //List<AdornmentPrototype> prototypeList = new List<AdornmentPrototype>();
                    if (onGetOutfit != null && outfitList != null)
                    {
                        foreach (GSData data in outfitList)
                        {
                            
                            if(!data.ContainsKey("data"))
                            {
                                outfitPrototype.adornmentList.Add(new AdornmentPrototype()
                                {
                                    type = data.GetString("shortCode"),
                                    name = data.GetString("name"),
                                    url = data.GetString("url")
                                });
                            }
                            else
                            {
                                if(data.GetString("shortCode") == "skinColor")
                                {
                                    GSData skinData = data.GetGSData("data");
                                    outfitPrototype.skinColor.r = (float)skinData.GetFloat("r");
                                    outfitPrototype.skinColor.g = (float)skinData.GetFloat("g");
                                    outfitPrototype.skinColor.b = (float)skinData.GetFloat("b");
                                    outfitPrototype.skinColor.a = (float)skinData.GetFloat("a");
                                }
                                if (data.GetString("shortCode") == "hairColor")
                                {
                                    GSData hairData = data.GetGSData("data");
                                    outfitPrototype.hairColor.r = (float)hairData.GetFloat("r");
                                    outfitPrototype.hairColor.g = (float)hairData.GetFloat("g");
                                    outfitPrototype.hairColor.b = (float)hairData.GetFloat("b");
                                    outfitPrototype.hairColor.a = (float)hairData.GetFloat("a");
                                }
                                if (data.GetString("shortCode") == "isPlayerOutfit")
                                {
                                    outfitPrototype.isPlayerOutfit = (bool)data.GetBoolean("data");
                                }
                                if (data.GetString("shortCode") == "reactiveEyelids")
                                {
                                    outfitPrototype.reactiveEyelids = (bool)data.GetBoolean("data");
                                }
                            }
                        }
                        onGetOutfit(outfitPrototype);
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// returns the adornment
    /// </summary>
    public delegate void onGetAdornment(AdornmentPrototype adornment);

    /// <summary>
    /// Gets the adornment details for the ID requested
    /// </summary>
    /// <param name="adornment_id">Adornment ID</param>
    /// <param name="onGetAdornment">returns the adornment</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void GetAdornment(string adornment_id, onGetAdornment onGetAdornment, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Fetching Adornment...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getAdornment")
            .SetEventAttribute("adornment_id", adornment_id)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Adornment Retrieved....");
                    if (onGetAdornment != null && response.ScriptData.GetGSData("adornment") != null)
                    {
                        onGetAdornment(new AdornmentPrototype(response.ScriptData.GetGSData("adornment").GetString("shortCode"),response.ScriptData.GetGSData("adornment").GetString("url")));
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// returns an array of adornments
    /// </summary>
    public delegate void onGetAdornments(AdornmentPrototype[] adornments);

    /// <summary>
    /// Gets the adornments.
    /// </summary>
    /// <param name="adornment_ids">Adornment ID list.</param>
    /// <param name="onGetAdornments">returns array of adornments</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void GetAdornments(List<string> adornment_ids, onGetAdornments onGetAdornments, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Fetching Adornments...");
     
        GSRequestData adList = new GSRequestData();
        adList.AddStringList("adornment_ids", adornment_ids);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getAdornments")
            .SetEventAttribute("adornment_ids", adList)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                        Debug.LogWarning(response.ScriptData.JSON);
                    Debug.Log("GSM| Adornments Retrieved....");
                    if (onGetAdornments != null && response.ScriptData.GetGSDataList("adornments") != null)
                    {
                        List<AdornmentPrototype> adsList = new List<AdornmentPrototype>();
                        foreach (GSData ads in response.ScriptData.GetGSDataList("adornments"))
                        {
                            adsList.Add(new AdornmentPrototype(ads.GetString("shortCode"), ads.GetString("url")));
                         }
                        onGetAdornments(adsList.ToArray());
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });

    }

    /// <summary>
    /// returns true or false if the adornment is available
    /// </summary>
    public delegate void isAdornmentAvailable(bool isAvailable);

    /// <summary>
    /// Checks if the adornmnet is available
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="adornment_id">Adornment ID</param>
    /// <param name="isAdornmentAvailable">returns bool</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void IsAdornmentAvailable(string character_id, string adornment_id, isAdornmentAvailable isAdornmentAvailable, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Checking is Adornment is Available...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("isAdornmentAvailable")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("adornment_id", adornment_id)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Adornments Retrieved....");
                    if (isAdornmentAvailable != null && response.ScriptData.GetBoolean("available") != null)
                    {
                        isAdornmentAvailable(response.ScriptData.GetBoolean("available").Value);
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    /// <summary>
    /// return the costume ID
    /// </summary>
    public delegate void onSetFixedCostume(int costume_id);

    /// <summary>
    /// Sets the fixed costume give the out-fit ID
    /// </summary>
    /// <param name="character_id">Character ID</param>
    /// <param name="outfit_id">Outfit ID</param>
    /// <param name="onSetFixedCostume">costume ID.</param>
    /// <param name="onRequestFailed">On request failed. Receives GameSparksError, contains enum & string errorString</param>
    public void SetFixedCostume(string character_id, int outfit_id, onSetFixedCostume onSetFixedCostume, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Setting Fixed Costume...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("setFixedCostume")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("outfit_id", outfit_id)
            .SetDurable(true)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("GSM| Adornments Retrieved....");
                    if (onSetFixedCostume != null && response.ScriptData.GetInt("costume-id") != null)
                    {
                        onSetFixedCostume(response.ScriptData.GetInt("costume-id").Value);
                    }
                }
                else
                {
                    if (onRequestFailed != null)
                    {
                        onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                    }
                }
            });
    }

    #endregion

    #region QUEST


    public delegate void onLoadQuest(QuestData quest);


    public void LoadQuest(string character_id, string quest_id,  onLoadQuest onLoadQuest, onRequestFailed onRequestFailed)
    {
        Debug.Log("GMS| Fetching Quest Info...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("loadQuest")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("quest_id", quest_id)
            .SetDurable(true)
            .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        if(onLoadQuest != null && response.ScriptData.GetGSData("questProgress") != null)
                        {
                            GSData questData = response.ScriptData.GetGSData("questProgress");
                            QuestData newQuest = new QuestData();

                            foreach(var questField in typeof(QuestData).GetFields())
                            {
                                if(questField.FieldType == typeof(string) && !questField.IsNotSerialized)
                                {
                                    questField.SetValue(newQuest, questData.GetString(questField.Name));
                                }
                                else if(questField.FieldType == typeof(bool) && !questField.IsNotSerialized)
                                {
                                    questField.SetValue(newQuest, questData.GetBoolean(questField.Name).GetValueOrDefault(false));
                                }
                                else if(questField.FieldType == typeof(int) && !questField.IsNotSerialized)
                                {
                                    questField.SetValue(newQuest, questData.GetNumber(questField.Name).GetValueOrDefault(0));
                                }
                                else
                                {
                                    if(questField.FieldType == typeof(List<string>) && !questField.IsNotSerialized)
                                    {
                                        questField.SetValue(newQuest, questData.GetStringList(questField.Name));
                                    }
                                    else if(questField.FieldType == typeof(List<StageData>) && !questField.IsNotSerialized)
                                    {
                                        List<GSData> respStageList = questData.GetGSDataList("stages");
                                        List<StageData> stageList = new List<StageData>();
                                        foreach(GSData gs_stage in respStageList)
                                        {
                                            StageData stage = new StageData();
                                            foreach(var stageField in typeof(StageData).GetFields())
                                            {
                                                if(stageField.FieldType == typeof(string) && !stageField.IsNotSerialized)
                                                {
                                                    stageField.SetValue(stage, gs_stage.GetString(stageField.Name));
                                                }
                                                else if(stageField.FieldType == typeof(bool) && !stageField.IsNotSerialized)
                                                {
                                                    stageField.SetValue(stage, gs_stage.GetBoolean(stageField.Name).GetValueOrDefault(false));
                                                }
                                                else if(stageField.FieldType == typeof(int) && !stageField.IsNotSerialized)
                                                {
                                                    stageField.SetValue(stage, gs_stage.GetNumber(stageField.Name).GetValueOrDefault(0));
                                                }
                                                else
                                                {
                                                    if(stageField.FieldType == typeof(List<string>) && !stageField.IsNotSerialized)
                                                    {
                                                        stageField.SetValue(stage, gs_stage.GetStringList(stageField.Name));
                                                    }
                                                    else if(stageField.FieldType == typeof(List<QuestStep>)  && !stageField.IsNotSerialized)
                                                    {
                                                        List<GSData> respStepList = gs_stage.GetGSDataList("steps");
                                                        List<QuestStep> stepList = new List<QuestStep>();
                                                        foreach(GSData gs_step in respStepList)
                                                        {
                                                            QuestStep step = new QuestStep();
                                                            foreach(var stepField in typeof(QuestStep).GetFields())
                                                            {
                                                                if(stepField.FieldType == typeof(string)  && !stepField.IsNotSerialized)
                                                                {
                                                                    stepField.SetValue(step, gs_step.GetString(stepField.Name));
                                                                }
                                                            }
                                                            stepList.Add(step);
                                                        }
                                                        stage.SetSteps(stepList);
                                                    }
                                                }
                                            }
                                            stageList.Add(stage);
                                        }
                                        newQuest.SetStages(stageList);
                                    }
                                }
                            }
                            onLoadQuest(newQuest);
                        }
                    }
                    else
                    {
                        if (onRequestFailed != null)
                        {
                            onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                        }
                    }
                });
    }


    public void SaveQuest(string character_id, QuestData quest,  onRequestSuccess onRequestSuccess, onRequestFailed onRequestFailed)
    {
        GSRequestData gsQuestData = ObjectToGSData(quest) as GSRequestData;
        Debug.Log("GMS| Saving Quest Info...");
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("saveQuest")
            .SetEventAttribute("character_id", character_id)
            .SetEventAttribute("quest_id", quest.questID)
            .SetEventAttribute("questData", gsQuestData)
            .SetDurable(true)
            .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        Debug.Log("GSM| Quest Data Set....");
                        Debug.Log(gsQuestData.JSON);
                        if (onRequestSuccess != null)
                        {
                            onRequestSuccess();
                        }
                    }
                    else
                    {
                        if (onRequestFailed != null)
                        {
                            onRequestFailed(new GameSparksError(ProcessGSErrors(response.Errors)));
                        }
                    }
                });
    }


    #endregion

    /// <summary>
    /// Converts a given object to GSdata for sending to the server.
    /// Converts string, bool, int, float, bool, QuestData, QuestStep, StageData, SceneState and list of these types.
    /// It also serialises Dictionary<string, object> or these types
    /// </summary>
    /// <returns>The to GS data.</returns>
    /// <param name="obj">Object.</param>
    public GSData ObjectToGSData(object obj)
    {
        GSRequestData gsData = new GSRequestData();
        gsData.AddString("type", obj.GetType().ToString());
        foreach(var field in obj.GetType().GetFields())
        {
            if(!field.IsNotSerialized && field.GetValue(obj) != null && field.FieldType == typeof(Dictionary<string, object>))
            {
                GSRequestData dictionaryData = new GSRequestData();
                foreach(KeyValuePair<string, object> entry in  field.GetValue(obj) as Dictionary<string, object>)
                {
                    if(entry.Value.GetType() == typeof(string))
                    {
                        dictionaryData.AddString(entry.Key, entry.Value.ToString());
                    }
                    else if(entry.Value.GetType() == typeof(int) || entry.Value.GetType() == typeof(float))
                    {
                        dictionaryData.AddNumber(entry.Key, (entry.Value.GetType() == typeof(int)) ? Int32.Parse(entry.Value.ToString()) : Convert.ToDouble(entry.Value.ToString()) );
                    }
                    else 
                    {
                        dictionaryData.AddObject(entry.Key, ObjectToGSData(entry.Value));
                    }
                }
                gsData.AddObject(field.Name, dictionaryData);
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && field.GetValue(obj).GetType() == typeof(bool))
            {
                gsData.AddBoolean(field.Name, bool.Parse(field.GetValue(obj).ToString()));
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && (field.GetValue(obj).GetType() == typeof(int) || field.GetValue(obj).GetType() == typeof(float)))
            {
                gsData.AddNumber(field.Name, (field.GetValue(obj).GetType() == typeof(int)) ? Convert.ToInt32(field.GetValue(obj)) : Convert.ToDouble(field.GetValue(obj)) );
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && field.GetValue(obj).GetType() == typeof(string))
            {
                gsData.AddString(field.Name, field.GetValue(obj).ToString());
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && (field.GetValue(obj).GetType() == typeof(List<string>) || field.GetValue(obj).GetType() == typeof(string[])))
            {
                gsData.AddStringList(field.Name,  (field.GetValue(obj).GetType() == typeof(List<string>)) ? field.GetValue(obj) as List<string> : new List<string>(field.GetValue(obj) as string[]));
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && (field.GetValue(obj).GetType() == typeof(List<int>) || field.GetValue(obj).GetType() == typeof(int[])))
            {
                gsData.AddNumberList(field.Name,  (field.GetValue(obj).GetType() == typeof(List<int>)) ? field.GetValue(obj) as List<int> : new List<int>(field.GetValue(obj) as int[]));
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && (field.GetValue(obj).GetType() == typeof(List<float>) || field.GetValue(obj).GetType() == typeof(float[])))
            {
                gsData.AddNumberList(field.Name,  (field.GetValue(obj).GetType() == typeof(List<float>)) ? field.GetValue(obj) as List<float> : new List<float>(field.GetValue(obj) as float[]));
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && (field.GetValue(obj).GetType() == typeof(List<bool>)))
            {
                GSRequestData boolData = new GSRequestData();
                List<bool> boolList = field.GetValue(obj) as List<bool>;
                for(int i = 0; i < boolList.Count; i++)
                {
                    boolData.AddBoolean(i.ToString(), boolList[i]);
                }
                gsData.AddObject(field.Name, boolData);
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && (field.GetValue(obj).GetType() == typeof(bool[])))
            {
                GSRequestData boolData = new GSRequestData();
                bool[] boolList = field.GetValue(obj) as bool[];
                for(int i = 0; i < boolList.Length; i++)
                {
                    boolData.AddBoolean(i.ToString(), boolList[i]);
                }
                gsData.AddObject(field.Name, boolData);
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && field.FieldType == typeof(QuestStep))
            {
                gsData.AddObject(field.Name, ObjectToGSData(field.GetValue(obj)));
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && field.FieldType == typeof(List<QuestStep>))
            {
                List<GSData> questList = new List<GSData>();
                foreach(QuestStep qs in field.GetValue(obj) as List<QuestStep>)
                {
                    questList.Add(ObjectToGSData(qs));
                }
                gsData.AddObjectList(field.Name, questList);
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && field.FieldType == typeof(StageData))
            {
                gsData.AddObject(field.Name, ObjectToGSData(field.GetValue(obj)));
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && field.FieldType == typeof(List<StageData>))
            {
                List<GSData> stageDataList = new List<GSData>();
                foreach(StageData sd in field.GetValue(obj) as List<StageData>)
                {
                    stageDataList.Add(ObjectToGSData(sd));
                }
                gsData.AddObjectList(field.Name, stageDataList);
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && field.FieldType == typeof(QuestData))
            {
                gsData.AddObject(field.Name, ObjectToGSData(field.GetValue(obj)));
            }
            else if(!field.IsNotSerialized && field.GetValue(obj) != null && field.FieldType == typeof(List<QuestData>))
            {
                List<GSData> questDataList = new List<GSData>();
                foreach(QuestData qd in field.GetValue(obj) as List<QuestData>)
                {
                    questDataList.Add(ObjectToGSData(qd));
                }
                gsData.AddObjectList(field.Name, questDataList);
            }

        }
        return gsData;
    }

    /// <summary>
    /// Processes all GameSparks error to get the correct error-string root and parse it to an int for checking
    /// </summary>
    /// <returns>GameSparksError, enum</returns>
    /// <param name="error">GSData Error response</param>
    public GameSparksErrorMessage ProcessGSErrors(GSData error)
    {
        Debug.LogWarning("GSM| Error: " + error.JSON);
        if (error.GetString("@authentication") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@authentication"));
        }
        else if (error.GetString("error") != null && error.GetString("error") == "timeout")
        {
            // timeout will take 10sec, after which the socket will be closed //
            // this is a default function of the GameSparks SDK and cannot be modified, though the duration //
            // of the timeout can be changed //
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), "request_timeout");
        }
        else if (error.GetGSData("error") != null && error.GetGSData("error").GetString("@removeItem") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetGSData("error").GetString("@removeItem"));
        }
        else if (error.GetGSData("error") != null && error.GetGSData("error").GetString("@pickUpItem") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetGSData("error").GetString("@pickUpItem"));
        }
        else if (error.GetGSData("error") != null && error.GetGSData("error").GetString("@equipItem") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetGSData("error").GetString("@equipItem"));
        }
        else if (error.GetString("@useItem") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@useItem"));
        }
        else if (error.GetString("@getInventory") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getInventory"));
        }
        else if (error.GetString("@getSceneState") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getSceneState"));
        }
        else if (error.GetString("@setSceneState") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@setSceneState"));
        }
        else if (error.GetString("@enterScene") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@enterScene"));
        }
        else if (error.GetString("@enterScene") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@enterScene"));
        }
        // CHARACTER //
        else if (error.GetString("@giveXp") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@giveXp"));
        }
        else if (error.GetString("@getLevelAndExperience") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getLevelAndExperience"));
        }
        else if (error.GetString("@getCharacter") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getCharacter"));
        }
        else if (error.GetString("@createCharacter") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@createCharacter"));
        }
        // PLAYER & SYSTEM //
        else if (error.GetString("@passReset") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@passReset"));
        }
        else if (error.GetString("@submitParentEmail") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@submitParentEmail"));
        }
        else if (error.GetString("@getParentEmailStatus") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getParentEmailStatus"));
        }
        else if (error.GetString("@getServerVersion") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getServerVersion"));
        }
        else if (error.GetString("@changePassword") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@changePassword"));
        }
        else if (error.GetString("@getServerVersion") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getServerVersion"));
        }
        else if (error.GetString("@registration") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@registration"));
        }
        else if (error.GetString("@checkUsername") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@checkUsername"));
        }
        // INBOX SYSTEM
        else if (error.GetString("@getMessages") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getMessages"));
        }
        else if (error.GetString("@sendPrivateMessage") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@sendPrivateMessage"));
        }
        else if (error.GetString("@sendPrivateMessage") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@sendPrivateMessage"));
        }
        else if (error.GetString("@deleteMessage") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@deleteMessage"));
        }
        // ISLANDS //
        else if (error.GetString("@getAvailableIslands") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getAvailableIslands"));
        }
        else if (error.GetString("@visitIsland") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@visitIsland"));
        }
        else if (error.GetString("@leaveIsland") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@leaveIsland"));
        }
        else if (error.GetString("@completeIsland") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@completeIsland"));
        }
        // CHARACTER OUTFITS //
        else if (error.GetString("@setOutfit") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@setOutfit"));
        }
        else if (error.GetString("@getOutfit") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getOutfit"));
        }
        else if (error.GetString("@getAdornment") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getAdornment"));
        }
        else if (error.GetString("@getAdornment") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@getAdornment"));
        }
        else if (error.GetString("@isAdornmentAvailable") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@isAdornmentAvailable"));
        }
        else if (error.GetString("@setFixedCostume") != null)
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), error.GetString("@setFixedCostume"));
        }
        else if (error.GetString("authentication") == "NOTAUTHORIZED")
        {
            return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), "not_authorized");
        }
        Debug.LogError(error.GetString("authentication"));

        return (GameSparksErrorMessage)Enum.Parse(typeof(GameSparksErrorMessage), "request_failed");
    }

    /// <summary>
    /// USes a regex to check the email is valid before sending it to the server.
    /// </summary>
    /// <param name="_callback">email</param>
    private bool IsValidEmail(string _email)
    {
        string expression;
        expression = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        if (Regex.IsMatch(_email, expression))
        {
            if (Regex.Replace(_email, expression, string.Empty).Length == 0)
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


namespace GameSparks.Api.Messages
{

    public class ScriptMessage_globalUserMessage : ScriptMessage
    {

        public new static Action<ScriptMessage_globalUserMessage> Listener;

        public ScriptMessage_globalUserMessage(GSData data) : base(data)
        {
        }

        private static ScriptMessage_globalUserMessage Create(GSData data)
        {
            ScriptMessage_globalUserMessage message = new ScriptMessage_globalUserMessage(data);
            return message;
        }

        static ScriptMessage_globalUserMessage()
        {
            handlers.Add(".ScriptMessage_globalUserMessage", Create);

        }

        override public void NotifyListeners()
        {
            if (Listener != null)
            {
                Listener(this);
            }
        }
    }

    public class ScriptMessage_privateUserMessage : ScriptMessage
    {

        public new static Action<ScriptMessage_privateUserMessage> Listener;

        public ScriptMessage_privateUserMessage(GSData data)
            : base(data)
        {
        }

        private static ScriptMessage_privateUserMessage Create(GSData data)
        {
            ScriptMessage_privateUserMessage message = new ScriptMessage_privateUserMessage(data);
            return message;
        }

        static ScriptMessage_privateUserMessage()
        {
            handlers.Add(".ScriptMessage_privateUserMessage", Create);

        }

        override public void NotifyListeners()
        {
            if (Listener != null)
            {
                Listener(this);
            }
        }
    }

}





public class GameSparksError
{
    public GameSparksErrorMessage errorMessage;
    public string errorString;

    public GameSparksError(GameSparksErrorMessage error, string errorString){

        this.errorMessage = error;
        this.errorString = errorString;
    }

    public GameSparksError(GameSparksErrorMessage error){

        this.errorMessage = error;
    }
}

public enum GameSparksErrorMessage
{

    invalid_username,
    invalid_password,
    request_failed,
    request_timeout,
    invalid_item_id,
    username_taken,
    no_character_record,
    player_has_item,
    invalid_scene_id,
    item_not_found_in_scene,
    min_level_not_met,
    max_level_exceeded,
    invalid_email,
    email_already_validated,
    character_name_already_taken,
    invalid_gender,
    invalid_adornment_id,
    invalid_char_id,
    invalid_character_id,
    invalid_outfit_id,
    dberror,
    message_not_deleted,
    invalid_island_id,
    no_email_registered,
    no_email_history,
    no_scene_data,
    no_player_scene_record,
    no_player_record,
    no_inventory,
    no_level_definition,
    invalid_version,
    incompatible_protocol_version,
    invalid_character_name,
    not_authorized,
    invalid_response, // used when the response details are invalid i.e. checkusername()
    invalid_request // used when the request data is incorrect, i.e. checkusername()
}


public class AuthFailed : GameSparksError
{
    public string isPop1Player;
    public string firstname, lastname, has_parent_email, parent_email, memstatus, memdate, gender, look; 
    public int age;

    public AuthFailed(GameSparksErrorMessage error, string isPop1Player) : base (error, string.Empty)
    {
        if(isPop1Player != null)
        {
            this.isPop1Player = isPop1Player;
        }
        else
        {
            this.isPop1Player = string.Empty;
        }

        this.errorMessage = error;
    }


    public AuthFailed(GameSparksErrorMessage error, GSData data) : base (error, string.Empty)
    {
        isPop1Player = "true";
        this.errorMessage = error;
        if(data.GetString("firstname") != null)
        {
            this.firstname = data.GetString("firstname");
        }
        if(data.GetString("lastname") != null)
        {
            lastname = data.GetString("lastname");
        }
        if(data.GetString("age") != null)
        {
            age = int.Parse(data.GetString("age"));
        }
        if(data.GetString("has_parent_email") != null)
        {
            has_parent_email = data.GetString("has_parent_email");
        }
        if(data.GetString("parent_email") != null)
        {
            parent_email = data.GetString("parent_email");
        }
        if(data.GetString("memstatus") != null)
        {
            memstatus = data.GetString("memstatus");
        }
        if(data.GetString("memdate") != null)
        {
            memdate = data.GetString("memdate");
        }
        if (data.GetString("look") != null)
        {
            look = data.GetString("look");
        }
        if (data.GetString("gender") != null)
        {
            gender = data.GetString("gender");
        }
    }

    public void Print()
    {
        Debug.Log("First Name:"+firstname+", LastName:"+lastname+", age:"+age+", has parent email:"+has_parent_email);
        Debug.Log("Parent:"+parent_email+"Memstatus:"+memstatus+", mem-date:"+memdate);
    }
}


public class AuthResponse
{
    public string[] characterIDs;
    public string lastCharacterID;
    public bool hasParentEmail;
    public string gender;
    public int age;

    public AuthResponse(string[] characterIDs, string lastCharacterID, bool hasParentEmail, string gender, int age)
    {
        this.characterIDs = characterIDs;
        this.lastCharacterID = lastCharacterID;
        this.hasParentEmail = hasParentEmail;
        this.gender = gender;
        this.age = age;
    }

    public void Print()
    {
        Debug.Log("Last Char:" + lastCharacterID + ", # Characters:" + characterIDs.Length + ", hasParentEmail:" + hasParentEmail + ", Gender:" + gender + ", Age:" + age);
    }
}



public class ParentEmailStatus
{

    public ParentEmailStatus(string email, DateTime dateAdded, string status, DateTime verifiedDate)
    {
        this.email = email;
        this.dateAdded = dateAdded;
        this.status = status;
        this.verifiedDate = verifiedDate;
    }

    public ParentEmailStatus(string email, DateTime dateAdded, string status)
    {
        this.email = email;
        this.dateAdded = dateAdded;
        this.status = status;
    }

    string email, status;
    DateTime dateAdded, verifiedDate;

    public void Print()
    {
        Debug.Log("Email:" + email + ", Status:" + status + ", Verified:" + verifiedDate + ", Added:" + dateAdded);
    }
}
