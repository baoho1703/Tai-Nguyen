using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baocaovuonquocgia.Models;


namespace baocaovuonquocgia.Controllers
{
    public class ManagerController : Controller
    {
        [CheckSession]
        // GET: Account
        public ActionResult Account(int? tab)
        {
            ControllerTab controllerTab = new ControllerTab();
            if (tab == null)
            {
                controllerTab.Tab = 1;
            }
            else
            {
                controllerTab.Tab = tab.Value;
            }
            return View(controllerTab);
        }

        [CheckSession]
        // GET: Precious
        public ActionResult Precious(int? tab, int? IdChinhSua)
        {
            ControllerTab controllerTab = new ControllerTab();
            if (tab == null)
            {
                controllerTab.Tab = 1;
            }
            else
            {
                controllerTab.Tab = tab.Value;
                if (tab == 3)
                {
                    controllerTab.IdChinhSua = IdChinhSua.Value;
                }
            }
            return View(controllerTab);
        }

        #region -- Account --

        [CheckSessionAjax]
        public JsonResult GetAccountTable(int start, int length, int draw)
        {
            string search = Request["search[value]"];
            AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
            return Json(new baocaovuonquocgia.Models.Manager.MAccount().GetAccountTable(start, length, draw, search, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult ThemMoiTaiKhoan(baocaovuonquocgia.Models.Manager.MAccount.AccountAdd accountNew)
        {
            AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
            return Json(new baocaovuonquocgia.Models.Manager.MAccount().ThemMoiTaiKhoan(accountNew, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GuiMatKhau(int IdAccount)
        {
            AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
            return Json(new baocaovuonquocgia.Models.Manager.MAccount().GuiMatKhau(IdAccount, accountCookie), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region -- Precious --


        [CheckSessionAjax]
        public JsonResult ThemMoiQuy(Models.Manager.MPrecious.PreciousAdd preciousAdd)
        {
            AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
            return Json(new Models.Manager.MPrecious().ThemMoiQuy(preciousAdd, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GetPreciousTable(int start, int length, int draw)
        {
            string search = Request["search[value]"];
            return Json(new baocaovuonquocgia.Models.Manager.MPrecious().GetPreciousTable(start, length, draw, search), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult ChinhSuaQuy(Models.Manager.MPrecious.PreciousAdd preciousEdit)
        {
            return Json(new baocaovuonquocgia.Models.Manager.MPrecious().ChinhSuaQuy(preciousEdit), JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}