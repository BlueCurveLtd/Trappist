using System;
using System.Collections.Generic;

namespace Trappist.Wpf.Bedrock.Controls.Routing;

public sealed class Route
{
    public Type? ViewType { get; set; }
    public string? ViewAlias { get; set; }
    public List<Condition>? Conditions { get; set; }
}

public abstract class Condition
{
    public virtual bool IsSatisfied { get; }        
}
