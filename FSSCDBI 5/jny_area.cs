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

    public partial class jny_area
    {
        public int local_id { get; set; }
        public Nullable<System.DateTime> _updated_at { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public Nullable<int> is_complete { get; set; }
        public Nullable<int> is_template { get; set; }
        public string name { get; set; }
        public Nullable<System.Guid> site_id { get; set; }
        public Nullable<int> site_type { get; set; }
        public Nullable<int> sort_order { get; set; }
        public string status { get; set; }
        public Nullable<System.Guid> template_id { get; set; }
        public Nullable<System.Guid> id { get; set; }
        public string display_name { get; set; }
    }
}
