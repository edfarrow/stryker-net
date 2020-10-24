﻿using Stryker.Core.Exceptions;
using Stryker.Core.TestRunners;
using System;

namespace Stryker.Core.Options.Options
{
    // Deprecated, might be removed soon
    public class TestRunnerOption : BaseStrykerOption<TestRunner>
    {
        static TestRunnerOption()
        {
            HelpText = "Choose which testrunner should be used to run your tests.";
            DefaultValue = TestRunner.VsTest;
        }

        public override StrykerOption Type => StrykerOption.TestRunner;

        public TestRunnerOption(string testRunner)
        {
            if (testRunner is { })
            {
                if (Enum.TryParse(testRunner, true, out TestRunner result))
                {
                    Value = result;
                }
                else
                {
                    throw new StrykerInputException($"The given test runner ({testRunner}) is invalid.");
                }
            }
        }
    }
}
