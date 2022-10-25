using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Trappist.Wpf.Bedrock.ObjectModel;

public abstract class ValidableObjectModel : ValidableObjectBase, IDataErrorInfo
{
    /// <summary>
    /// Gets the errors associated with the specified column name.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    /// <returns>The concatenated errors.</returns>
    [NotNull]
    public string this[[DisallowNull] string columnName]
    {
        get
        {
            if (this.Errors.TryGetValue(columnName, out var results))
            {
                return string.Join(", ", results.Select(x => x.ErrorMessage));
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// Gets an error message indicating what is wrong with this object.
    /// </summary>
    [NotNull]
    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "WPF")]
    public virtual string Error => "";

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidableObjectModel"/> class.
    /// </summary>
    /// <param name="validateOnInitialization">if set to <c>true</c> first validation happen when object is built.</param>
    protected ValidableObjectModel(bool validateOnInitialization = false)
        : base(validateOnInitialization)
    {

    }
}
