//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FSSCDBI_5
{
    using System;
    using System.Collections.Generic;

    public partial class jny_site
    {
        public int local_id { get; set; }
        public Nullable<System.DateTime> _updated_at { get; set; }
        public Nullable<System.DateTime> date_first { get; set; }
        public Nullable<System.DateTime> date_last { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string altitude { get; set; }
        public string horizontal_accuracy { get; set; }
        public string vertical_accuracy { get; set; }
        public Nullable<int> import_current { get; set; }
        public Nullable<int> import_total { get; set; }
        public Nullable<int> is_complete { get; set; }
        public string name { get; set; }
        public Nullable<int> site_type { get; set; }
        public Nullable<int> sort_order { get; set; }
        public Nullable<System.Guid> worker_id { get; set; }
        public Nullable<System.Guid> id { get; set; }
        public string display_name { get; set; }
    }
}
