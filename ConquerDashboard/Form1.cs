using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ConquerDashboard
{
	public partial class Form1 : Form
	{
		private int[] hotkeys = new int[] { (int)Keys.G, (int)Keys.I, (int)Keys.S, (int)Keys.L, (int)Keys.H, (int)Keys.P, (int)Keys.C, (int)Keys.W };
		private ctlTownSummary[] townSummary = new ctlTownSummary[16];
		private Lotr2Inspector.Form1 frmXml;
		private Dashboard dash = new Dashboard();

		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		const int MOD_CONTROL = 0x0002;
		const int MOD_SHIFT = 0x0004;
		public const int WM_HOTKEY = 0x0312;

		public Form1()
		{
			InitializeComponent();
		}

		public void showOnScreen(int screenNumber)
		{
			Screen[] screens = Screen.AllScreens;

			if (screenNumber >= 0 && screenNumber < screens.Length)
			{
				bool maximised = false;
				if (WindowState == FormWindowState.Normal)
				{
					WindowState = FormWindowState.Normal;
					maximised = true;
				}

				Location = screens[screenNumber].WorkingArea.Location;
				if (maximised)
				{
					WindowState = FormWindowState.Maximized;
				}
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// Cheater's hotkeys ;)
			foreach(int k in hotkeys)
			{
				RegisterHotKey(this.Handle, k, MOD_CONTROL, k);
			}

			// Init as much game data
			Lotr2Inspector.Game.init();

			if(!brainerTrainer.IsBusy) brainerTrainer.RunWorkerAsync();

			int i = 0;
			int yOffset = 32;

			for(int y = 0; y < 3; y++)
			{
				for(int x = 0; x < 6; x++)
				{
					// There's no easy way to break nested loops in C#... (shrug) PHP can do that ;P
					if(i < Lotr2Inspector.Config.MAX_TOWNS)
					{
						this.townSummary[i] = new ctlTownSummary();
						this.townSummary[i].setTown(Lotr2Inspector.Game.Towns[i]);
						this.townSummary[i].Location = new System.Drawing.Point(x * 163, y * 276 + yOffset);

						this.Controls.Add(this.townSummary[i]);

						i++;
					}
				}
			}
		}

		private void brainerTrainer_DoWork(object sender, DoWorkEventArgs e)
		{
			// Setup and run dashboard
			dash.setConsole(this.Controls.Find("lblConsole", false)[0]);
			dash.setTownSummaries(townSummary);
			dash.setForm(this);
			dash.RunDash();
		}

		private void lblConsole_Click(object sender, EventArgs e)
		{
			frmXml = new Lotr2Inspector.Form1();
			frmXml.Show();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			foreach (int k in hotkeys)
			{
				UnregisterHotKey(this.Handle, k);
			}
		}

		protected override void WndProc(ref Message m)
		{
			dash.incomingHotkey(ref m);
			base.WndProc(ref m);
		}
	}
}
