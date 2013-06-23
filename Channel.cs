/*
 * This file is part of KopiXChat.
 * 
 * Copyright (C) 2013 Megax <http://megax.yeahunter.hu/>
 * 
 * KopiXChat is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * KopiXChat is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with KopiXChat.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System.Net.IRC.Client
{
	public delegate void ChannelMessage(Server server, Channel channel, string message);

	public class Channel
	{
		//public event ChannelMessage ChannelEvent;

		private string _name;
		private string _title = string.Empty;
		// private string titleCreater;
		private Server _server;
		//private List<string> nickNames;
		private List<User> _nickNames;

		//private List<string> messages = new List<string>();
		private string _messages = string.Empty;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Title
		{
			get { return _title; }
		}

		public Server Server
		{
			get { return _server; }
		}

		public string Messages
		{
			get { return _messages; }
		}

		public List<User> NickNames
		{
			get { return _nickNames; }
		}

		public Channel(Server server, string name)
		{
			_nickNames = new List<User>();
			_server = server;
			_name = name;
		}

		public void Join(string username)
		{
			Console.WriteLine("join: " + username);
			User user = new User(username);
			_nickNames.Add(user);
			//_nickNames.Sort();
			_server.ReceivedChannelCommands(this, "JOIN");
		}

		public void Leave(string username)
		{
			Console.WriteLine("leave: " + username);
			// _nickNames.Remove(username);
			_server.ReceivedChannelCommands(this, "PART");
		}

		public void recevedCommands(string[] commandParts)
		{
			switch(commandParts[1])
			{
			case "332":
			case "333":
				setTitle(commandParts);
				break;
			case "353":
				setNickNames(commandParts);
				break;
			case "366":
				break;
			case "328":
				break;
			case "JOIN":
				Join(commandParts[0]);
				break;
			case "PART":
				Leave(commandParts[0]);
				break;
			case "MODE":
				break;
			case "NICK":
				break;
			case "KICK":
				break;
			case "QUIT":
				break;
			case "PRIVMSG":
				ReceveMessage(commandParts);
				break;
			default:
				ReceveMessage(commandParts);
				break;
			}
		}

		public void setTitle(string[] commandParts)
		{
			if(commandParts[1] == "332")
			{
				string title = string.Empty;
				if(commandParts[4].Substring(0, 1) == ":")
					commandParts[4] = commandParts[4].Remove(0, 1);

				for(int intI = 4; intI < commandParts.Length; intI++)
					title += " " + commandParts[intI];

				_messages += title + " \n";
				_title += title;
				_server.ReceivedChannelMessages(this, title);
				_server.ReceivedChannelCommands(this, "TOPIC");
			}
			else if (commandParts[1] == "333")
			{
				string message = "Topic for " + _name + " set by " + commandParts[4] + " on " + " \n";
				_messages += message;
				_server.ReceivedChannelMessages(this, message);
			}
		}

		public void setNickNames(string[] commandParts)
		{
			for(int intI = 5; intI < commandParts.Length; intI++)
			{
				User newUser = new User(commandParts[intI]);
				_nickNames.Add(newUser);
			}
			//_nickNames.Sort();
			_server.ReceivedChannelCommands(this, "NAMES");
		}

		public void ReceveMessage(string[] commandParts)
		{
			string channelMessage = string.Empty;
			for(int intI = 2; intI < commandParts.Length; intI++)
				channelMessage += " " + commandParts[intI];

			_messages += commandParts[0] + " " + channelMessage + " \n";
			_server.ReceivedChannelMessages(this, commandParts[0] + " " + channelMessage);
		}

		public void ReceveMessage(string user, string channelMessage)
		{
			_messages += user + " " + channelMessage+" \n";
			_server.ReceivedChannelMessages(this, channelMessage);
		}

		public void SendMessage(string message)
		{
			_messages += message + "\r\n";
			_server.SentChannelMessages(this, message);
		}

		public void sendCommand(string command, string value, bool print)
		{

			if(command == "PRIVMSG")
				SendMessage(value);
			else
			{
				if(print)
					_messages += command + " " + value + " \n";
			}
		}
	}
}
