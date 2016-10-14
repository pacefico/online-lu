/// <reference path="../../typings/jquery/jquery.d.ts"/>
/// <reference path="../../typings/kendo/kendo.all.d.ts"/>
/// <reference path="../../typings/fileapi/fileapi.d.ts"/>

/// <reference path="../../TypeLite.d.ts"/>

module Controllers.Execution {

    export class IndexController {
         
        private gridHistory: kendo.ui.Grid;
        private dataSourceProject: kendo.data.DataSource;
        private kendoDatePickerStart: kendo.ui.DatePicker;
        private kendoDatePickerEnd: kendo.ui.DatePicker;
        private kendoComboOrigin: kendo.ui.ComboBox;
        private kendoNumericRange: kendo.ui.NumericTextBox;

        private maxRetries = 3;
        private blockLength = 0;
        private numberOfBlocks = 0;
        private currentChunk = 0;
        private retryAfterSeconds = 3;
        private precision = 10;

        private isLoaded: boolean;

        constructor() {
        
            this.loadDataSource();
        }

        public onLoad() {

            var _this = this;

            function onSelect(e) {
                var dataItem = this.dataItem(e.item.index());

                if (dataItem.value == 0) {
                    document.getElementById('div-new-existent').style.display = 'block';
                    document.getElementById('div-new-aleatory').style.display = 'none';
                    document.getElementById('div-new-upload').style.display = 'none';
                }

                if (dataItem.value == 1) {
                    document.getElementById('div-new-existent').style.display = 'none';
                    document.getElementById('div-new-aleatory').style.display = 'block';
                    document.getElementById('div-new-upload').style.display = 'none';
                }

                if (dataItem.value == 2) {
                    document.getElementById('div-new-existent').style.display = 'none';
                    document.getElementById('div-new-aleatory').style.display = 'none';
                    document.getElementById('div-new-upload').style.display = 'block';
                }
            };

            this.kendoComboOrigin = $("#combo-origin").kendoComboBox({
                dataTextField: "text",
                dataValueField: "value",
                dataSource: [
                    { text: "Existente", value: "0" },
                    //{ text: "Aleatório", value: "1" },
                    { text: "Upload", value: "2" }
                ],
                index: 0,
                select: onSelect,
            }).data("kendoComboBox");

            this.kendoNumericRange = $("#combo-range-number").kendoNumericTextBox().data("kendoNumericTextBox");
            //$("#combo-range-number-aleatory").kendoNumericTextBox();

            var _dataRange = [
                { text: "Todos", value: "0" },
                { text: "10", value: "1" },
                { text: "100", value: "2" },
                { text: "1000", value: "3" },
                { text: "5000", value: "4" },
                { text: "10000", value: "5" }
            ];

            $("#combo-range").kendoComboBox({
                dataTextField: "text",
                dataValueField: "value",
                dataSource: _dataRange,
                filter: "contains",
                suggest: true,
                index: 1
            });

            //this.kendoDatePickerStart = $("#date-from-filter").kendoDatePicker({
            //    culture: "pt-br",
            //    format: "d",
            //}).data("kendoDatePicker");
            //var date = new Date();
            //date.setDate(date.getDate() - 2);
            //this.kendoDatePickerStart.value(date);

            //this.kendoDatePickerEnd = $("#date-to-filter").kendoDatePicker({
            //    culture: "pt-br",
            //    format: "d",
            //}).data("kendoDatePicker");
            //this.kendoDatePickerEnd.value(new Date());

            $("#combo-range-filter").kendoComboBox({
                dataTextField: "text",
                dataValueField: "value",
                dataSource: _dataRange,
                //filter: "contains",
                //suggest: true,
                index: 0
            });

            $("#combo-status-filter").kendoComboBox({
                dataTextField: "text",
                dataValueField: "value",
                dataSource: [
                    { text: "Todos", value: "0" },
                    { text: "Concluido", value: "1" },
                    { text: "Pendente", value: "2" }
                ],
                //filter: "contains",
                //suggest: true,
                index: 2
            });

            $("#grid").kendoGrid({
                dataSource: this.dataSourceProject,
                groupable: true,
                sortable: true,
                //detailInit: detailInit,
                dataBound: function () {
                    //this.expandRow(this.tbody.find("tr.k-master-row").first());
                },
                columns: [
                    {
                        field: "ExecutionDate",
                        title: "Data", 
                        width: "auto",
                        //format: "dd/MM/yyyy",
                        template: "#= kendo.toString(kendo.parseDate(ExecutionDate, 'yyyy-MM-dd'), 'dd/MM/yyyy') #"
                    },
                    {
                        field: "Range",
                        title: "Ordem",
                        width: "auto"
                    },
                    {
                        field: "Status",
                        title: "Status",
                        template: "#= Status ? 'Concluido' : 'Pendente' #",
                        groupHeaderTemplate: "Status: #= value ? 'Concluido' : 'Pendente' #"
                    },

                ]
            });
             
            $("#button-generate").click(() => {

                var _index = parseInt($("#combo-origin").data("kendoComboBox").value());
                
                if (_index == 0) {
                    _this.GenerateProcessExistent();
                }
                else if (_index == 2) {
                    this.beginUpload();
                }
            });

            $("#button-filter").click(() => {
                _this.dataSourceProject.read();
            });

            this.isLoaded = true;
//            this.InitializeReader();
        }

