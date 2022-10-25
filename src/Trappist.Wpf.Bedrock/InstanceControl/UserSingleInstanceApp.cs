using System.Diagnostics;
using System.Linq;

namespace Trappist.Wpf.Bedrock.InstanceControl;

/// <summary>
/// User session, single instance implementation using process check. This class cannot be inherited.
/// Implements the <see cref="ISingleInstanceApp" />
/// </summary>
/// <seealso cref="ISingleInstanceApp" />
internal sealed class UserSingleInstanceApp : ISingleInstanceApp
{
    /// <summary>Checks if the application is already started.</summary>
    /// <returns>
    ///   <c>true</c> if the application is already started, <c>false</c> otherwise.
    /// </returns>
    public bool AlreadyStarted()
    {
        var currentProcess = Process.GetCurrentProcess();
        return Process.GetProcessesByName(currentProcess.ProcessName).Any(p => p.SessionId == currentProcess.SessionId && p.Id != currentProcess.Id);
    }

    public void Release()
    {
    }
}
