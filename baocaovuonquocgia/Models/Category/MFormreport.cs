using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Models.Category
{
    public class MFormreport
    {
        public class listBaoCao
        {
            public int? id { get; set; }
            public int? sapxep { get; set; }
        }

        

        public class TreeBaoCao
        {
            public string key { get; set; }
            public string title { get; set; }
            public bool folder { get; set; }
            public bool lazy { get; set; }
            public object dataSource { get; set; }
            public List<TreeBaoCao> children { get; set; }
        }

        public class HeaderAdd : m_header
        {
            public int IdBaoCao { get; set; }
            public int HeaderId { get; set; }
        }
        public class TableReport
        {
            public string title { get; set; }
            public int order { get; set; }
            public int level { get; set; }
            public int colspan { get; set; }
            public int rowspan { get; set; }
            public object extend { get; set; }
        }
        public class HeaderComponent
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int TypeValue { get; set; }
            public bool TextBold { get; set; }
            public string TextAlign { get; set; }
            public bool TextUpper { get; set; }
        }

        public class TableComponent
        {
            public int RowId { get; set; }
            public List<HeaderComponent> headers { get; set; }
        }
        public class ComponentUpdate
        {
            public int RowId { get; set; }
            public int HeaderId { get; set; }
            public string Title { get; set; }
            public int TypeValue { get; set; }
            public int IdBaoCao { get; set; }
        }



        public ResSubmit XoaComponent(int RowId, int IdBaoCao)
        {
            ResSubmit resSubmit = new ResSubmit(true, "Xóa thành công");
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            if (tongcuclamnghiep.m_reporttable.Any(a => a.id == IdBaoCao))
            {
                if (!tongcuclamnghiep.m_component.Any(a=>a.orderrow== RowId && a.reporttable_id == IdBaoCao && a.m_unit_datareport.Any()))
                {
                    resSubmit = new ResSubmit(true, "Xóa thành công");
                    resSubmit.extend = tongcuclamnghiep.m_component.ToList().Select(a=>new { a.id, a.orderrow });
                    tongcuclamnghiep.m_component.Where(a => a.orderrow == RowId && a.reporttable_id == IdBaoCao).ToList().All(a =>
                    {
                        tongcuclamnghiep.m_component.Remove(a);
                        return true;
                    });
                    tongcuclamnghiep.SaveChanges();
                }
                else
                {
                    resSubmit = new ResSubmit(true, "Đã có dữ liệu, không được xóa!");
                }
            }
            else
            {
                resSubmit = new ResSubmit(true, "Không có báo cáo này!");
            }
            return resSubmit;
        }

        public ResSubmit CapNhatThanhPhanDataType(ComponentUpdate componentUpdate)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            var _Component = tongcuclamnghiep.m_component.Where(a => a.header_id == componentUpdate.HeaderId && a.reporttable_id == componentUpdate.IdBaoCao && a.orderrow == componentUpdate.RowId).FirstOrDefault();
            if (!tongcuclamnghiep.m_reporttable.Any(a => a.id == componentUpdate.IdBaoCao))
            {
                resSubmit = new ResSubmit(false, "Không có báo cáo này");
            }
            if(resSubmit.success && _Component == null)
            {
                resSubmit = new ResSubmit(false, "Không có thành phần này");
            }
            if (resSubmit.success)
            {
                if(_Component.m_header.is_no==true || _Component.m_header.is_component == true)
                {
                    _Component.componentname = componentUpdate.Title;
                    _Component.typevalue = 1;
                }
                else
                {
                    _Component.typevalue = componentUpdate.TypeValue;
                }
                if (tongcuclamnghiep.SaveChanges() != 1)
                {
                    resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                }
                else
                {
                    TableComponent tableComponent = new TableComponent();
                    tableComponent.RowId = componentUpdate.RowId;
                    List<HeaderComponent> headers = new List<HeaderComponent>();
                    tongcuclamnghiep.m_component.Where(a => a.orderrow == componentUpdate.RowId && a.reporttable_id == componentUpdate.IdBaoCao).ToList().ForEach(b => {
                        headers.Add(new HeaderComponent()
                        {
                            Id = b.header_id.Value,
                            Title = b.componentname,
                            TypeValue = b.typevalue.Value,
                            TextAlign = b.m_header.valuetextalign,
                            TextBold = b.m_header.is_valuebold.Value,
                            TextUpper = b.m_header.is_valueupper.Value
                        });
                    });
                    tableComponent.headers = headers;
                    resSubmit.extend = tableComponent;
                }
            }
            return resSubmit;
        }



        public List<TableComponent> GetTableComponentReport(int IdBaoCao)
        {
            List<TableComponent> tableComponents = new List<TableComponent>();
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            tongcuclamnghiep.m_component.Where(a => a.reporttable_id == IdBaoCao).GroupBy(a => a.orderrow).ToList().ForEach(a=> {
                TableComponent tableComponent = new TableComponent();
                tableComponent.RowId = a.Key.Value;
                List<HeaderComponent> headers = new List<HeaderComponent>();
                a.ToList().ForEach(b=> {
                    headers.Add(new HeaderComponent()
                    {
                        Id = b.header_id.Value,
                        Title = b.componentname,
                        TypeValue = b.typevalue.Value,
                        TextAlign=b.m_header.valuetextalign,
                        TextBold=b.m_header.is_valuebold.Value,
                        TextUpper=b.m_header.is_valueupper.Value
                    });
                });
                tableComponent.headers = headers;
                tableComponents.Add(tableComponent);
            });

            return tableComponents;
        }

        public ResSubmit ThemMoiThanhPhan(int IdBaoCao, int RowId)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Thêm mới thành công");
            if(!tongcuclamnghiep.m_reporttable.Any(a=>a.id == IdBaoCao))
            {
                resSubmit = new ResSubmit(false, "Không có báo cáo này");
            }
            if(resSubmit.success && tongcuclamnghiep.m_component.Any(a=>a.reporttable_id==IdBaoCao && a.orderrow== RowId))
            {
                resSubmit = new ResSubmit(false, "Dòng này đã thêm");
            }
            if (resSubmit.success)
            {
                tongcuclamnghiep.m_header.Where(a => a.reporttable_id == IdBaoCao).ToList().ForEach(a =>
                {
                    if (!tongcuclamnghiep.m_header.Any(b => b.header_id == a.id))
                    {
                        m_component _Component = new m_component();
                        if (a.is_component == true || a.is_no == true)
                        {
                            _Component.componentname = "";
                        }
                        _Component.typevalue = 1;
                        _Component.header_id = a.id;
                        _Component.orderrow = RowId;
                        _Component.reporttable_id = IdBaoCao;
                        _Component.status = true;
                        tongcuclamnghiep.m_component.Add(_Component);
                    }    
                });
                if (tongcuclamnghiep.SaveChanges() < 1)
                {
                    resSubmit = new ResSubmit(false, "Thêm mới thất bại");
                }
                else
                {
                    TableComponent tableComponent = new TableComponent();
                    tableComponent.RowId = RowId;
                    List<HeaderComponent> headers = new List<HeaderComponent>();
                    tongcuclamnghiep.m_component.Where(a=>a.orderrow==RowId && a.reporttable_id==IdBaoCao).ToList().ForEach(b => {
                        headers.Add(new HeaderComponent()
                        {
                            Id = b.header_id.Value,
                            Title = b.componentname,
                            TypeValue = b.typevalue.Value,
                            TextAlign = b.m_header.valuetextalign,
                            TextBold = b.m_header.is_valuebold.Value,
                            TextUpper = b.m_header.is_valueupper.Value
                        });
                    });
                    tableComponent.headers = headers;
                    resSubmit.extend = tableComponent;
                }
            }
            return resSubmit;
        }


        List<TableReport> getByLevel(int level, int IdBaoCao)
        {
            List<TableReport> tableReports = new List<TableReport>();
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            tongcuclamnghiep.m_header.Where(a => a.reporttable_id == IdBaoCao && a.level == level && a.status == true).OrderBy(a => a.order).ToList().ForEach(a =>
            {
                tableReports.Add(new TableReport()
                {
                    colspan = a.colspan.Value,
                    level = a.level.Value,
                    order = a.order.Value,
                    rowspan = a.rowspan.Value,
                    title = a.headername,
                    extend = new
                    {
                        a.id,
                        a.is_bold,
                        a.header_id,
                        a.is_component,
                        a.is_no,
                    }
                });
            });
            if (tongcuclamnghiep.m_header.Any(b =>b.reporttable_id == IdBaoCao && b.status == true && b.level==level+1))
            {
                List<TableReport> tableReportTemp = getByLevel(level + 1, IdBaoCao);
                tableReportTemp.ForEach(b =>
                {
                    tableReports.Add(b);
                });
            }
            return tableReports;
        }


        public List<TableReport> GetTableReportHorizontal(int IdBaoCao)
        {
            List<TableReport> tableReports = getByLevel(1, IdBaoCao);
            return tableReports;
        }



        public List<TreeBaoCao> GetTreeBaoCaoThanhPhan(int IdBaoCao, int IdHeaderCha, bool isFirst)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            List<TreeBaoCao> treeBaoCaos = new List<TreeBaoCao>();
            var _ReportTable = tongcuclamnghiep.m_reporttable.Where(a => a.id == IdBaoCao).FirstOrDefault();
            if (_ReportTable != null)
            {
                if (isFirst)
                {
                    treeBaoCaos.Add(new TreeBaoCao()
                    {
                        key = "_1",
                        title = _ReportTable.title,
                        lazy = tongcuclamnghiep.m_header.Any(a => a.reporttable_id == IdBaoCao),
                        folder = tongcuclamnghiep.m_header.Any(a => a.reporttable_id == IdBaoCao)
                    });
                }
                else
                {
                    treeBaoCaos = tongcuclamnghiep.m_header.Where(a => a.reporttable_id == IdBaoCao && (IdHeaderCha <= 0 ? (a.level == 1) : (a.header_id == IdHeaderCha))).ToList()
                        .Select(a => new TreeBaoCao()
                        {
                            key = string.Format("{0}", a.id),
                            title = a.headername,
                            lazy = tongcuclamnghiep.m_header.Any(b => b.header_id == a.id),
                            folder = tongcuclamnghiep.m_header.Any(b => b.header_id == a.id),
                            dataSource = new
                            {
                                a.colspan,
                                a.createdate,
                                a.headername,
                                a.header_id,
                                a.id,
                                a.is_bold,
                                a.is_upper,
                                a.is_valuebold,
                                a.is_valueupper,
                                a.level,
                                a.order,
                                a.reporttable_id,
                                a.rowspan,
                                a.status,
                                a.valuetextalign,
                                a.is_no,
                                a.is_component
                            }
                        }).ToList();
                }
            }
            return treeBaoCaos;
        }

        public ResSubmit ThemMoiHeader(HeaderAdd headerAdd)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Thêm mới thành công");
            var _HeaderCha = tongcuclamnghiep.m_header.Where(a => a.id == headerAdd.HeaderId).FirstOrDefault();
            var _BaoCao = tongcuclamnghiep.m_reporttable.Where(a => a.id == headerAdd.IdBaoCao).FirstOrDefault();
            if (_BaoCao == null)
            {
                resSubmit = new ResSubmit(false, "Không có báo cáo này");
            }
            if(resSubmit.success && _HeaderCha==null && headerAdd.HeaderId != 0)
            {
                resSubmit = new ResSubmit(false, "Không có tiêu đề cha này");
            }
            if(resSubmit.success && (tongcuclamnghiep.m_header.Any(a=>a.headername==headerAdd.headername && a.reporttable_id==headerAdd.IdBaoCao && (headerAdd.HeaderId==0 ? a.level==1 : (a.header_id==headerAdd.HeaderId)))))
            {
                resSubmit = new ResSubmit(false, "Tên cột đã tồn tại");
            }
            if (resSubmit.success)
            {
                m_header _Header = new m_header();
                _Header.colspan = ((headerAdd.colspan == 0 || headerAdd.colspan == null) ? 1 : headerAdd.colspan);
                _Header.headername = headerAdd.headername;
                _Header.header_id = (headerAdd.HeaderId == 0 ? (int?)null : headerAdd.HeaderId);
                _Header.is_bold = headerAdd.is_bold;
                if (headerAdd.is_no == true && headerAdd.is_component == true)
                {
                    _Header.is_no = true;
                    _Header.is_component = false;
                }
                else
                {
                    _Header.is_no = headerAdd.is_no == true;
                    _Header.is_component = headerAdd.is_component == true;
                }
                if (tongcuclamnghiep.m_header.Any(a => a.header_id == headerAdd.HeaderId))
                {
                    _Header.order = tongcuclamnghiep.m_header.Max(a => a.order.Value);
                }
                else
                {
                    _Header.order = 1;
                }
                _Header.is_upper = headerAdd.is_upper;
                _Header.is_valuebold = headerAdd.is_valuebold;
                _Header.is_valueupper = headerAdd.is_valueupper;
                _Header.level = (_HeaderCha == null ? 1 : _HeaderCha.level + 1);
                _Header.reporttable_id = headerAdd.IdBaoCao;
                _Header.rowspan = ((headerAdd.rowspan == 0 || headerAdd.rowspan == null) ? 1 : headerAdd.rowspan);
                _Header.status = headerAdd.status;
                _Header.valuetextalign = (headerAdd.valuetextalign == "right" ? "right" : (headerAdd.valuetextalign == "center" ? "center" : "left"));
                tongcuclamnghiep.m_header.Add(_Header);
                if (tongcuclamnghiep.SaveChanges() != 1)
                {
                    resSubmit = new ResSubmit(false, "Thêm mới thất bại");
                }
            }
            return resSubmit;
        }

        public ResSubmit CapNhatHeader(HeaderAdd headerEdit)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            var _Header = tongcuclamnghiep.m_header.Where(a => a.id == headerEdit.id && a.reporttable_id == headerEdit.IdBaoCao).FirstOrDefault();
            var _BaoCao = tongcuclamnghiep.m_reporttable.Where(a => a.id == headerEdit.IdBaoCao).FirstOrDefault();
            if (_BaoCao == null)
            {
                resSubmit = new ResSubmit(false, "Không có báo cáo này");
            }
            if(resSubmit.success && _Header == null)
            {
                resSubmit = new ResSubmit(false, "Không có tiêu đề cột này");
            }
            if(resSubmit.success && tongcuclamnghiep.m_header.Any(a=>a.id!=headerEdit.id && a.reporttable_id==headerEdit.IdBaoCao && a.headername == headerEdit.headername))
            {
                resSubmit = new ResSubmit(false, "Tên tiêu đề cột này đã tồn tại trong báo cáo");
            }
            if (resSubmit.success)
            {
                _Header.colspan = ((headerEdit.colspan == 0 || headerEdit.colspan == null) ? 1 : headerEdit.colspan);
                _Header.headername = headerEdit.headername;
                _Header.is_bold = headerEdit.is_bold;
                _Header.is_upper = headerEdit.is_upper;
                _Header.is_valuebold = headerEdit.is_valuebold;
                _Header.is_valueupper = headerEdit.is_valueupper;
                if (headerEdit.is_no==true && headerEdit.is_component == true)
                {
                    _Header.is_no = true;
                    _Header.is_component = false;
                }
                else
                {
                    _Header.is_no = headerEdit.is_no.Value==true;
                    _Header.is_component = headerEdit.is_component.Value == true;
                }
                _Header.rowspan= ((headerEdit.rowspan == 0 || headerEdit.rowspan == null) ? 1 : headerEdit.rowspan);
                _Header.valuetextalign = (headerEdit.valuetextalign == "right" ? "right" : (headerEdit.valuetextalign == "center" ? "center" : "left"));
                if (tongcuclamnghiep.SaveChanges() != 1)
                {
                    resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                }
            }
            return resSubmit;
        }

        public List<TreeBaoCao> GetTreeBaoCao(int IdBaoCao, int IdHeaderCha, bool isFirst)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            List<TreeBaoCao> treeBaoCaos = new List<TreeBaoCao>();
            var _ReportTable = tongcuclamnghiep.m_reporttable.Where(a => a.id == IdBaoCao).FirstOrDefault();
            if (_ReportTable != null)
            {
                if (isFirst)
                {
                    treeBaoCaos.Add(new TreeBaoCao()
                    {
                        key = "_1",
                        title=_ReportTable.title,
                        lazy= tongcuclamnghiep.m_header.Any(a=>a.reporttable_id==IdBaoCao),
                        folder= tongcuclamnghiep.m_header.Any(a => a.reporttable_id == IdBaoCao)
                    });
                }
                else
                {
                    treeBaoCaos = tongcuclamnghiep.m_header.Where(a => a.reporttable_id == IdBaoCao && (IdHeaderCha <= 0 ? (a.level == 1) : (a.header_id == IdHeaderCha))).ToList()
                        .Select(a => new TreeBaoCao()
                        {
                            key = string.Format("{0}", a.id),
                            title=a.headername,
                            lazy= tongcuclamnghiep.m_header.Any(b=>b.header_id==a.id),
                            folder = tongcuclamnghiep.m_header.Any(b => b.header_id == a.id),
                            dataSource =new
                            {
                                a.colspan,
                                a.createdate,
                                a.headername,
                                a.header_id,
                                a.id,
                                a.is_bold,
                                a.is_upper,
                                a.is_valuebold,
                                a.is_valueupper,
                                a.level,
                                a.order,
                                a.reporttable_id,
                                a.rowspan,
                                a.status,
                                a.valuetextalign,
                                a.is_no,
                                a.is_component
                            }
                        }).ToList();
                }
            }
            return treeBaoCaos;
        }

        public ResSubmit CapNhatSapXepBaoCao(List<listBaoCao> listBaoCaos)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            tongcuclamnghiep.m_reporttable.ToList().All(a =>
            {
                if (listBaoCaos.Any(b => b.id == a.id))
                {
                    var sapxep = listBaoCaos.Where(b => b.id == a.id).FirstOrDefault();
                    a.order = sapxep.sapxep;
                }
                else
                {
                    a.order = null;
                }
                return true;
            });
            tongcuclamnghiep.SaveChanges();
            return resSubmit;
        }
    }
}