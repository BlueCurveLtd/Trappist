using System.Collections.ObjectModel;
using System.ComponentModel;
using static InGroupe.Innovation.Wpf.Bedrock.Controls.Breadcrumb;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.ViewModels
{
    public sealed class BreadcrumbViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<InternalBreadcrumbNode> InternalNodes { get; }

        internal BreadcrumbViewModel() 
            => this.InternalNodes = new ObservableCollection<InternalBreadcrumbNode>();

        public void Clear() => this.InternalNodes.Clear();

        public void AddOrRemove(InternalBreadcrumbNode internalBreadcrumbNode, InternalBreadcrumbNode separator, bool clear)
        {
            if (clear)
            {
                this.InternalNodes.Clear();
            }

            if (this.InternalNodes.Count == 0)
            {
                this.InternalNodes.Add(internalBreadcrumbNode);
            }
            else
            {
                var index = this.InternalNodes.IndexOf(internalBreadcrumbNode);
                
                if (index == -1) // not found
                {
                    this.InternalNodes.Add(separator);
                    this.InternalNodes.Add(internalBreadcrumbNode);
                }
                else
                {
                    for (var i = this.InternalNodes.Count - 1; i > index; --i)
                    {
                        this.InternalNodes.RemoveAt(i);
                    }
                }
            }
        }

        public void Remove(InternalBreadcrumbNode internalBreadcrumbNode)
        {
            this.InternalNodes.Remove(internalBreadcrumbNode);

            var lastNode = this.InternalNodes[this.InternalNodes.Count - 1];

            if (lastNode.IsSeparator)
            {
                this.InternalNodes.Remove(lastNode);
            }
        }
    }
}