        private InitializeReader() {

            $("#fileUpload").click(() => {
                this.beginUpload();
            });

            //$(document).on("click", "#fileUpload", this.beginUpload);
            //$("#progressBar").progressbar(0);

        }

        private beginUpload() {
            var _this = this;
            var fileControl = document.getElementById('selectFile');

            if (fileControl.files.length > 0) {
                for (var i = 0; i < fileControl.files.length; i++) {
                    _this.uploadMetaData(fileControl.files[i], i);
                }
            }
        }

        private uploadMetaData(file, index) {
            
            var _this = this;
            var size = file.size;

            //    numberOfBlocks = Math.ceil(file.size / blockLength);
            this.numberOfBlocks = Math.sqrt(size / this.precision);
            this.blockLength = this.numberOfBlocks * this.precision;
            var name = file.name;
            this.currentChunk = 0;

            $.ajax({
                type: "POST",
                async: true,
                url: "/Home/SetMetadata?blocksCount=" + this.numberOfBlocks + "&fileName=" + name + "&fileSize=" + size,
            }).done((state) => {
                    if (state === true) {
                        _this.displayStatusMessage("Starting Upload");
                        _this.sendFile(file, _this.blockLength);
                    }
                }).fail(() => {
                    _this.displayStatusMessage("Failed to send MetaData");
                });
        }

