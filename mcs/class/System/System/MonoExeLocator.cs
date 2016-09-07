using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System {

	public static class MonoExeLocator {

		public static string GacPath { get; }
		public static string MonoPath { get; }
		public static string McsPath { get; }
		public static string VbncPath { get; }
		
		static MonoExeLocator () {

			PropertyInfo gac = typeof (Environment).GetProperty ("GacPath", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo get_gac = gac.GetGetMethod (true);
			string p = Path.GetDirectoryName ((string) get_gac.Invoke (null, null));
			GacPath = p;

			string monoPath = null;
			string mcsPath = null;
			string vbncPath = null;

			if (Path.DirectorySeparatorChar == '\\') {
				string processExe = Process.GetCurrentProcess ().MainModule.FileName;
				if (processExe != null) {
					string fileName = Path.GetFileName (processExe);
					if (fileName.StartsWith ("mono") && fileName.EndsWith (".exe"))
						monoPath = processExe;
				}

				if (!File.Exists (monoPath))
					monoPath = Path.Combine (
						Path.GetDirectoryName (
							Path.GetDirectoryName (p)),
						"bin\\mono.exe");

				if (!File.Exists (monoPath))
					monoPath = Path.Combine (
						Path.GetDirectoryName (
							Path.GetDirectoryName (
								Path.GetDirectoryName (p))),
						"mono\\mini\\mono.exe");

				if (!File.Exists (monoPath))
					throw new FileNotFoundException ("Windows mono path not found: " + monoPath);

				mcsPath = Path.Combine (p, "4.5\\mcs.exe");
				if (!File.Exists (mcsPath))
					mcsPath = Path.Combine (Path.GetDirectoryName (p), "lib\\build\\mcs.exe");

				if (!File.Exists (mcsPath))
					throw new FileNotFoundException ("Windows mcs path not found: " + mcsPath);

				vbncPath = Path.Combine (p, "2.0\\vbnc.exe");
					
			} else {
				monoPath = Path.Combine (GacPath, "bin/mono");
				if (!File.Exists (MonoPath))
					monoPath = "mono";

				var mscorlibPath = new Uri (typeof (object).Assembly.CodeBase).LocalPath;
				mcsPath = Path.GetFullPath( Path.Combine (mscorlibPath, "..", "..", "..", "..", "bin", "mcs"));
				if (!File.Exists (mcsPath))
					mcsPath = "mcs";

				vbncPath = Path.Combine (Path.GetDirectoryName (mcsPath), "vbnc");
				if (!File.Exists (vbncPath))
					vbncPath = "vbnc";
			}

			McsPath = mcsPath;
			MonoPath = monoPath;
			VbncPath = vbncPath;
		}

		
	}
}
