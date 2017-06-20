﻿using System;

namespace YH
{
	static class MainClass
	{
        [STAThread]
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			new Window(800/2, 600/2).Run(60.0, 0.0);
		}
	}
}
