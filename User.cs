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
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace System.Net.IRC.Client
{
	public class User
	{
		private string nickname = string.Empty;
		//private string connection = "";
		private Color nickcolor;
		//private string[] modes;

		public string NickName
		{
			get { return nickname; }
			set { nickname = value; }
		}

		public Color Color
		{
			get { return nickcolor; }
		}

		/*public string[] Modes
		{
			get { return this.modes; }
		}*/

		public User(string Nickname)
		{
			//split on !~
			nickname = Nickname;
			var random = new Random();
			nickcolor = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
		}

		public override string ToString()
		{
			return nickname;
		}
	}
}
