using System;
using System.Collections.Generic;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!
//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!
//THIS FILE IS AUTO GENERATED, DO NOT MODIFY!!

namespace GameSparks.Api.Requests{
	public class LogEventRequest_createCharacter : GSTypedRequest<LogEventRequest_createCharacter, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_createCharacter() : base("LogEventRequest"){
			request.AddString("eventKey", "createCharacter");
		}
		
		public LogEventRequest_createCharacter Set_name( string value )
		{
			request.AddString("name", value);
			return this;
		}
		
		public LogEventRequest_createCharacter Set_gender( string value )
		{
			request.AddString("gender", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_createCharacter : GSTypedRequest<LogChallengeEventRequest_createCharacter, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_createCharacter() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "createCharacter");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_createCharacter SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_createCharacter Set_name( string value )
		{
			request.AddString("name", value);
			return this;
		}
		public LogChallengeEventRequest_createCharacter Set_gender( string value )
		{
			request.AddString("gender", value);
			return this;
		}
	}
	
	public class LogEventRequest_getAdornment : GSTypedRequest<LogEventRequest_getAdornment, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getAdornment() : base("LogEventRequest"){
			request.AddString("eventKey", "getAdornment");
		}
		public LogEventRequest_getAdornment Set_adornment_id( long value )
		{
			request.AddNumber("adornment_id", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_getAdornment : GSTypedRequest<LogChallengeEventRequest_getAdornment, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getAdornment() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getAdornment");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getAdornment SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_getAdornment Set_adornment_id( long value )
		{
			request.AddNumber("adornment_id", value);
			return this;
		}			
	}
	
	public class LogEventRequest_getCharacter : GSTypedRequest<LogEventRequest_getCharacter, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getCharacter() : base("LogEventRequest"){
			request.AddString("eventKey", "getCharacter");
		}
		
		public LogEventRequest_getCharacter Set_char_id( string value )
		{
			request.AddString("char_id", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_getCharacter : GSTypedRequest<LogChallengeEventRequest_getCharacter, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getCharacter() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getCharacter");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getCharacter SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_getCharacter Set_char_id( string value )
		{
			request.AddString("char_id", value);
			return this;
		}
	}
	
	public class LogEventRequest_getOutfit : GSTypedRequest<LogEventRequest_getOutfit, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getOutfit() : base("LogEventRequest"){
			request.AddString("eventKey", "getOutfit");
		}
		
		public LogEventRequest_getOutfit Set_character_id( string value )
		{
			request.AddString("character_id", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_getOutfit : GSTypedRequest<LogChallengeEventRequest_getOutfit, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getOutfit() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getOutfit");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getOutfit SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_getOutfit Set_character_id( string value )
		{
			request.AddString("character_id", value);
			return this;
		}
	}
	
	public class LogEventRequest_isAdornmentAvailable : GSTypedRequest<LogEventRequest_isAdornmentAvailable, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_isAdornmentAvailable() : base("LogEventRequest"){
			request.AddString("eventKey", "isAdornmentAvailable");
		}
		public LogEventRequest_isAdornmentAvailable Set_adornment_id( long value )
		{
			request.AddNumber("adornment_id", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_isAdornmentAvailable : GSTypedRequest<LogChallengeEventRequest_isAdornmentAvailable, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_isAdornmentAvailable() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "isAdornmentAvailable");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_isAdornmentAvailable SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_isAdornmentAvailable Set_adornment_id( long value )
		{
			request.AddNumber("adornment_id", value);
			return this;
		}			
	}
	
	public class LogEventRequest_setOutfit : GSTypedRequest<LogEventRequest_setOutfit, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_setOutfit() : base("LogEventRequest"){
			request.AddString("eventKey", "setOutfit");
		}
		
		public LogEventRequest_setOutfit Set_character_id( string value )
		{
			request.AddString("character_id", value);
			return this;
		}
		public LogEventRequest_setOutfit Set_outfit( GSData value )
		{
			request.AddObject("outfit", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_setOutfit : GSTypedRequest<LogChallengeEventRequest_setOutfit, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_setOutfit() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "setOutfit");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_setOutfit SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_setOutfit Set_character_id( string value )
		{
			request.AddString("character_id", value);
			return this;
		}
		public LogChallengeEventRequest_setOutfit Set_outfit( GSData value )
		{
			request.AddObject("outfit", value);
			return this;
		}
		
	}
	
	public class LogEventRequest_deleteMessage : GSTypedRequest<LogEventRequest_deleteMessage, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_deleteMessage() : base("LogEventRequest"){
			request.AddString("eventKey", "deleteMessage");
		}
		
		public LogEventRequest_deleteMessage Set_message_id( string value )
		{
			request.AddString("message_id", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_deleteMessage : GSTypedRequest<LogChallengeEventRequest_deleteMessage, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_deleteMessage() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "deleteMessage");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_deleteMessage SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_deleteMessage Set_message_id( string value )
		{
			request.AddString("message_id", value);
			return this;
		}
	}
	
	public class LogEventRequest_sendGlobalMessage : GSTypedRequest<LogEventRequest_sendGlobalMessage, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_sendGlobalMessage() : base("LogEventRequest"){
			request.AddString("eventKey", "sendGlobalMessage");
		}
		
		public LogEventRequest_sendGlobalMessage Set_header( string value )
		{
			request.AddString("header", value);
			return this;
		}
		
		public LogEventRequest_sendGlobalMessage Set_body( string value )
		{
			request.AddString("body", value);
			return this;
		}
		public LogEventRequest_sendGlobalMessage Set_payload( GSData value )
		{
			request.AddObject("payload", value);
			return this;
		}			
		public LogEventRequest_sendGlobalMessage Set_query( GSData value )
		{
			request.AddObject("query", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_sendGlobalMessage : GSTypedRequest<LogChallengeEventRequest_sendGlobalMessage, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_sendGlobalMessage() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "sendGlobalMessage");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_sendGlobalMessage SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_sendGlobalMessage Set_header( string value )
		{
			request.AddString("header", value);
			return this;
		}
		public LogChallengeEventRequest_sendGlobalMessage Set_body( string value )
		{
			request.AddString("body", value);
			return this;
		}
		public LogChallengeEventRequest_sendGlobalMessage Set_payload( GSData value )
		{
			request.AddObject("payload", value);
			return this;
		}
		
		public LogChallengeEventRequest_sendGlobalMessage Set_query( GSData value )
		{
			request.AddObject("query", value);
			return this;
		}
		
	}
	
	public class LogEventRequest_dismissGlobalMessage : GSTypedRequest<LogEventRequest_dismissGlobalMessage, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_dismissGlobalMessage() : base("LogEventRequest"){
			request.AddString("eventKey", "dismissGlobalMessage");
		}
		
		public LogEventRequest_dismissGlobalMessage Set_messages( string value )
		{
			request.AddString("messages", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_dismissGlobalMessage : GSTypedRequest<LogChallengeEventRequest_dismissGlobalMessage, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_dismissGlobalMessage() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "dismissGlobalMessage");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_dismissGlobalMessage SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_dismissGlobalMessage Set_messages( string value )
		{
			request.AddString("messages", value);
			return this;
		}
	}
	
	public class LogEventRequest_dismissGlobalMessageBulk : GSTypedRequest<LogEventRequest_dismissGlobalMessageBulk, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_dismissGlobalMessageBulk() : base("LogEventRequest"){
			request.AddString("eventKey", "dismissGlobalMessageBulk");
		}
		public LogEventRequest_dismissGlobalMessageBulk Set_query( GSData value )
		{
			request.AddObject("query", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_dismissGlobalMessageBulk : GSTypedRequest<LogChallengeEventRequest_dismissGlobalMessageBulk, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_dismissGlobalMessageBulk() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "dismissGlobalMessageBulk");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_dismissGlobalMessageBulk SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_dismissGlobalMessageBulk Set_query( GSData value )
		{
			request.AddObject("query", value);
			return this;
		}
		
	}
	
	public class LogEventRequest_getGlobalMessages : GSTypedRequest<LogEventRequest_getGlobalMessages, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getGlobalMessages() : base("LogEventRequest"){
			request.AddString("eventKey", "getGlobalMessages");
		}
		public LogEventRequest_getGlobalMessages Set_messageIDs( GSData value )
		{
			request.AddObject("messageIDs", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_getGlobalMessages : GSTypedRequest<LogChallengeEventRequest_getGlobalMessages, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getGlobalMessages() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getGlobalMessages");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getGlobalMessages SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_getGlobalMessages Set_messageIDs( GSData value )
		{
			request.AddObject("messageIDs", value);
			return this;
		}
		
	}
	
	public class LogEventRequest_getMessages : GSTypedRequest<LogEventRequest_getMessages, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getMessages() : base("LogEventRequest"){
			request.AddString("eventKey", "getMessages");
		}
		public LogEventRequest_getMessages Set_limit( long value )
		{
			request.AddNumber("limit", value);
			return this;
		}			
		public LogEventRequest_getMessages Set_offset( long value )
		{
			request.AddNumber("offset", value);
			return this;
		}			
		
		public LogEventRequest_getMessages Set_character_id( string value )
		{
			request.AddString("character_id", value);
			return this;
		}
		
		public LogEventRequest_getMessages Set_type( string value )
		{
			request.AddString("type", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_getMessages : GSTypedRequest<LogChallengeEventRequest_getMessages, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getMessages() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getMessages");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getMessages SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_getMessages Set_limit( long value )
		{
			request.AddNumber("limit", value);
			return this;
		}			
		public LogChallengeEventRequest_getMessages Set_offset( long value )
		{
			request.AddNumber("offset", value);
			return this;
		}			
		public LogChallengeEventRequest_getMessages Set_character_id( string value )
		{
			request.AddString("character_id", value);
			return this;
		}
		public LogChallengeEventRequest_getMessages Set_type( string value )
		{
			request.AddString("type", value);
			return this;
		}
	}
	
	public class LogEventRequest_sendPrivateMessage : GSTypedRequest<LogEventRequest_sendPrivateMessage, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_sendPrivateMessage() : base("LogEventRequest"){
			request.AddString("eventKey", "sendPrivateMessage");
		}
		
		public LogEventRequest_sendPrivateMessage Set_character_id_to( string value )
		{
			request.AddString("character_id_to", value);
			return this;
		}
		
		public LogEventRequest_sendPrivateMessage Set_header( string value )
		{
			request.AddString("header", value);
			return this;
		}
		
		public LogEventRequest_sendPrivateMessage Set_body( string value )
		{
			request.AddString("body", value);
			return this;
		}
		public LogEventRequest_sendPrivateMessage Set_payload( GSData value )
		{
			request.AddObject("payload", value);
			return this;
		}			
		
		public LogEventRequest_sendPrivateMessage Set_character_id_from( string value )
		{
			request.AddString("character_id_from", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_sendPrivateMessage : GSTypedRequest<LogChallengeEventRequest_sendPrivateMessage, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_sendPrivateMessage() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "sendPrivateMessage");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_sendPrivateMessage SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_sendPrivateMessage Set_character_id_to( string value )
		{
			request.AddString("character_id_to", value);
			return this;
		}
		public LogChallengeEventRequest_sendPrivateMessage Set_header( string value )
		{
			request.AddString("header", value);
			return this;
		}
		public LogChallengeEventRequest_sendPrivateMessage Set_body( string value )
		{
			request.AddString("body", value);
			return this;
		}
		public LogChallengeEventRequest_sendPrivateMessage Set_payload( GSData value )
		{
			request.AddObject("payload", value);
			return this;
		}
		
		public LogChallengeEventRequest_sendPrivateMessage Set_character_id_from( string value )
		{
			request.AddString("character_id_from", value);
			return this;
		}
	}
	
	public class LogEventRequest_completeIsland : GSTypedRequest<LogEventRequest_completeIsland, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_completeIsland() : base("LogEventRequest"){
			request.AddString("eventKey", "completeIsland");
		}
		public LogEventRequest_completeIsland Set_island_id( long value )
		{
			request.AddNumber("island_id", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_completeIsland : GSTypedRequest<LogChallengeEventRequest_completeIsland, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_completeIsland() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "completeIsland");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_completeIsland SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_completeIsland Set_island_id( long value )
		{
			request.AddNumber("island_id", value);
			return this;
		}			
	}
	
	public class LogEventRequest_enterScene : GSTypedRequest<LogEventRequest_enterScene, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_enterScene() : base("LogEventRequest"){
			request.AddString("eventKey", "enterScene");
		}
		
		public LogEventRequest_enterScene Set_scene_id( string value )
		{
			request.AddString("scene_id", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_enterScene : GSTypedRequest<LogChallengeEventRequest_enterScene, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_enterScene() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "enterScene");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_enterScene SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_enterScene Set_scene_id( string value )
		{
			request.AddString("scene_id", value);
			return this;
		}
	}
	
	public class LogEventRequest_getAvailableIslands : GSTypedRequest<LogEventRequest_getAvailableIslands, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getAvailableIslands() : base("LogEventRequest"){
			request.AddString("eventKey", "getAvailableIslands");
		}
	}
	
	public class LogChallengeEventRequest_getAvailableIslands : GSTypedRequest<LogChallengeEventRequest_getAvailableIslands, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getAvailableIslands() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getAvailableIslands");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getAvailableIslands SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
	}
	
	public class LogEventRequest_leaveIsland : GSTypedRequest<LogEventRequest_leaveIsland, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_leaveIsland() : base("LogEventRequest"){
			request.AddString("eventKey", "leaveIsland");
		}
		public LogEventRequest_leaveIsland Set_island_id( long value )
		{
			request.AddNumber("island_id", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_leaveIsland : GSTypedRequest<LogChallengeEventRequest_leaveIsland, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_leaveIsland() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "leaveIsland");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_leaveIsland SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_leaveIsland Set_island_id( long value )
		{
			request.AddNumber("island_id", value);
			return this;
		}			
	}
	
	public class LogEventRequest_visitIsland : GSTypedRequest<LogEventRequest_visitIsland, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_visitIsland() : base("LogEventRequest"){
			request.AddString("eventKey", "visitIsland");
		}
		
		public LogEventRequest_visitIsland Set_island_id( string value )
		{
			request.AddString("island_id", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_visitIsland : GSTypedRequest<LogChallengeEventRequest_visitIsland, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_visitIsland() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "visitIsland");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_visitIsland SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_visitIsland Set_island_id( string value )
		{
			request.AddString("island_id", value);
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
		public LogEventRequest_equipItem Set_item_id( long value )
		{
			request.AddNumber("item_id", value);
			return this;
		}			
		
		public LogEventRequest_equipItem Set_equip_location( string value )
		{
			request.AddString("equip_location", value);
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
		public LogChallengeEventRequest_equipItem Set_item_id( long value )
		{
			request.AddNumber("item_id", value);
			return this;
		}			
		public LogChallengeEventRequest_equipItem Set_equip_location( string value )
		{
			request.AddString("equip_location", value);
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
		public LogEventRequest_pickUpItem Set_item_id( long value )
		{
			request.AddNumber("item_id", value);
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
		public LogChallengeEventRequest_pickUpItem Set_item_id( long value )
		{
			request.AddNumber("item_id", value);
			return this;
		}			
		public LogChallengeEventRequest_pickUpItem Set_sceneId( long value )
		{
			request.AddNumber("sceneId", value);
			return this;
		}			
	}
	
	public class LogEventRequest_removeItem : GSTypedRequest<LogEventRequest_removeItem, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_removeItem() : base("LogEventRequest"){
			request.AddString("eventKey", "removeItem");
		}
		public LogEventRequest_removeItem Set_item_id( long value )
		{
			request.AddNumber("item_id", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_removeItem : GSTypedRequest<LogChallengeEventRequest_removeItem, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_removeItem() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "removeItem");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_removeItem SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_removeItem Set_item_id( long value )
		{
			request.AddNumber("item_id", value);
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
		public LogEventRequest_useItem Set_item_id( long value )
		{
			request.AddNumber("item_id", value);
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
		public LogChallengeEventRequest_useItem Set_item_id( long value )
		{
			request.AddNumber("item_id", value);
			return this;
		}			
	}
	
	public class LogEventRequest_getActiveQuests : GSTypedRequest<LogEventRequest_getActiveQuests, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getActiveQuests() : base("LogEventRequest"){
			request.AddString("eventKey", "getActiveQuests");
		}
	}
	
	public class LogChallengeEventRequest_getActiveQuests : GSTypedRequest<LogChallengeEventRequest_getActiveQuests, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getActiveQuests() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getActiveQuests");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getActiveQuests SetChallengeInstanceId( String challengeInstanceId )
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
	
	public class LogEventRequest_getServerVersion : GSTypedRequest<LogEventRequest_getServerVersion, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getServerVersion() : base("LogEventRequest"){
			request.AddString("eventKey", "getServerVersion");
		}
	}
	
	public class LogChallengeEventRequest_getServerVersion : GSTypedRequest<LogChallengeEventRequest_getServerVersion, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getServerVersion() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getServerVersion");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getServerVersion SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
	}
	
	public class LogEventRequest_resetPassword : GSTypedRequest<LogEventRequest_resetPassword, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_resetPassword() : base("LogEventRequest"){
			request.AddString("eventKey", "resetPassword");
		}
		
		public LogEventRequest_resetPassword Set_old_password( string value )
		{
			request.AddString("old_password", value);
			return this;
		}
		
		public LogEventRequest_resetPassword Set_new_password( string value )
		{
			request.AddString("new_password", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_resetPassword : GSTypedRequest<LogChallengeEventRequest_resetPassword, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_resetPassword() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "resetPassword");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_resetPassword SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_resetPassword Set_old_password( string value )
		{
			request.AddString("old_password", value);
			return this;
		}
		public LogChallengeEventRequest_resetPassword Set_new_password( string value )
		{
			request.AddString("new_password", value);
			return this;
		}
	}
	
	public class LogEventRequest_submitParentEmail : GSTypedRequest<LogEventRequest_submitParentEmail, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_submitParentEmail() : base("LogEventRequest"){
			request.AddString("eventKey", "submitParentEmail");
		}
		
		public LogEventRequest_submitParentEmail Set_parent_email( string value )
		{
			request.AddString("parent_email", value);
			return this;
		}
	}
	
	public class LogChallengeEventRequest_submitParentEmail : GSTypedRequest<LogChallengeEventRequest_submitParentEmail, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_submitParentEmail() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "submitParentEmail");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_submitParentEmail SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_submitParentEmail Set_parent_email( string value )
		{
			request.AddString("parent_email", value);
			return this;
		}
	}
	
	public class LogEventRequest_getLevelAndExperience : GSTypedRequest<LogEventRequest_getLevelAndExperience, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_getLevelAndExperience() : base("LogEventRequest"){
			request.AddString("eventKey", "getLevelAndExperience");
		}
	}
	
	public class LogChallengeEventRequest_getLevelAndExperience : GSTypedRequest<LogChallengeEventRequest_getLevelAndExperience, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_getLevelAndExperience() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "getLevelAndExperience");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_getLevelAndExperience SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
	}
	
