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
    
    public partial class m_role
    {
        public m_role()
        {
            this.m_account = new HashSet<m_account>();
            this.m_controller_role = new HashSet<m_controller_role>();
        }
    
        public int id { get; set; }
        public string rolename { get; set; }
        public Nullable<bool> status { get; set; }
    
        public virtual ICollection<m_account> m_account { get; set; }
        public virtual ICollection<m_controller_role> m_controller_role { get; set; }
    }
}
