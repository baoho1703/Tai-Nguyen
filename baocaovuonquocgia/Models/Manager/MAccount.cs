using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Models.Manager
{
    public class MAccount
    {

        public class AccountAdd
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public int Gender { get; set; }
            public int Role { get; set; }

        }

        public ResSubmit GuiMatKhau(int IdAccount, AccountCookie accountCookie)
        {
            ResSubmit resSubmit = new ResSubmit(true, "Gửi email thành công");
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Account = tongcuclamnghiep.m_account.Where(a => a.id == IdAccount).FirstOrDefault();
            if (accountCookie.Role_Id != 1)
            {
                resSubmit = new ResSubmit(false, "Bạn không có quyền này");
            }
            if (resSubmit.success && _Account == null)
            {
                resSubmit = new ResSubmit(false, "Không có tài khoản này");
            }
            if (resSubmit.success)
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
		</tbody></table>", _Account.username, _Account.password);
                if (!FuncExtend.SendMail(_Account.email, body,"Thông tin tài khoản"))
                {
                    resSubmit = new ResSubmit(false, "Gửi email thất bại");
                }
            }
            return resSubmit;
        }

        public ResSubmit ThemMoiTaiKhoan(AccountAdd accountNew, AccountCookie accountCookie)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit respons = new ResSubmit(true, "Thêm mới thành công");
            var regex = new Regex(@"^[a-zA-Z0-9]+$");
            if (!regex.IsMatch(accountNew.UserName))
            {
                respons = new ResSubmit(false, "Username không đúng định dạng");
            }
            regex = new Regex(@"^((([a-zA-Z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-zA-Z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-zA-Z]|\d|-|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-zA-Z]|\d|-|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$");
            if (respons.success == true && !regex.IsMatch(accountNew.Email))
            {
                respons = new ResSubmit(false, "Email không đúng định dạng");
            }
            if (respons.success == true)
            {
                string password = Membership.GeneratePassword(12, 1);
                
                if (tongcuclamnghiep.m_account.Any(a => a.email == accountNew.Email))
                {
                    respons = new ResSubmit(false, "Email đã tồn tại trong hệ thống");
                }
                if (tongcuclamnghiep.m_account.Any(a => a.username == accountNew.UserName))
                {
                    respons = new ResSubmit(false, "Username đã tồn tại trong hệ thống");
                }
                if (!tongcuclamnghiep.m_role.Any(a => a.id == accountNew.Role && a.status == true))
                {
                    respons = new ResSubmit(false, "Quyền không tồn tại trong hệ thống");
                }
                if (respons.success == true)
                {
                    m_account _Account = new m_account();
                    _Account.createdate = DateTime.Now;
                    _Account.email = accountNew.Email;
                    _Account.fullname = accountNew.FullName;
                    _Account.gender = accountNew.Gender==1;
                    _Account.role_id = accountNew.Role;
                    _Account.password = password;
                    _Account.status = true;
                    _Account.username = accountNew.UserName;
                    tongcuclamnghiep.m_account.Add(_Account);

                    if (tongcuclamnghiep.SaveChanges() != 1)
                    {
                        respons = new ResSubmit(false, "Thêm mới thất bại");
                    }
                }
            }
            return respons;
        }

        public ResTableData GetAccountTable(int start, int length, int draw, string search, AccountCookie accountCookie)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var dataAll = tongcuclamnghiep.m_account.Where(a => a.id != accountCookie.Id && (search.Length > 0 ? (a.username.Contains(search) || a.email.Contains(search) || a.fullname.Contains(search)) : true))
                .Select(a => new
                {
                    a.id,
                    a.username,
                    a.email,
                    a.createdate,
                    a.gender,
                    a.fullname,
                    a.status,
                    rolename = a.m_role.rolename
                });
            var data = dataAll.ToList().Skip((start)).Take(length);
            return new ResTableData()
            {
                data= data,
                draw = (draw+ 1),
                recordsFiltered= dataAll.Count(),
                recordsTotal = dataAll.Count()
            };
        }
    }
}