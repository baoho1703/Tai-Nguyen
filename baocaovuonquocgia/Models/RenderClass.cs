using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace baocaovuonquocgia.Models
{
    public class ResetPass
    {
        public string MatKhauCu { get; set; }
        public string MatKhauMoi { get; set; }
        public string ReMatKhauMoi { get; set; }
    }
    public class AccountLogin
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
    public class AccountCookie
    {
        public string UserName { get; set; }
        public int? Id { get; set; }
        public int? Role_Id { get; set; }
        public string FullName { get; set; }
    }
    public class MenuTop
    {
        public string Title { get; set; }
        public string ControllerUrl { get; set; }
        public List<MenuBottom> Menus { get; set; }
    }
    public class MenuBottom
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class ControllerTab
    {
        public int Tab { get; set; }
        public int IdChinhSua { get; set; }
    }

    public class ResTableData
    {
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public object data { get; set; }
        public object extend { get; set; }
        public int draw { get; set; }
    }

    public class ResSubmit
    {
        public ResSubmit(bool success, string message)
        {
            this.success = success;
            this.message = message;
        }
        public bool success { get; set; }
        public string message { get; set; }
        public object extend { get; set; }
    }

    
    
}