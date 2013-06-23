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
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace System.Net.IRC.Client
{
	public delegate void ServerMessage(Server server, string message);
	public delegate void CommandSent(Server server, string command);

	public class Server
	{
		// messages the user sends to the server 
		public event ChannelMessage ChannelSentMessages;
		// messages the client receives from the server 
		public event ChannelMessage ChannelReceivedMessages;
		// commands the user sends to the server 
		public event ChannelMessage ChannelSentCommands;
		// commands the client receives from the server 
		public event ChannelMessage ChannelReceivedCommands;
		public event ServerMessage ServerCommandsSent;
		public event ServerMessage ServerCommandsReceived;
		private string _name;
		private string _host;
		private int _port;
		private string _username;
		private string _password;
		private string _nickname;
		private string _realName = "Timothy's IRC Client";
		private bool isConnected = false;
		private bool isInvisble = false;
		private string selectedChannel;
		private SortedDictionary<string,Channel> connectedChannels = new SortedDictionary<string,Channel>();
		private string[] _hostNameParts;
		private TcpClient _connection;
		private StreamReader _reader;
		private StreamWriter _writer;
		private Channel _serverChannel;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Host
		{
			get { return _host; }
		}

		public int Port
		{
			get { return _port; }
		}

		public string NickName
		{
			get { return _nickname; }
			set { ChangeNickName(value); }
		}

		public bool ISConnected
		{
			get { return isConnected; }
		}

		public string SelectedChannel
		{
			get { return selectedChannel; }
			set { selectedChannel = value; }
		}

		public Channel Channel
		{
			get { return getChannel(selectedChannel); }
		}

		public Channel getChannel(string channelName)
		{
			return channelName == _hostNameParts[1] ? _serverChannel : connectedChannels[channelName];
		}

		public Server()
		{

		}

		public void Connect(string host, int port)
		{
			Connect(host, port, null, null, null, null);
		}

		public void Connect(string host, int port, string nickname)
		{
			Connect(host, port, null, null, null, nickname);
		}

		public void Connect(string host, int port, string password, string nickname)
		{
			Connect(host, port, null, null, password, nickname);
		}

		public void Connect(string Host, int Port, string UserName, string Name, string Password, string NickName)
		{
			_hostNameParts = Host.Split('.');
			_serverChannel = new Channel(this, _hostNameParts[1]);
			_host = Host;
			_port = Port;
			_username = UserName;
			_nickname = NickName;

			if(_name == null)
				_name = _hostNameParts[1];
			else
				_name = Name;

			_password = Password;
			_connection = new TcpClient(_host, _port);

			if(ServerCommandsSent != null)
				ServerCommandsSent(this, "CONNECT");

			_reader = new StreamReader(_connection.GetStream());
			_writer = new StreamWriter(_connection.GetStream()) { AutoFlush = true };
			Authenticate();
			isConnected = true;
			Listen();
		}

		public void SentChannelMessages(Channel channel, string message)
		{
			if(selectedChannel == channel.Name)
			{
				if(ChannelSentMessages != null)
					ChannelSentMessages(this, channel, message.Trim());
			}
		}

		public void ReceivedChannelMessages(Channel channel, string message)
		{
			if(selectedChannel == channel.Name)
			{
				if(ChannelReceivedMessages != null)
					ChannelReceivedMessages(this, channel, message.Trim());
			}
		}

		public void SentChannelCommands(Channel channel, string message)
		{
			if(selectedChannel == channel.Name)
			{
				if(ChannelSentCommands != null)
					ChannelSentCommands(this, channel, message);
			}
		}

		public void ReceivedChannelCommands(Channel channel, string message)
		{
			if(selectedChannel == channel.Name)
			{
				if(ChannelReceivedCommands != null)
					ChannelReceivedCommands(this, channel, message);
			}
		}

		public void Disconnect()
		{
			_writer.Close();
			_reader.Close();
			_connection.Close();
			isConnected = false;
		}

		private void Authenticate()
		{
			if(_password != null)
				SendCommand("PASS", _password);

			SendCommand("NICK", _nickname);

			string isVisable = isInvisble ? "8" : "0";

			if(_username != null)
				SendCommand("USER", _username + " " + isVisable + " " + _realName);
			else
				SendCommand("USER", _nickname + " " + isVisable + " " + _realName);
		}

		private void Listen()
		{
			while(true)
			{
				string command;
				while((command = _reader.ReadLine()) != null)
				{
					if(ServerCommandsReceived != null)
						ServerCommandsReceived(this, command);

					string[] commandParts = command.Split(' ');

					if(commandParts[0].Substring(0, 1) == ":")
						commandParts[0] = commandParts[0].Remove(0, 1);

					string[] commandhostParts = commandParts[0].Split('.');

					if(commandParts.Length > 3 && (connectedChannels.ContainsKey(commandParts[2])))
						connectedChannels[commandParts[2]].recevedCommands(commandParts);
					else if(commandParts.Length > 4 && (connectedChannels.ContainsKey(commandParts[3])))
						connectedChannels[commandParts[3]].recevedCommands(commandParts);
					else if(commandParts.Length > 5 && (connectedChannels.ContainsKey(commandParts[4])))
						connectedChannels[commandParts[4]].recevedCommands(commandParts);
					else
					{
						if((commandParts[0] == _host) || (commandhostParts.Length > 1 && commandhostParts[1] == _hostNameParts[1] && commandhostParts[2] == _hostNameParts[2]))
						{
							switch(commandParts[1])
							{
								// case "332": break;
								// case "333":
								// case "353": /*serverChannel.setNickNames(commandParts);//*/connectedChannels[commandParts[4]].setNickNames(commandParts);/* setNamesList(commandParts);*/
								//   break;
								/* case "431":*/
								case "451":
									Authenticate();
									break;
								default:
									IncomingServerMessage(commandParts);
									break;
							}
						}
						else if(commandParts[0] == "PING")
							Ping(commandParts);
						else
						{
							switch(commandParts[1])
							{
							case "JOIN":
								break;
							case "PART":
								break;
							case "MODE":
								break;
							case "NICK":
								break;
							case "KICK":
								break;
							case "QUIT":
								break;
							case "ERROR":
								Disconnect();
								IncomingServerMessage(commandParts);
								break;
							default:
								IncomingServerMessage(commandParts);
								break;
							}
						}
					}
				}

				Disconnect();
				break;
			}
		}

		private void Ping(string[] command)
		{
			string PingHash = string.Empty;
			for(int intI = 1; intI < command.Length; intI++)
				PingHash += command[intI] + " ";

			SendCommand("PONG", PingHash, false);

			if(ServerCommandsSent != null)
				ServerCommandsSent(this, "PONG " + PingHash);
		}

		private void IncomingServerMessage(string[] command)
		{
			string serverMessage = string.Empty;
			for(int intI = 0; intI < command.Length; intI++)
				serverMessage += command[intI] + " ";

			_serverChannel.recevedCommands(command);
		}

		private void ReceveMessage(string[] commandParts)
		{
			var channel = connectedChannels[commandParts[2]];
			string channelMessage = string.Empty;
			for(int intI = 2; intI <commandParts.Length; intI++)
				channelMessage += " " + commandParts[intI];

			channel.ReceveMessage(commandParts[0], channelMessage);
		}

		/*
		 * for when the client joins a channel
		 */
		public void JoinChannel(string channelName)
		{
			SendCommand("JOIN", channelName);
			var joinedChannel = new Channel(this, channelName);
			connectedChannels.Add(channelName, joinedChannel);

			if(ChannelSentCommands != null)
				ChannelSentCommands(this, joinedChannel, "JOIN");
		}

		/*
		 * for when the client leaves a channel
		 */
		public void LeaveChannel(string channelName)
		{
			if(connectedChannels.ContainsKey(channelName))
			{
				SendCommand("PART", channelName);

				if(ChannelSentCommands != null)
					ChannelSentCommands(this, connectedChannels[channelName], "PART"); 

				connectedChannels.Remove(channelName);
			}
			else
			{
				// return system message (You are not a memeber if the channel)
			}
		}

		public void SendCommand(string Command, string value)
		{
			SendCommand(Command, value, true);
		}

		public void SendCommand(string Command, string value, bool print)
		{
			SendCommand(Command, value, null, print);
		}

		public void SendCommand(string Command, string value, string channel)
		{
			SendCommand(Command, value, channel, true);
		}

		public void SendCommand(string command, string value, string channel, bool print)
		{
			if(channel != null)
			{
				if(print)
				{
					if(ServerCommandsSent != null)
						ServerCommandsSent(this, command + " " + channel + " " + value);

					connectedChannels[channel].sendCommand(command, value, print);   
				}

				_writer.WriteLine(command + " " + channel + " " + value);
			}
			else
			{
				if(print)
				{
					if(ServerCommandsSent != null)
						ServerCommandsSent(this, command + " " + value);

					_serverChannel.sendCommand(command, value, print);
				}

				_writer.WriteLine(command + " " + value);
			}
		}

		public void ChangeNickName(string newNickname)
		{
			if(newNickname != _nickname)
			{
				SendCommand("NICK", newNickname);
				_nickname = newNickname;
			}
		}
	}
} 