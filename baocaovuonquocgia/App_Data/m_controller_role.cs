//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace baocaovuonquocgia.App_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class m_controller_role
    {
        public int id { get; set; }
        public Nullable<int> controller_id { get; set; }
        public Nullable<int> role_id { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<bool> status { get; set; }
    
        public virtual m_controller m_controller { get; set; }
        public virtual m_role m_role { get; set; }
    }
}