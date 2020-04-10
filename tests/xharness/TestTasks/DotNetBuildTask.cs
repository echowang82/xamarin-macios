﻿using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared.Execution;
using Microsoft.DotNet.XHarness.iOS.Shared.Logging;

namespace Xharness.TestTasks {
	public class DotNetBuildTask : MSBuildTask {

		public DotNetBuildTask (string msbuildPath,
							IProcessManager processManager,
							IResourceManager resourceManager,
							IEventLogger eventLogger,
							IEnvManager envManager,
							IErrorKnowledgeBase errorKnowledgeBase)
			: base (msbuildPath, processManager, resourceManager, eventLogger, envManager, errorKnowledgeBase) { }

		public override List<string> GetToolArguments (string projectPlatform, string projectConfiguration, string projectFile, ILog buildLog)
		{
			var args = base.GetToolArguments (projectPlatform, projectConfiguration, projectFile, buildLog);
			args.Remove ("--");
			args.Insert (0, "build");
			return args;
		}
	}
}
