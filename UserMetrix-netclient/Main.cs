using System;
using System.Threading;

namespace UserMetrix
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			Configuration c = new Configuration(0);
			UserMetrix.initalise(c);
			Thread.Sleep(500);
			UserMetrix.shutdown();
		}
	}
}
