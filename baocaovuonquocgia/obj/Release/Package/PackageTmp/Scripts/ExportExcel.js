if (!String.prototype.format) {
    String.prototype.format = function () {
        var args;
        args = arguments;
        if (args.length === 1 && args[0] !== null && typeof args[0] === 'object') {
            args = args[0];
        }
        return this.replace(/{([^}]*)}/g, function (match, key) {
            return (typeof args[key] !== "undefined" ? args[key] : match);
        });
    };
}
var Base64 = (function () {

    // private property
    var keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

    // private method for UTF-8 encoding
    function utf8Encode(string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";
        for (var n = 0; n < string.length; n++) {
            var c = string.charCodeAt(n);
            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }
        }
        return utftext;
    }

    // public method for encoding
    return {
        //This was the original line, which tries to use Firefox's built in Base64 encoder, but this kept throwing exceptions....
        // encode : (typeof btoa == 'function') ? function(input) { return btoa(input); } : function (input) {


        encode: function (input) {
            var output = "";
            var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
            var i = 0;
            input = utf8Encode(input);
            while (i < input.length) {
                chr1 = input.charCodeAt(i++);
                chr2 = input.charCodeAt(i++);
                chr3 = input.charCodeAt(i++);
                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                enc4 = chr3 & 63;
                if (isNaN(chr2)) {
                    enc3 = enc4 = 64;
                } else if (isNaN(chr3)) {
                    enc4 = 64;
                }
                output = output +
                    keyStr.charAt(enc1) + keyStr.charAt(enc2) +
                    keyStr.charAt(enc3) + keyStr.charAt(enc4);
            }
            return output;
        }
    };
})();

function findFistChild($headers, $head) {
    var $child;
    if ($headers.filter(function ($item) {
        return $item.HeaderId == $head.Id;
    }).length > 0) {
        var Childs = $headers.filter(function ($item) {
            return $item.HeaderId == $head.Id;
        });
        for (var i = 0; i < Childs.length; i++) {
            if (i == 0) {
                var $childTemp = Childs[i];
                if ($headers.filter(function ($item) {
                    return $item.HeaderId == $childTemp.Id;
                }).length > 0) {
                    $child = findFistChild($headers, $childTemp);
                }
                else {
                    $child = $childTemp;
                }
            }
        }
    }
    return $child;
}


function createHeaderExcel($level, $headers, $headerNotChild) {
    var arrRow = [
        '   <Row>'
    ];
    var STT = 1;
    var $filter = $headers.filter(function ($head) {
        return $head.Level == $level;
    });
    for (var i = 1; i <= $filter.length; i++) {
        var $head = $filter[i-1];
        var IndexColumn = i;
        if ($level > 1) {
            var $child = findFistChild($headers, $head);
            if ($child == undefined) {
                $child = $head;
            }
            var iTemp = -1;
            for (var j = 0; j < $headerNotChild.length; j++) {
                if ($headerNotChild[j].Id == $child.Id) {
                    iTemp = j+1;
                    break;
                }
            }
            if (iTemp > -1) {
                IndexColumn = iTemp;
            }
        }

        var TieuDe = $head.TieuDe;
        var ColSpan = $head.ColSpan;
        var RowSpan = $head.RowSpan;
        if ($level > 1) {
            if (ColSpan > 1) {
                ColSpan--;
                arrRow.push('    <Cell ss:Index="{IndexColumn}" ss:MergeAcross="{ColSpan}" ss:StyleID="s78"><Data ss:Type="String">{TieuDe}</Data></Cell>'.format({ ColSpan: ColSpan, TieuDe: TieuDe, IndexColumn: IndexColumn }));
            }
            else {
                if (RowSpan > 1) {
                    RowSpan--;
                    arrRow.push('    <Cell ss:Index="{IndexColumn}" ss:MergeDown="{RowSpan}" ss:StyleID="s78"><Data ss:Type="String">{TieuDe}</Data></Cell>'.format({ RowSpan: RowSpan, TieuDe: TieuDe, IndexColumn: IndexColumn }));
                }
                else {
                    arrRow.push('    <Cell ss:Index="{IndexColumn}" ss:StyleID="s78"><Data ss:Type="String">{TieuDe}</Data></Cell>'.format({ TieuDe: TieuDe, IndexColumn: IndexColumn }));
                }
            }
        }
        else {
            if (ColSpan > 1) {
                ColSpan--;
                arrRow.push('    <Cell ss:MergeAcross="{ColSpan}" ss:StyleID="s78"><Data ss:Type="String">{TieuDe}</Data></Cell>'.format({ ColSpan: ColSpan, TieuDe: TieuDe, IndexColumn: IndexColumn }));
            }
            else {
                if (RowSpan > 1) {
                    RowSpan--;
                    arrRow.push('    <Cell ss:MergeDown="{RowSpan}" ss:StyleID="s78"><Data ss:Type="String">{TieuDe}</Data></Cell>'.format({ RowSpan: RowSpan, TieuDe: TieuDe, IndexColumn: IndexColumn }));
                }
                else {
                    arrRow.push('    <Cell ss:StyleID="s78"><Data ss:Type="String">{TieuDe}</Data></Cell>'.format({ TieuDe: TieuDe, IndexColumn: IndexColumn }));
                }
            }
        }
        
    }
    arrRow.push('   </Row>');
    
    if ($headers.filter(function ($head) {
        return $head.Level == ($level+1);
    }).length > 0) {
        var arrTemp = createHeaderExcel(($level + 1), $headers, $headerNotChild);
        if (arrTemp.length > 0) {
            arrTemp.forEach(function ($row) {
                arrRow.push($row)
            });
        }
    }
    return arrRow;
}