	public class LogEventRequest_giveXp : GSTypedRequest<LogEventRequest_giveXp, LogEventResponse>
	{
	
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogEventResponse (response);
		}
		
		public LogEventRequest_giveXp() : base("LogEventRequest"){
			request.AddString("eventKey", "giveXp");
		}
		public LogEventRequest_giveXp Set_amount( long value )
		{
			request.AddNumber("amount", value);
			return this;
		}			
	}
	
	public class LogChallengeEventRequest_giveXp : GSTypedRequest<LogChallengeEventRequest_giveXp, LogChallengeEventResponse>
	{
		public LogChallengeEventRequest_giveXp() : base("LogChallengeEventRequest"){
			request.AddString("eventKey", "giveXp");
		}
		
		protected override GSTypedResponse BuildResponse (GSObject response){
			return new LogChallengeEventResponse (response);
		}
		
		/// <summary>
		/// The challenge ID instance to target
		/// </summary>
		public LogChallengeEventRequest_giveXp SetChallengeInstanceId( String challengeInstanceId )
		{
			request.AddString("challengeInstanceId", challengeInstanceId);
			return this;
		}
		public LogChallengeEventRequest_giveXp Set_amount( long value )
		{
			request.AddNumber("amount", value);
			return this;
		}			
	}
	
}
	

