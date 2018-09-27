namespace TestApp.Core.Helpers
{
    public class ResourceQuery
    {
        //we could ofc use uint for this purpose, but unit -> int overflow is also pretty nasty
        
        private int pageSize = 50;
        public int PageSize
        {
            get { return pageSize; }
            set { if (value >= 0) pageSize = value; }
        }

        private int pageIndex = 0;
        public int PageIndex
        {
            get { return pageIndex; }
            set { if (value >= 0) pageIndex = value; }
        }
    }
}