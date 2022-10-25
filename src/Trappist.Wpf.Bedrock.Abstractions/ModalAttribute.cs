namespace Trappist.Wpf.Bedrock.Abstractions;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ModalAttribute : Attribute
{
    public bool FullScreen { get; set; }
}
