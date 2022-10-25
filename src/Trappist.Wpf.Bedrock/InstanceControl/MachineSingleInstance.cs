using System.Threading;

namespace Trappist.Wpf.Bedrock.InstanceControl;

/// <summary>
/// Machine wide, single instance implementation using mutex. This class cannot be inherited.
/// Implements the <see cref="ISingleInstanceApp" />
/// </summary>
/// <seealso cref="ISingleInstanceApp" />
internal sealed class MachineSingleInstance : ISingleInstanceApp
{
    private readonly string uniqueIdentifier;
    private readonly Mutex mutex;
    private readonly bool created;

    /// <summary>
    /// Initializes a new instance of the <see cref="MachineSingleInstance"/> class.
    /// </summary>
    /// <param name="uniqueIdentifier">The unique identifier.</param>
    public MachineSingleInstance(string uniqueIdentifier)
    {
        this.uniqueIdentifier = uniqueIdentifier;
        this.mutex = new Mutex(true, this.uniqueIdentifier, out this.created);
    }

    /// <summary>Checks if the application is already started.</summary>
    /// <returns>
    ///   <c>true</c> if the application is already started, <c>false</c> otherwise.
    /// </returns>
    public bool AlreadyStarted() => !this.created;

    public void Release()
    {
        this.mutex.ReleaseMutex();
        this.mutex.Dispose();
    }
}
