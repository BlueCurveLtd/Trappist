using System.Diagnostics;
using System.Linq;

namespace InGroupe.Innovation.Wpf.Bedrock;

/// <summary>
/// User session, single instance implementation using process check. This class cannot be inherited.
/// Implements the <see cref="InGroupe.Innovation.Wpf.Bedrock.ISingleInstanceApp" />
/// </summary>
/// <seealso cref="InGroupe.Innovation.Wpf.Bedrock.ISingleInstanceApp" />
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