function ExportTableTemplateExcel($table) {
    
    
    var arrColGroup = [];
    $table.DanhSachTieuDeCol.forEach(function ($head) {
        var $head2 = $table.DanhSachTieuDe.filter(function ($item) {
            return $item.Id == $head.Id;
        })[0];
        
        if ($head2.IsNo == true) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="60"/>');
        }
        if ($head2.IsComponent == true) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="144"/>');
        }
        if (!($head2.IsNo == true || $head2.IsComponent == true)) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="120"/>');
        }
    });
    var strColGroup = arrColGroup.join("");
    var ExpandedColumnCount = 1;
    var ExpandedRowCount = 10;
    ExpandedColumnCount = $table.DanhSachTieuDeCol.length;
    var strTieuDe = $table.TieuDe;
    ExpandedRowCount += Math.max.apply(Math, $table.DanhSachTieuDe.map(function (o) { return o.Level; }));
    var strRowHeader = createHeaderExcel(1, $table.DanhSachTieuDe, $table.DanhSachTieuDeCol).join("");
    console.log(strRowHeader)
    var DanhSachDong = $table.DanhSachDong;
    var arrDong = [];
    if (DanhSachDong.length > 0) {
        DanhSachDong.forEach(function ($row) {
            ExpandedRowCount++;
            var arrCol = [
                '<Row>',
            ];
            var styleId = '';
            $row.DanhSachCot.forEach(function ($col) {
                switch ($col.TieuDeCot.TextAlignValue) {
                    case 'right':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's101';
                        }
                        else {
                            styleId = 's100';
                        }
                        break;
                    case 'left':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's103';
                        }
                        else {
                            styleId = 's102';
                        }
                        break;
                    case 'center':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's105';
                        }
                        else {
                            styleId = 's104';
                        }
                        break;
                }
                var DuLieuText = $col.DuLieuText;
                if ($col.TieuDeCot.TextBoldValue == true && ($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                    DuLieuText = String(DuLieuText).toUpperCase();
                }
                if (($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                    arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="String">{DuLieuText}</Data></Cell>'.format({ StyleID: styleId, DuLieuText: DuLieuText }));
                }
                else {
                    if ($col.TypeValue == 1) {
                        arrCol.push('<Cell ss:StyleID="s82"><Data ss:Type="String"></Data></Cell>');
                    }
                    else {
                        arrCol.push('<Cell ss:StyleID="s82"><Data ss:Type="Number"></Data></Cell>');
                    }
                }
            });
            arrCol.push('</Row>');
            arrDong.push(arrCol.join(""))
        });
    }
    var strArrDong = arrDong.join("");

    var arrTemplate = [
        '<?xml version="1.0"?>',
        '<?mso-application progid="Excel.Sheet"?>',
        '<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"',
        ' xmlns:o="urn:schemas-microsoft-com:office:office"',
        ' xmlns:x="urn:schemas-microsoft-com:office:excel"',
        ' xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"',
        ' xmlns:html="http://www.w3.org/TR/REC-html40">',
        ' <DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">',
        '  <Author>Administrator</Author>',
        '  <LastAuthor>Administrator</LastAuthor>',
        '  <Created>2018-12-10T04:17:44Z</Created>',
        '  <LastSaved>2018-12-10T04:48:43Z</LastSaved>',
        '  <Version>16.00</Version>',
        ' </DocumentProperties>',
        ' <OfficeDocumentSettings xmlns="urn:schemas-microsoft-com:office:office">',
        '  <AllowPNG/>',
        ' </OfficeDocumentSettings>',
        ' <ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">',
        '  <WindowHeight>7545</WindowHeight>',
        '  <WindowWidth>20490</WindowWidth>',
        '  <WindowTopX>32767</WindowTopX>',
        '  <WindowTopY>32767</WindowTopY>',
        '  <ProtectStructure>False</ProtectStructure>',
        '  <ProtectWindows>False</ProtectWindows>',
        ' </ExcelWorkbook>',
        ' <Styles>',
        '  <Style ss:ID="Default" ss:Name="Normal">',
        '   <Alignment ss:Vertical="Bottom"/>',
        '   <Borders/>',
        '   <Font ss:FontName="Calibri" x:Family="Swiss" ss:Size="11" ss:Color="#000000"/>',
        '   <Interior/>',
        '   <NumberFormat/>',
        '   <Protection/>',
        '  </Style>',
        '  <Style ss:ID="s66">',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s77">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center"/>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s78">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s79">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s81">',
        '   <Alignment ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s82">',
        '   <Alignment ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s83">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        /*custom*/
        '  <Style ss:ID="s100">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s101">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s102">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s103">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s104">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s105">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        ' </Styles>',
        /**/
        ' <Worksheet ss:Name="Sheet1">',
        '  <Table ss:ExpandedColumnCount="{ExpandedColumnCount}" ss:ExpandedRowCount="{ExpandedRowCount}" x:FullColumns="1"',
        '   x:FullRows="1" ss:StyleID="s66" ss:DefaultRowHeight="15">',
        '   {strColGroup}',
        '   <Row ss:AutoFitHeight="0" ss:Height="25.5">',
        '    <Cell ss:MergeAcross="{MergeAcrossTieuDe}" ss:StyleID="s77"><Data ss:Type="String">{strTieuDe}</Data></Cell>',
        '   </Row>',
        '   {strRowHeader}',
        '   {strArrDong}',
        '  </Table>',
        '  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">',
        '   <PageSetup>',
        '    <Header x:Margin="0.3"/>',
        '    <Footer x:Margin="0.3"/>',
        '    <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>',
        '   </PageSetup>',
        '   <Print>',
        '    <ValidPrinterInfo/>',
        '    <HorizontalResolution>600</HorizontalResolution>',
        '    <VerticalResolution>600</VerticalResolution>',
        '   </Print>',
        '   <Selected/>',
        '   <Panes>',
        '    <Pane>',
        '     <Number>1</Number>',
        '     <ActiveRow>1</ActiveRow>',
        '     <ActiveCol>1</ActiveCol>',
        '    </Pane>',
        '   </Panes>',
        '   <ProtectObjects>False</ProtectObjects>',
        '   <ProtectScenarios>False</ProtectScenarios>',
        '  </WorksheetOptions>',
        ' </Worksheet>',
        '</Workbook>'
    ];
    
    var strArrTemplate = arrTemplate.join("").format({ ExpandedColumnCount: ExpandedColumnCount, ExpandedRowCount: ExpandedRowCount, strColGroup: strColGroup, strTieuDe: strTieuDe, strRowHeader: strRowHeader, strArrDong: strArrDong, MergeAcrossTieuDe: ExpandedColumnCount - 1 });
    var uri = 'data:application/vnd.ms-excel;base64,' + Base64.encode(strArrTemplate);

    var downloadLink = document.createElement("a");
    downloadLink.href = uri;
    downloadLink.download = (new Date()).toString().replace(/\S+\s(\S+)\s(\d+)\s(\d+)\s.*/, '$2_$1_$3') + '.xls';

    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
}

function ExportTableReportEmpty($IdBaoCao) {
    JqueryPostData('/Category/GetTableEmpty', {}, function (res) {
        if (res.length > 0) {
            CreateAllSheet(res, $IdBaoCao);
        }
    })
}

