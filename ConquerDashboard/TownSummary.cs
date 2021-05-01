using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lotr2Inspector.Structs;

namespace ConquerDashboard
{
	public partial class ctlTownSummary : UserControl
	{
		private Town town;
		private Color Red = Color.FromArgb(255, 56, 56);
		private Color Green = Color.FromArgb(0, 200, 0);
		private Color Black = Color.FromArgb(0, 0, 0);

		public ctlTownSummary()
		{
			InitializeComponent();

			Label[] transparentLabels = new Label[] { txtTax, txtRation, txtActualTax, txtActualRation, txtActualHappiness, txtActualPopulation, txtTown };

			foreach(Label l in transparentLabels)
			{
				var pos = PointToScreen(l.Location);
				pos = picMenuBackground.PointToClient(pos);
				l.Parent = picMenuBackground;
				l.Location = pos;
				l.BackColor = Color.Transparent;
			}

			Control[] transparentPics = new Control[] { picPeasant, panelCow, panelWheat, panelSerf, panelWood, panelStone, panelOre, panelWeapon };

			foreach(Control p in transparentPics)
			{
				p.BackColor = Color.Transparent;
				p.Parent = picMenuBackground;
			}
		}

		public void setTown(Town town)
		{
			if(this.town != default(Town))
			{
				throw new Exception("Town is already set");
			}

			this.town = town;

			this.updateControl();
		}