namespace GameSparks.Api.Messages {

		public class ScriptMessage_globalUserMessage : ScriptMessage {
		
			public new static Action<ScriptMessage_globalUserMessage> Listener;
	
			public ScriptMessage_globalUserMessage(GSData data) : base(data){}
	
			private static ScriptMessage_globalUserMessage Create(GSData data)
			{
				ScriptMessage_globalUserMessage message = new ScriptMessage_globalUserMessage (data);
				return message;
			}
	
			static ScriptMessage_globalUserMessage()
			{
				handlers.Add (".ScriptMessage_globalUserMessage", Create);
	
			}
			
			override public void NotifyListeners()
			{
				if (Listener != null)
				{
					Listener (this);
				}
			}
		}
		public class ScriptMessage_privateUserMessage : ScriptMessage {
		
			public new static Action<ScriptMessage_privateUserMessage> Listener;
	
			public ScriptMessage_privateUserMessage(GSData data) : base(data){}
	
			private static ScriptMessage_privateUserMessage Create(GSData data)
			{
				ScriptMessage_privateUserMessage message = new ScriptMessage_privateUserMessage (data);
				return message;
			}
	
			static ScriptMessage_privateUserMessage()
			{
				handlers.Add (".ScriptMessage_privateUserMessage", Create);
	
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