function CreateAllSheet($DanhSachBaoCao,$IdBaoCao) {
    var AllSheet = '';
    var arrAllSheet = [];
    var CheckAll = true;
    if ($IdBaoCao != undefined) {
        CheckAll = !($DanhSachBaoCao.filter(function ($item) {
            return $item.Id == $IdBaoCao
        }).length == 1);
    }
    if (CheckAll == true) {
        arrAllSheet.push(CreateSheetThongTin())
    }
    $DanhSachBaoCao.forEach(function ($table) {
        if (CheckAll == false) {
            if ($table.Id == $IdBaoCao) {
                arrAllSheet.push(CreateSheetByTable($table))
            }
        }
        else {
            arrAllSheet.push(CreateSheetByTable($table))
        }
    });


    var arrTemplate = [
        '<?xml version="1.0"?>',
        '<?mso-application progid="Excel.Sheet"?>',
        '<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"',
        ' xmlns:o="urn:schemas-microsoft-com:office:office"',
        ' xmlns:x="urn:schemas-microsoft-com:office:excel"',
        ' xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"',
        ' xmlns:html="http://www.w3.org/TR/REC-html40">',
        ' <DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">',
        '  <Author>Administrator</Author>',
        '  <LastAuthor>Administrator</LastAuthor>',
        '  <Created>2018-12-10T04:17:44Z</Created>',
        '  <LastSaved>2018-12-10T04:48:43Z</LastSaved>',
        '  <Version>16.00</Version>',
        ' </DocumentProperties>',
        ' <OfficeDocumentSettings xmlns="urn:schemas-microsoft-com:office:office">',
        '  <AllowPNG/>',
        ' </OfficeDocumentSettings>',
        ' <ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">',
        '  <WindowHeight>7545</WindowHeight>',
        '  <WindowWidth>20490</WindowWidth>',
        '  <WindowTopX>32767</WindowTopX>',
        '  <WindowTopY>32767</WindowTopY>',
        '  <ProtectStructure>False</ProtectStructure>',
        '  <ProtectWindows>False</ProtectWindows>',
        ' </ExcelWorkbook>',
        ' <Styles>',
        '  <Style ss:ID="Default" ss:Name="Normal">',
        '   <Alignment ss:Vertical="Bottom"/>',
        '   <Borders/>',
        '   <Font ss:FontName="Calibri" x:Family="Swiss" ss:Size="11" ss:Color="#000000"/>',
        '   <Interior/>',
        '   <NumberFormat/>',
        '   <Protection/>',
        '  </Style>',
        '  <Style ss:ID="s66">',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s77">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center"/>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s78">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s79">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s81">',
        '   <Alignment ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s82">',
        '   <Alignment ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s83">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        /*thongtin*/
        '  <Style ss:ID="m2130864667504">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864663968">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864663988">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864666336">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864666356">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s67">',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s71">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s73">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s74">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000" ss:Bold="1"/>',        '  </Style>',        '  <Style ss:ID="s85">',        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="12"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s89">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s97">',        '   <Alignment ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',
        /*custom*/
        '  <Style ss:ID="s100">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s101">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s102">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s103">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s104">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s105">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        ' </Styles>',
        ' {AllSheet}',
        '</Workbook>'
    ];

    var strArrTemplate = arrTemplate.join("").format({ AllSheet: arrAllSheet.join(""), });
    var uri = 'data:application/vnd.ms-excel;base64,' + Base64.encode(strArrTemplate);

    var downloadLink = document.createElement("a");
    downloadLink.href = uri;
    downloadLink.download = (new Date()).toString().replace(/\S+\s(\S+)\s(\d+)\s(\d+)\s.*/, '$2_$1_$3') + '.xls';

    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
}


function CreateSheetByTable($table) {
    var strSheet = '';
    var arrColGroup = [];
    var SheetName = 'Sheet{Id}'.format({ Id: $table.Id });
    $table.DanhSachTieuDeCol.forEach(function ($head) {
        var $head2 = $table.DanhSachTieuDe.filter(function ($item) {
            return $item.Id == $head.Id;
        })[0];

        if ($head2.IsNo == true) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="60"/>');
        }
        if ($head2.IsComponent == true) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="144"/>');
        }
        if (!($head2.IsNo == true || $head2.IsComponent == true)) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="120"/>');
        }
    });
    var strColGroup = arrColGroup.join("");
    var ExpandedColumnCount = 1;
    var ExpandedRowCount = 10;
    ExpandedColumnCount = $table.DanhSachTieuDeCol.length;
    var strTieuDe = $table.TieuDe;
    ExpandedRowCount += Math.max.apply(Math, $table.DanhSachTieuDe.map(function (o) { return o.Level; }));
    var strRowHeader = createHeaderExcel(1, $table.DanhSachTieuDe, $table.DanhSachTieuDeCol).join("");
    console.log(strRowHeader)
    var DanhSachDong = $table.DanhSachDong;
    var arrDong = [];
    if (DanhSachDong.length > 0) {
        DanhSachDong.forEach(function ($row) {
            ExpandedRowCount++;
            var arrCol = [
                '<Row>',
            ];
            var styleId = '';
            $row.DanhSachCot.forEach(function ($col) {
                switch ($col.TieuDeCot.TextAlignValue) {
                    case 'right':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's101';
                        }
                        else {
                            styleId = 's100';
                        }
                        break;
                    case 'left':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's103';
                        }
                        else {
                            styleId = 's102';
                        }
                        break;
                    case 'center':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's105';
                        }
                        else {
                            styleId = 's104';
                        }
                        break;
                }
                var DuLieuText = $col.DuLieuText;
                if (DuLieuText == null) {
                    DuLieuText = '';
                }
                if ($col.TieuDeCot.TextBoldValue == true && ($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                    DuLieuText = String(DuLieuText).toUpperCase();
                }
                if (($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                    arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="String">{DuLieuText}</Data></Cell>'.format({ StyleID: styleId, DuLieuText: DuLieuText }));
                }
                else {
                    if ($col.TypeValue == 1) {
                        arrCol.push('<Cell ss:StyleID="s82"><Data ss:Type="String"></Data></Cell>');
                    }
                    else {
                        arrCol.push('<Cell ss:StyleID="s82"><Data ss:Type="Number"></Data></Cell>');
                    }
                }
            });
            arrCol.push('</Row>');
            arrDong.push(arrCol.join(""))
        });
    }
    var strArrDong = arrDong.join("");

    strSheet = [
        ' <Worksheet ss:Name="{SheetName}">',
        '  <Table ss:ExpandedColumnCount="{ExpandedColumnCount}" ss:ExpandedRowCount="{ExpandedRowCount}" x:FullColumns="1"',
        '   x:FullRows="1" ss:StyleID="s66" ss:DefaultRowHeight="15">',
        '   {strColGroup}',
        '   <Row ss:AutoFitHeight="0" ss:Height="25.5">',
        '    <Cell ss:MergeAcross="{MergeAcrossTieuDe}" ss:StyleID="s77"><Data ss:Type="String">{strTieuDe}</Data></Cell>',
        '   </Row>',
        '   {strRowHeader}',
        '   {strArrDong}',
        '  </Table>',
        '  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">',
        '   <PageSetup>',
        '    <Header x:Margin="0.3"/>',
        '    <Footer x:Margin="0.3"/>',
        '    <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>',
        '   </PageSetup>',
        '   <Print>',
        '    <ValidPrinterInfo/>',
        '    <HorizontalResolution>600</HorizontalResolution>',
        '    <VerticalResolution>600</VerticalResolution>',
        '   </Print>',
        '   <Selected/>',
        '   <Panes>',
        '    <Pane>',
        '     <Number>1</Number>',
        '     <ActiveRow>1</ActiveRow>',
        '     <ActiveCol>1</ActiveCol>',
        '    </Pane>',
        '   </Panes>',
        '   <ProtectObjects>False</ProtectObjects>',
        '   <ProtectScenarios>False</ProtectScenarios>',
        '  </WorksheetOptions>',
        ' </Worksheet>',
    ].join("").format({ ExpandedColumnCount: ExpandedColumnCount, ExpandedRowCount: ExpandedRowCount, strColGroup: strColGroup, strTieuDe: strTieuDe, strRowHeader: strRowHeader, strArrDong: strArrDong, MergeAcrossTieuDe: ExpandedColumnCount - 1, SheetName: SheetName });
    return strSheet;
}
function CreateSheetThongTin() {
    var XTemplate = [        ' <Worksheet ss:Name="ThongTinChung">',        '  <Table ss:ExpandedColumnCount="4" ss:ExpandedRowCount="13" x:FullColumns="1"',        '   x:FullRows="1" ss:StyleID="s67" ss:DefaultRowHeight="15">',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="126"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="141"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="136.5"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="130.5"/>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s74"><Data ss:Type="String">THÔNG TIN CHUNG</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s71"><Data ss:Type="String">Tên, địa chỉ (email, điện thoại liên lạc) người thực hiện báo cáo:</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s73"><Data ss:Type="String">Ngày báo cáo:</Data></Cell>',        '   </Row>',        '   <Row ss:AutoFitHeight="0" ss:Height="21.75">',        '    <Cell ss:StyleID="s85"><Data ss:Type="String">Tên khu rừng</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="s85"><Data ss:Type="String"></Data></Cell>',        '    <Cell ss:StyleID="s85"><Data ss:Type="String">Ghi chú</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Loại/ hạng</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864666336"><Data ss:Type="String"></Data></Cell>',        '    <Cell ss:StyleID="s78"/>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Vị trí</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864666356"><Data ss:Type="String"></Data></Cell>',        '    <Cell ss:StyleID="s78"/>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Ngày thành lập:</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864663968"><Data ss:Type="String"></Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String"></Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Cơ quan, đơn vị quản lý:</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864663988"><Data ss:Type="String"></Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String"></Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Diện tích khu rừng:</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Tổng :</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Phân khu :</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String"></Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Tổng số cán bộ:</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Trong biên chế :</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Hợp đồng :</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String"></Data></Cell>',        '   </Row>',        '   <Row ss:Height="30">',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Ngân sách nhà nước</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Bảo vệ rừng; khôi phục rừng :</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Xây dựng cơ sở hạ tầng, trang thiết bị :</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">Đầu tư khác</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Nhiệm vụ chính trong năm</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String"></Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String"></Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String"></Data></Cell>',        '   </Row>',        '   <Row ss:Height="30">',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Các chương trình, dự án, đè ắn khác liên quan</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864667504"><Data ss:Type="String"></Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String"></Data></Cell>',        '   </Row>',        '  </Table>',        '  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">',        '   <PageSetup>',        '    <Header x:Margin="0.3"/>',        '    <Footer x:Margin="0.3"/>',        '    <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>',        '   </PageSetup>',        '   <Print>',        '    <ValidPrinterInfo/>',        '    <HorizontalResolution>600</HorizontalResolution>',        '    <VerticalResolution>600</VerticalResolution>',        '   </Print>',        '   <Selected/>',        '   <Panes>',        '    <Pane>',        '     <Number>3</Number>',        '     <ActiveRow>8</ActiveRow>',        '     <ActiveCol>3</ActiveCol>',        '    </Pane>',        '   </Panes>',        '   <ProtectObjects>False</ProtectObjects>',        '   <ProtectScenarios>False</ProtectScenarios>',        '  </WorksheetOptions>',        ' </Worksheet>',
    ];
    return XTemplate.join("");
}

