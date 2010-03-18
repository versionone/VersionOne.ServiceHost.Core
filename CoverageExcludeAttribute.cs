using System;

namespace VersionOne.ServiceHost.Core 
{
    /// <summary>
    /// An attribute to mark assemblies that should be excluded from coverage report
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CoverageExcludeAttribute : Attribute 
    {
    }
}
