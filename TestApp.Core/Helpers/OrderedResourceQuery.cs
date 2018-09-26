namespace TestApp.Core.Helpers
{
    public abstract class OrderedResourceQuery<TOrderKey> : ResourceQuery where TOrderKey: struct 
    {
        public SortOrder? SortOrder { get; set; }
        public abstract TOrderKey? SortBy { get; set; }
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }
}