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
			Logger l = UserMetrix.getLogger<MainClass>();
			l.frustration("Unable to configure logger");
			try {
				throw new Exception("WTF?");
				testMethod();
			} catch (Exception e) {
				l.error(e);
			}

			UserMetrix.shutdown();
		}

		public static void testMethod() {
			throw new Exception("Moo!");
		}
	}
}
