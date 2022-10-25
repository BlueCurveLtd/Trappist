using InGroupe.Innovation.Wpf.Bedrock.ObjectModel;

namespace InGroupe.Innovation.Wpf.Bedrock;

public abstract class ViewModelBase : ValidableObjectBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    /// <param name="validateOnInitialization">if set to <c>true</c> first validation happen when object is built.</param>
    protected ViewModelBase(bool validateOnInitialization = false)
        : base(validateOnInitialization)
    {

    }
}