function ExportTableReportData(isAllUnit, isOneTable, $table, $allData, $dataDonVi) {
    var AllSheet = '';
    var arrAllSheet = [];
    if (isAllUnit == false) {
        arrAllSheet.push(CreateSheetDataThongTin($dataDonVi))
    }
    if (isOneTable) {
        arrAllSheet.push(CreateSheetByTableDataGroup($table, isAllUnit))
    }
    else {
        $allData.forEach(function ($item) {
            arrAllSheet.push(CreateSheetByTableDataGroup($item, isAllUnit))
        });
    }
    

    var arrTemplate = [
        '<?xml version="1.0"?>',
        '<?mso-application progid="Excel.Sheet"?>',
        '<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"',
        ' xmlns:o="urn:schemas-microsoft-com:office:office"',
        ' xmlns:x="urn:schemas-microsoft-com:office:excel"',
        ' xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"',
        ' xmlns:html="http://www.w3.org/TR/REC-html40">',
        ' <DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">',
        '  <Author>Administrator</Author>',
        '  <LastAuthor>Administrator</LastAuthor>',
        '  <Created>2018-12-10T04:17:44Z</Created>',
        '  <LastSaved>2018-12-10T04:48:43Z</LastSaved>',
        '  <Version>16.00</Version>',
        ' </DocumentProperties>',
        ' <OfficeDocumentSettings xmlns="urn:schemas-microsoft-com:office:office">',
        '  <AllowPNG/>',
        ' </OfficeDocumentSettings>',
        ' <ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">',
        '  <WindowHeight>7545</WindowHeight>',
        '  <WindowWidth>20490</WindowWidth>',
        '  <WindowTopX>32767</WindowTopX>',
        '  <WindowTopY>32767</WindowTopY>',
        '  <ProtectStructure>False</ProtectStructure>',
        '  <ProtectWindows>False</ProtectWindows>',
        ' </ExcelWorkbook>',
        ' <Styles>',
        '  <Style ss:ID="Default" ss:Name="Normal">',
        '   <Alignment ss:Vertical="Bottom"/>',
        '   <Borders/>',
        '   <Font ss:FontName="Calibri" x:Family="Swiss" ss:Size="11" ss:Color="#000000"/>',
        '   <Interior/>',
        '   <NumberFormat/>',
        '   <Protection/>',
        '  </Style>',
        '  <Style ss:ID="s66">',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s77">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center"/>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s78">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s79">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s81">',
        '   <Alignment ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s82">',
        '   <Alignment ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s83">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        /*thongtin*/
        '  <Style ss:ID="m2130864667504">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864663968">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864663988">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864666336">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864666356">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s67">',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s71">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s73">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s74">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000" ss:Bold="1"/>',        '  </Style>',        '  <Style ss:ID="s85">',        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="12"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s89">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s97">',        '   <Alignment ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',
        /*custom*/
        '  <Style ss:ID="s100">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s101">',
        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s102">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s103">',
        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        '  <Style ss:ID="s104">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000"/>',
        '  </Style>',
        '  <Style ss:ID="s105">',
        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',
        '   <Borders>',
        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',
        '   </Borders>',
        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',
        '    ss:Color="#000000" ss:Bold="1"/>',
        '  </Style>',
        ' </Styles>',
        ' {AllSheet}',
        '</Workbook>'
    ];

    var strArrTemplate = arrTemplate.join("").format({ AllSheet: arrAllSheet.join(""), });
    var uri = 'data:application/vnd.ms-excel;base64,' + Base64.encode(strArrTemplate);

    var downloadLink = document.createElement("a");
    downloadLink.href = uri;
    downloadLink.download = (new Date()).toString().replace(/\S+\s(\S+)\s(\d+)\s(\d+)\s.*/, '$2_$1_$3') + '.xls';

    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
}

