﻿using Lotr2Inspector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lotr2Inspector.Structs;

namespace ConquerDashboard
{
	class Dashboard
	{
        public const int THREAD_SLEEP = 200;

        private Control Console;
        private ctlTownSummary[] TownSummary;
        private Form1 f1;

        public void setForm(Form1 f1)
        {
            if(this.f1 != default(Form1))
            {
                throw new Exception("Form1 is already set");
			}

            this.f1 = f1;
        }

        public void setConsole(Control console)
        {
            if(this.Console != default(Control))
            {
                throw new Exception("Console is already set");
			}

            this.Console = console;
		}

        public void setTownSummaries(ctlTownSummary[] townSummaries)
        {
            if(this.TownSummary != default(ctlTownSummary[]))
            {
                throw new Exception("Town summaries already set");
			}

            this.TownSummary = townSummaries;
		}

        public void RunDash()
        {
            System.Diagnostics.Process[] lords2Processes;
            
            // Acquire process pid, exits when program is closed
            // @TODO Also check if dashboard is closed, getting random instance errors when closing
            while(true)
            {
                Console.Invoke(new System.Action(() =>
                {
                    Console.Text = $"$$  Waiting for {Process.LORDS2_PROCESS_NAME}...";
                }));

                lords2Processes = System.Diagnostics.Process.GetProcessesByName(Process.LORDS2_PROCESS_NAME);

                // Multiple instances aren't supported
                if (lords2Processes.Length == 1)
                {
                    MemStruct.ProcessHandle = Process.OpenProcess(Process.LORDS2_PROCESS_ALL_ACCESS, false, lords2Processes[0].Id);

                    // No clue why it needs to be initialized so late. AFAIK all the memory is always there.
                    if(Lotr2Inspector.Game.Maps == null)
                    {
                        Lotr2Inspector.Game.Maps = new Maps();
                    }

                    if (Lotr2Inspector.Game.MapTowns == null)
                    {
                        MapTowns.FindMapsTowns();

                        Lotr2Inspector.Game.MapTowns = MapTowns.Towns;
                    }

                    while (!lords2Processes[0].HasExited)
                    {
                        Console.Invoke(new System.Action(() =>
                        {
                            Console.Text = $"$$ {Process.LORDS2_PROCESS_NAME} is not fullscreen...";
                        }));

                        // Wait for L2 to fully switch to fullscreen before interacting with the UI
                        if (!lords2Processes[0].HasExited && Lotr2Inspector.Game.State.IsActive())
                        {
                            Console.Invoke(new System.Action(() =>
                            {
                                Console.Text = "$$ Waiting for fullscreen...";
                            }));

                            Thread.Sleep(800);

                            // Pop up trainer on second monitor
                            this.f1.Invoke(new System.Action(() =>
                            {
                                this.f1.showOnScreen(1);
                            }));

                            while(!lords2Processes[0].HasExited && Lotr2Inspector.Game.State.IsActive())
                            {
                                Lotr2Inspector.Game.State.JumpySnapshot();

                                for(int i = 0; i < Lotr2Inspector.Config.MAX_TOWNS; i++)
                                {
                                    TownSummary[i].updateControl();
								}

                                Thread.Sleep(THREAD_SLEEP);

                                Console.Invoke(new System.Action(() =>
                                {
                                    Console.Text = $"Playing: #{Lotr2Inspector.Game.CurrentGame.Get("current map")}: {Lotr2Inspector.Game.Maps.Get(Lotr2Inspector.Game.CurrentGame.Get("current map").ToString(), false)} ({Lotr2Inspector.Game.MapTowns[Lotr2Inspector.Game.CurrentGame.Get("current map")][0]}), Turn: {Lotr2Inspector.Game.Calendar.Get("turn")}";
                                }));
                            }
                        }
                    }
                }

                Thread.Sleep(THREAD_SLEEP * 2);
            }
        }
    }
}