		public void updateControl()
		{
			if(town == default(Town))
			{
				throw new Exception("Town is not set");
			}

			String townName = "";

			if(Lotr2Inspector.Game.MapTowns != null && Lotr2Inspector.Game.MapTowns[Lotr2Inspector.Game.CurrentGame.Get("current map")].Count - 1 > town.Item)
			{
				townName = Lotr2Inspector.Game.MapTowns[Lotr2Inspector.Game.CurrentGame.Get("current map")][town.Item + 1];
			}

			if(town.Item == Lotr2Inspector.Game.CurrentGame.Get("selected town") - 1)
			{
				this.BackColor = Black;
			}

			else
			{
				this.BackColor = default(Color);
			}

			txtTown.Invoke(new System.Action(() =>
			{
				txtTown.Text = townName;
			}));

			txtActualPopulation.Invoke(new System.Action(() =>
			{
				txtActualPopulation.Text = $"{town.Get("population")}";
			}));

			// txtActualHappiness
			txtActualHappiness.Invoke(new System.Action(() =>
			{
				txtActualHappiness.Text = $"{town.Get("happiness")}";
			}));

			// txtActualTax
			txtActualTax.Invoke(new System.Action(() =>
			{
				txtActualTax.Text = $"{town.Get("taxes")}%";
			}));

			// txtActualRation
			txtActualRation.Invoke(new System.Action(() =>
			{
				txtActualRation.Text = town.getRation(town.Get("ration achieved"));

				if(town.Get("ration achieved") < town.Get("ration wanted"))
				{
					txtActualRation.ForeColor = Red;
					txtActualRation.Font = new Font(txtActualRation.Font, FontStyle.Bold);
				}

				else
				{
					txtActualRation.ForeColor = Black;
					txtActualRation.Font = new Font(txtActualRation.Font, FontStyle.Regular);
				}
			}));

			// picPeasant
			picPeasant.Invoke(new System.Action(() =>
			{
				int Y = picPeasant.Location.Y;

				picPeasant.Location = new Point((int) ((51f / 103f) * town.Get("peasant slider") + 55), Y);
				
				if(town.Get("idle townsfolk") > 0)
				{
					picPeasant.BackColor = Color.Blue;
				}

				else
				{
					picPeasant.BackColor = Color.Transparent;
				}
			}));

			// lblCattleProduction
			lblCattleProduction.Invoke(new System.Action(() =>
			{
				if(town.Get("cows") > 0 || town.Get("cow fields") > 0)
				{
					int cowProduction = town.Get("cow production");
					String production = cowProduction.ToString();

					lblCowCounty.Text = town.Get("cows").ToString();

					if (cowProduction == 0)
					{
						lblCattleProduction.Text = "";
					}

					else if (cowProduction > 0)
					{
						lblCattleProduction.Text = $"+{cowProduction}";
						lblCattleProduction.ForeColor = Green;
						lblCattleProduction.Font = new Font(lblCattleProduction.Font, FontStyle.Bold);
					}

					else
					{
						lblCattleProduction.Text = $"-{cowProduction}";
						lblCattleProduction.ForeColor = Red;
						lblCattleProduction.Font = new Font(lblCattleProduction.Font, FontStyle.Bold);
					}

					if (town.Get("dairy maids needed?") > town.Get("dairy maids"))
					{
						picCow.BackColor = Red;
					}

					else
					{
						picCow.BackColor = Color.Transparent;
					}

					panelCow.Visible = true;
				}

				else
				{
					panelCow.Visible = false;
				}
			}));

			// lblWheatProduction
			lblWheatProduction.Invoke(new System.Action(() =>
			{
				if(town.Get("wheat") > 0 || town.Get("wheat fields") > 0)
				{
					//lblWheatProduction.Text = town.Get("wheat production").ToString();
					lblCountyWheat.Text = town.Get("wheat").ToString();

					int wheatProduction = town.Get("wheat production");
					String production = wheatProduction.ToString();

					if (wheatProduction == 0)
					{
						lblWheatProduction.Text = "";
					}

					else if (wheatProduction > 0)
					{
						lblWheatProduction.Text = $"+{wheatProduction}";
						lblWheatProduction.ForeColor = Green;
						lblWheatProduction.Font = new Font(lblCattleProduction.Font, FontStyle.Bold);
					}

					else
					{
						lblWheatProduction.Text = $"-{wheatProduction}";
						lblWheatProduction.ForeColor = Red;
						lblWheatProduction.Font = new Font(lblWheatProduction.Font, FontStyle.Bold);
					}

					panelWheat.Visible = true;

					if(town.Get("farmers needed") > town.Get("farmers"))
					{
						picWheat.BackColor = Red;
					}

					else
					{
						picWheat.BackColor = Color.Transparent;
					}
					
				}

				else
				{
					panelWheat.Visible = false;
				}
			}));

			// lblWoodProduction
			lblWoodProduction.Invoke(new System.Action(() =>
			{
				if(town.Get("forestry state") == 1)
				{
					panelWood.Visible = true;
					lblWoodProduction.Text = town.Get("wood production").ToString();
				}

				else
				{
					panelWood.Visible = false;
				}
			}));

			// lblStoneProduction
			lblStoneProduction.Invoke(new System.Action(() =>
			{
				if (town.Get("quarry state") == 1)
				{
					panelStone.Visible = true;
					lblStoneProduction.Text = town.Get("stone production").ToString();
				}

				else
				{
					panelStone.Visible = false;
				}
			}));

			// lblOreProduction
			lblOreProduction.Invoke(new System.Action(() =>
			{
				if(town.Get("mine state") == 1)
				{
					panelOre.Visible = true;

					lblOreProduction.Text = town.Get("ore production").ToString();
				}

				else
				{
					panelOre.Visible = false;
				}
			}));

			// lblReclaimedSeasonsLeft
			lblReclaimedSeasonsLeft.Invoke(new System.Action(() =>
			{
				if(town.Get("reclaiming fields") > 0)
				{
					panelSerf.Visible = true;

					lblReclaimedSeasonsLeft.Text = town.Get("reclaim fields seasons left").ToString();
				}

				else
				{
					panelSerf.Visible = false;
				}
			}));


			// picWeapon
			picWeapon.Invoke(new System.Action(() =>
			{
				if(town.Get("blacksmith state") == 1)
				{
					panelWeapon.Visible = true;

					lblWeaponProduction.Text = this.town.Get("weapon production").ToString();

					switch(town.Get("blacksmith selected weapon"))
					{
						default:
						case 0:
							picWeapon.Image = Properties.Resources.Crossbow;
							break;

						case 1:
							picWeapon.Image = Properties.Resources.Mace;
							break;

						case 2:
							picWeapon.Image = Properties.Resources.Sword;
							break;

						case 3:
							picWeapon.Image = Properties.Resources.Pike;
							break;

						case 4:
							picWeapon.Image = Properties.Resources.Bow;
							break;

						case 5:
							picWeapon.Image = Properties.Resources.Knight;
							break;
					}
				}

				else
				{
					panelWeapon.Visible = false;
				}
			}));

			///////////////////////////////////////////////////////////////////////////////
			// txtDebug
			txtDebug.Invoke(new System.Action(() =>
			{
				txtDebug.Text = $"{this.town.Get("dairy maids needed?").ToString()} > {this.town.Get("dairy maids").ToString()}";
			}));
		}

		private void ctlTownSummary_Load(object sender, EventArgs e)
		{
			
		}
	}
}
