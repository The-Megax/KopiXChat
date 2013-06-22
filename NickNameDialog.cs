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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KopiXChat
{
	public partial class NickNameDialog : Form
	{
		private MainWindow parent;

		public NickNameDialog(MainWindow parent)
		{
			this.parent = parent;
			InitializeComponent();
		}

		private void nickOKbutton_Click(object sender, EventArgs e)
		{
			this.changeNick();
		}

		private void nick_KeyDown(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == (char)Keys.Enter)
			{
				this.changeNick();
			}
		}

		private void changeNick()
		{
			this.parent.ircClient.NickName = (string)nickNameTextBox.Text.ToString();
			this.Close();
		}
	}
}
