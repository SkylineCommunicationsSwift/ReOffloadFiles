namespace ReOffloadFiles
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using Skyline.DataMiner.Automation;
	using System.IO;
	using System.Linq;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			try
			{
				RunSafe(engine);
			}
			catch (ScriptAbortException)
			{
				// Catch normal abort exceptions (engine.ExitFail or engine.ExitSuccess)
				throw; // Comment if it should be treated as a normal exit of the script.
			}
			catch (ScriptForceAbortException)
			{
				// Catch forced abort exceptions, caused via external maintenance messages.
				throw;
			}
			catch (ScriptTimeoutException)
			{
				// Catch timeout exceptions for when a script has been running for too long.
				throw;
			}
			catch (InteractiveUserDetachedException)
			{
				// Catch a user detaching from the interactive script by closing the window.
				// Only applicable for interactive scripts, can be removed for non-interactive scripts.
				throw;
			}
			catch (Exception e)
			{
				engine.ExitFail("Run|Something went wrong: " + e);
			}
		}

		private void RunSafe(IEngine engine)
		{
			string sourceFolder = @"C:\Skyline DataMiner\System Cache\offload\Data\failure";
			string destinationFolder = @"C:\Skyline DataMiner\System Cache\offload\Data";

			if (Directory.Exists(sourceFolder))
			{
				var files = Directory.GetFiles(sourceFolder).Take(20).ToList();

				if (files.Any())
				{
					foreach (var file in files)
					{
						string fileName = Path.GetFileName(file);
						string destFile = Path.Combine(destinationFolder, fileName);

						File.Move(file, destFile);
					}

					engine.ExitSuccess("Files moved successfully.");
				}
				else
				{
					engine.ExitSuccess("No files to move.");
				}
			}
			else
			{
				engine.ExitFail("Source folder does not exist.");
			}
		}
	}
}
