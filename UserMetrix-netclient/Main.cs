using System;

namespace UserMetrix
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			Configuration c = new Configuration(0);
			UserMetrix.initalise(c);
			UserMetrix.shutdown();
		}
	}
}
