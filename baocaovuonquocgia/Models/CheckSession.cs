using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Models
{
    public class CheckSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Session["AccountCookie"] == null)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
            }
            else
            {
                AccountCookie accountCookie = (AccountCookie)filterContext.HttpContext.Session["AccountCookie"];
                string actionName = filterContext.ActionDescriptor.ActionName;
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string menu_url = string.Format("/{0}/{1}", controllerName, actionName);
                if (controllerName != "Home")
                {
                    tongcuclamnghiepEntities dataBase = new tongcuclamnghiepEntities();
                    var acmenu = dataBase.m_controller_role.Where(a => a.role_id == accountCookie.Role_Id && a.m_controller.controllerurl == menu_url).FirstOrDefault();
                    if (acmenu == null)
                    {
                        //if (accountCookie.Role_Id == 1)
                        //{
                        //    filterContext.Result = new RedirectResult("/Report/Reportlist");
                        //}
                        //else
                        //{
                        //    filterContext.Result = new RedirectResult("/Home/Index");
                        //}
                        filterContext.Result = new RedirectResult("/Home/Index");
                    }
                }
                //else
                //{
                //    if (accountCookie.Role_Id == 1)
                //    {
                //        filterContext.Result = new RedirectResult("/Report/Reportlist");
                //    }
                //}
            }
        }
    }

    public class CheckSessionAjax : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Session["AccountCookie"] == null)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
            }
        }
    }
}