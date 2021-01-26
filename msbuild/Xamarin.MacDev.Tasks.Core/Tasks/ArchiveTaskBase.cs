﻿using System;
using System.IO;
using System.Diagnostics;

using Microsoft.Build.Framework;

using System.Globalization;

namespace Xamarin.MacDev.Tasks
{
	public abstract class ArchiveTaskBase : XamarinTask
	{
		protected readonly DateTime Now = DateTime.Now;

		#region Inputs

		[Required]
		public ITaskItem AppBundleDir { get; set; }

		public string InsightsApiKey { get; set; }

		[Required]
		public string OutputPath { get; set; }

		[Required]
		public string ProjectName { get; set; }

		public string ProjectGuid { get; set; }

		public string ProjectTypeGuids { get; set; }

		public string SolutionPath { get; set; }

		[Required]
		public string SigningKey { get; set; }

		#endregion

		#region Outputs

		[Output]
		public string ArchiveDir { get; set; }

		#endregion

		protected string DSYMDir {
			get { return AppBundleDir.ItemSpec + ".dSYM"; }
		}

		protected string MSYMDir {
			get { return AppBundleDir.ItemSpec + ".mSYM"; }
		}

		protected string XcodeArchivesDir {
			get {
				var home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);

				return Path.Combine (home, "Library", "Developer", "Xcode", "Archives");
			}
		}

		protected string CreateArchiveDirectory ()
		{
			var timestamp = Now.ToString ("M-dd-yy h.mm tt", CultureInfo.InvariantCulture);
			var folder = Now.ToString ("yyyy-MM-dd");
			var baseArchiveDir = XcodeArchivesDir;
			string archiveDir, name;
			int unique = 1;

			do {
				if (unique > 1)
					name = string.Format ("{0} {1} {2}.xcarchive", ProjectName, timestamp, unique);
				else
					name = string.Format ("{0} {1}.xcarchive", ProjectName, timestamp);

				archiveDir = Path.Combine (baseArchiveDir, folder, name);
				unique++;
			} while (Directory.Exists (archiveDir));

			Directory.CreateDirectory (archiveDir);

			return archiveDir;
		}

		protected void ArchiveDSym (string dsymDir, string archiveDir)
		{
			if (Directory.Exists (dsymDir)) {
				var destDir = Path.Combine (archiveDir, "dSYMs", Path.GetFileName (dsymDir));
				Ditto (dsymDir, destDir);
			}
		}

		protected void ArchiveMSym (string msymDir, string archiveDir)
		{
			if (Directory.Exists (msymDir)) {
				var destDir = Path.Combine (archiveDir, "mSYMs", Path.GetFileName (msymDir));
				Ditto (msymDir, destDir);
			}
		}

		protected static int Ditto (string source, string destination)
		{
			var args = new CommandLineArgumentBuilder ();

			args.AddQuoted (source);
			args.AddQuoted (destination);

			var psi = new ProcessStartInfo ("/usr/bin/ditto", args.ToString ()) {
				RedirectStandardOutput = false,
				RedirectStandardError = false,
				UseShellExecute = false,
				CreateNoWindow = true,
			};

			using (var process = Process.Start (psi)) {
				process.WaitForExit ();

				return process.ExitCode;
			}
		}
	}
}
