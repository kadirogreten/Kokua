using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokuaApi.Models
{
    public class FilterViewModel
    {

        public DateTime CreatedAt { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }

    }


    public class PagedCollectionResponse<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }
    }


    public abstract class FilterModelBase : ICloneable
    {
        public int Page { get; set; }
        public int Limit { get; set; }

        public FilterModelBase()
        {
            this.Page = 1;
            this.Limit = 100;
        }

        public abstract object Clone();
    }

    public enum SortedBy
    {
        CreatedAt
    }

    public enum SortedType
    {
        Artan,
        Azalan
    }

    public class SampleFilterModel : FilterModelBase
    {
        public string Term { get; set; }
        public DateTime? CreatedAt { get; set; }
        public SortedBy SortedBy { get; set; }
        public SortedType SortedType { get; set; }


        public SampleFilterModel() : base()
        {
            this.Limit = 1;
        }


        public override object Clone()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject(jsonString, this.GetType());
        }
    }
}
