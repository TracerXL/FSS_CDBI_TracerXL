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

    public partial class jny_material
    {
        public int local_id { get; set; }
        public Nullable<System.DateTime> _updated_at { get; set; }
        public string code { get; set; }
        public Nullable<int> condition { get; set; }
        public string description { get; set; }
        public Nullable<System.Guid> element_id { get; set; }
        public Nullable<int> is_complete { get; set; }
        public Nullable<System.Guid> material_type_id { get; set; }
        public string name { get; set; }
        public Nullable<int> photo { get; set; }
        public Nullable<decimal> qty { get; set; }
        public Nullable<decimal> qty_inspect { get; set; }
        public Nullable<decimal> qty_no_action { get; set; }
        public Nullable<decimal> qty_repair { get; set; }
        public Nullable<decimal> qty_replace { get; set; }
        public Nullable<decimal> qty_service { get; set; }
        public Nullable<decimal> qty_upgrade { get; set; }
        public string remarks { get; set; }
        public Nullable<System.Guid> site_id { get; set; }
        public string status { get; set; }
        public string uom { get; set; }
        public Nullable<System.Guid> id { get; set; }
        public string display_name { get; set; }
    }
}
