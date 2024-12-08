using System;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Utilities;


namespace Utils;

[Serializable]
public class DockerComposeDownSettings : DockerComposeSettings
{
    protected override Arguments ConfigureProcessArguments(Arguments arguments)
    {
        arguments = base.ConfigureProcessArguments(arguments);
        arguments.Add("down");
        return arguments;
    }
}