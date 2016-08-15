using System;
using System.Collections.Generic;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!
//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!
//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!

namespace GameSparks.Api.Requests{
	public class LogEventRequest_dropItem : GSTypedRequest<LogEventRequest_dropItem, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_dropItem() : base("LogEventRequest"){
			request.AddString("eventKey", "dropItem");
		}
		public LogEventRequest_dropItem Set_inventoryItemId( long value )
		{
			request.AddNumber("inventoryItemId", value);
			return this;
		}			
		public LogEventRequest_dropItem Set_sceneId( long value )
		{
			request.AddNumber("sceneId", value);
			return this;
		}			
		public LogEventRequest_dropItem Set_x( long value )
		{
			request.AddNumber("x", value);
			return this;
		}			
		public LogEventRequest_dropItem Set_y( long value )
		{
			request.AddNumber("y", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_dropItem : GSTypedRequest<LogChallengeEventRequest_dropItem, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_dropItem() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "dropItem");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_dropItem SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_dropItem Set_inventoryItemId( long value )
		{
			request.AddNumber("inventoryItemId", value);
			return this;
		}			
		public LogChallengeEventRequest_dropItem Set_sceneId( long value )
		{
			request.AddNumber("sceneId", value);
			return this;
		}			
		public LogChallengeEventRequest_dropItem Set_x( long value )
		{
			request.AddNumber("x", value);
			return this;
		}			
		public LogChallengeEventRequest_dropItem Set_y( long value )
		{
			request.AddNumber("y", value);
			return this;
		}			
	}
	
	public class LogEventRequest_equipItem : GSTypedRequest<LogEventRequest_equipItem, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_equipItem() : base("LogEventRequest"){
			request.AddString("eventKey", "equipItem");
		}
		public LogEventRequest_equipItem Set_inventoryItemID( long value )
		{
			request.AddNumber("inventoryItemID", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_equipItem : GSTypedRequest<LogChallengeEventRequest_equipItem, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_equipItem() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "equipItem");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_equipItem SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_equipItem Set_inventoryItemID( long value )
		{
			request.AddNumber("inventoryItemID", value);
			return this;
		}			
	}
	
	public class LogEventRequest_getInventory : GSTypedRequest<LogEventRequest_getInventory, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getInventory() : base("LogEventRequest"){
			request.AddString("eventKey", "getInventory");
		}
	}
	
	public class LogChallengeEventRequest_getInventory : GSTypedRequest<LogChallengeEventRequest_getInventory, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getInventory() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getInventory");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getInventory SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
	}
	
	public class LogEventRequest_getSceneState : GSTypedRequest<LogEventRequest_getSceneState, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getSceneState() : base("LogEventRequest"){
			request.AddString("eventKey", "getSceneState");
		}
		public LogEventRequest_getSceneState Set_scene_id( long value )
		{
			request.AddNumber("scene_id", value);
			return this;
		}			
		public LogEventRequest_getSceneState Set_island_id( long value )
		{
			request.AddNumber("island_id", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_getSceneState : GSTypedRequest<LogChallengeEventRequest_getSceneState, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getSceneState() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getSceneState");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getSceneState SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_getSceneState Set_scene_id( long value )
		{
			request.AddNumber("scene_id", value);
			return this;
		}			
		public LogChallengeEventRequest_getSceneState Set_island_id( long value )
		{
			request.AddNumber("island_id", value);
			return this;
		}			
	}
	
	public class LogEventRequest_moveItem : GSTypedRequest<LogEventRequest_moveItem, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_moveItem() : base("LogEventRequest"){
			request.AddString("eventKey", "moveItem");
		}
		public LogEventRequest_moveItem Set_inventoryItemID( long value )
		{
			request.AddNumber("inventoryItemID", value);
			return this;
		}			
		public LogEventRequest_moveItem Set_destinationSlot( long value )
		{
			request.AddNumber("destinationSlot", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_moveItem : GSTypedRequest<LogChallengeEventRequest_moveItem, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_moveItem() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "moveItem");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_moveItem SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_moveItem Set_inventoryItemID( long value )
		{
			request.AddNumber("inventoryItemID", value);
			return this;
		}			
		public LogChallengeEventRequest_moveItem Set_destinationSlot( long value )
		{
			request.AddNumber("destinationSlot", value);
			return this;
		}			
	}
	
	public class LogEventRequest_pickUpItem : GSTypedRequest<LogEventRequest_pickUpItem, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_pickUpItem() : base("LogEventRequest"){
			request.AddString("eventKey", "pickUpItem");
		}
		public LogEventRequest_pickUpItem Set_placedItemId( long value )
		{
			request.AddNumber("placedItemId", value);
			return this;
		}			
		public LogEventRequest_pickUpItem Set_sceneId( long value )
		{
			request.AddNumber("sceneId", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_pickUpItem : GSTypedRequest<LogChallengeEventRequest_pickUpItem, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_pickUpItem() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "pickUpItem");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_pickUpItem SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_pickUpItem Set_placedItemId( long value )
		{
			request.AddNumber("placedItemId", value);
			return this;
		}			
		public LogChallengeEventRequest_pickUpItem Set_sceneId( long value )
		{
			request.AddNumber("sceneId", value);
			return this;
		}			
	}
	
	public class LogEventRequest_setSceneState : GSTypedRequest<LogEventRequest_setSceneState, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_setSceneState() : base("LogEventRequest"){
			request.AddString("eventKey", "setSceneState");
		}
		public LogEventRequest_setSceneState Set_scene_id( long value )
		{
			request.AddNumber("scene_id", value);
			return this;
		}			
		public LogEventRequest_setSceneState Set_island_id( long value )
		{
			request.AddNumber("island_id", value);
			return this;
		}			
		public LogEventRequest_setSceneState Set_states( GSData value )
		{
			request.AddObject("states", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_setSceneState : GSTypedRequest<LogChallengeEventRequest_setSceneState, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_setSceneState() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "setSceneState");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_setSceneState SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_setSceneState Set_scene_id( long value )
		{
			request.AddNumber("scene_id", value);
			return this;
		}			
		public LogChallengeEventRequest_setSceneState Set_island_id( long value )
		{
			request.AddNumber("island_id", value);
			return this;
		}			
		public LogChallengeEventRequest_setSceneState Set_states( GSData value )
		{
			request.AddObject("states", value);
			return this;
		}
		
	}
	
	public class LogEventRequest_useItem : GSTypedRequest<LogEventRequest_useItem, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_useItem() : base("LogEventRequest"){
			request.AddString("eventKey", "useItem");
		}
		public LogEventRequest_useItem Set_inventoryItemID( long value )
		{
			request.AddNumber("inventoryItemID", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_useItem : GSTypedRequest<LogChallengeEventRequest_useItem, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_useItem() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "useItem");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_useItem SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_useItem Set_inventoryItemID( long value )
		{
			request.AddNumber("inventoryItemID", value);
			return this;
		}			
	}
	
}
	

