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

namespace KopiXChat.Config
{
	public static class Consts
	{
		//private static readonly Utilities sUtilities = Singleton<Utilities>.Instance;
		public const string KopiXChatDescription = "KopiXChat Irc Client";
#if DEBUG
		public const string KopiXChatConfiguration = "Debug";
#else
		public const string KopiXChatConfiguration = "Release";
#endif
		public const string KopiXChatCompany = "KopiXChat Productions";
		public const string KopiXChatProduct = "KopiXChat";
		public const string KopiXChatCopyright = "Copyright (C) 2013 Megax <http://megax.yeahunter.hu/>";
		public const string KopiXChatTrademark = "GNU General Public License";
		public const string KopiXChatVersion = "0.0.1";
		public const string KopiXChatFileVersion = "0.0.1.0";
		public const string KopiXChatProgrammedBy = "Csaba Jakosa (Megax)";
		public const string KopiXChatDevelopers = "Csaba Jakosa (Megax)";
		public const string KopiXChatWebsite = "https://github.com/megax/KopiXChat";
		//public static string KopiXChatUserAgent = KopiXChatBase.Title + KopiXChatBase.Space + sUtilities.GetVersion() + " / .NET " + Environment.Version;
		//public const string KopiXChatReferer = "http://yeahunter.hu";
	}
}