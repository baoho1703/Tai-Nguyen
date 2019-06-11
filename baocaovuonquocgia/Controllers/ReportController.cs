using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baocaovuonquocgia.Models;
using Newtonsoft.Json;

namespace baocaovuonquocgia.Controllers
{
    public class ReportController : Controller
    {
        [CheckSession]
        // GET: Reportnew
        public ActionResult Reportnew()
        {
            return View();
        }

        [CheckSession]
        // GET: Reportlist
        public ActionResult Reportlist(int? tab, int? IdChinhSua)
        {
            ControllerTab controllerTab = new ControllerTab();
            if (tab == null)
            {
                controllerTab.Tab = 1;
            }
            else
            {
                controllerTab.Tab = tab.Value;
                if (tab == 2)
                {
                    controllerTab.IdChinhSua = IdChinhSua.Value;
                }
            }
            return View(controllerTab);
        }

        [CheckSession]
        // GET: Reportlistunit
        public ActionResult Reportlistunit(int? tab, int? IdChinhSua)
        {
            ControllerTab controllerTab = new ControllerTab();
            if (tab == null)
            {
                controllerTab.Tab = 1;
            }
            else
            {
                controllerTab.Tab = tab.Value;
                if (tab == 2)
                {
                    controllerTab.IdChinhSua = IdChinhSua.Value;
                }
            }
            return View(controllerTab);
        }


        #region -- Reportnew --

        [CheckSessionAjax]
        public JsonResult GetDanhSachBaoCaoUnitNew()
        {
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new Models.Report.MReportnew().GetDanhSachBaoCaoUnitNew(accountCookie),JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult CapNhatDataComponentReportUnit(int IdComponent, string TextValue)
        {
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new Models.Report.MReportnew().CapNhatDataComponentReportUnit(IdComponent, TextValue, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GetTableReportAutoRow(int IdBaoCao)
        {
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new Models.Report.MReportnew().GetTableReportAutoRow(IdBaoCao, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult ThemMoiThanhPhanAuto(int IdBaoCao, int RowId)
        {
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new Models.Report.MReportnew().ThemMoiThanhPhanAuto(IdBaoCao, RowId, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult CapNhatDataComponentAutoReportUnit(int RowHeaderId, string TextValue)
        {
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new Models.Report.MReportnew().CapNhatDataComponentAutoReportUnit(RowHeaderId, TextValue, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult XoaDuLieuBaoCaoUnitAuto(int RowId, int IdBaoCao)
        {
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new Models.Report.MReportnew().XoaDuLieuBaoCaoUnitAuto(RowId, IdBaoCao, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult HoanThanhBaoCaoUnit()
        {
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new Models.Report.MReportnew().HoanThanhBaoCaoUnit(accountCookie), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region -- Reportlistunit --

        [CheckSessionAjax]
        public JsonResult GetPreciousTable(int start, int length, int draw)
        {
            string search = Request["search[value]"];
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new baocaovuonquocgia.Models.Report.MReportlistunit().GetPreciousTable(start, length, draw, search, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GetDanhSachBaoCaoUnit(int IdPrecious)
        {
            AccountCookie accountCookie = (AccountCookie)Session["accountCookie"];
            return Json(new baocaovuonquocgia.Models.Report.MReportlistunit().GetDanhSachBaoCao(IdPrecious, accountCookie), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region -- Reportlist --

        [CheckSessionAjax]
        public JsonResult GetPreciousTableAll(int start, int length, int draw)
        {
            string search = Request["search[value]"];
            return Json(new baocaovuonquocgia.Models.Report.MReportlist().GetPreciousTableAll(start, length, draw, search), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetDanhSachBaoCao(int IdDonVi, int IdPrecious)
        {
            //string output = JsonConvert.SerializeObject(new baocaovuonquocgia.Models.Report.MReportlist().GetDanhSachBaoCao());
            //return Content(output, "application/json");
            return Json(new baocaovuonquocgia.Models.Report.MReportlist().GetDanhSachBaoCao(IdDonVi, IdPrecious), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}