        private sendFile (file, chunkSize) {

            var _this = this;
            var start = 0,
                end = Math.min(chunkSize, file.size),
                retryCount = 0,
                sendNextChunk, fileChunk;
            this.displayStatusMessage("");

            sendNextChunk = () => {
                fileChunk = new FormData();

                if (file.slice) {
                    fileChunk.append('Slice', file.slice(start, end));
                }
                else if (file.webkitSlice) {
                    fileChunk.append('Slice', file.webkitSlice(start, end));
                }
                else if (file.mozSlice) {
                    fileChunk.append('Slice', file.mozSlice(start, end));
                }
                else {
                    //this.displayStatusMessage(operationType.UNSUPPORTED_BROWSER);
                    return;
                }
                var timeout = 5000;
                if (_this.currentChunk == _this.numberOfBlocks - 1) {
                    timeout = 60 * 60000; // 60 minutes for the last one
                }
                var jqxhr = $.ajax({
                    async: true,
                    url: ('/Home/UploadChunk?id=' + _this.currentChunk),
                    data: fileChunk,
                    cache: false,
                    contentType: false,
                    processData: false,
                    timeout: timeout,
                    type: 'POST'
                }).fail((request, error) => {
                    if (error !== 'abort' && retryCount < this.maxRetries) {
                            ++retryCount;
                            setTimeout(sendNextChunk, _this.retryAfterSeconds * 1000);
                        }

                        if (error === 'abort') {
                            this.displayStatusMessage("Aborted");
                        }
                        else {
                            if (retryCount === this.maxRetries) {
                                this.displayStatusMessage("Upload timed out.");
                                //resetControls();
                                //uploader = null;
                            }
                            else {
                                this.displayStatusMessage("Resuming Upload");
                            }
                        }

                        return;
                }).done( (notice) => {
                        if (notice.error || notice.isLastBlock) {
                            _this.displayStatusMessage(notice.message);
                            return;
                        }
                        ++ _this.currentChunk;
                        start = (this.currentChunk) * this.blockLength;
                        end = Math.min((this.currentChunk +1) * this.blockLength, file.size);
                        retryCount = 0;
                        _this.updateProgress();
                        if (_this.currentChunk < _this.numberOfBlocks) {
                            sendNextChunk();
                        }
                    });
            }
            sendNextChunk();
        }

        private displayStatusMessage (message) {
            $("#statusMessage").text(message);
        }

        private updateProgress ()
        {
            var progress = this.currentChunk / this.numberOfBlocks * 100;
            if (progress <= 100) {
                //$("#progressBar").progressbar("option", "value", parseInt(progress));
                this.displayStatusMessage("Uploaded " + progress.toFixed(2) + "%");
            }
        }

        private GenerateProcessExistent() {
            
            var _this = this;
            var _dataCombo = $("#combo-origin").data("kendoComboBox").value();

            if (_dataCombo == 0) {
                var _rangeIndex = parseInt($("#combo-range").data("kendoComboBox").value());
                if (_rangeIndex > 0) {
                    var _range = parseInt($("#combo-range").data("kendoComboBox").text());
                    var _project: ProjectModels.ProjectSimpleModelVM = {
                        ID: 0,
                        ExecutionDate: null,
                        Range: _range,
                        Status: false,
                        History: null
                    };

                    $.ajax({
                        type: "POST",
                        url: "/Home/GenerateProjectExecution",
                        data: _project,
                        success: function (response) {
                            if (response) {
                                if (response.Result == true) {
                                    alert("Execução cadastrada com sucesso!");
                                    _this.dataSourceProject.read();
                                }
                                else {
                                    alert("Houve erro na solicitação!");
                                }
                            }
                        },
                        dataType: "json",
                        traditional : true,
                    });
                } else {
                    alert("É necessário selecionar uma ordem correta!");
                }
            }
        }

        private loadDataSource() {
            var _this = this;
            this.dataSourceProject = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "/Home/GetExecutions",
                        type: "POST",
                        data: () => {
                            return _this.GetProjectFillterParam();
                            }
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
                            Status: {
                                type: "boolean",
                                editable: false
                            },
                        }
                    },
                    data: function (jsonresp) {
                        return jsonresp.Result;
                    }
                },
            });
        }

        private GetProjectFillterParam(): ProjectModels.ProjectModelParamVM {
            
            if (this.isLoaded) {
                var _paramVM: ProjectModels.ProjectModelParamVM = Object.create({});

                var _rangeIndex = parseInt($("#combo-range-filter").data("kendoComboBox").value());
                if (_rangeIndex > 0) {
                    _paramVM.Range = parseInt($("#combo-range-filter").data("kendoComboBox").text());
                }

                var _statusComboIndex = parseInt($("#combo-status-filter").data("kendoComboBox").value());
                if (_statusComboIndex > 0) {
                    _paramVM.Status = _statusComboIndex == 1 ? true : false;
                }

                return _paramVM;
            }
            return null;
        }
    }
}