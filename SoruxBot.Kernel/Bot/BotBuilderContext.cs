﻿namespace SoruxBot.Kernel.Bot
{
    public class BotBuilderContext
    {
        public BotBuilderContext(BuildEnvironmentType buildEnvironmentType)
            => (BuildEnvironment) = (buildEnvironmentType);

        public BuildEnvironmentType BuildEnvironment { get; init; }
    }

    public enum BuildEnvironmentType
    {
        Normal,
        Debug,
        Developer
    }
}