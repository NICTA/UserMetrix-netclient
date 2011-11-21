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
			UserMetrix.Initalise(c);
			Thread.Sleep(500);
			Logger l = UserMetrix.GetLogger<MainClass>();
			l.View("main view");
			l.Event("triggered button.");
			l.Frustration("Unable to configure logger");
			try {
				throw new Exception("WTF?");
				testMethod();
			} catch (Exception e) {
				l.Error("gringle", e);
			}

			UserMetrix.Shutdown();
		}

		public static void testMethod() {
			throw new Exception("Moo!");
		}
	}
}
