var Controllers;
(function (Controllers) {
    /// <reference path="../../typings/jquery/jquery.d.ts"/>
    /// <reference path="../../typings/kendo/kendo.all.d.ts"/>
    /// <reference path="../../TypeLite.d.ts"/>
    (function (History) {
        var IndexController = (function () {
            function IndexController() {
                this.loadDataSource();
            }
            IndexController.prototype.onLoad = function () {
                var _this = this;
                $("#grid").kendoGrid({
                    dataSource: _this.dataSourceHistory,
                    groupable: true,
                    sortable: true,
                    //pageable: {
                    //    refresh: true,
                    //    pageSizes: true,
                    //    buttonCount: 5
                    //},
                    detailInit: detailInit,
                    dataBound: function () {
                        this.expandRow(this.tbody.find("tr.k-master-row").first());
                    },
                    columns: [
                        {
                            field: "ExecutionDate",
                            title: "Data",
                            width: "auto",
                            //format: "{0:d}",
                            template: "#= kendo.toString(kendo.parseDate(ExecutionDate, 'yyyy-MM-dd'), 'dd/MM/yyyy') #"
                        },
                        {
                            field: "Range",
                            title: "Ordem",
                            width: "auto"
                        },
                        {
                            field: "TimeInitialUploadMs",
                            title: "Upload(ms) Inicial",
                            width: "auto",
                            groupFooterTemplate: "<div>Média(ms): #=average#</div>"
                        },
                        {
                            field: "TimeDonwloadMs",
                            title: "Download(ms)",
                            width: "auto",
                            groupFooterTemplate: "<div>Média(ms): #=average#</div>"
                        },
                        {
                            field: "CudaTimeMs",
                            title: "Cuda(ms)",
                            width: "auto",
                            groupFooterTemplate: "<div>Média(ms): #=average#</div>"
                        },
                        {
                            field: "TimeUploadMs",
                            title: "Upload(ms)",
                            width: "auto",
                            groupFooterTemplate: "<div>Média: #=average#</div>"
                        },
                        {
                            field: "TotalTimeMs",
                            title: "Total(ms)",
                            width: "auto",
                            groupFooterTemplate: "<div>Média(ms): #=average#</div>"
                        },
                        {
                            field: "Success",
                            title: "Sucesso",
                            template: "#= Success ? 'Sim' : 'Não' #",
                            groupHeaderTemplate: "Sucesso: #= value ? 'Sim' : 'Não' #"
                        }
                    ]
                });

                function detailInit(e) {
                    $("<div/>").appendTo(e.detailCell).kendoGrid({
                        dataSource: {
                            data: e.data.Details,
                            schema: {
                                model: {
                                    fields: {
                                        BytesInitialUpload: { type: "string" },
                                        RateKbsInitialUpload: { type: "string" },
                                        BytesUpload: { type: "string" },
                                        RateKbsUpload: { type: "string" },
                                        BytesDownload: { type: "string" },
                                        RateKbsDownload: { type: "string" }
                                    }
                                }
                            }
                        },
                        columnMenu: true,
                        //groupable: {
                        //    messages: {
                        //        empty: "Arraste uma coluna e solte aqui para agrupar."
                        //    }
                        //},
                        //selectable: "multiple",
                        columns: [
                            {
                                field: "BytesInitialUpload",
                                title: "Bytes Upload Inicial",
                                width: "auto",
                                groupHeaderTemplate: "Bytes Download: #=value#"
                            },
                            {
                                field: "RateKbsInitialUpload",
                                title: "Taxa de Upload Inicial",
                                width: "auto",
                                groupHeaderTemplate: "Taxa de Upload Inicial: #=value#"
                            },
                            {
                                field: "BytesDownload",
                                title: "Bytes Download",
                                width: "auto",
                                groupHeaderTemplate: "Bytes Download: #=value#"
                            },
                            {
                                field: "RateKbsDownload",
                                title: "Taxa de Download",
                                width: "auto",
                                groupHeaderTemplate: "Taxa de Download: #=value#"
                            },
                            {
                                field: "BytesUpload",
                                title: "Bytes Upload",
                                width: "auto",
                                groupHeaderTemplate: "Bytes Upload: #=value#"
                            },
                            {
                                field: "RateKbsUpload",
                                title: "Taxa de Upload",
                                width: "auto",
                                groupHeaderTemplate: "Taxa de Upload: #=value#"
                            }
                        ]
                    });
                }
            };

            IndexController.prototype.loadDataSource = function () {
                this.dataSourceHistory = new kendo.data.DataSource({
                    transport: {
                        read: {
                            //url: Enviroment.appRootPath + "Administration/Home/RetrieveHistory",
                            url: "/Home/RetrieveHistory",
                            type: "POST"
                        }
                    },
                    schema: {
                        model: {
                            id: "ID",
                            fields: {
                                ExecutionDate: {
                                    type: "date",
                                    editable: false
                                },
                                Range: {
                                    type: "number",
                                    editable: false
                                },
                                TimeInitialUploadMs: {
                                    editable: false
                                },
                                TimeDonwloadMs: {
                                    editable: false
                                },
                                CudaTimeMs: {
                                    type: "number",
                                    editable: false
                                },
                                TimeUploadMs: {
                                    type: "number",
                                    editable: false
                                },
                                TotalTimeMs: {
                                    type: "number",
                                    editable: false
                                },
                                Success: {
                                    type: "boolean",
                                    editable: false
                                }
                            }
                        },
                        data: function (jsonresp) {
                            //Mithril.Utils.RestUtils.messageResponseErrorHandling(jsonresp.Result);
                            return jsonresp.Result;
                        },
                        total: function (jsonresp) {
                            //return jsonresp.Total;
                        }
                    },
                    group: {
                        field: "Range",
                        aggregates: [
                            { field: "TimeInitialUploadMs", aggregate: "average" },
                            { field: "TimeDonwloadMs", aggregate: "average" },
                            { field: "CudaTimeMs", aggregate: "average" },
                            { field: "TimeUploadMs", aggregate: "average" },
                            { field: "TotalTimeMs", aggregate: "average" }
                        ]
                    },
                    aggregate: [
                        { field: "TimeInitialUploadMs", aggregate: "average" },
                        { field: "TimeDonwloadMs", aggregate: "average" },
                        { field: "CudaTimeMs", aggregate: "average" },
                        { field: "TimeUploadMs", aggregate: "average" },
                        { field: "TotalTimeMs", aggregate: "average" }
                    ],
                    pageSize: 10,
                    serverPaging: true,
                    requestStart: function (e) {
                        //_this.setIsBusy(true, "Aguarde, carregando dados...");
                    },
                    requestEnd: function (e) {
                        //_this.setIsBusy(false);
                    }
                });
            };
            return IndexController;
        })();
        History.IndexController = IndexController;
    })(Controllers.History || (Controllers.History = {}));
    var History = Controllers.History;
})(Controllers || (Controllers = {}));