function CreateSheetByTableDataGroup($table,isAll) {
    var strSheet = '';
    var arrColGroup = [];
    var SheetName = 'Sheet{Id}'.format({ Id: $table.Id });
    $table.DanhSachTieuDeCol.forEach(function ($head) {
        var $head2 = $table.DanhSachTieuDe.filter(function ($item) {
            return $item.Id == $head.Id;
        })[0];

        if ($head2.IsNo == true) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="60"/>');
        }
        if ($head2.IsComponent == true) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="144"/>');
        }
        if (!($head2.IsNo == true || $head2.IsComponent == true)) {
            arrColGroup.push('<Column ss:StyleID="s66" ss:AutoFitWidth="0" ss:Width="120"/>');
        }
    });
    var strColGroup = arrColGroup.join("");
    var ExpandedColumnCount = 1;
    var ExpandedRowCount = 10;
    ExpandedColumnCount = $table.DanhSachTieuDeCol.length;
    var strTieuDe = $table.TieuDe;
    ExpandedRowCount += Math.max.apply(Math, $table.DanhSachTieuDe.map(function (o) { return o.Level; }));
    var strRowHeader = createHeaderExcel(1, $table.DanhSachTieuDe, $table.DanhSachTieuDeCol).join("");
    
    var DanhSachDong = $table.DanhSachDong;
    var arrDong = [];
    if (DanhSachDong.length > 0) {
        DanhSachDong.forEach(function ($row) {
            ExpandedRowCount++;
            var arrCol = [
                '<Row>',
            ];
            var styleId = 's102';
            $row.DanhSachCot.forEach(function ($col) {
                switch ($col.TieuDeCot.TextAlignValue) {
                    case 'right':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's101';
                        }
                        else {
                            styleId = 's100';
                        }
                        break;
                    case 'left':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's103';
                        }
                        else {
                            styleId = 's102';
                        }
                        break;
                    case 'center':
                        if ($col.TieuDeCot.TextBoldValue == true) {
                            styleId = 's105';
                        }
                        else {
                            styleId = 's104';
                        }
                        break;
                }
                //var DuLieuText = $col.DuLieuText;
                //if (DuLieuText == null) {
                //    DuLieuText = '';
                //}
                //if ($col.TieuDeCot.TextBoldValue == true && ($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                //    DuLieuText = String(DuLieuText).toUpperCase();
                //}
                //if (($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                //    arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="String">{DuLieuText}</Data></Cell>'.format({ StyleID: styleId, DuLieuText: DuLieuText }));
                //}
                //else {
                //    if ($col.TypeValue == 1) {
                //        arrCol.push('<Cell ss:StyleID="s82"><Data ss:Type="String"></Data></Cell>');
                //    }
                //    else {
                //        arrCol.push('<Cell ss:StyleID="s82"><Data ss:Type="Number"></Data></Cell>');
                //    }
                //}
                if ($table.Loai == 0) {
                    if (($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                        var DuLieuText = $col.DuLieuText
                        if ($col.TieuDeCot.TextBoldValue == true && ($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                            DuLieuText = String(DuLieuText).toUpperCase();
                        }
                        arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="String">{DuLieuText}</Data></Cell>'.format({ StyleID: styleId, DuLieuText: DuLieuText }));
                    }
                    else {
                        if ($col.TypeValue == 1) {
                            if (isAll == true) {
                                var strDulieu = '';
                                var $DuLieuTextTong = $col.DuLieuTextTong;
                                if ($DuLieuTextTong != null && $DuLieuTextTong.length > 0) {
                                    $DuLieuTextTong.forEach(function ($UnitReport) {
                                        strDulieu += '- ' + $UnitReport.UnitName + ': ' + $UnitReport.Data + '\n';
                                    });
                                }
                                if ($col.TieuDeCot.TextBoldValue == true && ($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                                    strDulieu = String(strDulieu).toUpperCase();
                                }
                                arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="String">{strDulieu}</Data></Cell>'.format({ strDulieu: strDulieu, StyleID: styleId }));
                            }
                            else {
                                var DuLieuText = ($col.DuLieuText == null ? '' : $col.DuLieuText)
                                if ($col.TieuDeCot.TextBoldValue == true && ($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                                    DuLieuText = String(DuLieuText).toUpperCase();
                                }
                                arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="String">{strDulieu}</Data></Cell>'.format({ strDulieu: DuLieuText, StyleID: styleId }));
                            }
                        }
                        else {
                            arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="Number">{strDulieu}</Data></Cell>'.format({ strDulieu: $col.TongNumber, StyleID: styleId }));
                        }
                    }
                }
                else {
                    if (isAll == true) {
                        var strDulieu = '';
                        var $DuLieuTextTong = $col.DuLieuTextTong;
                        if ($DuLieuTextTong != null && $DuLieuTextTong.length > 0) {
                            $DuLieuTextTong.forEach(function ($UnitReport) {
                                strDulieu += '- ' + $UnitReport.UnitName + ': ' + $UnitReport.Data + '\n';
                            });
                        }
                        if ($col.TieuDeCot.TextBoldValue == true && ($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                            strDulieu = String(strDulieu).toUpperCase();
                        }
                        arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="String">{strDulieu}</Data></Cell>'.format({ strDulieu: strDulieu, StyleID: styleId  }));
                    }
                    else {
                        var DuLieuText = ($col.DuLieuText == null ? '' : $col.DuLieuText)
                        if ($col.TieuDeCot.TextBoldValue == true && ($col.TieuDeCot.IsNo == true || $col.TieuDeCot.IsComponent == true)) {
                            DuLieuText = String(DuLieuText).toUpperCase();
                        }
                        arrCol.push('<Cell ss:StyleID="{StyleID}"><Data ss:Type="String">{strDulieu}</Data></Cell>'.format({ strDulieu: DuLieuText, StyleID: styleId  }));
                    }
                }
                
            });
            arrCol.push('</Row>');
            arrDong.push(arrCol.join(""))
        });
    }
    var strArrDong = arrDong.join("");

    strSheet = [
        ' <Worksheet ss:Name="{SheetName}">',
        '  <Table ss:ExpandedColumnCount="{ExpandedColumnCount}" ss:ExpandedRowCount="{ExpandedRowCount}" x:FullColumns="1"',
        '   x:FullRows="1" ss:StyleID="s66" ss:DefaultRowHeight="15">',
        '   {strColGroup}',
        '   <Row ss:AutoFitHeight="0" ss:Height="25.5">',
        '    <Cell ss:MergeAcross="{MergeAcrossTieuDe}" ss:StyleID="s77"><Data ss:Type="String">{strTieuDe}</Data></Cell>',
        '   </Row>',
        '   {strRowHeader}',
        '   {strArrDong}',
        '  </Table>',
        '  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">',
        '   <PageSetup>',
        '    <Header x:Margin="0.3"/>',
        '    <Footer x:Margin="0.3"/>',
        '    <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>',
        '   </PageSetup>',
        '   <Print>',
        '    <ValidPrinterInfo/>',
        '    <HorizontalResolution>600</HorizontalResolution>',
        '    <VerticalResolution>600</VerticalResolution>',
        '   </Print>',
        '   <Selected/>',
        '   <Panes>',
        '    <Pane>',
        '     <Number>1</Number>',
        '     <ActiveRow>1</ActiveRow>',
        '     <ActiveCol>1</ActiveCol>',
        '    </Pane>',
        '   </Panes>',
        '   <ProtectObjects>False</ProtectObjects>',
        '   <ProtectScenarios>False</ProtectScenarios>',
        '  </WorksheetOptions>',
        ' </Worksheet>',
    ].join("").format({ ExpandedColumnCount: ExpandedColumnCount, ExpandedRowCount: ExpandedRowCount, strColGroup: strColGroup, strTieuDe: strTieuDe, strRowHeader: strRowHeader, strArrDong: strArrDong, MergeAcrossTieuDe: ExpandedColumnCount - 1, SheetName: SheetName });
    return strSheet;
}

function CreateSheetDataThongTin($data) {
    var XTemplate = [        ' <Worksheet ss:Name="ThongTinChung">',        '  <Table ss:ExpandedColumnCount="4" ss:ExpandedRowCount="13" x:FullColumns="1"',        '   x:FullRows="1" ss:StyleID="s67" ss:DefaultRowHeight="15">',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="126"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="141"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="136.5"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="130.5"/>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s74"><Data ss:Type="String">THÔNG TIN CHUNG</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s71"><Data ss:Type="String">Tên, địa chỉ (email, điện thoại liên lạc) người thực hiện báo cáo: {tendiachi}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s73"><Data ss:Type="String">Ngày báo cáo:</Data></Cell>',        '   </Row>',        '   <Row ss:AutoFitHeight="0" ss:Height="21.75">',        '    <Cell ss:StyleID="s85"><Data ss:Type="String">Tên khu rừng</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="s85"><Data ss:Type="String">{unitname}</Data></Cell>',        '    <Cell ss:StyleID="s85"><Data ss:Type="String">Ghi chú</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Loại/ hạng</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864666336"><Data ss:Type="String">{loaihang}</Data></Cell>',        '    <Cell ss:StyleID="s78"/>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Vị trí</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864666356"><Data ss:Type="String">{address}</Data></Cell>',        '    <Cell ss:StyleID="s78"/>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Ngày thành lập:</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864663968"><Data ss:Type="String">{ngaythanhlap}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{ngaythanhlapghichu}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Cơ quan, đơn vị quản lý:</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864663988"><Data ss:Type="String">{cacdonviql}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{cacdonviqlghichu}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Diện tích khu rừng:</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Tổng : {dientich1}</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Phân khu : {dientich2}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{dientichghichu}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Tổng số cán bộ:</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Trong biên chế : {tongcanbo1}</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Hợp đồng : {tongcanbo2}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{tongcanboghichu}</Data></Cell>',        '   </Row>',        '   <Row ss:Height="30">',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Ngân sách nhà nước</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Bảo vệ rừng; khôi phục rừng : {ngansachnhannuoc1}</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Xây dựng cơ sở hạ tầng, trang thiết bị : {ngansachnhanuoc2}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">Đầu tư khác {dautukhac}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Nhiệm vụ chính trong năm</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">{nhiemvuchinh1}</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">{nhiemvuchinh2}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{nhiemvuchinhghichu}</Data></Cell>',        '   </Row>',        '   <Row ss:Height="30">',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Các chương trình, dự án, đè ắn khác liên quan</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864667504"><Data ss:Type="String">{cacchuongtrinhlq}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{cacchuongtrinhlqghichu}</Data></Cell>',        '   </Row>',        '  </Table>',        '  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">',        '   <PageSetup>',        '    <Header x:Margin="0.3"/>',        '    <Footer x:Margin="0.3"/>',        '    <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>',        '   </PageSetup>',        '   <Print>',        '    <ValidPrinterInfo/>',        '    <HorizontalResolution>600</HorizontalResolution>',        '    <VerticalResolution>600</VerticalResolution>',        '   </Print>',        '   <Selected/>',        '   <Panes>',        '    <Pane>',        '     <Number>3</Number>',        '     <ActiveRow>8</ActiveRow>',        '     <ActiveCol>3</ActiveCol>',        '    </Pane>',        '   </Panes>',        '   <ProtectObjects>False</ProtectObjects>',        '   <ProtectScenarios>False</ProtectScenarios>',        '  </WorksheetOptions>',        ' </Worksheet>'
    ];
    return XTemplate.join("").format($data);
}

function CreateTableThongTin($data) {
    var XTemplate = [
        '<?xml version="1.0"?>',        '<?mso-application progid="Excel.Sheet"?>',        '<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"',        ' xmlns:o="urn:schemas-microsoft-com:office:office"',        ' xmlns:x="urn:schemas-microsoft-com:office:excel"',        ' xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"',        ' xmlns:html="http://www.w3.org/TR/REC-html40">',        ' <DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">',        '  <Author>Administrator</Author>',        '  <LastAuthor>Administrator</LastAuthor>',        '  <Created>2018-12-14T08:46:52Z</Created>',        '  <LastSaved>2018-12-14T08:59:36Z</LastSaved>',        '  <Version>16.00</Version>',        ' </DocumentProperties>',        ' <OfficeDocumentSettings xmlns="urn:schemas-microsoft-com:office:office">',        '  <AllowPNG/>',        ' </OfficeDocumentSettings>',        ' <ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">',        '  <WindowHeight>7545</WindowHeight>',        '  <WindowWidth>20490</WindowWidth>',        '  <WindowTopX>32767</WindowTopX>',        '  <WindowTopY>32767</WindowTopY>',        '  <ProtectStructure>False</ProtectStructure>',        '  <ProtectWindows>False</ProtectWindows>',        ' </ExcelWorkbook>',        ' <Styles>',        '  <Style ss:ID="Default" ss:Name="Normal">',        '   <Alignment ss:Vertical="Bottom"/>',        '   <Borders/>',        '   <Font ss:FontName="Calibri" x:Family="Swiss" ss:Size="11" ss:Color="#000000"/>',        '   <Interior/>',        '   <NumberFormat/>',        '   <Protection/>',        '  </Style>',        '  <Style ss:ID="m2130864667504">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864663968">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864663988">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864666336">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="m2130864666356">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s67">',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s71">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s73">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Bottom"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s74">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000" ss:Bold="1"/>',        '  </Style>',        '  <Style ss:ID="s78">',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s85">',        '   <Alignment ss:Horizontal="Center" ss:Vertical="Center"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="12"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s89">',        '   <Alignment ss:Horizontal="Left" ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s97">',        '   <Alignment ss:Vertical="Center" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        ' </Styles>',        ' <Worksheet ss:Name="ThongTinChung">',        '  <Table ss:ExpandedColumnCount="4" ss:ExpandedRowCount="13" x:FullColumns="1"',        '   x:FullRows="1" ss:StyleID="s67" ss:DefaultRowHeight="15">',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="126"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="141"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="136.5"/>',        '   <Column ss:StyleID="s67" ss:AutoFitWidth="0" ss:Width="130.5"/>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s74"><Data ss:Type="String">THÔNG TIN CHUNG</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s71"><Data ss:Type="String">Tên, địa chỉ (email, điện thoại liên lạc) người thực hiện báo cáo: {tendiachi}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:MergeAcross="3" ss:StyleID="s73"><Data ss:Type="String">Ngày báo cáo:</Data></Cell>',        '   </Row>',        '   <Row ss:AutoFitHeight="0" ss:Height="21.75">',        '    <Cell ss:StyleID="s85"><Data ss:Type="String">Tên khu rừng</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="s85"><Data ss:Type="String">{unitname}</Data></Cell>',        '    <Cell ss:StyleID="s85"><Data ss:Type="String">Ghi chú</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Loại/ hạng</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864666336"><Data ss:Type="String">{loaihang}</Data></Cell>',        '    <Cell ss:StyleID="s78"/>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Vị trí</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864666356"><Data ss:Type="String">{address}</Data></Cell>',        '    <Cell ss:StyleID="s78"/>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Ngày thành lập:</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864663968"><Data ss:Type="String">{ngaythanhlap}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{ngaythanhlapghichu}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Cơ quan, đơn vị quản lý:</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864663988"><Data ss:Type="String">{cacdonviql}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{cacdonviqlghichu}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Diện tích khu rừng:</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Tổng : {dientich1}</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Phân khu : {dientich2}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{dientichghichu}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Tổng số cán bộ:</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Trong biên chế : {tongcanbo1}</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Hợp đồng : {tongcanbo2}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{tongcanboghichu}</Data></Cell>',        '   </Row>',        '   <Row ss:Height="30">',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Ngân sách nhà nước</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Bảo vệ rừng; khôi phục rừng : {ngansachnhannuoc1}</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">Xây dựng cơ sở hạ tầng, trang thiết bị : {ngansachnhanuoc2}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">Đầu tư khác {dautukhac}</Data></Cell>',        '   </Row>',        '   <Row>',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Nhiệm vụ chính trong năm</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">{nhiemvuchinh1}</Data></Cell>',        '    <Cell ss:StyleID="s97"><Data ss:Type="String">{nhiemvuchinh2}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{nhiemvuchinhghichu}</Data></Cell>',        '   </Row>',        '   <Row ss:Height="30">',        '    <Cell ss:StyleID="s89"><Data ss:Type="String">Các chương trình, dự án, đè ắn khác liên quan</Data></Cell>',        '    <Cell ss:MergeAcross="1" ss:StyleID="m2130864667504"><Data ss:Type="String">{cacchuongtrinhlq}</Data></Cell>',        '    <Cell ss:StyleID="s78"><Data ss:Type="String">{cacchuongtrinhlqghichu}</Data></Cell>',        '   </Row>',        '  </Table>',        '  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">',        '   <PageSetup>',        '    <Header x:Margin="0.3"/>',        '    <Footer x:Margin="0.3"/>',        '    <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>',        '   </PageSetup>',        '   <Print>',        '    <ValidPrinterInfo/>',        '    <HorizontalResolution>600</HorizontalResolution>',        '    <VerticalResolution>600</VerticalResolution>',        '   </Print>',        '   <Selected/>',        '   <Panes>',        '    <Pane>',        '     <Number>3</Number>',        '     <ActiveRow>8</ActiveRow>',        '     <ActiveCol>3</ActiveCol>',        '    </Pane>',        '   </Panes>',        '   <ProtectObjects>False</ProtectObjects>',        '   <ProtectScenarios>False</ProtectScenarios>',        '  </WorksheetOptions>',        ' </Worksheet>',        '</Workbook>',
    ];
    var strArrTemplate = XTemplate.join("").format($data);
    var uri = 'data:application/vnd.ms-excel;base64,' + Base64.encode(strArrTemplate);

    var downloadLink = document.createElement("a");
    downloadLink.href = uri;
    downloadLink.download = (new Date()).toString().replace(/\S+\s(\S+)\s(\d+)\s(\d+)\s.*/, '$2_$1_$3') + '.xls';

    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
}


function CreateBaoCaoTienDo($DuLieu,$KieuBaoCao) {
    var Title = '';
    var DanhSachDonVi = $DuLieu.DanhSachDonVi;
    var DanhSachQuy = $DuLieu.tienDos;
    switch ($KieuBaoCao) {
        case 1:
            Title = 'TỔNG HỢP TIẾN ĐỘ BÁO CÁO CÁC QUÝ (Đúng tiến độ)';
            break;
        case 2:
            Title = 'TỔNG HỢP TIẾN ĐỘ BÁO CÁO CÁC QUÝ (Chậm tiến độ)';
            break;
        case 3:
            Title = 'TỔNG HỢP TIẾN ĐỘ BÁO CÁO CÁC QUÝ (Không báo cáo)';
            break;
    }
    var ExpandedColumnCount = DanhSachQuy.length + 2;
    var ExpandedRowCount = DanhSachDonVi.length + 3;
    var MergeAcrossTitle = ExpandedColumnCount - 1;
    var Columns = [
        '   <Column ss:Index="2" ss:StyleID="s62" ss:AutoFitWidth="0" ss:Width="167.25"/>',
    ];
    var arrHeader = [
        '   <Row ss:Index="3">',        '    <Cell ss:StyleID="s67"><Data ss:Type="String">STT</Data></Cell>',        '    <Cell ss:StyleID="s67"><Data ss:Type="String">Đơn Vị</Data></Cell>'
    ];
    for (var i = 0; i < DanhSachQuy.length; i++) {
        var Item = DanhSachQuy[i];
        arrHeader.push('    <Cell ss:StyleID="s67"><Data ss:Type="String">{TenQuy}</Data></Cell>'.format({ TenQuy: Item.QuyBaoCao }))
        Columns.push('<Column ss:StyleID="s62" ss:AutoFitWidth="0" ss:Width="78"/>');
    }
    arrHeader.push('   </Row>');

    var arrDataReport = arrHeader;
    for (var i = 0; i < DanhSachDonVi.length; i++) {
        var DonVi = DanhSachDonVi[i];
        var arrDonVi = [
            '   <Row>',            '    <Cell ss:StyleID="s68"><Data ss:Type="Number">{STT}</Data></Cell>'.format({ STT: (i + 1) }),            '    <Cell ss:StyleID="s66"><Data ss:Type="String">{UnitName}</Data></Cell>'.format({ UnitName: DonVi.UnitName})
        ];
        for (var j = 0; j < DanhSachQuy.length; j++) {
            var $Quy = DanhSachQuy[j];
            var $DuLieuTienDo;
            switch ($KieuBaoCao) {
                case 1:
                    $DuLieuTienDo = $Quy.SoDonViDungTienDo;
                    break;
                case 2:
                    $DuLieuTienDo = $Quy.SoDonViChamTienDo;
                    break;
                case 3:
                    $DuLieuTienDo = $Quy.SoDonViKhongBaoCao;
                    break;
            }
            var finDonVi = $DuLieuTienDo.filter(function ($Item) {
                return $Item.Id == DonVi.Id;
            });
            if (finDonVi.length == 1) {
                arrDonVi.push('    <Cell ss:StyleID="s77"><Data ss:Type="Number">1</Data></Cell>');
            }
            else {
                arrDonVi.push('    <Cell ss:StyleID="s77"><Data ss:Type="Number">0</Data></Cell>');
            }
        }
        arrDonVi.push('   </Row>');
        arrDataReport.push(arrDonVi.join(""))
    }
    ExpandedRowCount++;
    var arrFooter = [
            '   <Row>',        '    <Cell ss:StyleID="s68"><Data ss:Type="String">Tổng</Data></Cell>',
        '    <Cell ss:StyleID="s69"><Data ss:Type="Number">{Tong}</Data></Cell>'.format({ Tong: DanhSachDonVi.length })
    ]
    for (var i = 0; i < DanhSachQuy.length; i++) {
        var Item = DanhSachQuy[i];
        switch ($KieuBaoCao) {
            case 1:
                arrFooter.push('    <Cell ss:StyleID="s69"><Data ss:Type="Number">{Tong}</Data></Cell>'.format({ Tong: Item.SoDonViDungTienDo.length }))
                break;
            case 2:
                arrFooter.push('    <Cell ss:StyleID="s69"><Data ss:Type="Number">{Tong}</Data></Cell>'.format({ Tong: Item.SoDonViChamTienDo.length }))
                break;
            case 3:
                arrFooter.push('    <Cell ss:StyleID="s69"><Data ss:Type="Number">{Tong}</Data></Cell>'.format({ Tong: Item.SoDonViKhongBaoCao.length }))
                break;
        }
    }

    arrFooter.push('   </Row>')

    var DataRePort = arrDataReport.join("");
    var XTemplate = [
        '<?xml version="1.0"?>',        '<?mso-application progid="Excel.Sheet"?>',        '<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"',        ' xmlns:o="urn:schemas-microsoft-com:office:office"',        ' xmlns:x="urn:schemas-microsoft-com:office:excel"',        ' xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"',        ' xmlns:html="http://www.w3.org/TR/REC-html40">',        ' <DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">',        '  <Author>Administrator</Author>',        '  <LastAuthor>Administrator</LastAuthor>',        '  <Created>2018-12-20T07:28:13Z</Created>',        '  <LastSaved>2018-12-20T07:40:05Z</LastSaved>',        '  <Version>16.00</Version>',        ' </DocumentProperties>',        ' <OfficeDocumentSettings xmlns="urn:schemas-microsoft-com:office:office">',        '  <AllowPNG/>',        ' </OfficeDocumentSettings>',        ' <ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">',        '  <WindowHeight>7545</WindowHeight>',        '  <WindowWidth>20490</WindowWidth>',        '  <WindowTopX>32767</WindowTopX>',        '  <WindowTopY>32767</WindowTopY>',        '  <ProtectStructure>False</ProtectStructure>',        '  <ProtectWindows>False</ProtectWindows>',        ' </ExcelWorkbook>',        ' <Styles>',        '  <Style ss:ID="Default" ss:Name="Normal">',        '   <Alignment ss:Vertical="Bottom"/>',        '   <Borders/>',        '   <Font ss:FontName="Calibri" x:Family="Swiss" ss:Size="11" ss:Color="#000000"/>',        '   <Interior/>',        '   <NumberFormat/>',        '   <Protection/>',        '  </Style>',        '  <Style ss:ID="s62">',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s66">',        '   <Alignment ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s67">',        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000" ss:Bold="1"/>',        '  </Style>',        '  <Style ss:ID="s68">',        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s69">',        '   <Alignment ss:Horizontal="Right" ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s75">',        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '  </Style>',        '  <Style ss:ID="s76">',        '   <Alignment ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '   <Interior/>',        '  </Style>',        '  <Style ss:ID="s77">',        '   <Alignment ss:Horizontal="Center" ss:Vertical="Bottom" ss:WrapText="1"/>',        '   <Borders>',        '    <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>',        '    <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>',        '   </Borders>',        '   <Font ss:FontName="Times New Roman" x:Family="Roman" ss:Size="11"',        '    ss:Color="#000000"/>',        '   <Interior/>',        '  </Style>',        ' </Styles>',        ' <Worksheet ss:Name="Sheet1">',        '  <Table ss:ExpandedColumnCount="{ExpandedColumnCount}" ss:ExpandedRowCount="{ExpandedRowCount}" x:FullColumns="1"',        '   x:FullRows="1" ss:StyleID="s62" ss:DefaultRowHeight="15">',        '   {Columns}',        '   <Row>',        '    <Cell ss:MergeAcross="{MergeAcrossTitle}" ss:StyleID="s75"><Data ss:Type="String">{Title}</Data></Cell>',        '   </Row>',        '  {DataRePort}',        '  {Footer}',        '  </Table>',        '  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">',        '   <PageSetup>',        '    <Header x:Margin="0.3"/>',        '    <Footer x:Margin="0.3"/>',        '    <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>',        '   </PageSetup>',        '   <Print>',        '    <ValidPrinterInfo/>',        '    <HorizontalResolution>600</HorizontalResolution>',        '    <VerticalResolution>600</VerticalResolution>',        '   </Print>',        '   <Selected/>',        '   <Panes>',        '    <Pane>',        '     <Number>3</Number>',        '     <ActiveRow>1</ActiveRow>',        '     <ActiveCol>1</ActiveCol>',        '    </Pane>',        '   </Panes>',        '   <ProtectObjects>False</ProtectObjects>',        '   <ProtectScenarios>False</ProtectScenarios>',        '  </WorksheetOptions>',        ' </Worksheet>',        '</Workbook>',
    ];
    var strArrTemplate = XTemplate.join("").format({
        ExpandedColumnCount: ExpandedColumnCount,
        ExpandedRowCount: ExpandedRowCount,
        Title: Title,
        MergeAcrossTitle: MergeAcrossTitle,
        DataRePort: DataRePort,
        Columns: Columns.join(""),
        Footer: arrFooter.join("")
    });
    var uri = 'data:application/vnd.ms-excel;base64,' + Base64.encode(strArrTemplate);

    var downloadLink = document.createElement("a");
    downloadLink.href = uri;
    downloadLink.download = (new Date()).toString().replace(/\S+\s(\S+)\s(\d+)\s(\d+)\s.*/, '$2_$1_$3') + '.xls';

    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);

}

