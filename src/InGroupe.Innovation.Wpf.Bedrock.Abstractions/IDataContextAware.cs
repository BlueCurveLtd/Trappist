namespace InGroupe.Innovation.Wpf.Bedrock.Abstractions;

/// <summary>
/// Datacontext aware contract.
/// </summary>
public interface IDataContextAware
{
    /// <summary>
    /// Called when the data context is set to the view.
    /// </summary>
    void DataContextWireUp();
}
