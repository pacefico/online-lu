//http://malsup.com/jquery/form/#options-object

interface ajaxFormOptions {
    beforeSerialize?: ($form, options) => boolean;
    beforeSubmit?: (arr, $form, options) => boolean;
    clearForm?: boolean;
    data?: any;
    dataType?: string;
    error?: (err) => any;
    forceSync?: boolean;
    iframe?: boolean;
    iframeSrc?: string;
    iframeTarget?: string;
    replaceTarget?: boolean;
    resetForm?: boolean;
    semantic?: boolean;
    success?(response, status, xhr, element);
    target?: any;
    type?: string;
    uploadProgress?(event, position, total, percent);
    url?: string;
}

interface JQuery {
    ajaxForm(options?: ajaxFormOptions): JQuery;
    ajaxSubmit(options?: ajaxFormOptions): JQuery;
}
