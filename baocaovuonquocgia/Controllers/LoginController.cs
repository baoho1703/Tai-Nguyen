using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baocaovuonquocgia.App_Data;
using baocaovuonquocgia.Models;

namespace baocaovuonquocgia.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index(AccountLogin account)
        {
            if (!string.IsNullOrEmpty(account.PassWord) && !string.IsNullOrEmpty(account.UserName))
            {
                tongcuclamnghiepEntities dataBase = new tongcuclamnghiepEntities();
                var acc = dataBase.m_account.Where(a => a.password == account.PassWord && (a.username == account.UserName || a.email == account.UserName) && a.status == true).FirstOrDefault();
                if (acc != null)
                {
                    Session["AccountCookie"] = new AccountCookie() { Id = acc.id, UserName = acc.username != null ? acc.username : acc.email, Role_Id = acc.role_id, FullName = acc.fullname };
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "This account does not exist");
                    return View("~/Views/Login/Index.cshtml");
                }
            }
            else
            {
                return View();
            }

        }

        public ActionResult ResetMatKhau(AccountLogin account)
        {
            if (!string.IsNullOrEmpty(account.UserName))
            {
                tongcuclamnghiepEntities dataBase = new tongcuclamnghiepEntities();
                var acc = dataBase.m_account.Where(a => (a.email == account.UserName) && a.status == true).FirstOrDefault();
                if (acc != null)
                {
                    string body = string.Format(@"<table width='100%' cellspacing='0' cellpadding='0' border='0' style='width:100%;min-width:100%'>
			<tbody>
            <tr>
				<td align='left' style='padding:0;font-size:20px;line-height:26px;letter-spacing:-0.32px;font-weight:normal;font-family:'Segoe UI Semibold','Segoe UI',SUWSB,Arial,sans-serif;color:#0078d6'>
					Tóm tắt tài khoản
				</td>
			</tr>
			<tr>
				<td class='m_-8498465890789338487useremail' align='left' style='padding:3px 0 0;font-size:14px;line-height:19px;letter-spacing:-0.16px;font-weight:normal;font-family:'Segoe UI','Segoe UI Regular',SUWR,Arial,sans-serif;color:#6e6e6e'>
					<b style='font-family:'Segoe UI Semibold','Segoe UI',SUWSB,Arial,sans-serif;font-weight:normal'>Tên Tài khoản:</b> <a href='#m_-8498465890789338487_' style='text-decoration:none;color:#737373'>{0}</a>
				</td>
			</tr>
            <tr>
				<td class='m_-8498465890789338487useremail' align='left' style='padding:3px 0 0;font-size:14px;line-height:19px;letter-spacing:-0.16px;font-weight:normal;font-family:'Segoe UI','Segoe UI Regular',SUWR,Arial,sans-serif;color:#6e6e6e'>
					<b style='font-family:'Segoe UI Semibold','Segoe UI',SUWSB,Arial,sans-serif;font-weight:normal'>Mật khẩu:</b> <a href='#m_-8498465890789338487_' style='text-decoration:none;color:#737373'>{1}</a>
				</td>
			</tr>
			<tr>
				<td align='left' style='padding:3px 0 0;font-size:14px;line-height:19px;letter-spacing:-0.16px;font-weight:normal;font-family:'Segoe UI Semibold','Segoe UI',SUWSB,Arial,sans-serif;color:#6e6e6e'>
					<a href='http://baocaolamnghiep.quantriwebhanoi.com' style='color:#6e6e6e;text-decoration:underline' target='_blank' ><strong style='font-weight:normal'>Tài khoản của Tôi</strong></a>
				</td>
			</tr>
		</tbody></table>", acc.username, acc.password);
                    if (FuncExtend.SendMail(acc.email, body, "Thông tin tài khoản"))
                    {
                        ModelState.AddModelError("", "Gửi email thành công");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Gửi email thất bại");
                    }
                    return View("~/Views/Login/ResetMatKhau.cshtml");
                }
                else
                {
                    ModelState.AddModelError("", "Email tài khoản không tồn tại");
                    return View("~/Views/Login/ResetMatKhau.cshtml");
                }
            }
            else
            {
                return View();
            }

        }

        [CheckSessionAjax]
        public ActionResult Logout()
        {
            Session.Contents.RemoveAll();
            return RedirectToAction("Index");
        }
    }
}