namespace GameSparks.Api.Messages {

		public class ScriptMessage_onAcquiredItem : ScriptMessage {
		
			public new static Action<ScriptMessage_onAcquiredItem> Listener;
	
			public ScriptMessage_onAcquiredItem(GSData data) : base(data){}
	
			private static ScriptMessage_onAcquiredItem Create(GSData data)
			{
				ScriptMessage_onAcquiredItem message = new ScriptMessage_onAcquiredItem (data);
				return message;
			}
	
			static ScriptMessage_onAcquiredItem()
			{
				handlers.Add (".ScriptMessage_onAcquiredItem", Create);
	
			}
			
			override public void NotifyListeners()
			{
				if (Listener != null)
				{
					Listener (this);
				}
			}
		}
		public class ScriptMessage_onDroppedItem : ScriptMessage {
		
			public new static Action<ScriptMessage_onDroppedItem> Listener;
	
			public ScriptMessage_onDroppedItem(GSData data) : base(data){}
	
			private static ScriptMessage_onDroppedItem Create(GSData data)
			{
				ScriptMessage_onDroppedItem message = new ScriptMessage_onDroppedItem (data);
				return message;
			}
	
			static ScriptMessage_onDroppedItem()
			{
				handlers.Add (".ScriptMessage_onDroppedItem", Create);
	
			}
			
			override public void NotifyListeners()
			{
				if (Listener != null)
				{
					Listener (this);
				}
			}
		}
		public class ScriptMessage_onLostItem : ScriptMessage {
		
			public new static Action<ScriptMessage_onLostItem> Listener;
	
			public ScriptMessage_onLostItem(GSData data) : base(data){}
	
			private static ScriptMessage_onLostItem Create(GSData data)
			{
				ScriptMessage_onLostItem message = new ScriptMessage_onLostItem (data);
				return message;
			}
	
			static ScriptMessage_onLostItem()
			{
				handlers.Add (".ScriptMessage_onLostItem", Create);
	
			}
			
			override public void NotifyListeners()
			{
				if (Listener != null)
				{
					Listener (this);
				}
			}
		}
		public class ScriptMessage_onPickedUpItem : ScriptMessage {
		
			public new static Action<ScriptMessage_onPickedUpItem> Listener;
	
			public ScriptMessage_onPickedUpItem(GSData data) : base(data){}
	
			private static ScriptMessage_onPickedUpItem Create(GSData data)
			{
				ScriptMessage_onPickedUpItem message = new ScriptMessage_onPickedUpItem (data);
				return message;
			}
	
			static ScriptMessage_onPickedUpItem()
			{
				handlers.Add (".ScriptMessage_onPickedUpItem", Create);
	
			}
			
			override public void NotifyListeners()
			{
				if (Listener != null)
				{
					Listener (this);
				}
			}
		}
		public class ScriptMessage_onSceneStateChanged : ScriptMessage {
		
			public new static Action<ScriptMessage_onSceneStateChanged> Listener;
	
			public ScriptMessage_onSceneStateChanged(GSData data) : base(data){}
	
			private static ScriptMessage_onSceneStateChanged Create(GSData data)
			{
				ScriptMessage_onSceneStateChanged message = new ScriptMessage_onSceneStateChanged (data);
				return message;
			}
	
			static ScriptMessage_onSceneStateChanged()
			{
				handlers.Add (".ScriptMessage_onSceneStateChanged", Create);
	
			}
			
			override public void NotifyListeners()
			{
				if (Listener != null)
				{
					Listener (this);
				}
			}
		}

}
